using Moq;
using Reservas.Domain.Entities;
using Reservas.Application.Dtos;
using Reservas.Application.Services;
using Reservas.Application.Interfaces;
public class TurnoServiceTests
{
    [Fact]
    public async Task CrearAsync_CreaUnTurnoValido()
    {
        // Arrange
        var mockRepo = new Mock<ITurnoRepository>();
        var service = new TurnoService(mockRepo.Object);
        var nuevoTurno = new TurnoCreateDto
        {
            Nombre = "Turno Cena",
            HoraInicio = new TimeSpan(20, 0, 0),
            HoraFin = new TimeSpan(22, 0, 0),
            Capacidad = 30
        };

        // Act
        await service.CrearTurnoAsync(nuevoTurno);

        // Assert
        mockRepo.Verify(r => r.Crear(It.Is<Turno>(t =>
               t.Nombre == nuevoTurno.Nombre &&
               t.HoraInicio == nuevoTurno.HoraInicio &&
               t.HoraFin == nuevoTurno.HoraFin &&
               t.Capacidad == nuevoTurno.Capacidad &&
               t.Eliminado == false
           )), Times.Once);
        
        mockRepo.Verify(r => r.GuardarCambiosAsync(), Times.Once);
    }
}
