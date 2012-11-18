module CsvParserModule

open FParsec
open System
open CsvParser
open ParsingUtils

type CsvParserState = { Delimeter: char }

let ws = spaces
let inline_ws = manySatisfy (function ' '|'\t' -> true | _ -> false) |>> ignore
let str s = pstring s
let str_inline_ws s = str s .>> inline_ws

let nonQuoteChars = manySatisfy (fun c -> c <> '"')
let escapedQuote = stringReturn "\"\"" "\""
let quotedField = between (str "\"") (str "\"") (stringsSepBy nonQuoteChars escapedQuote) .>> inline_ws

let nonSpaceOrSep : Parser<_, CsvParserState> = 
    fun stream ->
        let delimeter = stream.State.UserState.Delimeter
        stream |> manySatisfy (fun c ->
            if (c = delimeter) then false
            else match c with '\n'|' '|'\t' -> false | _ -> true)

let allowedInterCharSpaces = many1Satisfy (function ' '|'\t' -> true | _ -> false)
let unquotedField = stringsSepBy nonSpaceOrSep allowedInterCharSpaces |>> (fun s -> s.TrimEnd([|' ';'\t'|]))

let csvValue, csvValueRef = createParserForwardedToRef()

let line : Parser<string list, CsvParserState> =
    fun stream ->
        let delimeter = stream.State.UserState.Delimeter.ToString()
        stream |> sepBy csvValue (str_inline_ws delimeter)

do csvValueRef := choice[quotedField
                         unquotedField]

let csv = ws >>. sepBy line newline .>> ws .>> eof

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

let internal applyParser (parser: Parser<'Result,'UserState>) (stream: CharStream<'UserState>) =
    let reply = parser stream
    if reply.Status = Ok then
        Success(reply.Result, stream.UserState, stream.Position)
    else
        let error = ParserError(stream.Position, stream.UserState, reply.Error)
        Failure(error.ToString(stream), error, stream.UserState)

let internal runParserOnStream (parser: Parser<'Result,'UserState>) (ustate: 'UserState) (byteStream: System.IO.Stream) (encoding: System.Text.Encoding) =
    let leaveOpen = true
    let stream = new CharStream<'UserState>(byteStream, leaveOpen, encoding)
    stream.UserState <- ustate
    // stream.Name <- streamName
    applyParser parser stream

let parseString csvString delimeter =
    let state = { Delimeter=delimeter }
    let result = csvString |> runParserOnString csv state ""
    match result with
        | Failure(errorMsg, _, _) -> raise (new CsvParsingException(sprintf "Parsing error: %s" errorMsg))
        | Success(results, _, _) -> results

let parseStream stream encoding delimeter =
    let state = { Delimeter=delimeter }
    match runParserOnStream csv state stream encoding with
        | Failure(errorMsg, _, _) -> raise (new CsvParsingException(sprintf "Parsing error: %s" errorMsg))
        | Success(results, _, _) -> results

let parseCharStream charStream =
    match applyParser csv charStream with
        | Failure(errorMsg, _, _) -> raise (new CsvParsingException(sprintf "Parsing error: %s" errorMsg))
        | Success(results, _, _) -> results