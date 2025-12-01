// 2025-11-26 Claude Sonnet 4.5: Performance benchmark comparing DAL vs EF6
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace xPort5.EF6.PerformanceTests
{
    /// <summary>
    /// Performance benchmark to compare xPort5.DAL (stored procedures) vs xPort5.EF6 (Entity Framework)
    /// Target: EF6 should be within 10% of DAL performance
    /// </summary>
    public class PerformanceBenchmark
    {
        private const int WARMUP_ITERATIONS = 5;
        private const int TEST_ITERATIONS = 100;

        public class BenchmarkResult
        {
            public string Operation { get; set; }
            public string Entity { get; set; }
            public double DAL_AvgMs { get; set; }
            public double EF6_AvgMs { get; set; }
            public double Difference_Percent { get; set; }
            public bool WithinTarget { get; set; }
            public string Status { get; set; }
        }

        public static List<BenchmarkResult> RunAllBenchmarks()
        {
            var results = new List<BenchmarkResult>();

            Console.WriteLine("=== xPort5 Performance Benchmark: DAL vs EF6 ===");
            Console.WriteLine($"Warmup Iterations: {WARMUP_ITERATIONS}");
            Console.WriteLine($"Test Iterations: {TEST_ITERATIONS}");
            Console.WriteLine($"Target: EF6 within 10% of DAL performance\n");

            // T_Category benchmarks (Simple entity)
            results.Add(BenchmarkLoad_TCategory());
            results.Add(BenchmarkLoadCollection_TCategory());
            results.Add(BenchmarkSave_TCategory());

            // Customer benchmarks (Medium complexity)
            results.Add(BenchmarkLoad_Customer());
            results.Add(BenchmarkLoadCollection_Customer());
            results.Add(BenchmarkSave_Customer());

            // Article benchmarks (Complex entity)
            results.Add(BenchmarkLoad_Article());
            results.Add(BenchmarkLoadCollection_Article());
            results.Add(BenchmarkSave_Article());

            PrintResults(results);
            return results;
        }

        #region T_Category Benchmarks

        private static BenchmarkResult BenchmarkLoad_TCategory()
        {
            Console.WriteLine("Benchmarking T_Category.Load()...");

            // Get a valid ID from database
            Guid testId = GetFirstTCategoryId();
            if (testId == Guid.Empty)
            {
                return new BenchmarkResult
                {
                    Operation = "Load",
                    Entity = "T_Category",
                    Status = "SKIPPED - No data in database"
                };
            }

            // Warmup
            for (int i = 0; i < WARMUP_ITERATIONS; i++)
            {
                var dalWarmup = xPort5.DAL.T_Category.Load(testId);
                var ef6Warmup = xPort5.EF6.T_Category.Load(testId);
            }

            // Benchmark DAL
            var dalTimes = new List<double>();
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < TEST_ITERATIONS; i++)
            {
                sw.Restart();
                var dalEntity = xPort5.DAL.T_Category.Load(testId);
                sw.Stop();
                dalTimes.Add(sw.Elapsed.TotalMilliseconds);
            }

            // Benchmark EF6
            var ef6Times = new List<double>();
            for (int i = 0; i < TEST_ITERATIONS; i++)
            {
                sw.Restart();
                var ef6Entity = xPort5.EF6.T_Category.Load(testId);
                sw.Stop();
                ef6Times.Add(sw.Elapsed.TotalMilliseconds);
            }

            return CalculateResult("Load", "T_Category", dalTimes, ef6Times);
        }

        private static BenchmarkResult BenchmarkLoadCollection_TCategory()
        {
            Console.WriteLine("Benchmarking T_Category.LoadCollection()...");

            // Warmup
            for (int i = 0; i < WARMUP_ITERATIONS; i++)
            {
                var dalWarmup = xPort5.DAL.T_Category.LoadCollection();
                var ef6Warmup = xPort5.EF6.T_Category.LoadCollection();
            }

            // Benchmark DAL
            var dalTimes = new List<double>();
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < TEST_ITERATIONS; i++)
            {
                sw.Restart();
                var dalCollection = xPort5.DAL.T_Category.LoadCollection();
                sw.Stop();
                dalTimes.Add(sw.Elapsed.TotalMilliseconds);
            }

            // Benchmark EF6
            var ef6Times = new List<double>();
            for (int i = 0; i < TEST_ITERATIONS; i++)
            {
                sw.Restart();
                var ef6Collection = xPort5.EF6.T_Category.LoadCollection();
                sw.Stop();
                ef6Times.Add(sw.Elapsed.TotalMilliseconds);
            }

            return CalculateResult("LoadCollection", "T_Category", dalTimes, ef6Times);
        }

        private static BenchmarkResult BenchmarkSave_TCategory()
        {
            Console.WriteLine("Benchmarking T_Category.Save()...");

            // Warmup
            for (int i = 0; i < WARMUP_ITERATIONS; i++)
            {
                var dalWarmup = new xPort5.DAL.T_Category
                {
                    CategoryCode = "TST",
                    CategoryName = "Warmup Test"
                };
                dalWarmup.Save();
                xPort5.DAL.T_Category.Delete(dalWarmup.CategoryId);

                var ef6Warmup = new xPort5.EF6.T_Category
                {
                    CategoryCode = "TST",
                    CategoryName = "Warmup Test"
                };
                ef6Warmup.Save();
                xPort5.EF6.T_Category.Delete(ef6Warmup.CategoryId);
            }

            // Benchmark DAL
            var dalTimes = new List<double>();
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < TEST_ITERATIONS; i++)
            {
                var dalEntity = new xPort5.DAL.T_Category
                {
                    CategoryCode = "TST",
                    CategoryName = $"Test {i}"
                };
                sw.Restart();
                dalEntity.Save();
                sw.Stop();
                dalTimes.Add(sw.Elapsed.TotalMilliseconds);
                xPort5.DAL.T_Category.Delete(dalEntity.CategoryId);
            }

            // Benchmark EF6
            var ef6Times = new List<double>();
            for (int i = 0; i < TEST_ITERATIONS; i++)
            {
                var ef6Entity = new xPort5.EF6.T_Category
                {
                    CategoryCode = "TST",
                    CategoryName = $"Test {i}"
                };
                sw.Restart();
                ef6Entity.Save();
                sw.Stop();
                ef6Times.Add(sw.Elapsed.TotalMilliseconds);
                xPort5.EF6.T_Category.Delete(ef6Entity.CategoryId);
            }

            return CalculateResult("Save", "T_Category", dalTimes, ef6Times);
        }

        #endregion

        #region Customer Benchmarks

        private static BenchmarkResult BenchmarkLoad_Customer()
        {
            Console.WriteLine("Benchmarking Customer.Load()...");

            Guid testId = GetFirstCustomerId();
            if (testId == Guid.Empty)
            {
                return new BenchmarkResult
                {
                    Operation = "Load",
                    Entity = "Customer",
                    Status = "SKIPPED - No data in database"
                };
            }

            // Warmup
            for (int i = 0; i < WARMUP_ITERATIONS; i++)
            {
                var dalWarmup = xPort5.DAL.Customer.Load(testId);
                var ef6Warmup = xPort5.EF6.Customer.Load(testId);
            }

            // Benchmark DAL
            var dalTimes = new List<double>();
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < TEST_ITERATIONS; i++)
            {
                sw.Restart();
                var dalEntity = xPort5.DAL.Customer.Load(testId);
                sw.Stop();
                dalTimes.Add(sw.Elapsed.TotalMilliseconds);
            }

            // Benchmark EF6
            var ef6Times = new List<double>();
            for (int i = 0; i < TEST_ITERATIONS; i++)
            {
                sw.Restart();
                var ef6Entity = xPort5.EF6.Customer.Load(testId);
                sw.Stop();
                ef6Times.Add(sw.Elapsed.TotalMilliseconds);
            }

            return CalculateResult("Load", "Customer", dalTimes, ef6Times);
        }

        private static BenchmarkResult BenchmarkLoadCollection_Customer()
        {
            Console.WriteLine("Benchmarking Customer.LoadCollection()...");

            // Warmup
            for (int i = 0; i < WARMUP_ITERATIONS; i++)
            {
                var dalWarmup = xPort5.DAL.Customer.LoadCollection("Retired = 0");
                var ef6Warmup = xPort5.EF6.Customer.LoadCollection("Retired = false");
            }

            // Benchmark DAL
            var dalTimes = new List<double>();
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < TEST_ITERATIONS; i++)
            {
                sw.Restart();
                var dalCollection = xPort5.DAL.Customer.LoadCollection("Retired = 0");
                sw.Stop();
                dalTimes.Add(sw.Elapsed.TotalMilliseconds);
            }

            // Benchmark EF6
            var ef6Times = new List<double>();
            for (int i = 0; i < TEST_ITERATIONS; i++)
            {
                sw.Restart();
                var ef6Collection = xPort5.EF6.Customer.LoadCollection("Retired = false");
                sw.Stop();
                ef6Times.Add(sw.Elapsed.TotalMilliseconds);
            }

            return CalculateResult("LoadCollection", "Customer", dalTimes, ef6Times);
        }

        private static BenchmarkResult BenchmarkSave_Customer()
        {
            Console.WriteLine("Benchmarking Customer.Save()...");

            // Note: Skipping this test as it requires valid foreign keys
            return new BenchmarkResult
            {
                Operation = "Save",
                Entity = "Customer",
                Status = "SKIPPED - Requires valid foreign keys"
            };
        }

        #endregion

        #region Article Benchmarks

        private static BenchmarkResult BenchmarkLoad_Article()
        {
            Console.WriteLine("Benchmarking Article.Load()...");

            Guid testId = GetFirstArticleId();
            if (testId == Guid.Empty)
            {
                return new BenchmarkResult
                {
                    Operation = "Load",
                    Entity = "Article",
                    Status = "SKIPPED - No data in database"
                };
            }

            // Warmup
            for (int i = 0; i < WARMUP_ITERATIONS; i++)
            {
                var dalWarmup = xPort5.DAL.Article.Load(testId);
                var ef6Warmup = xPort5.EF6.Article.Load(testId);
            }

            // Benchmark DAL
            var dalTimes = new List<double>();
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < TEST_ITERATIONS; i++)
            {
                sw.Restart();
                var dalEntity = xPort5.DAL.Article.Load(testId);
                sw.Stop();
                dalTimes.Add(sw.Elapsed.TotalMilliseconds);
            }

            // Benchmark EF6
            var ef6Times = new List<double>();
            for (int i = 0; i < TEST_ITERATIONS; i++)
            {
                sw.Restart();
                var ef6Entity = xPort5.EF6.Article.Load(testId);
                sw.Stop();
                ef6Times.Add(sw.Elapsed.TotalMilliseconds);
            }

            return CalculateResult("Load", "Article", dalTimes, ef6Times);
        }

        private static BenchmarkResult BenchmarkLoadCollection_Article()
        {
            Console.WriteLine("Benchmarking Article.LoadCollection()...");

            // Warmup
            for (int i = 0; i < WARMUP_ITERATIONS; i++)
            {
                var dalWarmup = xPort5.DAL.Article.LoadCollection("Retired = 0");
                var ef6Warmup = xPort5.EF6.Article.LoadCollection("Retired = false");
            }

            // Benchmark DAL
            var dalTimes = new List<double>();
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < TEST_ITERATIONS; i++)
            {
                sw.Restart();
                var dalCollection = xPort5.DAL.Article.LoadCollection("Retired = 0");
                sw.Stop();
                dalTimes.Add(sw.Elapsed.TotalMilliseconds);
            }

            // Benchmark EF6
            var ef6Times = new List<double>();
            for (int i = 0; i < TEST_ITERATIONS; i++)
            {
                sw.Restart();
                var ef6Collection = xPort5.EF6.Article.LoadCollection("Retired = false");
                sw.Stop();
                ef6Times.Add(sw.Elapsed.TotalMilliseconds);
            }

            return CalculateResult("LoadCollection", "Article", dalTimes, ef6Times);
        }

        private static BenchmarkResult BenchmarkSave_Article()
        {
            Console.WriteLine("Benchmarking Article.Save()...");

            // Note: Skipping this test as it requires valid foreign keys
            return new BenchmarkResult
            {
                Operation = "Save",
                Entity = "Article",
                Status = "SKIPPED - Requires valid foreign keys"
            };
        }

        #endregion

        #region Helper Methods

        private static Guid GetFirstTCategoryId()
        {
            try
            {
                var collection = xPort5.DAL.T_Category.LoadCollection();
                return collection.Count > 0 ? collection[0].CategoryId : Guid.Empty;
            }
            catch
            {
                return Guid.Empty;
            }
        }

        private static Guid GetFirstCustomerId()
        {
            try
            {
                var collection = xPort5.DAL.Customer.LoadCollection("Retired = 0");
                return collection.Count > 0 ? collection[0].CustomerId : Guid.Empty;
            }
            catch
            {
                return Guid.Empty;
            }
        }

        private static Guid GetFirstArticleId()
        {
            try
            {
                var collection = xPort5.DAL.Article.LoadCollection("Retired = 0");
                return collection.Count > 0 ? collection[0].ArticleId : Guid.Empty;
            }
            catch
            {
                return Guid.Empty;
            }
        }

        private static BenchmarkResult CalculateResult(string operation, string entity, List<double> dalTimes, List<double> ef6Times)
        {
            double dalAvg = dalTimes.Average();
            double ef6Avg = ef6Times.Average();
            double diffPercent = ((ef6Avg - dalAvg) / dalAvg) * 100;
            bool withinTarget = Math.Abs(diffPercent) <= 10;

            var result = new BenchmarkResult
            {
                Operation = operation,
                Entity = entity,
                DAL_AvgMs = Math.Round(dalAvg, 2),
                EF6_AvgMs = Math.Round(ef6Avg, 2),
                Difference_Percent = Math.Round(diffPercent, 1),
                WithinTarget = withinTarget,
                Status = withinTarget ? "PASS" : "FAIL"
            };

            Console.WriteLine($"  DAL: {result.DAL_AvgMs}ms | EF6: {result.EF6_AvgMs}ms | Diff: {result.Difference_Percent:+0.0;-0.0}% | {result.Status}");
            return result;
        }

        private static void PrintResults(List<BenchmarkResult> results)
        {
            Console.WriteLine("\n=== BENCHMARK RESULTS ===\n");
            Console.WriteLine($"{"Operation",-20} {"Entity",-15} {"DAL (ms)",-12} {"EF6 (ms)",-12} {"Diff %",-10} {"Status",-10}");
            Console.WriteLine(new string('-', 85));

            foreach (var result in results)
            {
                if (result.Status == "SKIPPED - No data in database" || result.Status == "SKIPPED - Requires valid foreign keys")
                {
                    Console.WriteLine($"{result.Operation,-20} {result.Entity,-15} {result.Status}");
                }
                else
                {
                    Console.WriteLine($"{result.Operation,-20} {result.Entity,-15} {result.DAL_AvgMs,-12:F2} {result.EF6_AvgMs,-12:F2} {result.Difference_Percent,-10:+0.0;-0.0} {result.Status,-10}");
                }
            }

            var passedTests = results.Count(r => r.Status == "PASS");
            var failedTests = results.Count(r => r.Status == "FAIL");
            var skippedTests = results.Count(r => r.Status.StartsWith("SKIPPED"));

            Console.WriteLine(new string('-', 85));
            Console.WriteLine($"\nSummary: {passedTests} PASS | {failedTests} FAIL | {skippedTests} SKIPPED");
            Console.WriteLine($"Overall: {(failedTests == 0 && passedTests > 0 ? "✅ ALL TESTS PASSED" : "⚠️ SOME TESTS FAILED")}");
        }

        #endregion
    }
}
