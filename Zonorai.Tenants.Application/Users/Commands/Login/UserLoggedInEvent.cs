using System;
using MediatR;

namespace Zonorai.Tenants.Application.Users.Commands.Login;

public record UserLoggedInEvent(string Email, string TenantId,DateTime DateLoggedIn) : INotification;