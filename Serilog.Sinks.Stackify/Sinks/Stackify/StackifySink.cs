// Copyright 2015 John Knoll <jpknoll@gmail.com>
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.IO;
using Serilog.Core;
using Serilog.Events;
using StackifyLib;
using StackifyLib.Models;

namespace Serilog.Sinks.Stackify
{
    public class StackifySink : ILogEventSink, IDisposable
    {
        private readonly IFormatProvider _formatProvider;
        private readonly JsonDataFormatter _dataFormatter;

        /// <summary>
        /// Construct a sink that saves logs to the specified storage account.
        /// </summary>
        public StackifySink(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;

            _dataFormatter = new JsonDataFormatter();

            AppDomain.CurrentDomain.DomainUnload += OnAppDomainUnloading;
            AppDomain.CurrentDomain.ProcessExit += OnAppDomainUnloading;
            AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnloading;
        }

        private void OnAppDomainUnloading(object sender, EventArgs args)
        {
            var exceptionEventArgs = args as UnhandledExceptionEventArgs;
            if (exceptionEventArgs != null && !exceptionEventArgs.IsTerminating)
                return;
            CloseAndFlush();
        }


        /// <summary>
        /// Emit the provided log event to the sink.
        /// </summary>
        /// <param name="logEvent">The log event to write.</param>
        public void Emit(LogEvent logEvent)
        {
            if (!Logger.CanSend())
                return;

            Logger.QueueLogObject(new LogMsg()
            {
                Level = LevelToSeverity(logEvent),
                Msg = logEvent.RenderMessage(_formatProvider),
                data = PropertiesToData(logEvent)
            }, logEvent.Exception);
        }

        private string PropertiesToData(LogEvent logEvent)
        {
            var payload = new StringWriter();
            _dataFormatter.FormatData(logEvent, payload);
            
            return payload.ToString();
        }

        static string LevelToSeverity(LogEvent logEvent)
        {
            switch (logEvent.Level)
            {
                case LogEventLevel.Debug:
                    return "DEBUG";
                case LogEventLevel.Error:
                    return "ERROR";
                case LogEventLevel.Fatal:
                    return "FATAL";
                case LogEventLevel.Verbose:
                    return "VERBOSE";
                case LogEventLevel.Warning:
                    return "WARNING";
                default:
                    return "INFORMATION";
            }
        }

        private void CloseAndFlush()
        {
            AppDomain.CurrentDomain.DomainUnload -= OnAppDomainUnloading;
            AppDomain.CurrentDomain.ProcessExit -= OnAppDomainUnloading;
            AppDomain.CurrentDomain.UnhandledException -= OnAppDomainUnloading;
            
            Logger.Shutdown();
        }

        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
                return;

            Logger.Shutdown();
            _disposed = true;
        }
    }
}
