namespace Reservas.Application.Dtos
{
    public record HourlySeriesDto
    {
        public int Hour { get; init; }
        public int Total { get; init; }

        public HourlySeriesDto(int hour, int total)
        {
            Hour = hour;
            Total = total;
        }
    }
}
