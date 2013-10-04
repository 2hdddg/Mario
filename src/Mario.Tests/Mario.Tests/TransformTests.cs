using System.Linq;
using Mario.Transform;
using NUnit.Framework;

namespace Mario.Tests
{
    [TestFixture]
    public class TransformTests
    {
        [Test]
        public void It_can_transform_to_enumerable_inputs()
        {
            var transform = new Transform<string, string, int, int>();
            var workitem = new Workitem(1);
            workitem.Submit("a string");
            var stepIos = new [] {new StepIo<int, int>(workitem)};

            var transformed = transform.Do(stepIos).Single();

            Assert.That(transformed.Input, Is.EqualTo("a string"));
        }
    }
}