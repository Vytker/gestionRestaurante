using Moq;
using Reservas.Application.Services;
using Reservas.Domain.Entities;

namespace Reservas.Tests.Services
{
    public class ReservaServiceTests
    {
        private readonly Mock<IReservaRepository> _mockReservaRepository;
        private readonly ReservaService _reservaService;
        public ReservaServiceTests()
        {
            _mockReservaRepository = new Mock<IReservaRepository>();
            _reservaService = new ReservaService(_mockReservaRepository.Object);
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
            _reservaService.ActualizarEstadoReserva(reservaId, Reserva.EstadoReserva.Confirmada);

            // Assert
            Assert.Equal(Reserva.EstadoReserva.Confirmada, reserva.Estado);
            _mockReservaRepository.Verify(r => r.Actualizar(reserva), Times.Once);
        }
        [Fact]
        public void CrearReserva_DeberiaLlamarAlRepositorioCrear()
        {
            // Arrange
            var mockRepo = new Mock<IReservaRepository>();
            var service = new ReservaService(mockRepo.Object);
            var nuevaReserva = new Reserva
            {
                Id = Guid.NewGuid(),
                NombreCliente = "Juan Pérez",
                NumeroComensales= 4,
                FechaReserva = DateTime.UtcNow,
                Estado = Reserva.EstadoReserva.Pendiente
            };

            // Act
            service.CrearReserva(nuevaReserva);

            // Assert
            mockRepo.Verify(r => r.Crear(It.Is<Reserva>(r => r.NombreCliente == "Juan Pérez")), Times.Once);
        }
    }
}
