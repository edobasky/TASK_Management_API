namespace TaskManagement.API.Model
{
    public class GenericResponse<T> where T : class
    {
        public string ResponseCode { get; set; }
        public bool Successful { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

    }
}
