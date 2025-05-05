
using Moq;
using Reservas.Application.Dtos;
using Reservas.Application.Interfaces;
using Reservas.Application.Services;
using Reservas.Domain.Entities;
using Xunit;
using FluentAssertions;

namespace Reservas.Tests.Services
{
    public class ReservaServiceTests
    {
        private readonly Mock<IReservaRepository> _repo;
        private readonly Mock<INotificationService> _notifier;
        private readonly ReservaService _svc;
        private readonly Guid _restId = Guid.NewGuid();

        public ReservaServiceTests()
        {
            _repo = new Mock<IReservaRepository>();
            _notifier = new Mock<INotificationService>();
            _svc = new ReservaService(_repo.Object, _notifier.Object);
        }

        [Fact]
        public async Task CrearReservaAsync_TurnoNoExiste_RetornaFalseConError()
        {
            // Arrange
            var dto = new ReservaCreateDto
            {
                NombreCliente = "Ana",
                FechaReserva = DateTime.UtcNow.AddDays(1),
                NumeroComensales = 2,
                TurnoId = 42
            };
            _repo
                .Setup(r => r.ObtenerTurnoConReservasPorIdAsync(42, _restId))
                .ReturnsAsync((Turno)null);

            // Act
            var (ok, err, code) = await _svc.CrearReservaAsync(_restId, dto);

            // Assert
            ok.Should().BeFalse();
            err.Should().Be("Turno no encontrado.");
            code.Should().BeEmpty();
        }

        [Fact]
        public async Task CrearReservaAsync_SinCapacidad_RetornaFalseConError()
        {
            // Arrange
            var turno = new Turno
            {
                RestauranteId = _restId,
                Nombre = "Turno de la tarde",
                HoraInicio = new TimeSpan(18, 0, 0),
                HoraFin = new TimeSpan(22, 0, 0),
                Id = 1,
                Capacidad = 4,
                Reservas = new List<Reserva> {
                  new Reserva {NombreCliente = "Prueba1", FechaReserva = DateTime.UtcNow.Date.AddDays(1), NumeroComensales = 3 }
                }
            };
            _repo
                .Setup(r => r.ObtenerTurnoConReservasPorIdAsync(1, _restId))
                .ReturnsAsync(turno);

            var dto = new ReservaCreateDto
            {
                NombreCliente = "Ana",
                FechaReserva = DateTime.UtcNow.Date.AddDays(1),
                NumeroComensales = 2,
                TurnoId = 1
            };

            // Act
            var (ok, err, code) = await _svc.CrearReservaAsync(turno.RestauranteId, dto);

            // Assert
            ok.Should().BeFalse();
            err.Should().Contain("No hay suficiente capacidad");
            code.Should().BeEmpty();
        }

        [Fact]
        public async Task CrearReservaAsync_Valido_RetornaTrueYGeneraCodigo()
        {
            // Arrange
            var turno = new Turno {Nombre = "Prueba1", HoraInicio = new TimeSpan(12,00,00), HoraFin = new TimeSpan(16,00,00), Id = 1, Capacidad = 10, Reservas = new List<Reserva>() };
            _repo.Setup(r => r.ObtenerTurnoConReservasPorIdAsync(1, _restId))
                 .ReturnsAsync(turno);
            _repo.Setup(r => r.ExistePorCode(It.IsAny<string>(), _restId))
                 .Returns(false);

            var dto = new ReservaCreateDto
            {
                NombreCliente = "Ana",
                FechaReserva = DateTime.UtcNow.AddDays(1),
                NumeroComensales = 2,
                TurnoId = 1
            };

            // Act
            var (ok, err, code) = await _svc.CrearReservaAsync(_restId, dto);

            // Assert
            ok.Should().BeTrue();
            err.Should().BeNull();
            code.Length.Should().Be(8);
            _repo.Verify(r => r.Crear(It.IsAny<Reserva>()), Times.Once);
            _notifier.Verify(n => n.NotifyReservationCreatedAsync(It.IsAny<Reserva>()), Times.Once);
        }

        [Fact]
        public void ActualizarEstadoReserva_Inexistente_LanzaKeyNotFound()
        {
            // Arrange
            _repo.Setup(r => r.ObtenerPorId(It.IsAny<Guid>(), _restId))
                 .Returns((Reserva)null);

            // Act & Assert
            Action act = () => _svc.ActualizarEstadoReserva(Guid.NewGuid(), _restId, Reserva.EstadoReserva.Confirmada);
            act.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public void ActualizarEstadoReserva_Existente_CambiaEstado()
        {
            // Arrange
            var res = new Reserva {NombreCliente = "Prueba1", FechaReserva = DateTime.Now, NumeroComensales = 3 ,Id = Guid.NewGuid(), Estado = Reserva.EstadoReserva.Pendiente };
            _repo.Setup(r => r.ObtenerPorId(res.Id, _restId)).Returns(res);

            // Act
            _svc.ActualizarEstadoReserva(res.Id, _restId, Reserva.EstadoReserva.Confirmada);

            // Assert
            res.Estado.Should().Be(Reserva.EstadoReserva.Confirmada);
            _repo.Verify(r => r.Actualizar(res), Times.Once);
        }
    }
}
