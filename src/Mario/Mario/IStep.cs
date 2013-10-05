using System.Collections.Generic;

namespace Mario
{
    public delegate IEnumerable<IStepIo<TInput, TOutput>> ProcessorDelegate<TInput, TOutput>(IEnumerable<IStepIo<TInput, TOutput>> workitems);
}