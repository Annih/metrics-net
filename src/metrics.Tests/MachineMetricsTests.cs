using NUnit.Framework;

namespace metrics.Tests
{
    [TestFixture]
    public class MachineMetricsTests
    {
        [Test]
        public void Can_load_all_metrics()
        {
            Metrics.Clear();

            MachineMetrics.InstallAll();

#if !COREFX
            Assert.IsTrue(Metrics.All.Count > 0);
#endif
        }
    }
}
