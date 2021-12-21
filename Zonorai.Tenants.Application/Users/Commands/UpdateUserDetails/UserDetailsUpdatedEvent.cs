using System;
using MediatR;

namespace Zonorai.Tenants.Application.Users.Commands.UpdateUserDetails;

internal record UserDetailsUpdatedEvent(string Id,
    string Email,
    string Name,
    string Surname,
    string PhoneNumber, DateTime DateUpdated) : INotification;