namespace Options
    {
    internal abstract class Option<T> : OperationOption
        {
        public T Value { get; }
        protected Option(T value)
            {
            Value = value;
            }
        }
    }