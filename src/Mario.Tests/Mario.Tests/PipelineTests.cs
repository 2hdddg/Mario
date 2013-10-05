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
        private class Processor<TInput, TOutput>
        {
            private readonly Func<TInput, TOutput> _convert;

            public Processor(Func<TInput, TOutput> convert)
            {
                _convert = convert;
            }

            public IEnumerable<IStepIo<TInput, TOutput>> Process(IEnumerable<IStepIo<TInput, TOutput>> workitems)
            {
                return workitems.Select(i => i.Output(_convert(i.Input)));
            }
        }

        private static class FromHere
        {
            public static IEnumerable<int> ToEternity()
            {
                var i = 0;
                while (true)
                {
                    yield return i++;
                }
            }
        }

        [Test]
        public void Should_throw_if_first_step_requests_input_of_other_type_than_seed()
        {
            var pipeline = new Pipeline<int>();
            var processor = new Processor<string, string>(s => s);

            Assert.Throws<Exception>(() => pipeline.Step<string, string>(processor.Process));
        }

        [Test]
        public void Can_get_result_from_pipeline_with_one_step()
        {
            var pipeline = new Pipeline<int>();
            var processor = new Processor<int, string>(n => n.ToString(CultureInfo.InvariantCulture));
            pipeline.Step<int, string>(processor.Process);

            var processed = pipeline.GetResult<string>(new[] { 7 });

            Assert.That(processed.Single(), Is.EqualTo("7"));
        }

        [Test]
        public void Can_get_result_from_pipeline_with_two_steps()
        {
            var pipeline = new Pipeline<int>();
            var processor1 = new Processor<int, string>(n => n.ToString(CultureInfo.InvariantCulture));
            var processor2 = new Processor<string, int>(int.Parse);
            pipeline.Step<int, string>(processor1.Process);
            pipeline.Step<string, int>(processor2.Process);

            var processed = pipeline.GetResult<int>(new[] { 7 });

            Assert.That(processed.Single(), Is.EqualTo(7));
        }

        [Test]
        public void It_does_yield()
        {
            var pipeline = new Pipeline<int>();
            var processor1 = new Processor<int, string>(n => n.ToString(CultureInfo.InvariantCulture));
            var processor2 = new Processor<string, int>(int.Parse);
            pipeline.Step<int, string>(processor1.Process);
            pipeline.Step<string, int>(processor2.Process);

            var processed = pipeline.GetResult<int>(FromHere.ToEternity());

            // If someones not yielding we will hang here forever
            Assert.That(processed.Take(5).ToArray(), Is.EqualTo(new[] { 0, 1, 2, 3, 4 }));
        }
    }
}