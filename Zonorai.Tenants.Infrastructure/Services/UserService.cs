using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.Application.Common.Configuration;
using Zonorai.Tenants.Application.Users.Commands;
using Zonorai.Tenants.Application.Users.Commands.Add;
using Zonorai.Tenants.Application.Users.Commands.Delete;
using Zonorai.Tenants.Application.Users.Commands.Login;
using Zonorai.Tenants.Application.Users.Commands.Register;
using Zonorai.Tenants.Application.Users.Commands.TryLogin;
using Zonorai.Tenants.ApplicationInterface.Users.Commands;
using Zonorai.Tenants.Domain;
using Zonorai.Tenants.Domain.Claims;
using Zonorai.Tenants.Domain.Common;
using Zonorai.Tenants.Domain.Tenants;
using Zonorai.Tenants.Domain.UserClaims;
using Zonorai.Tenants.Domain.Users;
using Zonorai.Tenants.Infrastructure.Persistence;

namespace Zonorai.Tenants.Infrastructure.Services
{
    internal class UserService : IUserService
    {
        private readonly TenantDbContext _context;
        private readonly IMultiTenantStore<TenantInformation> _tenantStore;
        private readonly ITokenService _tokenService;
        private readonly IOptions<TenantApplicationConfiguration> _options;
        private readonly IEventStore _eventStore;
        private readonly ITenantInfo _tenantInfo;

        public UserService(TenantDbContext context, IMultiTenantStore<TenantInformation> tenantStore,
            ITokenService tokenService, IOptions<TenantApplicationConfiguration> options, IEventStore eventStore,
            ITenantInfo tenantInfo)
        {
            _context = context;
            _tenantStore = tenantStore;
            _tokenService = tokenService;
            _eventStore = eventStore;
            _tenantInfo = tenantInfo;
            _options = options;
        }

        public async Task<Result> AddUser(CreateUser createUser)
        {
            var tenant = await _context.TenantInfo.SingleOrDefaultAsync(x => x.Id == _tenantInfo.Id);
            if (tenant == null)
            {
                return Result.Fail("Tenant Information not found");
            }

            User user;
            if (_context.Users.Any(x => x.Email == createUser.Email))
            {
                user = await _context.Users.Include(x => x.Tenants)
                    .SingleOrDefaultAsync(x => x.Email == createUser.Email);
                if (user.Tenants.Any(x => x.Id == _tenantInfo.Id))
                {
                    return Result.Fail($"User with email {createUser.Email} is already a part of your organization");
                }
            }
            else
            {
                user = new User(createUser);
            }

            user.Tenants.Add(tenant);
            _context.Users.Update(user);
            await _eventStore.AddEvent(new UserAddedEvent(user.Id, user.Email, user.Name, user.Surname, _tenantInfo.Id,
                DateTime.Now));
            await _context.SaveChangesAsync();
            return Result.Ok();
        }

        public async Task<Result> DeleteUser(string userId)
        {
            var tenant = await _context.TenantInfo.SingleOrDefaultAsync(x => x.Id == _tenantInfo.Id);
            if (tenant == null)
            {
                return Result.Fail("Tenant Information not found");
            }

            User user = await _context.Users.Include(x => x.Tenants).SingleOrDefaultAsync(x => x.Id == userId);
            await _eventStore.AddEvent(new UserDeletedEvent(user.Id, _tenantInfo.Id,
                DateTime.Now));
            if (user.Tenants.Count ! > 1)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return Result.Ok();
            }

            user.Tenants.Remove(tenant);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Result.Ok();
        }

        public async Task<LoginResult> Login(string email, string password)
        {
            var user = await _context.Users.Include(x => x.Tenants)
                .Include(x => x.Claims).ThenInclude(x => x.Claim)
                .SingleOrDefaultAsync(x => x.Email == email);

            if (user == null)
            {
                return new LoginResult(null, null, $"Account with Email '{email}' not found");
            }

            if (user.CanLogin(password))
            {
                if (user.Tenants.Count == 1)
                {
                    return await Login(user, user.Tenants.First().Id);
                }

                if (user.Tenants.Count > 1)
                {
                    List<TenantInformationDto> dtos = new List<TenantInformationDto>();
                    user.Tenants.ForEach(x =>
                    {
                        dtos.Add(new TenantInformationDto()
                        {
                            ConnectionString = x.ConnectionString,
                            Id = x.Id,
                            Identifier = x.Identifier,
                            Website = x.Website,
                            Name = x.Name
                        });
                    });
                    return new LoginResult(null, dtos, null);
                }
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return new LoginResult(null, null, "Password does not match stored password");
        }

        public async Task<LoginResult> Login(string email, string password, string tenantId)
        {
            var user = await _context.Users.Include(x => x.Tenants)
                .Include(x => x.Claims).ThenInclude(x => x.Claim)
                .SingleOrDefaultAsync(x => x.Email == email);

            if (user == null)
            {
                return new LoginResult(null, null, $"Account with Email '{email}' not found");
            }

            if (user.CanLogin(password))
            {
                return await Login(user, tenantId);
            }

            return new LoginResult(null, null, "Password does not match stored password");
        }

        public async Task<User> RegisterTenant(RegisterTenant registerTenant)
        {
            var validator = new RegisterTenantValidator();

            (await validator.ValidateAsync(registerTenant)).ThrowIfFailed();

            var tenant = new TenantInformation()
            {
                Identifier = Guid.NewGuid().ToString(),
                Id = Guid.NewGuid().ToString(),
                Name = registerTenant.CompanyName,
                Website = registerTenant.CompanyWebsite
            };

            var tenantPersisted = await _tenantStore.TryAddAsync(tenant);

            if (tenantPersisted == false)
            {
                throw new Exception("Could not register user");
            }

            var createUser = new CreateUser(registerTenant.Email, registerTenant.Name,
                registerTenant.Surname, registerTenant.Password, registerTenant.PhoneNumber);

            var user = new User(createUser);

            var adminClaim = _context.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Role && x.Value == "Admin");
            var ownerClaim = _context.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Role && x.Value == "Owner");

            if (adminClaim == null || ownerClaim == null)
            {
                ownerClaim = new SecurityClaim(ClaimTypes.Role, "Owner", tenant.Id);
                adminClaim = new SecurityClaim(ClaimTypes.Role, "Admin", tenant.Id);
                _context.Claims.Add(adminClaim);
                _context.Claims.Add(ownerClaim);
                await _context.SaveChangesAsync();
            }

            user.Tenants.Add(tenant);
            user.Claims.Add(new UserClaim(adminClaim.Id, user.Id, tenant.Id));
            user.Claims.Add(new UserClaim(ownerClaim.Id, user.Id, tenant.Id));
            _context.Users.Add(user);
            await _eventStore.AddEvent(new TenantRegisteredEvent(user.Id, user.Email, tenant.Name, user.Name,
                user.Surname, user.Tenants.First().Id, DateTime.Now));
            await _context.SaveChangesAsync();
            return user;
        }

        private async Task<LoginResult> Login(User user, string tenantId)
        {
            if (_options.Value.RequireConfirmedEmailForLogin)
            {
                if (user.EmailConfirmed == false)
                {
                    return new LoginResult(null, null, "Email must be confirmed before login");
                }
            }

            await _eventStore.AddEvent(new UserLoggedInEvent(user.Email, tenantId, DateTime.Now));
            var tenant = await _tenantStore.TryGetAsync(tenantId);
            if (tenant == null)
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return new LoginResult(null, null, $"Tenant with Id '{tenantId}' does not exist");
            }

            //Default Claims we need
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.GivenName, user.Name),
                new Claim(ClaimTypes.Surname, user.Surname),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("Id", user.Id),
                new Claim(TenantConstants.TenantIdentifier, tenant.Identifier),
                new Claim(TenantConstants.TenantId, tenant.Id)
            };


            //Add RoleClaims
            var storedClaims = GetUserClaims(user, tenantId);

            if (storedClaims.Any())
            {
                claims.AddRange(storedClaims);
            }

            var token = _tokenService.GenerateToken(new ClaimsIdentity(claims));
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return new LoginResult(token, null, null);
        }

        private List<Claim> GetUserClaims(User user, string tenantId)
        {
            List<Claim> claims = new List<Claim>();
            user.Claims.Where(x => x.TenantId == tenantId).ToList()
                .ForEach(x => claims.Add(new Claim(x.Claim.Type, x.Claim.Value)));
            return claims;
        }
    }
}