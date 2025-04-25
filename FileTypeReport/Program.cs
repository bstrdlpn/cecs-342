// Program.cs
//
// CECS 342 Assignment 2
// File Type Report
// Contributors:
//    Justin Do
//    Anh Le
//    Jonathan Martinez
//    Long Nguyen

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace FileTypeReport {
  internal static class Program {
    // 1. Enumerate all files in a folder recursively
    private static IEnumerable<string> EnumerateFilesRecursively(string path) {
      // TODO: Fill in your code here.
        if (!Directory.Exists (path))
        {
            Console.WriteLine($"[DEBUG] Directory not found: {path}");
            yield break; // directory from path does not exist
        }

        foreach (var file in Directory.EnumerateFiles(path))
        {
            yield return file;
        }

        // if there are subdirectories
        foreach (var directory in Directory.EnumerateDirectories(path))
        {
            foreach (var file in EnumerateFilesRecursively(directory))
            {
                yield return file;
            }
        } 
    
    }
        

    // Human readable byte size
    private static string FormatByteSize(long byteSize) {
        // TODO: Fill in your code here.

        // array of strings denoting file size unit
        string[] sizes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB"};
        double formattedSize = byteSize;
        int sizesIndex = 0;

        while (formattedSize >= 1024 && sizesIndex < sizes.Length - 1)
        // Divide the bytesize by 1024, number of times divided moves the 
        // sizesIndex up to correctly select the most pertinent size
        // Ex: 3145728 bytes / 1024 twice == "3 MB"
        {
            formattedSize /= 1000; 
            sizesIndex++;
        }
        // String formatting
        // {0:0.##} : use first parameter formattedSize, :0.## show a digit before
        // the decimal, optional to use 2 following digits
        // Ex: byteSize 3955623 should return "3.77 MB" 
        return string.Format("{0:0.##} {1}", formattedSize, sizes[sizesIndex]);
    }

    // Create an HTML report file
    private static XDocument CreateReport(IEnumerable<string> files) {
      // 2. Process data
      var query =
        from file in files
        let ext = Path.GetExtension(file).ToLower() // Stores the lowercase extension for each file
        group file by ext into fileGroup            // organize all files into groups
        select new {
          Type = fileGroup.Key == "" ? "[no extension]" : fileGroup.Key, // Checks if extension is empty & labels it
          Count = fileGroup.Count(), //count the amount of that file type in the group
          TotalSize = FormatByteSize(fileGroup.Sum(f => new FileInfo(f).Length)) //use formatbytesize to display how much storage the files take up
        };

      // 3. Functionally construct XML
      var alignment = new XAttribute("align", "right");
      var style = "table, th, td { border: 1px solid black; }";

      var tableRows = // TODO: Fill in your code here.
        from item in query
        select new XElement("tr",
            new XElement("td", item.Type),
            new XElement("td", new XAttribute("align","right"), item.Count),
            new XElement("td", new XAttribute("align", "right"), item.TotalSize)
        );
        
      var table = new XElement("table",
        new XElement("thead",
          new XElement("tr",
            new XElement("th", "Type"),
            new XElement("th", "Count"),
            new XElement("th", "Total Size"))),
        new XElement("tbody", tableRows));

      return new XDocument(
        new XDocumentType("html", null, null, null),
          new XElement("html",
            new XElement("head",
              new XElement("title", "File Report"),
              new XElement("style", style)),
            new XElement("body", table)));
    }
/*
    // Console application with two arguments
    public static void Main(string[] args) {
      try {
        string inputFolder = args[0];
        string reportFile  = args[1];
        CreateReport(EnumerateFilesRecursively(inputFolder)).Save(reportFile);
      } catch {
        Console.WriteLine("Usage: FileTypeReport <folder> <report file>");
      }
    }
  }
*/
    public static void Main(string[] args) {
  try {
    if (args.Length < 2) {
      Console.WriteLine($"[DEBUG] Not enough arguments (received {args.Length})");
      Console.WriteLine("Usage: FileTypeReport <folder> <report file>");
      return;
    }

    string inputFolder = args[0];
    string reportFile = args[1];

    Console.WriteLine($"[DEBUG] Input folder: {inputFolder}");
    Console.WriteLine($"[DEBUG] Report output: {reportFile}");

    var files = EnumerateFilesRecursively(inputFolder).ToList();
    Console.WriteLine($"[DEBUG] Found {files.Count} files");

    var report = CreateReport(files);
    report.Save(reportFile);
    Console.WriteLine("[DEBUG] Report saved successfully");
  }
  catch (Exception ex) {
    Console.WriteLine("[ERROR] Something went wrong:");
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);
  }
}

}}
