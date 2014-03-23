csvparser
=========

Simple CsvParser class. Understands quoted and non-quoted values. Unit tests included.

Parser works fine, but there are some outstanding issues:

* Parser may return empty lines. Especially at the end of file. You will have to filter those out yourself for now
* I have to include a custom build of FParsec at the moment. Waiting on a new Nuget build which will include a fix I made for the CharStream class

Installation
------------

Available on NuGet:
```
Install-Package CsvParser
```

Usage
-----

```C#
var parser = new CsvParser(delimeter: '|');

// Parse a string. Result will be [ [a","b"], ["c", "d"] ]
IEnumerable<string[]> lines = parser.Parse("a|\"b\"\nc|d");

// Parse a file
IEnumerable<string[]> lines = parser.ParseFile("filename.csv", Encoding.UTF8));
```
