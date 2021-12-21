using System;
using MediatR;

namespace Zonorai.Tenants.Application.Users.Commands.ResetPassword;

public record PasswordResetEvent(string UserId, DateTime DateReset) : INotification;