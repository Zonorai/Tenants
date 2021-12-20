using System;
using MediatR;

namespace Zonorai.Tenants.Application.Users.Commands.Delete;

public record UserDeletedEvent(string UserId,string TenantId,DateTime DateRemoved) : INotification;