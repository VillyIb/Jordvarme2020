﻿namespace eu.iamia.Configuration
{
    using System;
    using Microsoft.Extensions.Configuration;

    public class ConfigurationSetup
    {
        public static IConfiguration Init()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                    .AddJsonFile($"appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{env}.json", true, true)
                    .AddEnvironmentVariables()
                ;

            IConfiguration config = builder.Build();

            return config;
        }
    }
}
