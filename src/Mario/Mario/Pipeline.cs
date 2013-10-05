using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mario.Transform;

namespace Mario
{
    public class Pipeline<TSeed>
    {
        private struct StepTransform
        {
            public Type Input { get; set; }
            public Type Output { get; set; }
            public MethodInfo ProcessorMethod { get; set; }
            public object ProcessorTarget { get; set; }
        }

        private readonly IList<StepTransform> _stepTransforms;

        public Pipeline()
        {
            _stepTransforms = new List<StepTransform>();
        }

        private object Root(IEnumerable<TSeed> inputs)
        {
            var first = _stepTransforms[0];

            var rootTransformType = typeof(RootTransform<,>).MakeGenericType(new[] { typeof(TSeed), first.Output });
            var rootTransform = rootTransformType.GetConstructors().First().Invoke(new object[0]);
            var transformMethod = rootTransformType.GetMethod("Do");
            var transformOutput = transformMethod.Invoke(rootTransform, new object[] { inputs });
            var stepOutput = first.ProcessorMethod.Invoke(first.ProcessorTarget, new[] { transformOutput });

            return stepOutput;
        }

        private object Next(StepTransform current, object inputs)
        {
            var previous = _stepTransforms[_stepTransforms.Count - 1];
            var transformType = typeof(Transform<,,,>).MakeGenericType(new[] { current.Input, current.Output, previous.Input, previous.Output });
            var tranform = transformType.GetConstructors().First().Invoke(new object[0]);
            var transformMethod = transformType.GetMethod("Do");
            var transformOutput = transformMethod.Invoke(tranform, new[] { inputs });
            var stepOutput = current.ProcessorMethod.Invoke(current.ProcessorTarget, new[] { transformOutput });

            return stepOutput;
        }

        private object Build(IEnumerable<TSeed> inputs)
        {
            if (_stepTransforms.Count == 0) throw new Exception("Nothing to do");

            var output = Root(inputs);
            foreach (var current in _stepTransforms.Skip(1))
            {
                output = Next(current, output);
            }
            return output;
        }

        public void Step<TInput, TOutput>(ProcessorDelegate<TInput, TOutput> processor)
        {
            var input = typeof(TInput);
            var output = typeof(TOutput);

            // Check that requested input is outputted from prior steps in chain
            if (input != typeof(TSeed) &&
                _stepTransforms.All(s => s.Output != input))
            {
                throw new Exception("Requested input cannot be satisfied!");
            }

            var nextStep = new StepTransform
                {
                    Input = input,
                    Output = output,
                    ProcessorMethod = processor.Method,
                    ProcessorTarget = processor.Target
                };

            _stepTransforms.Add(nextStep);
        }

        public object Execute(IEnumerable<TSeed> inputs)
        {
            return Build(inputs);
        }
    }
}
