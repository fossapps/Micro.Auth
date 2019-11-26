using System;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Meter;
using App.Metrics.Timer;

namespace Micro.Auth.Api.Measurements
{
    public static class KeyGenerationWorkerExtension
    {
        public static KeyGenWorker KeyGenerationWorker(this IMetrics metrics)
        {
            return new KeyGenWorker(metrics);
        }
    }

    public class KeyGenWorker : BaseMeasurement
    {
        public KeyGenWorker(IMetrics metrics) : base(metrics)
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
