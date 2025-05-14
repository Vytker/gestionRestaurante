public record TurnoDto(
    int Id,
    string Nombre,
    TimeSpan HoraInicio,
    TimeSpan HoraFin,
    int Capacidad,
    bool Eliminado
);
