# Cors
Browsers use CORS policies to ensure if a resource call is okay to be made,
by default no API respond with any CORS values (and they shouldn't)

While on development, you may want to allow any origin, any header and any method, but on production you really need to whitelist what cors policies you allow.

This in no way is a protection against all security measures, but this needs to be configured correctly

## Configuration
CorsConfig is a configuration key where the configuration is stored.
It has `Origins` which is a array of string, `Headers` which is also array of string, a boolean field `AllowCredentials` which lets us choose if Credentials are allowed and `PolicyToUse` which is a string value of policies you can choose from

## Included policies:
Auth services comes with two policies included so you can choose which one to use.

### development (default)
This policy is meant to be used on development environment and for testing/staging purposes, this is not a policy one should be using in production environment

*Note: This policy can't allow credentials when there's allow any origin, if you wish to allow credentials on local, you'll have to use production settings and whitelist localhost on your origin list*

### production
This policy actually reads the `CorsConfig` configuration section and sets those values for cors.
Imagine you wanted to allow example.com and test.com as your allowed domains, and x-client-id, x-authorization-token as your allowed headers, you could simply set those values on your configuration and use development policy
