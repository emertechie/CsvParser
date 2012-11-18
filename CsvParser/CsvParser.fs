namespace CsvParser
open CsvParserModule
open System.IO
open FParsec

type public CsvParser() = 

    member x.Parse csv =
        let results = parseString csv
        results |> Seq.map (fun line -> line |> List.toArray)

    member x.ParseFile fileName encoding =
        use stream = File.OpenRead fileName
        x.ParseStream stream encoding

    member x.ParseStream stream encoding =
        let results = parseStream stream encoding
        results |> Seq.map (fun line -> line |> List.toArray)

    member x.ParseCharStream charStream =
        let results = parseCharStream charStream
        results |> Seq.map (fun line -> line |> List.toArray)
