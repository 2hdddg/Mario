namespace Mario
{
    public interface IStepIo<TInput, TOutput>
    {
        TInput Input { get; }
        IStepIo<TInput, TOutput> Output(TOutput output);
    }
}