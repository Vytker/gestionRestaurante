using MediatR;
using System.Collections.Generic;
using Turnos.Application.Dtos;

public class GetAllSlotsQuery : IRequest<IEnumerable<SlotDto>> { }