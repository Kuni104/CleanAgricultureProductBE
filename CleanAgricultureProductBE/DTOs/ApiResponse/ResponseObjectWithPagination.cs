namespace CleanAgricultureProductBE.DTOs.Response
{
    public class ResponseObjectWithPagination<T>
    {
        public string? Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public Pagination? Pagination { get; set; }
        public ResponseObjectWithPagination()
        {
        }
    }
}
