namespace Runner
{
    public class StateString : StateMessage<State, string>
    {
        public static StateString OK = new StateString(Runner.State.Success, null);

        public StateString(State state, string value)
            : base(state, value)
        {
        }

        public static StateString Ok(string value)
        {
            return new StateString(Runner.State.Success, value);
        }

        public static StateString Error(string value)
        {
            return new StateString(Runner.State.Fail, value);
        }
    }
}