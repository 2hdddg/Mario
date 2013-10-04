using System;
using Mario.Transform;
using NUnit.Framework;

namespace Mario.Tests
{
    [TestFixture]
    public class WorkitemTests
    {
        private class SubmitData
        {
            public string Data { get; set; }
        }

        [Test]
        public void Can_require_instance()
        {
            var workitem = new Workitem();
            workitem.Submit(new SubmitData { Data = "Keep it safe" });

            var required = workitem.Require<SubmitData>();

            Assert.That(required.Data, Is.EqualTo("Keep it safe"));
        }

        [Test]
        public void Can_construct_instance_with_data()
        {
            var workitem = new Workitem(new SubmitData { Data = "Keep it safe" });

            var required = workitem.Require<SubmitData>();

            Assert.That(required.Data, Is.EqualTo("Keep it safe"));
        }

        [Test]
        public void When_submitting_when_same_type_exists_it_replaces_instance()
        {
            var workitem = new Workitem(new SubmitData { Data = "Keep for only a while" });

            workitem.Submit(new SubmitData { Data = "This is better" });

            var required = workitem.Require<SubmitData>();
            Assert.That(required.Data, Is.EqualTo("This is better"));
        }

        [Test]
        public void It_throws_when_requiring_something_that_hasnt_been_submitted()
        {
            var workitem = new Workitem();

            Assert.Throws<Exception>(() => workitem.Require<SubmitData>());
        }
    }
}
