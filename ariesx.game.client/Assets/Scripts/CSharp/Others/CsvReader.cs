using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Protocol;
using System;
using UnityEngine.Profiling;

namespace Poukoute {
    public class CSVReader {
        public static Dictionary<string, string[]> ReadCSV(string path) {
            Dictionary<string, string[]> outPutDic = new Dictionary<string, string[]>();
            string[] csvText = File.ReadAllLines(path);
            int width = 0;

            for (int i = 0; i < csvText.Length; i++) {
                width = csvText[i].Length;
                if (width > 0) {
                    string[] row = SplitCsvLine(csvText[i]);
                    outPutDic.Add(row[0], row);
                }
            }
            return outPutDic;
        }
        public static string[] splitArray = { "\r\n", "\r", "\n" };
        public static void ReadCSV(TextAsset conf,
            Dictionary<ulong, string> dict) {
            string confText = conf.text;
            int width = 0;
            string[] lines = confText.CustomSplit(ref splitArray);
            for (int i = 0; i < lines.Length; i++) {
                width = lines[i].Length;
                if (width > 0) {
                    string[] row = SplitCsvLine(lines[i]);
                    ulong index = ulong.Parse(row[0]);
#if UNITY_EDITOR
                    if (dict.ContainsKey(index)) {
                        Debug.LogErrorf("{0} Already has key: " + row[0], conf.name);
                    }
#endif
                    dict.Add(index, row[1]);
                }
            }
        }


        private static string CsvLineSeperator = @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)";
        public static string[] SplitCsvLine(string line) {
            Profiler.BeginSample("SplitCsvLine");
            var value = (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
            CsvLineSeperator, System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                         select m.Groups[1].Value);
            Profiler.EndSample();
            return value.ToArray();
        }

        //static public string[] SplitCsvLine(string line) {
        //    List<string> buffer = new List<string>();
        //    bool quotationMark = false;
        //    int lastString = 0;
        //    int lastStringStarPoint = 0;
        //    int quotauionLeft = 0;
        //    int startIndex = 0;
        //    int quotauionRight = 0;
        //    bool lastCharIsquotauion = false;
        //    for (int i = 0; i < line.Length; i++) {
        //        if (line[i] == ',') {
        //            if (!(lastCharIsquotauion && i == startIndex)) {
        //                buffer.Add(line.Substring(startIndex, i - startIndex));
        //            }

        //            if (!quotationMark) {
        //                lastStringStarPoint = startIndex;
        //                lastString++;
        //            }
        //            startIndex = i + 1;
        //            lastCharIsquotauion = false;
        //        }
        //        if (!quotationMark) {
        //            if (i -1 >= 0&&line[i] == '"' && line[i - 1] == ',') {
        //                quotationMark = true;
        //                quotauionLeft = i;
        //            }
        //        } else {
        //            if (i+1< line.Length&&line[i] == '"' && line[i + 1] == ','||
        //                line[i] == '"'&& i + 1 == line.Length) {
        //                quotationMark = false;
        //                if (quotauionLeft != 0) {
        //                    buffer[lastString - 1] = line.Substring(lastStringStarPoint, quotauionLeft - 1);
        //                }
        //                buffer.RemoveRange(lastString, buffer.Count - lastString);
        //                buffer.Add(line.Substring(quotauionLeft + 1, i - quotauionLeft - 1));
        //                quotauionRight = i;
        //                lastCharIsquotauion = true;
        //                startIndex = i + 1;
        //            }
        //        }
        //    }
        //    int lastIndex = Math.Max(startIndex, quotauionRight + 1);
        //    if (lastIndex < line.Length) {
        //        buffer.Add(line.Substring(lastIndex, line.Length - lastIndex));
        //    }
        //    return buffer.ToArray();
        //}
    }
}
