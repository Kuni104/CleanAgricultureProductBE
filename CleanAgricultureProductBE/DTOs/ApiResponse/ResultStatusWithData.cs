namespace CleanAgricultureProductBE.DTOs.ApiResponse
{
    public class ResultStatusWithData<T>
    {
        public string? Status { get; set; }
        public T? Data { get; set; }
    }
}
