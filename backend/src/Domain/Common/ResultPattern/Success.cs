namespace Domain.Common.ResultPattern
{
    public class Success<TResult> : Result<TResult>
    {
        private readonly TResult _value;

        public override TResult Value => _value;

        protected override DataStatus DataStatus => DataStatus.Ok;

        public override string Info => "Success";

        public Success(TResult value)
        {
            _value = value;
        }
    }
}
