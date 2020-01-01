namespace Netatmo.Handler

open System.Threading.Tasks
open System.Net.Http

open FSharp.Control.Tasks.V2.ContextInsensitive

open Oryx
open Oryx.ResponseReaders

open Netatmo
open Netatmo.Model

[<RequireQualifiedAccess>]
module Netatmo =
    [<Literal>]
    let Url = "https://api.netatmo.com/api/"

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

    let getStationData (deviceId: string) (getFavorites: bool) =
        let query = [
            "device_id", deviceId
            "get_favorites", string getFavorites
        ]

        GET
        >=> setUrl (Url + "getstationsdata")
        >=> addQuery query
        >=> fetch
        >=> withError decodeError
        >=> json StationData.Decoder
