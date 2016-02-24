namespace MiniAbp
{
    public class AjaxResult
    {
        public bool IsSuccess { get; set; }
        public Errors Errors { get; set; }
        public object Result { get; set; }
    }

    public class Errors
    {
        public bool IsFriendlyError { get; set; }
        public string Message { get; set; }
        public string CallStack { get; set; }
    }
}
