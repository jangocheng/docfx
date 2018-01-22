﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.DocAsCode.Build.OverwriteDocuments
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class OverwriteUtility
    {
        private static readonly Regex OPathRegex =
            new Regex(
                @"^(?<propertyName>[:A-Za-z_][\w\.\-:]*)(\[(?<key>[:A-Za-z_][\w\.\-:]*)=((""(?<stringValue>[\w\(\)\.\{\}\[\]\|\/@ `<>:]+)"")|(?<numberValue>[-+]?[0-9]*(\.[0-9]+)?))\])?/?",
                RegexOptions.Compiled);

        public static List<OPathSegment> ParseOPath(string OPathString)
        {
            if (string.IsNullOrEmpty(OPathString))
            {
                throw new ArgumentException("OPathString cannot be null or empty.", nameof(OPathString));
            }

            if (OPathString.EndsWith("/"))
            {
                throw new ArgumentException($"{OPathString} is not a valid OPath");
            }

            var OPathSegments = new List<OPathSegment>();

            var leftString = Regex.Replace(OPathString, @"\s+(?=([^""]*""[^""]*"")*[^""]*$)", "");
            while (leftString.Length > 0)
            {
                var match = OPathRegex.Match(leftString);
                if (match.Length == 0 )
                {
                    throw new ArgumentException($"{OPathString} is not a valid OPath");
                }

                if (!match.Value.EndsWith("/") && match.Groups["key"].Success)
                {
                    throw new ArgumentException($"{OPathString} is not a valid OPath");
                }

                var newSegment = match.Groups["numberValue"].Success
                    ? new OPathSegment
                    {
                        SegmentName = match.Groups["propertyName"].Value,
                        Key = match.Groups["key"].Value,
                        Value = double.Parse(match.Groups["numberValue"].Value),
                        OriginalSegmentString = match.Value.TrimEnd('/')
                    }
                    : new OPathSegment
                    {
                        SegmentName = match.Groups["propertyName"].Value,
                        Key = match.Groups["key"].Value,
                        Value = match.Groups["stringValue"].Value,
                        OriginalSegmentString = match.Value.TrimEnd('/')
                    };
                OPathSegments.Add(newSegment);
                leftString = leftString.Substring(match.Length);
            }

            return OPathSegments;
        }
    }
}
