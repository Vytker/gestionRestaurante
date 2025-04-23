using Moq;
using Reservas.Application.Dtos;
using Reservas.Application.Services;
using Reservas.Domain.Entities;

namespace Reservas.Tests.Services
{
    public class ReservaServiceTests
    {
        private readonly Mock<IReservaRepository> _mockReservaRepository;
        
        public ReservaServiceTests()
        {
            _mockReservaRepository = new Mock<IReservaRepository>();
          
        }

        [Fact]
        public void ActualizarEstadoReserva_ReservaExistente_ActualizaEstadoCorrectamente()
        {
            // Arrange
            var reservaId = Guid.NewGuid();
            var reserva = new Reserva
            {
                Id = reservaId,
                NombreCliente = "Juan Pérez",
                NumeroComensales = 4,
                FechaReserva = DateTime.UtcNow,
                Estado = Reserva.EstadoReserva.Cancelada,
                Notas = "Reserva cancelada"
            };

            // Simula que el repositorio devuelve la reserva existente
            _mockReservaRepository.Setup(r => r.ObtenerPorId(reservaId)).Returns(reserva);

            // Act
            

            // Assert
            Assert.Equal(Reserva.EstadoReserva.Confirmada, reserva.Estado);
            _mockReservaRepository.Verify(r => r.Actualizar(reserva), Times.Once);
        }

        [Fact]
        public async Task CrearReservaAsync_CuandoTurnoTieneCapacidad_DeberiaCrearReserva()
        {
            // Arrange
            var mockRepo = new Mock<IReservaRepository>();
            var turno = new Turno
            {
                Id = 1,
                Nombre = "Turno Mañana", // Se establece el miembro requerido 'Nombre'
                HoraInicio = new TimeSpan(8, 0, 0), // Se establece el miembro requerido 'HoraInicio'
                HoraFin = new TimeSpan(12, 0, 0), // Se establece el miembro requerido 'HoraFin'
                Capacidad = 10,
                Reservas = new List<Reserva>
                {
                    new Reserva {NombreCliente = "Manolo", FechaReserva = DateTime.Today, NumeroComensales = 3 }
                }
            };
            mockRepo.Setup(r => r.ObtenerTurnoConReservasPorIdAsync(1)).ReturnsAsync(turno);

            

            var reservaDto = new ReservaCreateDto
            {
                NombreCliente = "Juan",
                FechaReserva = DateTime.Today,
                NumeroComensales = 4,
                Notas = "Mesa al fondo",
                TurnoId = 1
            };

            // Act
            

            // Assert
            
            mockRepo.Verify(r => r.Crear(It.IsAny<Reserva>()), Times.Once);
            mockRepo.Verify(r => r.GuardarCambiosAsync(), Times.Once);
        }
        [Fact]
        public async Task CrearReservaAsync_CuandoNoHayCapacidad_DeberiaRetornarError()
        {
            // Arrange
            var mockRepo = new Mock<IReservaRepository>();
            var turno = new Turno
            {
                Nombre = "Turno Tarde", // Se establece el miembro requerido 'Nombre'
                HoraInicio = new TimeSpan(14, 0, 0), // Se establece el miembro requerido 'HoraInicio'
                HoraFin = new TimeSpan(18, 0, 0), // Se establece el miembro requerido 'HoraFin'
                Id = 1,
                Capacidad = 5,
                Reservas = new List<Reserva>
        {
            new Reserva {NombreCliente = "Manolo",  FechaReserva = DateTime.Today, NumeroComensales = 3 },
            new Reserva {NombreCliente = "Pepe",  FechaReserva = DateTime.Today, NumeroComensales = 2 }
        }
            };
            mockRepo.Setup(r => r.ObtenerTurnoConReservasPorIdAsync(1)).ReturnsAsync(turno);

            

            var reservaDto = new ReservaCreateDto
            {
                NombreCliente = "Luis",
                FechaReserva = DateTime.Today,
                NumeroComensales = 1, // No cabe, ya hay 5/5
                Notas = "Sin alergias",
                TurnoId = 1
            };

            // Act
            

            // Assert
      
            mockRepo.Verify(r => r.Crear(It.IsAny<Reserva>()), Times.Never);
            mockRepo.Verify(r => r.GuardarCambiosAsync(), Times.Never);
        }



    }
}
