using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;

namespace Mario.Tests
{
    [TestFixture]
    public class PipelineTests
    {
        private class Step<TInput, TOutput> : IStep<TInput, TOutput>
        {
            private readonly Func<TInput, TOutput> _convert;

            public Step(Func<TInput, TOutput> convert)
            {
                _convert = convert;
            }

            public IEnumerable<IStepIo<TInput, TOutput>> Process(IEnumerable<IStepIo<TInput, TOutput>> workitems)
            {
                return workitems.Select(i => i.Output(_convert(i.Input)));
            }
        }

        [Test]
        public void Should_throw_if_first_step_requests_input_of_other_type_than_seed()
        {
            var pipeline = new Pipeline<int>(new int[0]);
            var step = new Step<string, string>(s => s);

            Assert.Throws<Exception>(() => pipeline.Step(step));
        }

        [Test]
        public void Can_get_result_from_pipeline_with_one_step()
        {
            var pipeline = new Pipeline<int>(new [] {7});
            var step = new Step<int, string>(n => n.ToString(CultureInfo.InvariantCulture));
            pipeline.Step(step);

            var processed = (IEnumerable<IStepIo<int, string>>)pipeline.Execute();

            Assert.That(processed.Single().Input, Is.EqualTo(7));
        }
    }
}