//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using ForexStrategyBuilder.Utils;
using Microsoft.CSharp;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     CSharpCompiler manages the compilation of source code to an assembly.
    ///     This class is thread safe, so multiple threads are capable
    ///     to utilize it simultaneously.
    /// </summary>
    public class CSharpCompiler
    {
        private readonly CompilerParameters compilerParameters;
        private volatile CSharpCodeProvider codeProvider;

        public CSharpCompiler()
        {
            var checker = new DotNetVersionChecker();
            codeProvider = checker.IsDonNet35Installed()
                               ? new CSharpCodeProvider(new Dictionary<String, String> {{"CompilerVersion", "v3.5"}})
                               : new CSharpCodeProvider();

            compilerParameters = new CompilerParameters();
        }

        /// <summary>
        ///     For the source code to compile, it needs to have a reference to assemblies
        ///     to use the IL code inside them.
        /// </summary>
        /// <param name="assembly">An assembly to add.</param>
        public void AddReferencedAssembly(Assembly assembly)
        {
            lock (this)
            {
                compilerParameters.ReferencedAssemblies.Add(assembly.Location);
            }
        }

        /// <summary>
        ///     Compile a single source file to assembly.
        /// </summary>
        /// <param name="source">Indicator source to compile.</param>
        /// <param name="compilerErrors">Compiler errors, if any.</param>
        /// <returns>Compiled assembly or null.</returns>
        public Assembly CompileSource(string source, out Dictionary<string, int> compilerErrors)
        {
            compilerErrors = new Dictionary<string, int>();
            compilerParameters.OutputAssembly = string.Empty;
            compilerParameters.GenerateInMemory = true;
            CompilerResults compilerResults = codeProvider.CompileAssemblyFromSource(compilerParameters, source);

            if (compilerResults.Errors.Count > 0)
            {
                // Compilation failed.
                foreach (CompilerError error in compilerResults.Errors)
                {
                    int errorLine = error.Line;
                    int errorColumn = error.Column;
                    string errorText = error.ErrorText;

                    string errorMessage = string.Format("Line {0} Column {1}: {2}.", errorLine, errorColumn, errorText);

                    if (!compilerErrors.ContainsKey(errorMessage))
                        compilerErrors.Add(errorMessage, errorLine);
                }

                return null;
            }

            return compilerResults.CompiledAssembly;
        }

        /// <summary>
        ///     Compiles a single source file to dll.
        /// </summary>
        public void CompileSourceToDll(string source, string targedFileName)
        {
            compilerParameters.GenerateInMemory = false;
            compilerParameters.OutputAssembly = targedFileName;
            codeProvider.CompileAssemblyFromSource(compilerParameters, source);
        }
    }
}