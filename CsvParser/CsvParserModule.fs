module CsvParserModule

open FParsec
open System

let (<!>) (p: Parser<_,_>) label : Parser<_,_> =
    fun stream ->
        printfn "%A: Entering %s" stream.Position label
        let reply = p stream
        printfn "%A: Leaving %s (%A)" stream.Position label reply.Status
        reply

let ws = spaces
let str s = pstring s
let str_ws s = str s .>> ws

let nonQuoteChars = manySatisfy (fun c -> c <> '"')
let escapedQuote = str ("\"\"")
let quotedField = between (str "\"") (str "\"") (stringsSepBy nonQuoteChars escapedQuote) <!> "quoted field"

// let unquotedField = manySatisfy (function ',' | '\n' -> false | _ -> true)
let nonSpaceOrSep = manySatisfy (function ','|'\n'|' '|'\t' -> false | _ -> true) <!> "nonSpaceOrSep"
let allowedSpaces = many1Satisfy (function ' '|'\t' -> true | _ -> false) <!> "allowedSpaces"
// let allowedSpaces = manyChars (anyOf " \t")
// let unquotedField = pipe2 nonSpaceOrSep (many allowedSpaces) (fun hd tl -> hd::tl |> List.reduce (+)) <!> "unquoted field"
// let unquotedField = pipe2 nonSpaceOrSep (many allowedSpaces) (fun hd tl -> hd::tl |> List.reduce (+)) // <|>% (str "")
// let unquotedField = pipe2 nonSpaceOrSep (many (allowedSpaces >>. nonSpaceOrSep)) (fun hd tl -> hd::tl |> List.reduce (+)) // <|>% (str "")
let unquotedField = stringsSepBy nonSpaceOrSep allowedSpaces |>> (fun s -> s.TrimEnd([|' ';'\t'|])) <!> "unquoted field"
// sepEndBy

let csvValue, csvValueRef = createParserForwardedToRef()

let line = sepBy csvValue (str_ws ",")

do csvValueRef := choice[quotedField
                         unquotedField] <!> "csvValueRef"

// "aaa,bbb"

let csv = ws >>. sepBy line newline .>> ws .>> eof

let parseString csvString =
    match run csv csvString with
        | Failure(errorMsg, _, _) -> raise (new Exception(sprintf "Parsing error: %s" errorMsg))
        | Success(results, _, _) -> results