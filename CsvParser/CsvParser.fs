namespace CsvParser
open CsvParserModule
open System.IO
open FParsec

type public CsvParser(delimeter: char) = 

    member x.Parse csvString =
        let results = parseString csvString delimeter
        results |> Seq.map (fun line -> line |> List.toArray)

    member x.ParseFile fileName encoding =
        use stream = File.OpenRead fileName
        x.ParseStream stream encoding

    member x.ParseStream stream encoding =
        let results = parseStream stream encoding delimeter
        results |> Seq.map (fun line -> line |> List.toArray)

    member x.ParseCharStream charStream =
        let results = parseCharStream charStream
        results |> Seq.map (fun line -> line |> List.toArray)
