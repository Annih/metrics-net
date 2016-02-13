using System;
using System.Collections.Generic;
using System.Threading;
using metrics.Reporting;
using metrics.Tests.Core;
using NUnit.Framework;

namespace metrics.Tests.Reporting
{
    [TestFixture]
    public class ConsoleReporterTests
    {
        private Metrics _metrics;

        [Test]
        public void Can_run_with_known_counters_and_human_readable_format()
        {
            RegisterMetrics();

            using(var reporter = new ConsoleReporter(_metrics))
            {
                reporter.Run();
            }
        }

        [Test]
        public void Can_run_with_known_counters_and_json_format()
        {
            RegisterMetrics();

            using(var reporter = new ConsoleReporter(new JsonReportFormatter(_metrics)))
            {
                reporter.Run();
            }
        }

        [Test]
        public void Can_run_in_background()
        {
            const int ticks = 3;
            var block = new ManualResetEvent(false);

            RegisterMetrics();

            ThreadPool.QueueUserWorkItem(
                s =>
                {
                    using (var reporter = new ConsoleReporter(_metrics))
                    {
                        reporter.Start(1, TimeUnit.Seconds);
                        while(true)
                        {
                            Thread.Sleep(500);
                            if (reporter.Runs >= ticks)
                            {
                                block.Set();
                                return;
                            }    
                        }
                    }
                });

            Assert.IsTrue(block.WaitOne(5000));
        }

        [Test]
        public void Can_stop()
        {
            var block = new ManualResetEvent(false);

            RegisterMetrics();

            ThreadPool.QueueUserWorkItem(
                s =>
                {
                    using (var reporter = new ConsoleReporter(_metrics))
                    {
                        reporter.Start(1, TimeUnit.Seconds);
                        reporter.Stopped += delegate { block.Set(); };
                        Thread.Sleep(2000);
                        reporter.Stop();
                    }
                });

            Assert.IsTrue(block.WaitOne(5000));
        }

        private void RegisterMetrics()
        {
            _metrics = new Metrics();

            var counter = _metrics.Counter(typeof(CounterTests), "Can_run_with_known_counters_counter");
            counter.Increment(100);

            var queue = new Queue<int>();
             _metrics.Gauge(typeof(GaugeTests), "Can_run_with_known_counters_gauge", () => queue.Count);
            queue.Enqueue(1);
            queue.Enqueue(2);
        }
    }
}
