using CleanAgricultureProductBE.DTOs.Response;

namespace CleanAgricultureProductBE.DTOs.ApiResponse
{
    public class ResponseDtoWithPagination<T>
    {
        public T? ResultObject { get; set; }
        public Pagination? Pagination { get; set; }
    }
}
