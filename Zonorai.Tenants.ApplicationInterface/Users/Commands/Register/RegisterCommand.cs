using MediatR;

namespace Zonorai.Tenants.ApplicationInterface.Users.Commands.Register
{
    public class RegisterCommand : IRequest<RegisterResult>
    {
        public string Company { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}