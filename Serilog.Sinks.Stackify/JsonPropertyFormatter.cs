using System;
using System.Collections.Generic;
using System.IO;
using Serilog.Events;
using Serilog.Formatting.Json;

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

namespace Serilog
{
    public class JsonPropertyFormatter : JsonFormatter
    {
        public new void Format(LogEvent logEvent, TextWriter output)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            output.Write("{");

            var delim = "";

            if (logEvent.Properties.Count != 0)
            {
                delim = ",";
                WriteProperties(logEvent.Properties, output);
            }

            if (logEvent.Exception != null)
                WriteException(logEvent.Exception, ref delim, output);

            output.Write("}");
        }

        protected override void WriteProperties(IReadOnlyDictionary<string, LogEventPropertyValue> properties, TextWriter output)
        {
            output.Write("\"{0}\":{{", "Properties");
            WritePropertiesValues(properties, output);
            output.Write("}");
        }
    }
}
