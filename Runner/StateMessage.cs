using System.Runtime.CompilerServices;

namespace Runner
{
    public abstract class StateMessage<TState, TValue>

    {
        protected StateMessage(TState state, TValue value)
        {
            this.State = state;
            this.Value = value;
        }

        public TState State { get; }

        public TValue Value { get; }

        public static bool operator ==(StateMessage<TState, TValue> a, StateMessage<TState, TValue> b)
        {
            return a != null && b != null && a.State != null && a.State.Equals(b.State);
        }

        public static bool operator !=(StateMessage<TState, TValue> a, StateMessage<TState, TValue> b)
        {
            return !(a != null && b != null && a.State != null && a.State.Equals(b.State));
        }

        public override bool Equals(object obj)
        {
            if (this.State != null)
            {
                var other = obj as StateMessage<TState, TValue>;
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
            return Value as string ?? Value.ToString();
        }
    }
}