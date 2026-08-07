using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace daybreak.Configuration
{
    public static class AppConfiguration
    {
        public static IConfigurationRoot Configuration { get; } =
            new ConfigurationBuilder()
                .AddUserSecrets(typeof(AppConfiguration).Assembly)
                .Build();
    }
}