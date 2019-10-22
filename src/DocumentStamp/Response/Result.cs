namespace DocumentStamp.Response
{
    public class Result<T>
    {
        public bool Success { set; get; }
        public T Value { set; get; }

        public Result(bool success, T value)
        {
            Success = success;
            Value = value;
        }
    }
}