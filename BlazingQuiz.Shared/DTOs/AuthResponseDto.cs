using System;

namespace BlazingQuiz.Shared.DTOs;

public record AuthResponseDto(string Token, string? ErrorMessage = null)
{
    public bool HasError => ErrorMessage != null;
}
