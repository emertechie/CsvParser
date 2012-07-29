using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CsvParser.Tests
{
	public class CsvTests
	{
		[Fact]
		public void CanParseBasicUnquotedValuesWithNoWhitespace()
		{
			var parser = new CsvParser();

			IEnumerable<string[]> lines = parser.Parse("aaa,bbb");

			Assert.Equal(1, lines.Count());
			var line = lines.Single();

			Assert.Equal(2, line.Length);
			Assert.Equal("aaa", line[0]);
			Assert.Equal("bbb", line[1]);
		}

		// Note: Keeping it simple for now
		[Fact]
		public void WhiteSpaceIsNotRemovedFromAroundUnquotedValues()
		{
			var parser = new CsvParser();

			IEnumerable<string[]> lines = parser.Parse(" aaa\t, \t bbb ");

			Assert.Equal(1, lines.Count());
			var line = lines.Single();

			Assert.Equal(2, line.Length);
			Assert.Equal(" aaa\t", line[0]);
			Assert.Equal(" \t bbb ", line[1]);
		}

		/*
		[Fact]
		public void WhiteSpaceIsRemovedFromAroundUnquotedValues()
		{
			var parser = new CsvParser();

			IEnumerable<string[]> lines = parser.Parse(" aaa\t, bbb \t");

			Assert.Equal(1, lines.Count());
			var line = lines.Single();

			Assert.Equal(2, line.Length);
			Assert.Equal("aaa", line[0]);
			Assert.Equal("bbb", line[1]);
		}
		*/

		[Fact]
		public void WhitespaceInMiddleOfUnquotedValuesIsPreserved()
		{
			var parser = new CsvParser();

			IEnumerable<string[]> lines = parser.Parse("a a a,b b b");

			Assert.Equal(1, lines.Count());
			var line = lines.Single();

			Assert.Equal(2, line.Length);
			Assert.Equal("a a a", line[0]);
			Assert.Equal("b b b", line[1]);
		}

		[Fact]
		public void CanParseBasicQuotedValuesWithNoWhitespace()
		{
			var parser = new CsvParser();

			IEnumerable<string[]> lines = parser.Parse("\"aaa\",\"bbb\"");

			Assert.Equal(1, lines.Count());
			var line = lines.Single();

			Assert.Equal(2, line.Length);
			Assert.Equal("aaa", line[0]);
			Assert.Equal("bbb", line[1]);
		}

		[Fact]
		public void WhiteSpaceIsNotRemovedFromAroundQuotedValues()
		{
			var parser = new CsvParser();

			IEnumerable<string[]> lines = parser.Parse(" \" aaa\t\"\t,\t\"\tbbb \" ");

			Assert.Equal(1, lines.Count());
			var line = lines.Single();

			Assert.Equal(2, line.Length);
			Assert.Equal(" aaa\t", line[0]);
			Assert.Equal("\tbbb ", line[1]);
		}

		/*
		[Fact]
		public void CanParseBasicQuotedValuesWithWhitespace()
		{
			var parser = new CsvParser();

			IEnumerable<string[]> lines = parser.Parse(" \" aaa\t\"\t,\t\"\tbbb \" ");

			Assert.Equal(1, lines.Count());
			var line = lines.Single();

			Assert.Equal(2, line.Length);
			Assert.Equal(" aaa\t", line[0]);
			Assert.Equal("\tbbb ", line[1]);
		}*/
	}
}
