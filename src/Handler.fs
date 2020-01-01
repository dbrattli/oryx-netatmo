namespace Netatmo.Handler

open System.Threading.Tasks
open System.Net.Http

open FSharp.Control.Tasks.V2.ContextInsensitive

open Oryx
open Oryx.ResponseReaders

open Netatmo
open Netatmo.Model
open Netatmo.Error

[<RequireQualifiedAccess>]
module Netatmo =
    [<Literal>]
    let Url = "https://api.netatmo.com/api/"

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
