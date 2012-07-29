module CsvParserModule

open FParsec
open System

let ws = spaces
let str s = pstring s
let str_ws s = str s .>> ws

let nonQuoteChars = manySatisfy (fun c -> c <> '"')
let escapedQuote = str ("\"\"")
let quotedField = between (str "\"") (str "\"") (stringsSepBy nonQuoteChars escapedQuote)

let unquotedField = manySatisfy (function ',' | '\n' -> false | _ -> true)

let csvValue, csvValueRef = createParserForwardedToRef()

let line = sepBy csvValue (str ",")

do csvValueRef := choice[quotedField
                         unquotedField]

let csv = sepBy line newline .>> ws .>> eof

let parseString csvString =
    match run csv csvString with
        | Failure(errorMsg, _, _) -> raise (new Exception(sprintf "Parsing error: %s" errorMsg))
        | Success(results, _, _) -> results