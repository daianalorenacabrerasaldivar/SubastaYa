namespace Domain.Common.ResultPattern
{
    public abstract class Result<TResult>
    {
        public abstract TResult Value { get; }

        protected abstract DataStatus DataStatus { get; }
        public abstract string Info { get; }

        public bool IsSuccess => DataStatus == DataStatus.Success;

        public DataStatus Status => DataStatus;

        public static implicit operator TResult(Result<TResult> @this)
        {
            return @this.Value;
        }

    }
    public enum DataStatus
    {
        Failed,
        Success,
        NullOrEmpty,
        NotFound,
        Conflict,
        Exception,
        RequestValidation
    }

}
