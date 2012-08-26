namespace CsvParser

open System

type public CsvParsingException =
    inherit Exception
    new() = { inherit Exception() }
    new(message) = { inherit Exception(message) }
