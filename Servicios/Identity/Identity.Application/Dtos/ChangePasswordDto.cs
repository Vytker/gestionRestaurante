// Identity.Application/Dtos/ChangePasswordDto.cs
using System.ComponentModel.DataAnnotations;

namespace Identity.Application.Dtos
{
    public record ChangePasswordDto
    {
        [Required] public string PasswordActual { get; init; } = null!;

        [Required, MinLength(6)]
        public string PasswordNueva { get; init; } = null!;

        // Campo de confirmación
        [Required, Compare(nameof(PasswordNueva),
                 ErrorMessage = "La confirmación no coincide.")]
        public string PasswordNuevaConfirm { get; init; } = null!;
    }
}
