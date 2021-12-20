using System;
using MediatR;

namespace Zonorai.Tenants.Application.UserClaims.Commands.Add;

public record ClaimAddedToUserEvent(string ClaimId,string UserId,DateTime DateAdded) : INotification;