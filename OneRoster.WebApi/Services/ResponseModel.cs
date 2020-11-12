namespace EdFi.OneRoster.WebApi.Services
{
    public class ResponseModel<T>
    {
        public int TotalCount { get; set; }
        public T Response { get; set; }
    }
}
