namespace Core.Models
{
    public class ResultViewModel<T> where T : class, new()
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; } = new T();
    }
}