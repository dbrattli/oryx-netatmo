namespace Netatmo

open Thoth.Json.Net
open Netatmo.Model

[<AutoOpen>]
module AuthExtensions =
    type Scope with
        static member Decoder : Decoder<Scope> =
            Decode.string |> Decode.map (fun scope ->
                match scope with
                | "read_station" -> ReadStation
                | "read_thermostat" -> ReadThermostat
                | "write_thermostat" -> WriteThermostat
                | "read_camera" -> ReadCamera
                | "write_camera" -> WriteCamera
                | "access_camera" -> AccessCamera
                | "read_presense" -> ReadPresence
                | "access_presense" -> AccessPresence
                | "read_homecouch" -> ReadHomeCoach
                | _ -> ReadStation
            )

    type Auth with
        static member Decoder : Decoder<Auth> =
            Decode.object (fun get ->
                {
                    AccessToken = get.Required.Field "access_token" Decode.string
                    ExpiresIn = get.Required.Field "expires_in" Decode.int
                    RefreshToken = get.Required.Field "refresh_token" Decode.string
                    Scope = get.Required.Field "scope" (Decode.array Scope.Decoder)
                })

    type SyntaxError with
        static member Decoder : Decoder<SyntaxError> =
            Decode.object (fun get ->
                {
                    Error = get.Required.Field "error" Decode.string
                    Description = get.Required.Field "error_description" Decode.string
                })

    type Error with
        static member Decoder : Decoder<Error> =
            Decode.object (fun get ->
                {
                    Code = get.Required.At [ "error"; "code" ]  Decode.int
                    Message = get.Required.At [ "error"; "description" ] Decode.string
                })

    type Administrative with
        static member Decoder : Decoder<Administrative> =
            Decode.object (fun get -> {
                Language = get.Required.Field "lang" Decode.string
                Locale = get.Required.Field "reg_locale" Decode.string
                Country = get.Required.Field "country" Decode.string
                Unit = get.Required.Field "unit" Decode.int
                WindUnit = get.Required.Field "windunit" Decode.int
                PressureUnit = get.Required.Field "pressureunit" Decode.int
                FeelLikeAlgo = get.Required.Field "feel_like_algo" Decode.int
            })

    type User with
        static member Decoder : Decoder<User> =
            Decode.object (fun get -> {
                Mail = get.Required.Field "mail" Decode.string
                Administrative = get.Required.Field "administrative" Administrative.Decoder
            })

    type DashboardData with
        static member Decoder dataTypes : Decoder<DashboardData> =
            Decode.object (fun get -> {
                TimeUtc = get.Required.Field "time_utc" Decode.int

                Temperature =
                    if dataTypes |> Seq.contains "Temperature" then
                        {
                            Temperature = get.Required.Field "Temperature" Decode.float
                            MinTemp = get.Required.Field "min_temp" Decode.float
                            MaxTemp = get.Required.Field "max_temp" Decode.float
                            DateMaxTemp = get.Required.Field "date_max_temp" Decode.int
                            DateMinTemp = get.Required.Field "date_min_temp" Decode.int
                            TempTrend = get.Required.Field "temp_trend" Decode.string
                        } |> Some
                    else None

                Co2 = get.Optional.Field "CO2" Decode.int
                Humidity = get.Optional.Field "Humidity" Decode.int

                Wind =
                    if dataTypes |> Seq.contains "Wind" then
                        {
                            WindStrenght = get.Required.Field "wind_strength" Decode.int
                            WindAngle = get.Required.Field "wind_angle" Decode.int
                            GustStrength = get.Required.Field "gust_strength" Decode.int
                            GustAngle = get.Required.Field "gust_angle" Decode.int
                            MaxWindStrenght = get.Required.Field "max_wind_str" Decode.int
                            MaxWindAngle =  get.Required.Field "max_wind_angle" Decode.int
                            DateMaxWindStrength = get.Required.Field "date_max_wind_str" Decode.int
                        } |> Some
                    else None
                Rain =
                    if dataTypes |> Seq.contains "Wind" then
                        {
                            Rain = get.Required.Field "rain" Decode.int
                            SumRain24 = get.Required.Field "sum_rain_24" Decode.int
                            SumRain1 = get.Required.Field "sum_rain_1" Decode.int
                        } |> Some
                    else None
            })

    type Module with
     static member Decoder : Decoder<Module> =
            Decode.object (fun get ->
                let dataTypes = get.Required.Field "data_type" (Decode.list Decode.string)

                {
                    Id = get.Required.Field "_id" Decode.string
                    Type = get.Required.Field "type" Decode.string
                    ModuleName = get.Required.Field "module_name" Decode.string
                    DataType = dataTypes
                    LastSetup =   get.Required.Field "last_setup" Decode.int
                    BatteryPercent =  get.Required.Field "battery_percent" Decode.int
                    Reachable = get.Required.Field "reachable" Decode.bool
                    Firmware = get.Required.Field "firmware" Decode.int
                    LastMessage = get.Required.Field "last_message" Decode.int
                    LastSeen = get.Required.Field "last_seen" Decode.int
                    RfStatus = get.Required.Field "rf_status" Decode.int
                    BatteryVp = get.Required.Field "battery_vp" Decode.int

                    DashboardData = get.Required.Field "dashboard_data" (DashboardData.Decoder dataTypes)
                })

    type Place with
        static member Decoder : Decoder<Place> =
            Decode.object (fun get ->
                let location = get.Required.Field "location" (Decode.array Decode.float)

                {
                    TimeZone = get.Required.Field "timezone" Decode.string
                    Country = get.Required.Field "country" Decode.string
                    Altitude = get.Required.Field "altitude" Decode.int
                    Location = {
                        Latitude = location.[0]
                        Longitude = location.[1]
                    }
                })

    type Device with
        static member Decoder : Decoder<Device> =
            Decode.object (fun get ->
                let dataTypes = get.Required.Field "data_type" (Decode.array Decode.string)

                {
                    Id = get.Required.Field "_id" Decode.string
                    StationName = get.Required.Field "station_name" Decode.string
                    DateSetup = get.Required.Field "date_setup" Decode.int
                    LastSetup = get.Required.Field "last_setup" Decode.int
                    Type = get.Required.Field "type" Decode.string
                    LastStatusStore = get.Required.Field "last_status_store" Decode.int
                    ModuleName = get.Required.Field "module_name" Decode.string
                    Firmware = get.Required.Field "firmware" Decode.int
                    LastUpgrade = get.Required.Field "last_upgrade" Decode.int
                    WifiStatus = get.Required.Field "wifi_status" Decode.int
                    Reachable = get.Required.Field "reachable" Decode.bool
                    Co2Calibrating = get.Required.Field "co2_calibrating" Decode.bool
                    DataType = dataTypes
                    Place = get.Required.Field "place" Place.Decoder
                    Readonly = get.Optional.Field "read_only" Decode.bool |> Option.defaultValue false

                    DashboardData = get.Required.Field "dashboard_data" (DashboardData.Decoder dataTypes)
                    Modules = get.Required.Field "modules" (Decode.array Module.Decoder)
                })

    type StationData with
        static member Decoder : Decoder<StationData> =
            Decode.object (fun get -> {
                Devices = get.Required.At [ "body"; "devices" ] (Decode.list Device.Decoder)
                User = get.Required.At [ "body"; "user" ] User.Decoder
                Status = get.Required.Field "status" Decode.string
                TimeExec = get.Required.Field "time_exec" Decode.float
                TimeServer = get.Required.Field "time_server" Decode.int
            })