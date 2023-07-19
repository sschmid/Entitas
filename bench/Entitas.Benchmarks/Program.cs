using BenchmarkDotNet.Running;
using Entitas.Benchmarks;

// BenchmarkRunner.Run<ComponentIndexBenchmarks>();
// BenchmarkRunner.Run<AggressiveInliningBenchmarks>();
// BenchmarkRunner.Run<CreateComponentBenchmarks>();
BenchmarkRunner.Run<DelegateBenchmarks>();
