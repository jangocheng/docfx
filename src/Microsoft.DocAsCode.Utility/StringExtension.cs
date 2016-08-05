﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.DocAsCode.Utility
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class StringExtension
    {
        public static string ForwardSlashCombine(this string baseAddress, string relativeAddress)
        {
            if (string.IsNullOrEmpty(baseAddress)) return relativeAddress;
            return baseAddress + "/" + relativeAddress;
        }

        public static string BackSlashToForwardSlash(this string input)
        {
            if (string.IsNullOrEmpty(input)) return null;
            return input.Replace('\\', '/');
        }

        public static string ToDelimitedString(this IEnumerable<string> input, string delimiter = ",")
        {
            if (input == null)
            {
                return null;
            }

            return string.Join(delimiter, input);
        }

        public static string GetNormalizedFullPathKey(this IEnumerable<string> list)
        {
            if (list == null) return null;

            // make sure the order consistent
            var nomalizedPaths = GetNormalizedFullPathList(list);
            return nomalizedPaths.ToDelimitedString();
        }

        public static IEnumerable<string> GetNormalizedFullPathList(this IEnumerable<string> paths)
        {
            if (paths == null) return null;
            return (from p in paths
                    where !string.IsNullOrEmpty(p)
                    select ToNormalizedFullPath(p)).Distinct().OrderBy(s => s);
        }

        public static IEnumerable<string> GetNormalizedPathList(this IEnumerable<string> paths)
        {
            if (paths == null) return null;
            return (from p in paths
                    where !string.IsNullOrEmpty(p)
                    select ToNormalizedPath(p)).Distinct().OrderBy(s => s);
        }

        /// <summary>
        /// Should not convert path to lower case as under Linux/Unix, path is case sensitive
        /// Also, Website URL should be case sensitive consider the server might be running under Linux/Unix
        /// So we could even not lower the path under Windows as the generated YAML should be ideally OS irrelevant
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ToNormalizedFullPath(this string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            return Path.GetFullPath(path).BackSlashToForwardSlash().TrimEnd('/');
        }

        public static string ToNormalizedPath(this string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            return path.BackSlashToForwardSlash().TrimEnd('/');
        }

        public static string ToDisplayPath(this string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }
    }
}