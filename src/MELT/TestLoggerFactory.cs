// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace MELT
{
    public class TestLoggerFactory : ITestLoggerFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ServiceProvider _serviceProvider;
        

        //internal TestLoggerFactory(ITestSink sink)
        //{
        //    _sink = sink;
        //    //AddProvider(new TestLoggerProvider(_sink));
        //}

        [Obsolete]
        public IEnumerable<LogEntry> LogEntries => Sink.LogEntries;

        [Obsolete]
        public IEnumerable<BeginScope> Scopes => Sink.Scopes;

        //public ITestLoggerSink Sink => _sink;

        

        private TestLoggerFactory(ILoggerFactory loggerFactory, ServiceProvider serviceProvider)
        {
            _loggerFactory = loggerFactory;
            _serviceProvider = serviceProvider;
        }

        public ITestLoggerSink Sink => _serviceProvider.GetRequiredService<ITestLoggerSink>();

        public void Dispose()
        {
            _serviceProvider.Dispose();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggerFactory.CreateLogger(categoryName);
        }

        public void AddProvider(ILoggerProvider provider)
        {
            _loggerFactory.AddProvider(provider);
        }

        public static ITestLoggerFactory Create()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddTest());
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            return new TestLoggerFactory(loggerFactory, serviceProvider);
        }

        public static ITestLoggerFactory Create(Action<ILoggingBuilder> configure)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddTest());
            serviceCollection.AddLogging(configure);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            return new TestLoggerFactory(loggerFactory, serviceProvider);
        }

        public static ITestLoggerFactory Create(Action<TestLoggerOptions> configure)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddTest(configure));
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            return new TestLoggerFactory(loggerFactory, serviceProvider);
        }
    }
}
