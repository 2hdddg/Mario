using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mario.Transform;

namespace Mario
{
    public delegate IEnumerable<IStepIo<TInput, TOutput>> ProcessorDelegate<TInput, TOutput>(IEnumerable<IStepIo<TInput, TOutput>> workitems);

    public class Pipeline<TSeed>
    {
        private struct StepDefinition
        {
            public Type Input { get; set; }
            public Type Output { get; set; }
            public MethodInfo ProcessorMethod { get; set; }
            public object ProcessorTarget { get; set; }
        }

        private readonly IList<StepDefinition> _definitions;

        public Pipeline()
        {
            _definitions = new List<StepDefinition>();
        }

        private static object First(StepDefinition first, IEnumerable<TSeed> inputs)
        {
            var transformType = typeof(FirstTransform<,>).MakeGenericType(new[] { typeof(TSeed), first.Output });
            var transform = transformType.GetConstructors().First().Invoke(new object[0]);
            var method = transformType.GetMethod("Do");
            var output = method.Invoke(transform, new object[] { inputs });
            var stepOutput = first.ProcessorMethod.Invoke(first.ProcessorTarget, new[] { output });

            return stepOutput;
        }

        private static object Next(StepDefinition current, StepDefinition previous,  object inputs)
        {
            var transformType = typeof(Transform<,,,>).MakeGenericType(new[] { current.Input, current.Output, previous.Input, previous.Output });
            var tranform = transformType.GetConstructors().First().Invoke(new object[0]);
            var method = transformType.GetMethod("Do");
            var output = method.Invoke(tranform, new[] { inputs });
            var stepOutput = current.ProcessorMethod.Invoke(current.ProcessorTarget, new[] { output });

            return stepOutput;
        }

        private static IEnumerable<TOutput> Last<TOutput>(StepDefinition last, object inputs)
        {
            var transformType = typeof(LastTransform<,,>).MakeGenericType(new[] { last.Input, last.Output, typeof(TOutput) });
            var transform = transformType.GetConstructors().First().Invoke(new object[0]);
            var method = transformType.GetMethod("Do");
            var output = method.Invoke(transform, new [] { inputs });

            return (IEnumerable<TOutput>)output;
        }

        private object Build(IEnumerable<TSeed> inputs)
        {
            if (_definitions.Count == 0) throw new Exception("Nothing to do");

            var first = _definitions.First();
            var output = First(first, inputs);
            var previous = first;
            foreach (var current in _definitions.Skip(1))
            {
                output = Next(current, previous, output);
                previous = current;
            }
            return output;
        }

        public void Step<TInput, TOutput>(ProcessorDelegate<TInput, TOutput> processor)
        {
            var input = typeof(TInput);
            var output = typeof(TOutput);

            // Check that requested input is outputted from prior steps in chain
            if (input != typeof(TSeed) &&
                _definitions.All(s => s.Output != input))
            {
                throw new Exception("Requested input cannot be satisfied!");
            }

            var nextStep = new StepDefinition
                {
                    Input = input,
                    Output = output,
                    ProcessorMethod = processor.Method,
                    ProcessorTarget = processor.Target
                };

            _definitions.Add(nextStep);
        }

        public IEnumerable<TOutput> GetResult<TOutput>(IEnumerable<TSeed> inputs)
        {
            return Last<TOutput>(_definitions.Last(), Build(inputs));
        }
    }
}
