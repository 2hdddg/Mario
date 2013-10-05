using System.Collections.Generic;

namespace Mario.Transform
{
    internal class LastTransform<TPreviousInput, TPreviousOutput, TRequestedOutput>
    {
        public IEnumerable<TRequestedOutput> Do(IEnumerable<IStepIo<TPreviousInput, TPreviousOutput>> inputs)
        {
            foreach (var input in inputs)
            {
                var workitem = ((IGetWorkitem)input).GetWorkitem();
                yield return workitem.Require<TRequestedOutput>();
            }
        }
    }
}