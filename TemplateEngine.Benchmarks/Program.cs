// // Module Name: Program.cs
// // Author:      Eddie Velasquez
// // Copyright (c) 2025, Intercode Consulting, Inc.

using BenchmarkDotNet.Running;
using MacroProcessingBenchmarks = TemplateEngine.Benchmarks.MacroProcessingBenchmarks;

BenchmarkSwitcher.FromAssembly(typeof(MacroProcessingBenchmarks).Assembly).Run(args);
