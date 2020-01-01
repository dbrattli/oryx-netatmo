namespace Netatmo.Auth

open Oryx
open Oryx.ResponseReaders

open Netatmo
open Netatmo.Model
open Netatmo.Error

[<RequireQualifiedAccess>]
module OAuth =
    [<Literal>]
    let Url = "https://api.netatmo.com/oauth2/"

    let authorize (clientId: string) (redirectUri: string) (scopes: Scope seq) (state: string) =
        let query = [
            "client_id", clientId
            "redirect_uri", redirectUri
            "scope", scopes |> Seq.map string |> String.concat "."
            "state", state
        ]

        POST
        >=> addQuery query
        >=> setUrl (Url + "authorize")
        >=> fetch
        >=> withError decodeSyntaxError
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
        >=> withError decodeSyntaxError
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
        >=> withError decodeSyntaxError
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
        >=> withError decodeSyntaxError
        >=> json Auth.Decoder
