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
let escapedQuote = stringReturn "\"\"" "\""
let quotedField = between (str "\"") (str "\"") (stringsSepBy nonQuoteChars escapedQuote) .>> ws <!> "quoted field"

let nonSpaceOrSep = manySatisfy (function ','|'\n'|' '|'\t' -> false | _ -> true) <!> "nonSpaceOrSep"
let allowedInterCharSpaces = many1Satisfy (function ' '|'\t' -> true | _ -> false) <!> "allowedSpaces"
let unquotedField = stringsSepBy nonSpaceOrSep allowedInterCharSpaces |>> (fun s -> s.TrimEnd([|' ';'\t'|])) <!> "unquoted field"
//let unquotedField = manyCharsTill (noneOf ",\n") (pchar ',') << doesn't work because it consumes the comma

let csvValue, csvValueRef = createParserForwardedToRef()

let line = sepBy csvValue (str_ws ",")

do csvValueRef := choice[quotedField
                         unquotedField] <!> "csvValueRef"

let csv = ws >>. sepBy line newline .>> ws .>> eof

let parseString csvString =
    match run csv csvString with
        | Failure(errorMsg, _, _) -> raise (new Exception(sprintf "Parsing error: %s" errorMsg))
        | Success(results, _, _) -> results