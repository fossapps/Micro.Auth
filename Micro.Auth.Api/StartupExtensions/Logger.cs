using System;
using Micro.Auth.Api.Configs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Slack;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Micro.Auth.Api.StartupExtensions
{
    public static class Logger
    {
        public static void ConfigureLoggerWithSlack(this ILoggerFactory loggerFactory, SlackLoggingConfig slackConfig,
            IHostEnvironment env)
        {
            if (string.IsNullOrEmpty(slackConfig.WebhookUrl))
            {
                return;
            }
            loggerFactory.AddSlack(new SlackConfiguration
            {
                MinLevel = slackConfig.MinLogLevel,
                WebhookUrl = new Uri(slackConfig.WebhookUrl)
            }, env.ApplicationName, env.EnvironmentName);
        }

        public static void ConfigureSerilog(this IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
            {
                var elasticConfiguration = hostingContext.Configuration.GetSection("ElasticConfiguration").Get<ElasticConfiguration>();
                var elasticSearchSinkOptions = new ElasticsearchSinkOptions(new Uri(elasticConfiguration.Host))
                {
                    ModifyConnectionSettings = x => x.BasicAuthentication(elasticConfiguration.Username, elasticConfiguration.Password),
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                };
                loggerConfiguration
                    .WriteTo.Elasticsearch(elasticSearchSinkOptions)
                    .WriteTo.Console();
            });
        }
    }
}