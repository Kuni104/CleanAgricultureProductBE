namespace CleanAgricultureProductBE.DTOs.Complaint
{
    public class UpdateComplaintStatusDto
    {
        public string Status { get; set; } = string.Empty; // Resolved | Rejected
        public string? Resolution { get; set; } // Exchange | Refund (required when Resolved)
    }
}
