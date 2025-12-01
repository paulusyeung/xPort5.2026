// 2025-11-26 Claude Sonnet 4.5: Console app entry point for performance benchmarks
using System;
using xPort5.EF6.PerformanceTests;

namespace xPort5.EF6.PerformanceTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("xPort5 DAL vs EF6 Performance Benchmark");
            Console.WriteLine("========================================\n");

            try
            {
                var results = PerformanceBenchmark.RunAllBenchmarks();

                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nERROR: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
