namespace Netatmo.Handler

open Oryx.Fetch
open Oryx.Handler
open Oryx.ResponseReaders

open Netatmo
open Netatmo.Model

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
        >=> json StationData.Decoder
