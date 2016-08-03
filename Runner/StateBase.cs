namespace Runner
{
    using System.Runtime.CompilerServices;

    public abstract class StateBase<TState, TValue>

    {
        protected StateBase(TState state, string value)
        {
            this.State = state;
            this.Message = value;
        }

        public TState State { get; }

        public TValue Value { get; set; }

        public string Message { get; }

        public static bool operator ==(StateBase<TState, TValue> a, StateBase<TState, TValue> b)
        {
            return a != null && b != null && a.State != null && a.State.Equals(b.State);
        }

        public static bool operator !=(StateBase<TState, TValue> a, StateBase<TState, TValue> b)
        {
            return !(a != null && b != null && a.State != null && a.State.Equals(b.State));
        }

        public override bool Equals(object obj)
        {
            if (this.State != null)
            {
                var other = obj as StateBase<TState, TValue>;
                if (other != null) return this.State.Equals(other.State);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this.State);
        }

        public override string ToString()
        {
            return this.Message ?? this.Message;
        }
    }
}