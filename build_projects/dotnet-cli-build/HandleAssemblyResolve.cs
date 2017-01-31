// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;
using System.Reflection;

namespace Microsoft.DotNet.Cli.Build
{
    /// <summary>
    /// Add an AssemblyResolve handler that looks in the specified SearchPath directory.
    /// 
    /// This is needed for loading the BuildTools assemblies when building with the full-framework
    /// MSBuild.
    /// </summary>
    public class HandleAssemblyResolve : Task
    {
        [Required]
        public string SearchPath { get; set; }

        public override bool Execute()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;

            return true;
        }

        private Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            string assemblySearchPath = Path.Combine(SearchPath, args.Name.Split(',')[0] + ".dll");

            Log.LogMessage(MessageImportance.Low, "Probing for '{0}'.", assemblySearchPath);
            if (File.Exists(assemblySearchPath))
            {
                Log.LogMessage(MessageImportance.Low, "Assembly found.");
                return Assembly.LoadFrom(assemblySearchPath);
            }
            else
            {
                Log.LogMessage(MessageImportance.Low, "Assembly not found.");
                return null;
            }
        }
    }
}
