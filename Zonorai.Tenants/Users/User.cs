using System;
using System.Collections.Generic;
using System.Security.Claims;
using FluentValidation;
using Zonorai.Tenants.Claims;
using Zonorai.Tenants.Common;
using Zonorai.Tenants.Entities;
using Zonorai.Tenants.UserClaims;

namespace Zonorai.Tenants.Users
{
    public class User
    {
        public User(CreateUser createUser)
        {
            CreateUserValidator validator = new CreateUserValidator();
            var validationResult = validator.Validate(createUser);
            
            if (validationResult.IsValid == false)
            {
                throw new ValidationException(validationResult.Errors);
            }

            Id = Guid.NewGuid().ToString();
            Email = createUser.Email;
            Username = createUser.Username;
            Name = createUser.Name;
            Surname = createUser.Surname;
            byte[] salt = HashHelper.CreateSalt();
            Password = HashHelper.HashPassword(createUser.Password, salt);
            Salt = Convert.ToBase64String(salt);
        }
        public string Id { get; init; }
        public string Email { get; private set; }
        public string Username { get; private set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public string Salt { get; private set; }
        public string Password { get; private set; }
        public bool Locked { get; private set; } = false;
        public DateTime? DateLocked { get; private set; }
        public int LoginAttempts { get; private set; } = 0;
        public string FullName => $"{Name} {Surname}";
        
        public List<UserClaim> Claims { get; set; }
        public List<TenantInformation> Tenants { get; set; }
        
        public void SetEmail(string email)
        {
           FluentValueValidator<string>.Validate(email,x => x.NotEmpty().NotNull().EmailAddress());
           Email = email;
        }
        public void SetName(string name)
        {
            FluentValueValidator<string>.Validate(name,x => x.NotEmpty().NotNull());
            Name = name;
        }
        public void SetSurname(string surname)
        {
            FluentValueValidator<string>.Validate(surname,x => x.NotEmpty().NotNull());
            Surname = surname;
        }
        public void SetUsername(string username)
        {
            FluentValueValidator<string>.Validate(username,x => x.NotEmpty().NotNull());
            Username = username;
        }

        public bool CanLogin(string password)
        {
            if (Locked)
            {
                if (DateLocked.HasValue == false)
                {
                    DateLocked = DateTime.Now;
                    return false;
                }
                
                bool canUnlock = DateLocked != null && DateTime.Now > DateLocked.Value.AddMinutes(5);
                
                if (canUnlock)
                {
                    Locked = false;
                    LoginAttempts = 1;
                }

                if (canUnlock == false)
                {
                    throw new Exception("User has exceeded login attempts, please try again later");
                }
                
            }
            FluentValueValidator<string>.Validate(password,x => x.NotEmpty().NotNull().MinimumLength(8));
            string testHash = HashHelper.HashPassword(password, Convert.FromBase64String(Salt));
            
            if (testHash == Password)
            {
                return true;
            }

            LoginAttempts++;
            if (LoginAttempts == 5)
            {
                Locked = true;
                DateLocked = DateTime.Now;
            }
            
            return false;
        }
    }
}