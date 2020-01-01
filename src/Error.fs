namespace Netatmo

open System.Net.Http
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Netatmo.Model

module Error =
    [<Literal>]
    let Url = "https://api.netatmo.com/oauth2/"

    let decodeError (response: HttpResponseMessage) : Task<HandlerError<Error>> = task {
        if response.Content.Headers.ContentType.MediaType = "application/json" then
            use! stream = response.Content.ReadAsStreamAsync ()
            let decoder = Error.Decoder
            let! result = decodeStreamAsync decoder stream
            match result with
            | Ok err -> return ResponseError err
            | Error reason -> return Panic <| JsonDecodeException reason
        else
            let error = { Code = 404; Message = "Unknown Error" }
            return ResponseError error
    }
    let decodeSyntaxError (response: HttpResponseMessage) : Task<HandlerError<SyntaxError>> = task {
        if response.Content.Headers.ContentType.MediaType = "application/json" then
            use! stream = response.Content.ReadAsStreamAsync ()
            let decoder = SyntaxError.Decoder
            let! result = decodeStreamAsync decoder stream
            match result with
            | Ok err -> return ResponseError err
            | Error reason -> return Panic <| JsonDecodeException reason
        else
            let error = { Error = "unknown error"; Description = "" }
            return ResponseError error
    }
