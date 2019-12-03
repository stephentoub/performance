// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using MicroBenchmarks;

namespace System.Text.RegularExpressions.Tests
{
    [BenchmarkCategory(Categories.CoreFX)]
    public class RegexRedux
    {
        static readonly string input = File.ReadAllText(
            Path.Combine(
                Path.GetDirectoryName(typeof(RegexRedux).Assembly.Location), 
                "corefx", "System.Text.RegularExpressions", "content", 
                "200_000.in"));

        [Benchmark]
        [Arguments(RegexOptions.None)]
        [Arguments(RegexOptions.Compiled)]
        public void RegexReduxMini(RegexOptions options)
        {
            string sequences = input;
            var initialLength = sequences.Length;
            sequences = Regex.Replace(sequences, ">.*\n|\n", "");

            var magicTask = Task.Run(() =>
            {
                var newseq = new Regex("tHa[Nt]", options).Replace(sequences, "<4>");
                newseq = new Regex("aND|caN|Ha[DS]|WaS", options).Replace(newseq, "<3>");
                newseq = new Regex("a[NSt]|BY", options).Replace(newseq, "<2>");
                newseq = new Regex("<[^>]*>", options).Replace(newseq, "|");
                newseq = new Regex("\\|[^|][^|]*\\|", options).Replace(newseq, "-");
                return newseq.Length;
            });

            var variant2 = Task.Run(() => regexCount(sequences, "[cgt]gggtaaa|tttaccc[acg]", options));
            var variant3 = Task.Run(() => regexCount(sequences, "a[act]ggtaaa|tttacc[agt]t", options));
            var variant7 = Task.Run(() => regexCount(sequences, "agggt[cgt]aa|tt[acg]accct", options));
            var variant6 = Task.Run(() => regexCount(sequences, "aggg[acg]aaa|ttt[cgt]ccct", options));
            var variant4 = Task.Run(() => regexCount(sequences, "ag[act]gtaaa|tttac[agt]ct", options));
            var variant5 = Task.Run(() => regexCount(sequences, "agg[act]taaa|ttta[agt]cct", options));
            var variant1 = Task.Run(() => regexCount(sequences, "agggtaaa|tttaccct", options));
            var variant9 = Task.Run(() => regexCount(sequences, "agggtaa[cgt]|[acg]ttaccct", options));
            var variant8 = Task.Run(() => regexCount(sequences, "agggta[cgt]a|t[acg]taccct", options));

            Task.WaitAll(magicTask, variant1, variant2, variant3, variant4, variant5, variant6, variant7, variant8, variant9);

            static string regexCount(string s, string r, RegexOptions options)
            {
                int c = 0;
                var m = new Regex(r, options).Match(s);
                while (m.Success) { c++; m = m.NextMatch(); }
                return r + " " + c;
            }
        }

        [Benchmark]
        [Arguments(RegexOptions.None)]
        [Arguments(RegexOptions.Compiled)]
        public void RegexReduxMini2(RegexOptions options)
        {
            string sequences = input;
            var initialLength = sequences.Length;
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

            var variant2 = Task.Run(() => regexCount(sequences, "[cgt]gggtaaa|tttaccc[acg]", options));
            var variant3 = Task.Run(() => regexCount(sequences, "a[act]ggtaaa|tttacc[agt]t", options));
            var variant7 = Task.Run(() => regexCount(sequences, "agggt[cgt]aa|tt[acg]accct", options));
            var variant6 = Task.Run(() => regexCount(sequences, "aggg[acg]aaa|ttt[cgt]ccct", options));
            var variant4 = Task.Run(() => regexCount(sequences, "ag[act]gtaaa|tttac[agt]ct", options));
            var variant5 = Task.Run(() => regexCount(sequences, "agg[act]taaa|ttta[agt]cct", options));
            var variant1 = Task.Run(() => regexCount(sequences, "agggtaaa|tttaccct", options));
            var variant9 = Task.Run(() => regexCount(sequences, "agggtaa[cgt]|[acg]ttaccct", options));
            var variant8 = Task.Run(() => regexCount(sequences, "agggta[cgt]a|t[acg]taccct", options));

            Task.WaitAll(magicTask, variant1, variant2, variant3, variant4, variant5, variant6, variant7, variant8, variant9);

            static string regexCount(string s, string r, RegexOptions options)
            {
                int c = 0;
                var m = Regex.Match(s, r, options);
                while (m.Success) { c++; m = m.NextMatch(); }
                return r + " " + c;
            }
        }
    }
}
