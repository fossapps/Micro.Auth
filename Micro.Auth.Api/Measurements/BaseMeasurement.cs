using System;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Meter;
using App.Metrics.Timer;

namespace Micro.Auth.Api.Measurements
{
    public abstract class BaseMeasurement
    {
        private readonly IMetrics _metrics;
        protected BaseMeasurement(IMetrics metrics)
        {
            _metrics = metrics;
        }

        protected T MeasureTime<T>(Func<T> fn, TimerOptions timerOptions)
        {
            using (_metrics?.Measure.Timer.Time(timerOptions))
            {
                return fn();
            }
        }

        protected async Task<T> MeasureTimeAsync<T>(Func<Task<T>> fn, TimerOptions timerOptions)
        {
            using (_metrics?.Measure.Timer.Time(timerOptions))
            {
                return await fn();
            }
        }

        protected async Task MeasureTimeAsync(Func<Task> fn, TimerOptions timerOptions)
        {
            using (_metrics?.Measure.Timer.Time(timerOptions))
            {
                await fn();
            }
        }

        protected void MeterMark(MeterOptions meterOptions)
        {
            _metrics?.Measure.Meter.Mark(meterOptions);
        }
    }
}
