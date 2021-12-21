using System;
using System.Collections.Generic;
using FluentValidation;
using Zonorai.Tenants.Domain.Common;
using Zonorai.Tenants.Domain.Tenants;
using Zonorai.Tenants.Domain.UserClaims;

namespace Zonorai.Tenants.Domain.Users
{
    public class User
    {
        public User(CreateUser createUser)
        {
            var validator = new CreateUserValidator();
            var validationResult = validator.Validate(createUser);

            if (validationResult.IsValid == false) throw new ValidationException(validationResult.Errors);

            Id = Guid.NewGuid().ToString();
            Email = createUser.Email;
            Name = createUser.Name;
            Surname = createUser.Surname;
            if (string.IsNullOrEmpty(createUser.PhoneNumber) == false) PhoneNumber = createUser.PhoneNumber;
            SetPassword(createUser.Password);
        }

        private User()
        {
        }

        public string Id { get; }
        public string Email { get; private set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public string Salt { get; private set; }
        public string Password { get; private set; }
        public bool Locked { get; private set; }
        public DateTime? DateLocked { get; private set; }
        public string PhoneNumber { get; private set; }
        public int LoginAttempts { get; private set; }
        public string FullName => $"{Name} {Surname}";
        public bool EmailConfirmed { get; private set; }
        public List<UserClaim> Claims { get; set; } = new();
        public List<TenantInformation> Tenants { get; set; } = new();

        public void SetEmail(string email)
        {
            FluentValueValidator<string>.Validate(email, x => x.NotEmpty().NotNull().EmailAddress());
            Email = email;
        }

        public void SetName(string name)
        {
            FluentValueValidator<string>.Validate(name, x => x.NotEmpty().NotNull());
            Name = name;
        }

        public void SetPhoneNumber(string phoneNumber)
        {
            FluentValueValidator<string>.Validate(phoneNumber, x => x.NotEmpty().NotNull());
            PhoneNumber = phoneNumber;
        }

        public void SetSurname(string surname)
        {
            FluentValueValidator<string>.Validate(surname, x => x.NotEmpty().NotNull());
            Surname = surname;
        }

        public void ConfirmEmail()
        {
            EmailConfirmed = true;
        }

        private void ValidatePassword(string password)
        {
            FluentValueValidator<string>.Validate(password, x => x.Password());
        }

        private bool ComparePassword(string password)
        {
            var testHash = HashHelper.HashPassword(password, Convert.FromBase64String(Salt));

            if (testHash == Password) return true;

            return false;
        }

        public void UpdatePassword(string old, string updated)
        {
            var isMatch = ComparePassword(old);
            if (!isMatch) throw new Exception("Password does not match stored password");
            ValidatePassword(updated);
            SetPassword(updated);
        }

        public void SetPassword(string password)
        {
            ValidatePassword(password);
            var salt = HashHelper.CreateSalt();
            Password = HashHelper.HashPassword(password, salt);
            Salt = Convert.ToBase64String(salt);
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

                var canUnlock = DateLocked != null && DateTime.Now > DateLocked.Value.AddMinutes(5);

                if (canUnlock)
                {
                    Locked = false;
                    LoginAttempts = 0;
                }

                if (canUnlock == false)
                    throw new Exception("User has exceeded login attempts, please try again in 5 minutes");
            }

            ValidatePassword(password);

            if (ComparePassword(password))
            {
                LoginAttempts = 0;
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