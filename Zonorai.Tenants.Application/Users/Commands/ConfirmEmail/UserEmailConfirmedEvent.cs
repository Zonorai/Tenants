using System;
using MediatR;

namespace Zonorai.Tenants.Application.Users.Commands.ConfirmEmail;

public record UserEmailConfirmedEvent(string UserId,DateTime DateConfirmed) : INotification;