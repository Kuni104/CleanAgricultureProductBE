namespace CleanAgricultureProductBE.DTOs
{
    public class ResultStatusWithData<T>
    {
        public string? Status { get; set; }
        public T? Data { get; set; }
    }
}
