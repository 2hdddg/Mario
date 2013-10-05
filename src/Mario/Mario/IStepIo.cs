namespace Mario
{
    /// <summary>
    /// Used as parameter to processors.
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput">Should be called to persist the result</typeparam>
    public interface IStepIo<TInput, TOutput>
    {
        TInput Input { get; }
        IStepIo<TInput, TOutput> Output(TOutput output);
    }
}