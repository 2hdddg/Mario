using System.Linq;
using Mario.Transform;
using NUnit.Framework;

namespace Mario.Tests
{
    [TestFixture]
    public class FirstTransformTests
    {
        [Test]
        public void It_can_transform_to_enumerable_inputs()
        {
            var inputs = new[] { 1, 2, 3 };
            var transform = new FirstTransform<int, string>();

            var transformed = transform.Do(inputs).ToArray();

            Assert.That(transformed[2].Input, Is.EqualTo(3));
        }
    }
}