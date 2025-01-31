﻿// <copyright file="GetPropReceiver.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion, yungd1plomat, wherewhere">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion, yungd1plomat, wherewhere. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AdvancedSharpAdbClient.DeviceCommands
{
    /// <summary>
    /// Parses the output of the <c>getprop</c> command, which lists all properties of an Android device.
    /// </summary>
    public sealed class GetPropReceiver : MultiLineReceiver
    {
        /// <summary>
        /// The path to the <c>getprop</c> executable to run on the device.
        /// </summary>
        public const string GetpropCommand = "/system/bin/getprop";

        /// <summary>
        /// A regular expression which can be used to parse the getprop output.
        /// </summary>
        private const string GetpropRegex = "^\\[([^]]+)\\]\\:\\s*\\[(.*)\\]$";

        /// <summary>
        /// Gets the list of properties which have been retrieved.
        /// </summary>
        public Dictionary<string, string> Properties { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// Processes the new lines.
        /// </summary>
        /// <param name="lines">The lines to process.</param>
        protected override void ProcessNewLines(IEnumerable<string> lines)
        {
            // We receive an array of lines. We're expecting
            // to have the build info in the first line, and the build
            // date in the 2nd line. There seems to be an empty line
            // after all that.
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line) || line.StartsWith("#") || line.StartsWith("$"))
                {
                    continue;
                }

                Match m = Regex.Match(line, GetpropRegex, RegexOptions.Compiled);
                if (m.Success)
                {
                    string label = m.Groups[1].Value.Trim();
                    string value = m.Groups[2].Value.Trim();

                    if (label.Length > 0)
                    {
                        Properties.Add(label, value);
                    }
                }
            }
        }
    }
}
