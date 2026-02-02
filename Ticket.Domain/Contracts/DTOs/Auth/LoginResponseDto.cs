using System;

namespace Ticket.Domain.Contracts.DTOs.Auth
{
    public record LoginResponseDto(
        string AccessToken,
        DateTime ExpiresAtUtc,
        string TokenType,
        string? RefreshToken
    );
}
