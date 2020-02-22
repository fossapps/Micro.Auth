using System;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Meter;
using App.Metrics.Timer;

namespace Micro.Auth.Api.Measurements
{
    public static class UsersControllerMetricsExtension
    {
        public static UsersControllerMetrics UsersControllerMetrics(this IMetrics metrics)
        {
            return new UsersControllerMetrics(metrics);
        }
    }

    public class UsersControllerMetrics : BaseMeasurement {
        public UsersControllerMetrics(IMetrics metrics) : base(metrics)
        {
        }

        public void MarkFindUserByEmail()
        {
            MeterMark(new MeterOptions
            {
                Name = "UsersController.FindUserByEmail",
                MeasurementUnit = Unit.Requests,
                RateUnit = TimeUnit.Seconds
            });
        }

        public void MarkFindUserByUsername()
        {
            MeterMark(new MeterOptions
            {
                Name = "UsersController.FindUserByUsername",
                MeasurementUnit = Unit.Requests,
                RateUnit = TimeUnit.Seconds
            });
        }

        public void MarkBadRequest()
        {
            MeterMark(new MeterOptions
            {
                Name = "UsersController.CreateAccount.BadRequest",
                MeasurementUnit = Unit.Errors,
                RateUnit = TimeUnit.Seconds
            });
        }

        public void MarkAccountCreated()
        {
            MeterMark(new MeterOptions
            {
                Name = "UsersController.CreateAccount.Success",
                MeasurementUnit = Unit.Items,
                RateUnit = TimeUnit.Seconds
            });
        }
        public async Task<T> RecordTimeCreateUser<T>(Func<Task<T>> fn)
        {
            return await MeasureTimeAsync(fn, new TimerOptions
            {
                Name = "UsersController.CreateAccount.TimeToCreateAccount",
                DurationUnit = TimeUnit.Milliseconds,
                RateUnit = TimeUnit.Milliseconds
            });
        }

        public void MarkFindUserException(string exceptionType)
        {
            MeterMark(new MeterOptions
            {
                Name = "UsersController.FindUser.Exception",
                MeasurementUnit = Unit.Items,
                RateUnit = TimeUnit.Seconds,
                Tags = new MetricTags("exceptionType", exceptionType)
            });
        }

        public void MarkCreateAccountException(string exceptionType)
        {
            MeterMark(new MeterOptions
            {
                Name = "UsersController.CreateAccount.Exception",
                MeasurementUnit = Unit.Items,
                RateUnit = TimeUnit.Seconds,
                Tags = new MetricTags("exceptionType", exceptionType)
            });
        }
    }
}
