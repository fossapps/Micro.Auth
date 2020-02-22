# Identity Config
This configuration configures auth service to behave in a different way in terms of user's authentication,

This might be one of the more important configuration while consuming auth service

# Available Configurations
## PasswordRequirements
This is internally PasswordOptions given by identity framework, it contains the following options:
- `RequireDigit: boolean` (defaults to false)
- `RequiredLength: int` (defaults to 6)
- `RequireNonAlphanumeric: boolean` (defaults to false)
- `RequireUppercase: boolean` (defaults to false)
- `RequireLowercase: boolean` (defaults to false)
- `RequireUniqueChars: int` (defaults to 4)

## LockoutOptions
This is internally modified LockoutOptions (except for timespan), and here are the options it offers:
- `AllowedForNewUsers: boolean` (defaults to true)
- `DefaultLockoutTimeSpanInSeconds: int` (defaults to 600)
- `MaxFailedAccessAttempts: int` (defaults to 5)

## UserOptions
This reconfigures user's ability to signup and here are the options:
- `AllowedUserNameCharacters: string` (defaults to: "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._")
- `RequireUniqueEmail: boolean` (defaults to true)

## SignInOptions
This configures ability to sign in with the following options:
- `RequireConfirmedEmail: boolean` (defaults to true)
- `RequireConfirmedAccount: boolean` (defaults to false)
- `RequireConfirmedPhoneNumber: boolean` (defaults to false)

## IssueForAudience: string
This doesn't provide any further config, it simply sets audience for jwt

## Issuer: string
Sets issuer for jwt

## Audiences: IEnumerable&lt;string&gt;
Validates one of the audience while trying to access this service.

*Note: this doesn't apply to other services, you'll need to configure each service for valid audiences*
