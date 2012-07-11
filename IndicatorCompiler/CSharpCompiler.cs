// CSharp_Compiler Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CSharp;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// CSharp_Compiler manages the compilation of source code to an assembly.
    /// This class is thread safe, so multiple threads are capable
    /// to utilize it simultaneously.
    /// </summary>
    public class CSharpCompiler
    {
        // Provides the actual compilation of source code.

        // Represents the parameters used to invoke a compiler.
        private readonly CompilerParameters _compilationParameters;
        private volatile CSharpCodeProvider _codeProvider;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CSharpCompiler()
        {
            _codeProvider = new CSharpCodeProvider();
            // Make sure we conduct all the operations "in memory".
            _compilationParameters = new CompilerParameters { GenerateInMemory = true };
        }

        /// <summary>
        /// For the source code to compile, it needs to have a reference to assemblies
        /// to use the IL code inside them.
        /// </summary>
        /// <param name="assembly">An assembly to add.</param>
        public void AddReferencedAssembly(Assembly assembly)
        {
            lock (this)
            {
                _compilationParameters.ReferencedAssemblies.Add(assembly.Location);
            }
        }

        /// <summary>
        /// Compile a single source file to assembly.
        /// </summary>
        /// <param name="source">Indicator source to compile.</param>
        /// <param name="compilerErrors">Compiler errors, if any.</param>
        /// <returns>Compiled assembly or null.</returns>
        public Assembly CompileSource(string source, out Dictionary<string, int> compilerErrors)
        {
            compilerErrors = new Dictionary<string, int>();

            CompilerResults compilerResults = _codeProvider.CompileAssemblyFromSource(_compilationParameters, source);

            if (compilerResults.Errors.Count > 0)
            {
                // Compilation failed.
                foreach (CompilerError error in compilerResults.Errors)
                {
                    string errorMessage = "Line " + error.Line + " Column " + error.Column + ": " + error.ErrorText + ".";
                    int errorLine = error.Line;

                    if (!compilerErrors.ContainsKey(errorMessage))
                        compilerErrors.Add(errorMessage, errorLine);
                }

                return null;
            }

            return compilerResults.CompiledAssembly;
        }
    }
}
