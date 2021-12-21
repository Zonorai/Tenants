using System;
using MediatR;

namespace Zonorai.Tenants.Application.Users.Commands.Register;

public record TenantRegisteredEvent(string UserId, string Email, string CompanyName, string Name, string Surname,
    string TenantId, DateTime DateRegistered) : INotification;