namespace CleanAgricultureProductBE.DTOs.ApiResponse
{
    public class AssignOrdersRequestDto
    {
        public Guid ScheduleId { get; set; }
        public List<Guid> OrderIds { get; set; } = new();
    }
}
