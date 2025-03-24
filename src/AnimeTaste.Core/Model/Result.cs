namespace AnimeTaste.Core.Model
{
    public class Result<T>
    {
        public T? Data { get; set; }

        public string? Message { get; set; }

        public bool IsSuccess { get; set; }
    }
}
