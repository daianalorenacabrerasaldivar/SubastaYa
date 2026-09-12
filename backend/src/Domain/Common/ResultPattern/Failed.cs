namespace Domain.Common.ResultPattern
{
    public class Failed<TResult> : Result<TResult>
    {
        private readonly TResult _value;

        private readonly string _info;
        private readonly DataStatus _dataStatus;

        public override TResult Value => _value;

        protected override DataStatus DataStatus => _dataStatus;

        public override string Info => _info;

        public Failed(string info, DataStatus resultStatus = DataStatus.Failed)
        {
            _info = info;
            _dataStatus = resultStatus;
            _value = default;
        }
    }
}
