using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TrashMem.Benchmark
{
    internal struct TestStruct
    {
        public int a;
        public int b;
        public long c;
    }

    internal class Program
    {
        private const int STATIC_ADDRESS_CHAR = 0x133A48C;
        private const int STATIC_ADDRESS_INT16 = 0x133A490;
        private const int STATIC_ADDRESS_INT32 = 0x133A494;
        private const int STATIC_ADDRESS_INT64 = 0x133A498;
        private const int STATIC_ADDRESS_STRING = 0xFCE478C;

        private const int TOTAL_RUNS = 100000;

        private delegate void BenchFunction();

        private static void Main(string[] args)
        {
            Console.Title = "TrashMem Benchmark";
            Process[] testProcesses = Process.GetProcessesByName("ShittyMcUlow");

            if (testProcesses.Length > 0)
            {
                TrashMem TrashMem = new TrashMem(testProcesses.ToList().First());

                Console.WriteLine($"TrashMem Benchmark doing {TOTAL_RUNS} runs for every function");
                Console.WriteLine($">> 1 Byte Char");
                PrettyPrintValue("Read<char>", BenchmarkFunction(() => TrashMem.ReadUnmanaged<char>(STATIC_ADDRESS_INT16)));
                PrettyPrintValue("ReadChar", BenchmarkFunction(() => TrashMem.ReadChar(STATIC_ADDRESS_INT16)));
                PrettyPrintValue("ReadCharSafe", BenchmarkFunction(() => TrashMem.ReadCharSafe(STATIC_ADDRESS_INT16)));
                Console.WriteLine($">> 2 Byte Short");
                PrettyPrintValue("Read<short>", BenchmarkFunction(() => TrashMem.ReadUnmanaged<short>(STATIC_ADDRESS_INT16)));
                PrettyPrintValue("ReadInt16", BenchmarkFunction(() => TrashMem.ReadInt16(STATIC_ADDRESS_INT16)));
                PrettyPrintValue("ReadInt16Safe", BenchmarkFunction(() => TrashMem.ReadInt16Safe(STATIC_ADDRESS_INT16)));
                Console.WriteLine($">> 4 Byte Integer");
                PrettyPrintValue("Read<int>", BenchmarkFunction(() => TrashMem.ReadUnmanaged<int>(STATIC_ADDRESS_INT32)));
                PrettyPrintValue("ReadInt32", BenchmarkFunction(() => TrashMem.ReadInt32(STATIC_ADDRESS_INT32)));
                PrettyPrintValue("ReadInt32Safe", BenchmarkFunction(() => TrashMem.ReadInt32Safe(STATIC_ADDRESS_INT32)));
                Console.WriteLine($">> 8 Byte Long");
                PrettyPrintValue("Read<long>", BenchmarkFunction(() => TrashMem.ReadUnmanaged<long>(STATIC_ADDRESS_INT64)));
                PrettyPrintValue("ReadInt64", BenchmarkFunction(() => TrashMem.ReadInt64(STATIC_ADDRESS_INT64)));
                PrettyPrintValue("ReadInt64Safe", BenchmarkFunction(() => TrashMem.ReadInt64Safe(STATIC_ADDRESS_INT64)));
                Console.WriteLine($">> 12 Byte String");
                PrettyPrintValue("ReadString", BenchmarkFunction(() => TrashMem.ReadString(STATIC_ADDRESS_STRING, Encoding.ASCII, 12)));
                Console.WriteLine($">> 16 Byte Struct");
                PrettyPrintValue("ReadStruct<TestStruct>", BenchmarkFunction(() => TrashMem.ReadStruct<TestStruct>(STATIC_ADDRESS_INT16)));
            }
            else
            {
                Console.WriteLine("Error: please make sure a ShittyMcUlow process is running...");
            }

            Console.ReadLine();
        }

        private static void PrettyPrintValue(string functionName, double value)
        {
            Console.ResetColor();
            Console.Write($"{functionName} \t=> ");
            if (value <= 30) Console.ForegroundColor = ConsoleColor.Green;
            else if (value <= 50) Console.ForegroundColor = ConsoleColor.Yellow;
            else if (value > 50) Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{value}");
            Console.ResetColor();
            Console.WriteLine(" timerticks");
        }

        private static double BenchmarkFunction(BenchFunction benchFunction)
        {
            List<long> benchmarkResult = new List<long>();

            benchFunction.Invoke();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Stopwatch stopwatch = new Stopwatch();
            for (int i = 0; i < TOTAL_RUNS; ++i)
            {
                stopwatch.Reset();
                stopwatch.Start();
                benchFunction.Invoke();
                stopwatch.Stop();
                benchmarkResult.Add(stopwatch.ElapsedTicks);
            }

            return Math.Round(benchmarkResult.Average(), 2);
        }
    }
}
