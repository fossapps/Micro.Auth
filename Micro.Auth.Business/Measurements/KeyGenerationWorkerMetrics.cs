using System;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Meter;
using App.Metrics.Timer;

namespace Micro.Auth.Business.Measurements
{
    public static class KeyGenerationWorkerExtension
    {
        public static KeyGenWorkerMetrics KeyGenerationWorker(this IMetrics metrics)
        {
            return new KeyGenWorkerMetrics(metrics);
        }
    }

    public class KeyGenWorkerMetrics : BaseMeasurement
    {
        public KeyGenWorkerMetrics(IMetrics metrics) : base(metrics)
        {
        }
        public async Task<T> RecordTimeToSavePublicKey<T>(Func<Task<T>> fn)
        {
            return await MeasureTimeAsync(fn, new TimerOptions
            {
                Name = "KeyGenerationWorker.TimeToSave",
                DurationUnit = TimeUnit.Milliseconds,
                RateUnit = TimeUnit.Milliseconds
            });
        }

        public void MarkErrorSavingToKeyService()
        {
            MeterMark(new MeterOptions
            {
                Name = "KeyGenerationWorker.Error",
                MeasurementUnit = Unit.Errors,
                RateUnit = TimeUnit.Seconds
            });
        }

        public void MarkErrorUnhandledType()
        {
            MeterMark(new MeterOptions
            {
                Name = "KeyGenerationWorker.UnhandledType",
                MeasurementUnit = Unit.Errors,
                RateUnit = TimeUnit.Seconds
            });
        }

    }
}
