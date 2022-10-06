namespace DS.Api.Base
{
    public class ResultWithException
    {
        public Exception ExceptionData { get; set; }
        public bool Success => ExceptionData == null;
    }
    public class ResultWithException<T>: ResultWithException
    {
        public T Data { get; set; }
    }
}
