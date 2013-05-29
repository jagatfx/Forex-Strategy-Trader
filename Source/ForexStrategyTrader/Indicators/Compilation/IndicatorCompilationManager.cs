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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using ForexStrategyBuilder.Indicators;
using ForexStrategyBuilder.Infrastructure.Interfaces;
using ForexStrategyBuilder.Library;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Manages the operation of indicators.
    /// </summary>
    public class IndicatorCompilationManager
    {
        private readonly CSharpCompiler compiler;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public IndicatorCompilationManager()
        {
            CustomIndicatorsList = new List<Indicator>();
            compiler = new CSharpCompiler();

            foreach (Assembly assembly in GetReferencedAndInitialAssembly(Assembly.GetEntryAssembly()))
            {
                compiler.AddReferencedAssembly(assembly);
                Console.WriteLine("############# " + assembly.FullName);
            }
        }

        /// <summary>
        ///     Gets a list of the loaded custom indicators
        /// </summary>
        public List<Indicator> CustomIndicatorsList { get; private set; }

        /// <summary>
        ///     Gather all assemblies referenced from current assembly.
        /// </summary>
        private static IEnumerable<Assembly> GetReferencedAndInitialAssembly(Assembly initialAssembly)
        {
            AssemblyName[] names = initialAssembly.GetReferencedAssemblies();
            var assemblies = new Assembly[names.Length + 1];

            for (int i = 0; i < names.Length; i++)
            {
                assemblies[i] = Assembly.Load(names[i]);
            }

            assemblies[assemblies.Length - 1] = initialAssembly;

            return assemblies;
        }

        /// <summary>
        ///     Loads an indicator from a dll.
        /// </summary>
        public void LoadDllIndicator(string dllPath, out string errorMessages)
        {
            errorMessages = string.Empty;
            Assembly assembly = Assembly.LoadFrom(dllPath);
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (!typeof(IIndicator).IsAssignableFrom(type))
                    continue;

                Indicator newIndicator;

                try
                {
                    newIndicator = Activator.CreateInstance(type) as Indicator;
                }
                catch (Exception exception)
                {
                    errorMessages = "ERROR: Loading '" + Path.GetFileName(dllPath) + "': " + exception.Message;
                    return;
                }

                if (newIndicator == null)
                {
                    errorMessages = "Cannot load: " + dllPath;
                    return;
                }

                newIndicator.Initialize(SlotTypes.NotDefined);
                newIndicator.CustomIndicator = true;
                newIndicator.LoaddedFromDll = true;
                IntegrateIndicator(dllPath, out errorMessages, newIndicator);
                return;
            }
        }

        /// <summary>
        ///     Load file, compile it and create/load the indicators into the CustomIndicatorsList.
        /// </summary>
        /// <param name="sourcePath">Path to the source file</param>
        /// <param name="errorMessages">Resulting error messages, if any.</param>
        public LibRecord LoadCompileSourceFile(string sourcePath, out string errorMessages)
        {
            string errorLoadSourceFile;
            string sourceCode = LoadSourceCode(sourcePath, out errorLoadSourceFile);

            if (string.IsNullOrEmpty(sourceCode))
            {
                errorMessages = errorLoadSourceFile;
                return null;
            }

            if (!ValidateSourceCode(sourcePath, sourceCode, out errorMessages))
                return null;

            Dictionary<string, int> dictCompilationErrors;
            Assembly assembly = compiler.CompileSource(sourceCode, out dictCompilationErrors);

            if (assembly == null)
            {
                var sbCompilationError = new StringBuilder();
                sbCompilationError.AppendLine("ERROR: Indicator compilation failed in file [" +
                                              Path.GetFileName(sourcePath) + "]");

                foreach (string error in dictCompilationErrors.Keys)
                    sbCompilationError.AppendLine('\t' + error);

                errorMessages = sbCompilationError.ToString();
                return null;
            }

            string errorGetIndicator;
            string indicatorFileName = Path.GetFileNameWithoutExtension(sourcePath);
            Indicator newIndicator = GetIndicatorInstanceFromAssembly(assembly, indicatorFileName, out errorGetIndicator);

            if (newIndicator == null)
            {
                errorMessages = errorGetIndicator;
                return null;
            }

            newIndicator.Initialize(SlotTypes.NotDefined);
            newIndicator.CustomIndicator = true;

            if (IntegrateIndicator(sourcePath, out errorMessages, newIndicator))
                return ExportIndicatorAsDll(sourcePath, sourceCode);

            return null;
        }

        private bool ValidateSourceCode(string sourcePath, string sourceCode, out string errorMessages)
        {
            errorMessages = string.Empty;

            bool isEntities = sourceCode.Contains("ForexStrategyBuilder.Infrastructure.Entities");
            bool isEnums = sourceCode.Contains("ForexStrategyBuilder.Infrastructure.Enums");
            bool isInterfaces = sourceCode.Contains("ForexStrategyBuilder.Infrastructure.Interfaces");
            bool isStore = sourceCode.Contains("ForexStrategyBuilder.Indicators.Store");

            if (isEntities && isEnums && isInterfaces && isStore)
                return true;

            errorMessages = "ERROR: Obsolete format of source code in file [" +
                            Path.GetFileName(sourcePath) + "]." + Environment.NewLine +
                            "Get a newer version from Code Repository.";
            return false;
        }

        /// <summary>
        ///     Checks and stores the indicator.
        /// </summary>
        private bool IntegrateIndicator(string filePath, out string errorMessages, Indicator newIndicator)
        {
            errorMessages = string.Empty;

            // Check for a repeated indicator name among the custom indicators
            foreach (Indicator indicator in CustomIndicatorsList)
                if (indicator.IndicatorName == newIndicator.IndicatorName)
                {
                    errorMessages = string.Format("Indicator '{0}' found in [{1}] is already loaded.",
                                                  newIndicator.IndicatorName,
                                                  Path.GetFileName(filePath));
                    return false;
                }

            // Check for a repeated indicator name among the original indicators
            foreach (string indicatorName in IndicatorManager.OriginalIndicatorNames)
                if (indicatorName == newIndicator.IndicatorName)
                    newIndicator.OverrideMainIndicator = true;

            // Test the new custom indicator
            string errorTestIndicator;
            if (!IndicatorTester.CustomIndicatorFastTest(newIndicator, out errorTestIndicator))
            {
                errorMessages = errorTestIndicator;
                return false;
            }

            // Adds the custom indicator to the list
            CustomIndicatorsList.Add(newIndicator);

            return true;
        }

        /// <summary>
        ///     Compiles and saves a dll from source code.
        /// </summary>
        /// <param name="sourcePath">Path to cs file.</param>
        /// <param name="sourceCode">Source code</param>
        /// <returns></returns>
        private LibRecord ExportIndicatorAsDll(string sourcePath, string sourceCode)
        {
            string name = Path.GetFileNameWithoutExtension(sourcePath);
            string targedPath = Path.Combine(Data.LibraryDir, name + ".dll");

            compiler.CompileSourceToDll(sourceCode, targedPath);

            var sourceInfo = new FileInfo(sourcePath);
            var record = new LibRecord
            {
                SourceFileName = name,
                SourceLastWriteTime = sourceInfo.LastWriteTime,
            };

            return record;
        }

        /// <summary>
        ///     Reads the source code from a file.
        /// </summary>
        private string LoadSourceCode(string sourcePath, out string errorLoadSourceFile)
        {
            string sourceCode = string.Empty;

            if (!File.Exists(sourcePath))
            {
                errorLoadSourceFile = "ERROR The source file does not exist: " + Path.GetFileName(sourcePath);
                return sourceCode;
            }

            try
            {
                using (var sr = new StreamReader(sourcePath))
                {
                    sourceCode = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch
            {
                errorLoadSourceFile = "ERROR Cannot read the file: " + Path.GetFileName(sourcePath);
                return sourceCode;
            }

            errorLoadSourceFile = string.Empty;
            return sourceCode;
        }

        /// <summary>
        ///     Creates an indicator instance from the assembly given.
        /// </summary>
        private static Indicator GetIndicatorInstanceFromAssembly(Assembly assembly, string indicatorFileName,
                                                                  out string errorMessage)
        {
            Type[] assemblyTypes = assembly.GetTypes();
            foreach (Type typeAssembly in assemblyTypes)
            {
                if (typeAssembly.IsSubclassOf(typeof(Indicator)))
                {
                    ConstructorInfo[] aConstructorInfo = typeAssembly.GetConstructors();

                    // Looking for an appropriate constructor
                    foreach (ConstructorInfo constructorInfo in aConstructorInfo)
                    {
                        if (!constructorInfo.IsConstructor ||
                            !constructorInfo.IsPublic ||
                            constructorInfo.GetParameters().Length != 0)
                            continue;

                        errorMessage = string.Empty;
                        try
                        {
                            return (Indicator)constructorInfo.Invoke(null);
                        }
                        catch (Exception exc)
                        {
                            errorMessage = "ERROR: [" + indicatorFileName + "] " + exc.Message;
                            if (exc.InnerException != null && !string.IsNullOrEmpty(exc.InnerException.Message))
                                errorMessage += Environment.NewLine + "\t" + exc.InnerException.Message;
                            return null;
                        }
                    }

                    errorMessage = "ERROR: Cannot find an appropriate constructor for " + indicatorFileName + ".";
                    return null;
                }
            }

            errorMessage = "ERROR: Cannot create an instance of an indicator from " + assembly + ".";
            return null;
        }
    }
}