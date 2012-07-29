namespace CsvParser
open CsvParserModule

type public CsvParser() = 
    member x.Parse csv =
        let results = parseString csv
        results |> Seq.map (fun line -> line |> List.toArray)