using NUnit.Framework;

// Enable parallel execution at the assembly level
// This allows test fixtures to run in parallel
[assembly: Parallelizable(ParallelScope.Fixtures)]