namespace Runner
{
    public class RunState : StateBase<State, string>
    {
        public static RunState OK = new RunState(Runner.State.Success, null);

        public RunState(State state, string value)
            : base(state, value)
        {
        }

        public static RunState Ok(string value)
        {
            return new RunState(Runner.State.Success, value);
        }

        public static RunState Fail(string value)
        {
            return new RunState(Runner.State.Fail, value);
        }

        public static RunState ParseError(string value)
        {
            return new RunState(Runner.State.ParseError, value);
        }
    }
}