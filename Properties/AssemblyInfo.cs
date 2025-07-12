using NUnit.Framework;

// Enable parallel execution at the assembly level
// This allows test fixtures to run in parallel
//[assembly: Parallelizable(ParallelScope.Fixtures)]

// Enable parallel execution at the assembly level
//[assembly: NUnit.Framework.Parallelizable(NUnit.Framework.ParallelScope.Fixtures)]

// Enable parallel execution for fixtures and for tests within fixtures.
[assembly: NUnit.Framework.Parallelizable(NUnit.Framework.ParallelScope.All)]

// Set number of worker threads
[assembly: NUnit.Framework.LevelOfParallelism(4)]