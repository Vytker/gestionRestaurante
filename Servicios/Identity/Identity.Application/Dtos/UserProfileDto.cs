public record UserProfileDto   // lo que devuelves a la UI
(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Telefono
);