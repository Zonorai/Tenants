using System;
using MediatR;

namespace Zonorai.Tenants.Application.Claims.Commands.Delete;

public record ClaimDeletedEvent(string ClaimId,DateTime DateDeleted) : INotification;