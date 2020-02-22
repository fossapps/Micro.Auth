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

        public async Task RecordTimeToSendActivationEmail(Func<Task> fn)
        {
            await MeasureTimeAsync(fn, new TimerOptions
            {
                Name = "UsersController.Activation.TimeToSendActivationEmail",
                DurationUnit = TimeUnit.Milliseconds,
                RateUnit = TimeUnit.Milliseconds
            });
        }

        public void MarkUserNotFoundActivation()
        {
            MeterMark(new MeterOptions
            {
                Name = "UsersController.Activation.UserNotFound",
                MeasurementUnit = Unit.Errors,
                RateUnit = TimeUnit.Seconds
            });
        }

        public void MarkEmailSendingFailure()
        {
            MeterMark(new MeterOptions
            {
                Name = "UsersController.Mail.SendingFailed",
                MeasurementUnit = Unit.Items,
                RateUnit = TimeUnit.Seconds,
            });
        }

        public void MarkExceptionActivation(string exceptionType)
        {
            MeterMark(new MeterOptions
            {
                Name = "UsersController.Activation.Exception",
                MeasurementUnit = Unit.Items,
                RateUnit = TimeUnit.Seconds,
                Tags = new MetricTags("exceptionType", exceptionType)
            });
        }

        public async Task RecordTimeToConfirmEmail(Func<Task> fn)
        {
            await MeasureTimeAsync(fn, new TimerOptions
            {
                Name = "UsersController.Activation.TimeToConfirmEmail",
                DurationUnit = TimeUnit.Milliseconds,
                RateUnit = TimeUnit.Milliseconds
            });
        }

        public void MarkSuccessfulConfirmation()
        {
            MeterMark(new MeterOptions
            {
                Name = "UsersController.Activation.SuccessfulConfirmation",
                MeasurementUnit = Unit.Requests,
                RateUnit = TimeUnit.Seconds
            });
        }

        public void MarkFailedToConfirmActivation()
        {
            MeterMark(new MeterOptions
            {
                Name = "UsersController.Activation.FailedToConfirm",
                MeasurementUnit = Unit.Errors,
                RateUnit = TimeUnit.Seconds
            });
        }

        public void MarkPasswordResetUserNotFound()
        {
            MeterMark(new MeterOptions
            {
                Name = "UsersController.RequestPasswordReset.UserNotFound",
                MeasurementUnit = Unit.Requests,
                RateUnit = TimeUnit.Seconds
            });
        }

        public async Task MeasureTimeToSendPasswordResetEmail(Func<Task> fn)
        {
            await MeasureTimeAsync(fn, new TimerOptions
            {
                Name = "UsersController.RequestPasswordReset.TimeToSendPasswordResetEmail",
                DurationUnit = TimeUnit.Milliseconds,
                RateUnit = TimeUnit.Milliseconds
            });
        }

        public async Task MeasureTimeToResetPassword(Func<Task> fn)
        {
            await MeasureTimeAsync(fn, new TimerOptions
            {
                Name = "UsersController.ResetPassword.TimeToSendPasswordResetEmail",
                DurationUnit = TimeUnit.Milliseconds,
                RateUnit = TimeUnit.Milliseconds
            });
        }

        public void MarkFailedToResetPassword()
        {
            MeterMark(new MeterOptions
            {
                Name = "UsersController.RequestPasswordReset.FailedToReset",
                MeasurementUnit = Unit.Requests,
                RateUnit = TimeUnit.Seconds
            });
        }

        public void MarkResetPasswordException(string exception)
        {
            MeterMark(new MeterOptions
            {
                Name = "UsersController.RequestPasswordReset.Exception",
                Tags = new MetricTags("name", exception),
                MeasurementUnit = Unit.Requests,
                RateUnit = TimeUnit.Seconds
            });
        }

        public void MarkPasswordChangeSuccess()
        {
            MeterMark(new MeterOptions
            {
                Name = "UsersController.ChangePassword.Success",
                MeasurementUnit = Unit.Requests,
                RateUnit = TimeUnit.Seconds
            });
        }

        public void MarkPasswordChangeFailure(string failure)
        {
            MeterMark(new MeterOptions
            {
                Name = "UsersController.ChangePassword.BadRequest",
                MeasurementUnit = Unit.Requests,
                RateUnit = TimeUnit.Seconds,
                Tags = new MetricTags("failure", failure)
            });
        }

        public void MarkPasswordChangeUserNotFound()
        {
            MeterMark(new MeterOptions
            {
                Name = "UsersController.ChangePassword.UserNotFound",
                MeasurementUnit = Unit.Requests,
                RateUnit = TimeUnit.Seconds
            });
        }

        public void MarkPasswordChangeException(string exceptionType)
        {
            MeterMark(new MeterOptions
            {
                Name = "UsersController.ChangePassword.Exception",
                MeasurementUnit = Unit.Requests,
                RateUnit = TimeUnit.Seconds,
                Tags = new MetricTags("exceptionType", exceptionType)
            });
        }
    }
}
