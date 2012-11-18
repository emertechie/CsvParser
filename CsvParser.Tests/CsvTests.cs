using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace CsvParser.Tests
{
	public class CsvTests
	{
		[Fact]
		public void CanParseBasicUnquotedValuesWithNoWhitespace()
		{
			var parser = CreateCsvParser();

			IEnumerable<string[]> lines = parser.Parse("aaa|bbb");

			Assert.Equal(1, lines.Count());
			var line = lines.Single();

			Assert.Equal(2, line.Length);
			Assert.Equal("aaa", line[0]);
			Assert.Equal("bbb", line[1]);
		}

		[Fact]
		public void CanParseBasicUnquotedValuesWithNoWhitespace_WithDifferentDelimeter()
		{
			var parser = CreateCsvParser(delimeter: ',');

			IEnumerable<string[]> lines = parser.Parse("aaa,bbb");

			Assert.Equal(1, lines.Count());
			var line = lines.Single();

			Assert.Equal(2, line.Length);
			Assert.Equal("aaa", line[0]);
			Assert.Equal("bbb", line[1]);
		}

		[Fact]
		public void SpacesAreRemovedFromAroundUnquotedValues()
		{
			var parser = CreateCsvParser();

			IEnumerable<string[]> lines = parser.Parse(" a | b ");

			Assert.Equal(1, lines.Count());
			var line = lines.Single();

			Assert.Equal(2, line.Length);
			Console.WriteLine("Expected:'{0}', Actual:'{1}'", "a", line[0]);
			Assert.Equal("a", line[0]);
			Assert.Equal("b", line[1]);
		}

		[Fact]
		public void TabsAreRemovedFromAroundUnquotedValues()
		{
			var parser = CreateCsvParser();

			IEnumerable<string[]> lines = parser.Parse(" aaa\t| bbb \t");

			Assert.Equal(1, lines.Count());
			var line = lines.Single();

			Assert.Equal(2, line.Length);
			Console.WriteLine("Expected:'{0}', Actual:'{1}'", "aaa", line[0]);
			Assert.Equal("aaa", line[0]);
			Assert.Equal("bbb", line[1]);
		}

		[Fact]
		public void WhitespaceInMiddleOfUnquotedValuesIsPreserved()
		{
			var parser = CreateCsvParser();

			IEnumerable<string[]> lines = parser.Parse("a a a|b b b");

			Assert.Equal(1, lines.Count());
			var line = lines.Single();

			Assert.Equal(2, line.Length);
			Assert.Equal("a a a", line[0]);
			Assert.Equal("b b b", line[1]);
		}

		[Fact]
		public void CanParseBasicQuotedValuesWithNoWhitespace()
		{
			var parser = CreateCsvParser();

			IEnumerable<string[]> lines = parser.Parse("\"aaa\"|\"bbb\"");

			Assert.Equal(1, lines.Count());
			var line = lines.Single();

			Assert.Equal(2, line.Length);
			Assert.Equal("aaa", line[0]);
			Assert.Equal("bbb", line[1]);
		}

		[Fact]
		public void WhiteSpaceWithinQuotesIsPreserved()
		{
			var parser = CreateCsvParser();

			IEnumerable<string[]> lines = parser.Parse("\" aaa\t\"|\"\tbbb \"");

			Assert.Equal(1, lines.Count());
			var line = lines.Single();

			Assert.Equal(2, line.Length);
			Assert.Equal(" aaa\t", line[0]);
			Assert.Equal("\tbbb ", line[1]);
		}

		[Fact]
		public void WhiteSpaceSurroundingQuotesIsIgnored()
		{
			var parser = CreateCsvParser();

			IEnumerable<string[]> lines = parser.Parse(" \" aaa\t\"\t|\t\"\tbbb \" ");

			Assert.Equal(1, lines.Count());
			var line = lines.Single();

			Assert.Equal(2, line.Length);
			Assert.Equal(" aaa\t", line[0]);
			Assert.Equal("\tbbb ", line[1]);
		}

		[Fact]
		public void CanParseLineWithMixedQuotedAndNonQuotedValues()
		{
			var parser = CreateCsvParser();

			IEnumerable<string[]> lines = parser.Parse("aaa|\"bbb\"|ccc");

			Assert.Equal(1, lines.Count());
			var line = lines.Single();

			Assert.Equal(3, line.Length);
			Assert.Equal("aaa", line[0]);
			Assert.Equal("bbb", line[1]);
			Assert.Equal("ccc", line[2]);
		}

		[Fact]
		public void EscapedQuotesWithinQuotesArePreserved()
		{
			var parser = CreateCsvParser();

			IEnumerable<string[]> lines = parser.Parse("aaa|\"\"\"bbb\"\"\"");

			Assert.Equal(1, lines.Count());
			var line = lines.Single();

			Assert.Equal(2, line.Length);
			Assert.Equal("aaa", line[0]);
			Assert.Equal("\"bbb\"", line[1]);
		}

		[Fact]
		public void NewlineWithinQuotesIsPreserved()
		{
			var parser = CreateCsvParser();

			IEnumerable<string[]> lines = parser.Parse("aaa|\"b1\nb2\nb3\"|ccc");

			Assert.Equal(1, lines.Count());
			var line = lines.Single();

			Assert.Equal(3, line.Length);
			Assert.Equal("aaa", line[0]);
			Assert.Equal("b1\nb2\nb3", line[1]);
			Assert.Equal("ccc", line[2]);
		}

		[Fact]
		public void DelimeterWithinQuotesIsPreserved()
		{
			var parser = CreateCsvParser();

			IEnumerable<string[]> lines = parser.Parse("aaa|\"b1,b2\"|ccc");

			Assert.Equal(1, lines.Count());
			var line = lines.Single();

			Assert.Equal(3, line.Length);
			Assert.Equal("aaa", line[0]);
			Assert.Equal("b1,b2", line[1]);
			Assert.Equal("ccc", line[2]);
		}

		[Fact]
		public void EmptyFieldsAreReturnedAsEmptyStrings()
		{
			var parser = CreateCsvParser();

			IEnumerable<string[]> lines = parser.Parse("aaa||ccc");

			Assert.Equal(1, lines.Count());
			var line = lines.Single();

			Assert.Equal(3, line.Length);
			Assert.Equal("aaa", line[0]);
			Assert.Equal("", line[1]);
			Assert.Equal("ccc", line[2]);
		}

		[Fact]
		public void CanParseMultipleLines()
		{
			var parser = CreateCsvParser();

			string[][] lines = parser.Parse("aaa|bbb\nccc|ddd").ToArray();

			Assert.Equal(2, lines.Length);

			Assert.Equal(2, lines[0].Length);
			Assert.Equal("aaa", lines[0][0]);
			Assert.Equal("bbb", lines[0][1]);

			Assert.Equal(2, lines[1].Length);
			Assert.Equal("ccc", lines[1][0]);
			Assert.Equal("ddd", lines[1][1]);
		}

		[Fact]
		public void CanParseMultipleLines_WithQuotesInLastValue()
		{
			var parser = CreateCsvParser();

			string[][] lines = parser.Parse("aaa|\"bbb\"\nccc|\"ddd\"").ToArray();

			Assert.Equal(2, lines.Length);

			Assert.Equal(2, lines[0].Length);
			Assert.Equal("aaa", lines[0][0]);
			Assert.Equal("bbb", lines[0][1]);

			Assert.Equal(2, lines[1].Length);
			Assert.Equal("ccc", lines[1][0]);
			Assert.Equal("ddd", lines[1][1]);
		}

		[Fact]
		public void CanParseMultipleLines_AllQuoted()
		{
			var parser = CreateCsvParser();

			string[][] lines = parser.Parse("\"aaa\"|\"bbb\"\n\"ccc\"|\"ddd\"").ToArray();

			Assert.Equal(2, lines.Length);

			Assert.Equal(2, lines[0].Length);
			Assert.Equal("aaa", lines[0][0]);
			Assert.Equal("bbb", lines[0][1]);

			Assert.Equal(2, lines[1].Length);
			Assert.Equal("ccc", lines[1][0]);
			Assert.Equal("ddd", lines[1][1]);
		}

		[Fact]
		public void CanParseMultipleLines_AllWithEscapedQuotes()
		{
			var parser = CreateCsvParser();

			string[][] lines = parser.Parse("\"\"\"aaa\"\"\"|\"\"\"bbb\"\"\"\n\"\"\"ccc\"\"\"|\"\"\"ddd\"\"\"").ToArray();

			Assert.Equal(2, lines.Length);

			Assert.Equal(2, lines[0].Length);
			Assert.Equal("\"aaa\"", lines[0][0]);
			Assert.Equal("\"bbb\"", lines[0][1]);

			Assert.Equal(2, lines[1].Length);
			Assert.Equal("\"ccc\"", lines[1][0]);
			Assert.Equal("\"ddd\"", lines[1][1]);
		}

		[Fact]
		public void CanParseMultipleLines_WithSeparatorAtBeginningOfSecondLine()
		{
			var parser = CreateCsvParser();

			string[][] lines = parser.Parse("aaa|bbb\n|ccc").ToArray();

			Assert.Equal(2, lines.Length);

			Assert.Equal(2, lines[0].Length);
			Assert.Equal("aaa", lines[0][0]);
			Assert.Equal("bbb", lines[0][1]);

			Assert.Equal(2, lines[1].Length);
			Assert.Equal("", lines[1][0]);
			Assert.Equal("ccc", lines[1][1]);
		}

		[Fact(Skip="TODO")]
		public void EmptyLinesAreIgnored()
		{
			var parser = CreateCsvParser();

			// Set up empty lines at start, middle, and end:
			string[][] lines = parser.Parse("\n\naaa|bbb\n\nccc|ddd\n\n").ToArray();

			Assert.Equal(2, lines.Length);

			Assert.Equal(2, lines[0].Length);
			Assert.Equal("aaa", lines[0][0]);
			Assert.Equal("bbb", lines[0][1]);

			Assert.Equal(2, lines[1].Length);
			Assert.Equal("ccc", lines[1][0]);
			Assert.Equal("ddd", lines[1][1]);
		}

		[Fact]
		public void LinesWithDifferentColumnCountsAreHandledWithoutError()
		{
			var parser = CreateCsvParser();

			string[][] lines = parser.Parse("aaa|bbb\nccc|ddd|eee\nfff").ToArray();

			Assert.Equal(3, lines.Length);

			Assert.Equal(2, lines[0].Length);
			Assert.Equal("aaa", lines[0][0]);
			Assert.Equal("bbb", lines[0][1]);

			Assert.Equal(3, lines[1].Length);
			Assert.Equal("ccc", lines[1][0]);
			Assert.Equal("ddd", lines[1][1]);
			Assert.Equal("eee", lines[1][2]);

			Assert.Equal(1, lines[2].Length);
			Assert.Equal("fff", lines[2][0]);
		}

		private static CsvParser CreateCsvParser(char delimeter = '|')
		{
			return new CsvParser(delimeter);
		}
	}
}
