using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mario.Transform;

namespace Mario
{
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

        private object First(IEnumerable<TSeed> inputs)
        {
            var first = _definitions.First();

            var transformType = typeof(FirstTransform<,>).MakeGenericType(new[] { typeof(TSeed), first.Output });
            var transform = transformType.GetConstructors().First().Invoke(new object[0]);
            var method = transformType.GetMethod("Do");
            var output = method.Invoke(transform, new object[] { inputs });
            var stepOutput = first.ProcessorMethod.Invoke(first.ProcessorTarget, new[] { output });

            return stepOutput;
        }

        private object Next(StepDefinition current, object inputs)
        {
            var previous = _definitions[_definitions.Count - 1];
            var transformType = typeof(Transform<,,,>).MakeGenericType(new[] { current.Input, current.Output, previous.Input, previous.Output });
            var tranform = transformType.GetConstructors().First().Invoke(new object[0]);
            var transformMethod = transformType.GetMethod("Do");
            var transformOutput = transformMethod.Invoke(tranform, new[] { inputs });
            var stepOutput = current.ProcessorMethod.Invoke(current.ProcessorTarget, new[] { transformOutput });

            return stepOutput;
        }

        private IEnumerable<TOutput> Last<TOutput>(object inputs)
        {
            var last = _definitions.Last();

            var transformType = typeof(LastTransform<,,>).MakeGenericType(new[] { last.Input, last.Output, typeof(TOutput) });
            var transform = transformType.GetConstructors().First().Invoke(new object[0]);
            var method = transformType.GetMethod("Do");
            var output = method.Invoke(transform, new [] { inputs });

            return (IEnumerable<TOutput>)output;
        }

        private object Build(IEnumerable<TSeed> inputs)
        {
            if (_definitions.Count == 0) throw new Exception("Nothing to do");

            var output = First(inputs);
            foreach (var current in _definitions.Skip(1))
            {
                output = Next(current, output);
            }
            return output;
        }

        public void Process<TInput, TOutput>(ProcessorDelegate<TInput, TOutput> processor)
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
            return Last<TOutput>(Build(inputs));
        }
    }
}
