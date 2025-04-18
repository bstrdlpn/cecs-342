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
using System.Xml.Linq;

namespace FileTypeReport {
  internal static class Program {
    // 1. Enumerate all files in a folder recursively
    private static IEnumerable<string> EnumerateFilesRecursively(string path) {
      // TODO: Fill in your code here.
        if (!Directory.exists (path))
        {
            yield break // directory from path does not exist
        }
        // yields all files in current directory
        foreach (string file in Directory.EnumerateFiles (path)) 
        {
            try
            {
                yield return file; // our generator
            }
            catch (UnauthorizedAccessException)
            {
                continue; // skips protected folders
            }
        }
        // yields all files in subdirectories
        foreach (string directory in Directory.EnumerateDirectories (path))
        {
            try
            {
                foreach (string file in EnumerateFilesRecursively (directory))
                {
                    yield return file;
                }
            }
            catch (UnauthorizedAccessException)
            {
                continue;
            }
            catch (PathTooLongException)
            {
                continue; // ignores really long paths (happens if we look at root)
            }
        }
    }

    // Human readable byte size
    private static string FormatByteSize(long byteSize) {
        // TODO: Fill in your code here.

        // array of strings denoting file size unit
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double formattedSize = byteSize
        int sizesIndex = 0

        while (formattedSize >= 1024 && sizesIndex < sizes.Length - 1)
        // Divide the bytesize by 1024, number of times divided moves the 
        // sizesIndex up to correctly select the most pertinent size
        // Ex: 3145728 bytes / 1024 twice == "3 MB"
        {
            formattedSize /= 1024; 
            sizesIndex++;
        }
        // String formatting
        // {0:0.##} : use first parameter formattedSize, :0.## show a digit before
        // the decimal, optional to use 2 following digits
        // Ex: byteSize 3955623 should return "3.77 MB" 
        return string.Format("{0:0.##} {1}", formattedSize, sizes[sizesIndex])
    }

    // Create an HTML report file
    private static XDocument CreateReport(IEnumerable<string> files) {
      // 2. Process data
      var query =
        from file in files
        // TODO: Fill in your code here.
        select new {
          Type =      // TODO: Fill in your code here.
          Count =     // TODO: Fill in your code here.
          TotalSize = // TODO: Fill in your code here.
        };

      // 3. Functionally construct XML
      var alignment = new XAttribute("align", "right");
      var style = "table, th, td { border: 1px solid black; }";

      var tableRows = // TODO: Fill in your code here.
        
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
}
