using System;
using System.Diagnostics;

namespace Prerelease.Main
{
    internal class PerfCounter
    {
        private Stopwatch watch = new Stopwatch();
        private double thresshold;
        private double numberOfSamples;

        public double AverageMsec { get; internal set; }
        public bool ExceedsThresshold => AverageMsec > thresshold;

        public PerfCounter(double thresshold, int numberOfSamples = 100)
        {
            this.thresshold = thresshold;
            this.numberOfSamples = numberOfSamples;
        }

        internal void Execute(Action action)
        {
            watch.Restart();
            action();
            watch.Stop();

            AverageMsec -= AverageMsec / numberOfSamples;
            AverageMsec += watch.Elapsed.TotalMilliseconds / numberOfSamples;
        }
    }
}