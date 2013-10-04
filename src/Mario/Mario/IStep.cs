using System.Collections.Generic;

namespace Mario
{
    public interface IStep<TInput, TOutput>
    {
        IEnumerable<IStepIo<TInput, TOutput>> Process(IEnumerable<IStepIo<TInput, TOutput>> workitems);
    }
}