// Learn more about F# at http://fsharp.org

open System
open System.Net.Http

open FSharp.Control.Tasks.V2.ContextInsensitive
open FsConfig

open Oryx
open Netatmo.Auth
open Netatmo.Model
open Netatmo.Handler

type Config = {
    [<CustomName("CLIENT_ID")>]
    ClientId: string
    [<CustomName("CLIENT_SECRET")>]
    ClientSecret: string
    [<CustomName("USER_NAME")>]
    UserName: string
    [<CustomName("PASSWORD")>]
    Password: string
}

let authenticate (config: Config) (ctx: HttpContext) = task {
    let scopes = [ ReadStation ]
    let req = OAuth.clientCredentials config.ClientId config.ClientSecret config.UserName config.Password scopes

    let! result = runAsync req ctx
    match result with
    | Ok auth -> return auth
    | Error err -> return failwith <| sprintf "Error: %A" err
}

let getStationData (stationId: string) (ctx: HttpContext) = task {
    let req = Netatmo.getStationData stationId false
    let! result = runAsync req ctx
    match result with
    | Ok res -> return printfn "%A" res
    | Error err -> return failwith <| sprintf "Error: %A" err
}

let asyncMain (argv: string array) = task {
    printfn "F# Client"

    let config =
        match EnvConfig.Get<Config>() with
        | Ok config -> config
        | Error error -> failwith "Failed to read config"

    let handler = new HttpClientHandler()
    let cb = Func<HttpRequestMessage,Security.Cryptography.X509Certificates.X509Certificate2,Security.Cryptography.X509Certificates.X509Chain,Net.Security.SslPolicyErrors,bool>(fun message cert chain errors -> true)
    handler.ServerCertificateCustomValidationCallback <- cb
    use client = new HttpClient(handler)
    let ctx =
        Context.defaultContext
        |> Context.setHttpClient client

    let! auth = authenticate config ctx
    let ctx =
        ctx
        |> Context.setToken auth.AccessToken

    let stationId = argv.[0]
    do! getStationData stationId ctx
}

[<EntryPoint>]
let main argv =
    asyncMain(argv).GetAwaiter().GetResult()
    0 // return an integer exit code
