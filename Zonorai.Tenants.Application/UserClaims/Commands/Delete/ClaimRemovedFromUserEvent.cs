using System;
using MediatR;

namespace Zonorai.Tenants.Application.UserClaims.Commands.Delete;

public record ClaimRemovedFromUserEvent(string ClaimId, string UserId, DateTime DateRemoved) : INotification;