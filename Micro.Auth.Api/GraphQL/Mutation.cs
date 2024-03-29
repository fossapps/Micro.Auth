using GraphQL;
using GraphQL.Types;
using Micro.Auth.Api.GraphQL.Directives.Extensions;
using Micro.Auth.Api.GraphQL.Extensions;
using Micro.Auth.Api.GraphQL.Inputs;
using Micro.Auth.Api.GraphQL.Types;
using Micro.Auth.Api.Internal.UserData.Extensions;
using Micro.Auth.Business.Common;
using Micro.Auth.Business.EmailVerification;
using Micro.Auth.Business.PasswordManager;
using Micro.Auth.Business.Sessions;
using Micro.Auth.Business.Users;
using Microsoft.AspNetCore.Http;

namespace Micro.Auth.Api.GraphQL
{
    public class Mutation : ObjectGraphType
    {
        public Mutation(IUserService userService, IPasswordManager passwordManager, IEmailVerificationService verification, ISessionService sessionService, IHttpContextAccessor contextAccessor)
        {
            FieldAsync<NonNullGraphType<UserType>, User>("register",
                arguments: new QueryArguments(RegisterInputType.BuildArgument()),
                resolve: x => userService.Create(x.GetArgument<RegisterInput>("input")));

            FieldAsync<NonNullGraphType<StringGraphType>, string>("refreshToken",
                resolve: x => sessionService.Refresh(contextAccessor.MustGetBearerToken()));

            FieldAsync<NonNullGraphType<TokensType>, LoginSuccessResponse>("login",
                resolve: x => sessionService.Login(contextAccessor.GetLoginRequest()));

            FieldAsync<NonNullGraphType<UserType>, User>("verifyEmail",
                arguments: new QueryArguments(VerifyEmailInputType.BuildArgument()),
                resolve: x => verification.ConfirmEmail(x.GetArgument<VerifyEmailInput>("input")));

            FieldAsync<NonNullGraphType<ResultType>, Result>("sendActivationEmail",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "login"}),
                resolve: x => verification.SendActivationEmail(x.GetArgument<string>("login")));

            FieldAsync<NonNullGraphType<ResultType>, Result>("requestPasswordReset",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "login"}),
                resolve: x => passwordManager.RequestPasswordReset(x.GetArgument<string>("login")));

            FieldAsync<NonNullGraphType<UserType>, User>("changePassword",
                arguments: new QueryArguments(ChangePasswordInput.BuildArgument()),
                resolve: x => passwordManager.ChangePassword(contextAccessor.GetUserId(), x.GetArgument<ChangePasswordRequest>("input"))).Authorize();

            FieldAsync<NonNullGraphType<UserType>, User>("resetPassword",
                arguments: new QueryArguments(ResetPasswordInput.BuildArgument()),
                resolve: x => passwordManager.ResetPassword(x.GetArgument<ResetPasswordRequest>("input")));
        }
    }
}
