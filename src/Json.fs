namespace Netatmo

open Thoth.Json.Net
open Netatmo.Model

[<AutoOpen>]
module AuthExtensions =
    type Auth with
        static member Decoder : Decoder<Model.Auth> =
            Decode.object (fun get ->
                {
                    AccessToken = get.Required.Field "access_token" Decode.string
                    ExpiresIn = get.Required.Field "expires_in" Decode.int
                    RefreshToken = get.Required.Field "refresh_token" Decode.string
                })


    type Error with
        static member Decoder : Decoder<Model.Error> =
            Decode.object (fun get ->
                {
                    Message = get.Required.Field "error" Decode.string
                })