namespace Netatmo.Auth

open System.Net.Http
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open Oryx
open Oryx.ResponseReaders

open Netatmo
open Netatmo.Model


[<RequireQualifiedAccess>]
module OAuth =
    [<Literal>]
    let Url = "https://api.netatmo.com/oauth2/"

    let decodeError (response: HttpResponseMessage) : Task<HandlerError<Error>> = task {
        if response.Content.Headers.ContentType.MediaType.Contains "application/json" then
            use! stream = response.Content.ReadAsStreamAsync ()
            let decoder = Error.Decoder
            let! result = decodeStreamAsync decoder stream
            match result with
            | Ok err -> return ResponseError err
            | Error reason -> return Panic <| JsonDecodeException reason
        else
            let error = { Message = "Unknown error" }
            return ResponseError error
    }

    let authorize (clientId: string) (redirectUri: string) (scopes: Scope seq) (state: string) =
        let query = [
            "client_id", clientId
            "redirect_uri", redirectUri
            "scope", scopes |> Seq.map string |> String.concat "."
            "state", state
        ]

        GET
        >=> addQuery query
        >=> setUrl (Url + "authorize")
        >=> fetch
        >=> withError decodeError
        >=> json Auth.Decoder

    let token (clientId: string) (clientSecret: string) (code: string) (redirectUri: string) (scopes: Scope seq) =
        let content = Content.UrlEncoded [
            "grant_type", "authorization_code"
            "client_id", clientId
            "client_secret", clientSecret
            "code", code
            "redirect_uri", redirectUri
            "scope", scopes |> Seq.map string |> String.concat "."
        ]

        POST
        >=> setContent content
        >=> setUrl (Url + "token")
        >=> fetch
        >=> withError decodeError
        >=> json Auth.Decoder


    let clientCredentials (clientId: string) (clientSecret: string) (userName: string) (password: string) (scopes: Scope seq) =
        let content = Content.UrlEncoded [
            "grant_type", "password"
            "client_id", clientId
            "client_secret", clientSecret
            "username", userName
            "password", password
            "scope", scopes |> Seq.map string |> String.concat " "
        ]

        POST
        >=> setContent content
        >=> setUrl (Url + "token")
        >=> fetch
        >=> withError decodeError
        >=> json Auth.Decoder

    let refresh (clientId: string) (clientSecret: string) (refreshToken: string) =
        let content = Content.UrlEncoded [
            "grant_type", "refresh_token"
            "client_id", clientId
            "client_secret", clientSecret
            "refresh_token", refreshToken
        ]

        POST
        >=> setContent content
        >=> setUrl Url
        >=> fetch
        >=> withError decodeError
        >=> json Auth.Decoder
