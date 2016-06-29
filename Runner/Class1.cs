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

        public TState State { get; private set; }
        public TValue Value { get; private set; }

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


    public enum OkError
    {
        Ok,
        Error,
    }

    public class OkErrorString : StateMessage<OkError, string>
    {
        public OkErrorString(OkError state, string value) : base(state, value)
        {
        }

        public static OkErrorString OK = new OkErrorString(OkError.Ok, null);

        public static OkErrorString Ok(string value)
        {
            return new OkErrorString(OkError.Ok, value);
        }

        public static OkErrorString Error(string value)
        {
            return new OkErrorString(OkError.Error, value);
        }
    }


}