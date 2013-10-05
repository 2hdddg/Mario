using System.Collections.Generic;

namespace Mario.Transform
{
    internal class FirstTransform<TInput, TOutput>
    {
        public IEnumerable<StepIo<TInput, TOutput>> Do(IEnumerable<TInput> inputs)
        {
            foreach (var input in inputs)
            {
                var workitem = new Workitem(input);
                var stepData = new StepIo<TInput, TOutput>(workitem);
                yield return stepData;
            }
        }
    }
}