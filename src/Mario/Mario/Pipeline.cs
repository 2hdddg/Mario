using System;
using System.Collections.Generic;
using System.Linq;
using Mario.Transform;

namespace Mario
{
    public class Pipeline<TSeed>
    {
        private readonly IEnumerable<TSeed> _inputs;

        private struct StepTransform
        {
            public Type Input { get; set; }
            public Type Output { get; set; }
            public object Enumerable { get; set; }
        }

        private readonly IList<StepTransform> _stepTransforms;

        public Pipeline(IEnumerable<TSeed> inputs)
        {
            _inputs = inputs;
            _stepTransforms = new List<StepTransform>();
        }

        public void Step<TInput, TOutput>(IStep<TInput, TOutput> step)
        {
            var nextStep = new StepTransform
            {
                Input = typeof(TInput),
                Output = typeof(TOutput)
            };

            // Check that requested input is outputted from prior steps in chain
            if (nextStep.Input != typeof(TSeed) &&
                _stepTransforms.All(s => s.Output != nextStep.Input))
            {
                throw new Exception("Requested input cannot be satisfied!");
            }

            var stepMethod = step.GetType().GetMethod("Process");

            if (_stepTransforms.Count == 0)
            {
                var rootTransformType = typeof(RootTransform<,>).MakeGenericType(new[] { typeof(TSeed), nextStep.Output });
                var rootTransform = rootTransformType.GetConstructors().First().Invoke(new object[0]);
                var transformMethod = rootTransformType.GetMethod("Do");
                var transformOutput = transformMethod.Invoke(rootTransform, new object[] { _inputs });
                var stepOutput = stepMethod.Invoke(step, new[] { transformOutput });
                nextStep.Enumerable = stepOutput;
            }
            else
            {
                var previousStep = _stepTransforms[_stepTransforms.Count - 1];
                var transformType = typeof(Transform<,,,>).MakeGenericType(new[] { nextStep.Input, nextStep.Output, previousStep.Input, previousStep.Output });
                var tranform = transformType.GetConstructors().First().Invoke(new object[0]);
                var transformMethod = transformType.GetMethod("Do");
                var transformOutput = transformMethod.Invoke(tranform, new[] { previousStep.Enumerable });
                var stepOutput = stepMethod.Invoke(step, new[] { transformOutput });
                nextStep.Enumerable = stepOutput;
            }

            _stepTransforms.Add(nextStep);
        }

        public object Execute()
        {
            var lastStep = _stepTransforms[_stepTransforms.Count - 1];
            return lastStep.Enumerable;
        }
    }
}
