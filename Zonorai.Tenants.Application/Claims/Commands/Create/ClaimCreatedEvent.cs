using System;
using MediatR;

namespace Zonorai.Tenants.Application.Claims.Commands.Create;

public record ClaimCreatedEvent(string Id, string Value, string Type, DateTime DateCreated) : INotification;