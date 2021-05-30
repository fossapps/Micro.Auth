using App.Metrics;
using App.Metrics.Meter;

namespace Micro.Auth.Business.Internal.Measurements
{
    public static class SessionControllerMetricsExtension
    {
        public static SessionControllerMetrics SessionController(this IMetrics metrics)
        {
            return new SessionControllerMetrics(metrics);
        }
    }
    public class SessionControllerMetrics : BaseMeasurement {
        public SessionControllerMetrics(IMetrics metrics) : base(metrics)
        {
        }

        public void MarkBadLoginAttempt(string problem)
        {
            MeterMark(new MeterOptions
            {
                Name = "SessionController.Login.BadRequest.BadLoginAttempt",
                MeasurementUnit = Unit.Errors,
                RateUnit = TimeUnit.Seconds,
                Tags = new MetricTags("problem", problem)
            });
        }

        public void MarkSuccessfulLoginAttempt()
        {
            MeterMark(new MeterOptions
            {
                Name = "SessionController.Login.Success",
                MeasurementUnit = Unit.Requests,
                RateUnit = TimeUnit.Seconds
            });
        }

        public void MarkTokenNotFoundInDb()
        {
            MeterMark(new MeterOptions
            {
                Name = "SessionController.Refresh.NoTokenPresentInDb",
                MeasurementUnit = Unit.Requests,
                RateUnit = TimeUnit.Seconds
            });
        }

        public void MarkLoginException(string exceptionName)
        {
            MeterMark(new MeterOptions
            {
                Name = "SessionController.New.Exception",
                MeasurementUnit = Unit.Requests,
                RateUnit = TimeUnit.Seconds,
                Tags = new MetricTags("exception", exceptionName)
            });
        }

        public void MarkBadAuthData()
        {
            MeterMark(new MeterOptions
            {
                Name = "SessionController.New.BadAuthData",
                MeasurementUnit = Unit.Requests,
                RateUnit = TimeUnit.Seconds,
            });
        }

        public void MarkException(string exceptionName)
        {
            MeterMark(new MeterOptions
            {
                Name = "SessionController.Refresh.Exception",
                MeasurementUnit = Unit.Requests,
                RateUnit = TimeUnit.Seconds,
                Tags = new MetricTags("exception", exceptionName)
            });
        }
    }
}
