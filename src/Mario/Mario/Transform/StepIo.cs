namespace Mario.Transform
{
    internal class StepIo<TInput, TOutput> : IStepIo<TInput, TOutput>, IGetWorkitem
    {
        private readonly Workitem _workitem;

        public StepIo(Workitem workitem)
        {
            _workitem = workitem;
        }

        public Workitem GetWorkitem()
        {
            return _workitem;
        }

        public TInput Input
        {
            get { return _workitem.Require<TInput>(); }
        }

        public IStepIo<TInput, TOutput> Output(TOutput output)
        {
            _workitem.Submit(output);
            return this;
        }
    }
}