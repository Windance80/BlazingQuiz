using System;
using System.Text.Json.Serialization;

namespace BlazingQuiz.Shared.DTOs;

public record AuthResponseDto(string Token, string? ErrorMessage = null)
{
    [JsonIgnore]
    public bool HasError => ErrorMessage != null;
}
