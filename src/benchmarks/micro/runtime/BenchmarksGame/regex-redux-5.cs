// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Adapted from regex-redux C# .NET Core #5 program
// http://benchmarksgame.alioth.debian.org/u64q/program.php?test=regexredux&lang=csharpcore&id=5
// aka (as of 2017-09-01) rev 1.3 of https://alioth.debian.org/scm/viewvc.php/benchmarksgame/bench/regexredux/regexredux.csharp-5.csharp?root=benchmarksgame&view=log
// Best-scoring C# .NET Core version as of 2017-09-01

/* The Computer Language Benchmarks Game
   http://benchmarksgame.alioth.debian.org/
 
   Regex-Redux by Josh Goldfoot
   order variants by execution time by Anthony Lloyd
*/

using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using MicroBenchmarks;

namespace BenchmarksGame
{
    [BenchmarkCategory(Categories.Runtime, Categories.BenchmarksGame, Categories.JIT, Categories.Regex, Categories.NoWASM)]
    public partial class RegexRedux_5
    {
        private string _sequences;

        [GlobalSetup]
        public void Setup()
        {
            RegexReduxHelpers helpers = new RegexReduxHelpers(bigInput: true);

            using (var inputStream = new FileStream(helpers.InputFile, FileMode.Open))
            using (var input = new StreamReader(inputStream))
            {
                _sequences = input.ReadToEnd();
            }
        }

        [Benchmark(Description = nameof(RegexRedux_5))]
        [Arguments(RegexOptions.None)]
        [Arguments(RegexOptions.Compiled)]
#if NET7_0_PREVIEW2_OR_GREATER
        [Arguments(RegexOptions.NonBacktracking)]
#endif
        public int RunBench(RegexOptions options)
        {
            var sequences = _sequences;
            sequences = Regex.Replace(sequences, ">.*\n|\n", "", options);

            var magicTask = Task.Run(() =>
            {
                var newseq = Regex.Replace(sequences, "tHa[Nt]", "<4>", options);
                newseq = Regex.Replace(newseq, "aND|caN|Ha[DS]|WaS", "<3>", options);
                newseq = Regex.Replace(newseq, "a[NSt]|BY", "<2>", options);
                newseq = Regex.Replace(newseq, "<[^>]*>", "|", options);
                newseq = Regex.Replace(newseq, "\\|[^|][^|]*\\|", "-", options);
                return newseq.Length;
            });

            var variant2 = Task.Run(() => Count(sequences, "[cgt]gggtaaa|tttaccc[acg]", options));
            var variant3 = Task.Run(() => Count(sequences, "a[act]ggtaaa|tttacc[agt]t", options));
            var variant7 = Task.Run(() => Count(sequences, "agggt[cgt]aa|tt[acg]accct", options));
            var variant6 = Task.Run(() => Count(sequences, "aggg[acg]aaa|ttt[cgt]ccct", options));
            var variant4 = Task.Run(() => Count(sequences, "ag[act]gtaaa|tttac[agt]ct", options));
            var variant5 = Task.Run(() => Count(sequences, "agg[act]taaa|ttta[agt]cct", options));
            var variant1 = Task.Run(() => Count(sequences, "agggtaaa|tttaccct", options));
            var variant9 = Task.Run(() => Count(sequences, "agggtaa[cgt]|[acg]ttaccct", options));
            var variant8 = Task.Run(() => Count(sequences, "agggta[cgt]a|t[acg]taccct", options));

            Task.WaitAll(variant1, variant2, variant3, variant4, variant5, variant6, variant7, variant8, variant9);

            return magicTask.Result;

            static string Count(string s, string r, RegexOptions regexOptions)
            {
                int c;
#if NET7_0_PREVIEW2_OR_GREATER
                c = Regex.Count(s, r, regexOptions);
#else
                c = 0;
                var m = Regex.Match(s, r, regexOptions);
                while (m.Success) { c++; m = m.NextMatch(); }
#endif
                return r + " " + c;
            }
        }

#if NET7_0_PREVIEW2_OR_GREATER
        [Benchmark(Description = nameof(RegexRedux_5) + "SourceGen")]
        public int RunBenchSourceGen()
        {
            var sequences = _sequences;
            sequences = Sanitize().Replace(sequences, "");

            var magicTask = Task.Run(() =>
            {
                var newseq = Replace4().Replace(sequences, "<4>");
                newseq = Replace3().Replace(newseq, "<3>");
                newseq = Replace2().Replace(newseq, "<2>");
                newseq = Replace1().Replace(newseq, "|");
                newseq = Replace0().Replace(newseq, "-");
                return newseq.Length;
            });

            var variant2 = Task.Run(() => Magic2().Count(sequences));
            var variant3 = Task.Run(() => Magic3().Count(sequences));
            var variant7 = Task.Run(() => Magic7().Count(sequences));
            var variant6 = Task.Run(() => Magic6().Count(sequences));
            var variant4 = Task.Run(() => Magic4().Count(sequences));
            var variant5 = Task.Run(() => Magic5().Count(sequences));
            var variant1 = Task.Run(() => Magic1().Count(sequences));
            var variant9 = Task.Run(() => Magic9().Count(sequences));
            var variant8 = Task.Run(() => Magic8().Count(sequences));

            Task.WaitAll(variant1, variant2, variant3, variant4, variant5, variant6, variant7, variant8, variant9);

            return magicTask.Result;
        }
        
        [RegexGenerator(">.*\n|\n")]
        private static partial Regex Sanitize();

        [RegexGenerator("tHa[Nt]")]
        private static partial Regex Replace4();

        [RegexGenerator("aND|caN|Ha[DS]|WaS")]
        private static partial Regex Replace3();

        [RegexGenerator("a[NSt]|BY")]
        private static partial Regex Replace2();

        [RegexGenerator("<[^>]*>")]
        private static partial Regex Replace1();

        [RegexGenerator("\\|[^|][^|]*\\|")]
        private static partial Regex Replace0();

        [RegexGenerator("agggtaaa|tttaccct")]
        private static partial Regex Magic1();

        [RegexGenerator("[cgt]gggtaaa|tttaccc[acg]")]
        private static partial Regex Magic2();
        
        [RegexGenerator("a[act]ggtaaa|tttacc[agt]t")]
        private static partial Regex Magic3();
        
        [RegexGenerator("ag[act]gtaaa|tttac[agt]ct")]
        private static partial Regex Magic4();
        
        [RegexGenerator("agg[act]taaa|ttta[agt]cct")]
        private static partial Regex Magic5();
        
        [RegexGenerator("aggg[acg]aaa|ttt[cgt]ccct")]
        private static partial Regex Magic6();
        
        [RegexGenerator("agggt[cgt]aa|tt[acg]accct")]
        private static partial Regex Magic7();
        
        [RegexGenerator("agggta[cgt]a|t[acg]taccct")]
        private static partial Regex Magic8();
        
        [RegexGenerator("agggtaa[cgt]|[acg]ttaccct")]
        private static partial Regex Magic9();
#endif
    }
}
