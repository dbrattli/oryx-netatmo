// Learn more about F# at http://fsharp.org

open System
open FSharp.Control.Tasks.V2.ContextInsensitive
open System.Net.Http
open Oryx
open FsConfig
open Netatmo.Auth
open Netatmo.Model

type Config = {
    [<CustomName("CLIENT_ID")>]
    ClientId: string
    [<CustomName("CLIENT_SECRET")>]
    ClientSecret: string
    [<CustomName("USERNAME")>]
    UserName: string
    [<CustomName("PASSWORD")>]
    Password: string
}

let authenticate (config: Config) (ctx: HttpContext) = task {
    let scopes = [ ReadStation ]
    let req = OAuth.clientCredentials config.ClientId config.ClientSecret config.UserName config.Password scopes

    let! result = runAsync req ctx
    match result with
    | Ok auth -> printfn "Auth: %A" auth
    | Error err -> printfn "Error: %A" err
}

let asyncMain argv = task {
    printfn "F# Client"

    let config =
        match EnvConfig.Get<Config>() with
        | Ok config -> config
        | Error error -> failwith "Failed to read config"

    use client = new HttpClient ()
    let ctx =
        Context.defaultContext
        |> Context.setHttpClient client

    do! authenticate config ctx
}

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    asyncMain().GetAwaiter().GetResult()
    0 // return an integer exit code
