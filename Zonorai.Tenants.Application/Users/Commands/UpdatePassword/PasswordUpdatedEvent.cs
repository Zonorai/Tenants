using System;
using MediatR;

namespace Zonorai.Tenants.Application.Users.Commands.UpdatePassword;

public record PasswordUpdatedEvent(string UserId,DateTime DateUpdated) : INotification;