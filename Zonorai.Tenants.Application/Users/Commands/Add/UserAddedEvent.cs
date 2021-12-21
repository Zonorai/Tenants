using System;
using MediatR;

namespace Zonorai.Tenants.Application.Users.Commands.Add;

public record UserAddedEvent(string UserId, string Email, string Name, string Surname, string TenantId,
    DateTime DateAdded) : INotification;