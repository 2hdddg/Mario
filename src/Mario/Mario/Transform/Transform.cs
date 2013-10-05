using System.Collections.Generic;

namespace Mario.Transform
{
    internal class Transform<TNextInput, TNextOutput, TCurrentInput, TCurrentOutput>
    {
        public IEnumerable<StepIo<TNextInput, TNextOutput>> Do(IEnumerable<IStepIo<TCurrentInput, TCurrentOutput>> inputs)
        {
            foreach (var input in inputs)
            {
                var workitem = ((IGetWorkitem)input).GetWorkitem();
                var nextStepData = new StepIo<TNextInput, TNextOutput>(workitem);
                yield return nextStepData;
            }
        }
    }
}