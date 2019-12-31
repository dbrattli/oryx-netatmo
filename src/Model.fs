namespace Netatmo

module Model =
    type Scope =
        /// To retrieve weather station data (Getstationsdata, Getmeasure)
        | ReadStation
        /// To retrieve thermostat data ( Homestatus, Getroommeasure)
        | ReadThermostat
        /// To set up the thermostat (Synchomeschedule, Setroomthermpoint)
        | WriteThermostat
        /// To retrieve Welcome data (Gethomedata, Getcamerapicture)
        | ReadCamera
        /// To tell Welcome a specific person or everybody has left the Home (Setpersonsaway, Setpersonshome)
        | WriteCamera
        /// To access the camera, the videos and the live stream
        | AccessCamera
        /// To retrieve Presence data (Gethomedata, Getcamerapicture)
        | ReadPresence
        /// To access the camera, the videos and the live stream
        | AccessPresence
        /// To read data coming from Healthy Home Coach (gethomecoachsdata)
        | ReadHomeCoach

        override this.ToString () =
            match this with
            | ReadStation -> "read_station"
            | ReadThermostat -> "read_thermostat"
            | WriteThermostat -> "write_thermostat"
            | ReadCamera -> "read_camera"
            | WriteCamera -> "write_camera"
            | AccessCamera -> "access_camera"
            | ReadPresence -> "read_presence"
            | AccessPresence -> "access_presence"
            | ReadHomeCoach -> "read_homecoach"

    type Auth = {
        AccessToken: string
        ExpiresIn: int
        RefreshToken: string
        Scope: Scope seq
    }

    type Error = {
        Message: string
    }

    type Scale =
        | HalfHour
        | OneHour
        | ThreeHours
        | OneDay
        | OneWeek
        | OneMonth

        override this.ToString () =
            match this with
            | HalfHour -> "30min"
            | OneHour -> "1hour"
            | ThreeHours -> "3hours"
            | OneDay -> "1day"
            | OneWeek -> "1week"
            | OneMonth -> "1month"

    type Measurement =
        | Temperature
        | Humidity
        | Co2
        | Pressure
        | Noise
        | Rain
        | WindStrenght
        | WindAngle
        | GustAngle
        | MinTemp
        | MaxTemp
        | DateMinTemp
        | DateMaxTemp
        | MinHum
        | MaxHum
        | MinPressure
        | MaxPressure
        | MinNoise
        | MaxNoise
        | SumRain
        | DateMaxGust
        | DateMaxHum
        | DateMinPressure
        | DateMaxPressure
        | DateMinNoise
        | DateMaxNoise
        | DateMinCo2
        | DateMaxCo2

        override this.ToString () =
            match this with
            | Temperature -> "temperature"
            | Humidity -> "humidity"
            | Co2 -> "co2"
            | Pressure -> "pressure"
            | Noise -> "noise"
            | Rain -> "rain"
            | WindStrenght -> "windstrength"
            | WindAngle -> "windangle"
            | GustAngle -> "gustangle"
            | MinTemp -> "mintemp"
            | MaxTemp -> "maxtemp"
            | DateMinTemp -> "datemintemp"
            | DateMaxTemp -> "datemaxtemp"
            | MinHum -> "minhum"
            | MaxHum -> "maxhum"
            | MinPressure -> "minpressure"
            | MaxPressure -> "maxpressure"
            | MinNoise -> "minnoise"
            | MaxNoise -> "maxnoise"
            | SumRain -> "sumrain"
            | DateMaxGust -> "datemaxgust"
            | DateMaxHum -> "datemaxhum"
            | DateMinPressure -> "dateminpressure"
            | DateMaxPressure -> "datemaxpressure"
            | DateMinNoise -> "dateminnoise"
            | DateMaxNoise -> "datemaxnoise"
            | DateMinCo2 -> "dateminco2"
            | DateMaxCo2 -> "datemaxco2"

    type Location = {
        Latitude: float
        Longitude: float
    }

    type Place = {
        TimeZone: string
        Country: string
        Altitude: int
        Location: Location
    }

    type RainData = {
        /// Rain in mm
        Rain: int
        /// Rain measured for past 24h (mm)
        SumRain24: int
        /// Rain measured for the last hour (mm)
        SumRain1: int
    }

    type WindData = {
        /// Wind strenght (km/h)
        WindStrenght: int
        /// Wind angle in degrees
        WindAngle: int
        /// Gust strengh (km/h)
        GustStrength: int
        /// Gust angle in degrees
        GustAngle: int
        /// Max gust strengh (km/h)
        MaxWindStrenght: int
        /// Max gust angle in degrees
        MaxWindAngle: int
        /// Timestamp of max wind strength
        DateMaxWindStrength: int
    }

    type TemperatureData = {
        Temperature: float
        MinTemp: float
        MaxTemp: float
        DateMaxTemp: int
        DateMinTemp:int
        TempTrend: string
    }

    type DashboardData = {
        /// Timestamp when data was measured
        TimeUtc: int

        Temperature: TemperatureData option
        Humidity: int option
        Co2: int option
        Wind: WindData option
        Rain: RainData option
    }

    type Module = {
        /// Mac address of the module
        Id: string
        /// Example: "NAModule3"
        Type: string
        /// Example: "Rain gauge"
        ModuleName: string
        /// Array of data measured by the device (e.g. "Temperature","Humidity")
        DataType: string seq
        /// Timestamp of the last installation
        LastSetup: int
        /// Percentage of battery remaining (10=low)
        BatteryPercent: int
        /// True if the station connected to Netatmo cloud within the last 4 hours
        Reachable: bool
        /// Version of the software
        Firmware: int
        /// Timestamp of the last measure update
        LastMessage: int
        /// Timestamp of the last status update
        LastSeen: int
        /// Current radio status per module. (90=low, 60=highest)
        RfStatus: int
        /// Current battery status per module
        BatteryVp: int

        DashboardData: DashboardData
    }

    type Device = {
        /// Mac address of the device
        Id: string
        /// Name of the station
        StationName: string
        /// Date when the weather station was set up
        DateSetup: int
        /// timestamp of the last installation
        LastSetup: int
        /// Type of the device
        Type: string
        /// Timestamp of the last status update
        LastStatusStore: int
        /// Name of the module
        ModuleName: string
        /// Version of the firmware
        Firmware: int
        /// Timestamp of the last upgrade
        LastUpgrade: int
        ///  Wifi status per Base station. (86=bad, 56=good)
        WifiStatus: int
        /// True if the station connected to Netatmo cloud within the last 4 hours
        Reachable: bool
        /// True if the station is calibrating
        Co2Calibrating: bool
        /// Array of data measured by the device (e.g. "Temperature","Humidity")
        DataType: string seq
        /// Place and location of the device.
        Place: Place
        /// True if the user owns the station, false if he is invited to a station
        Readonly: bool

        DashboardData: DashboardData
        Modules: Module seq
    }

    type Administrative = {
        /// User locale
        Language: string
        /// User regional preferences (used for displaying date)
        Locale: string
        /// Country
        Country: string
        /// 0 -> metric system, 1 -> imperial system
        Unit: int
        /// 0 -> kph, 1 -> mph, 2 -> ms, 3 -> beaufort, 4 -> knot
        WindUnit: int
        /// 0 -> mbar, 1 -> inHg, 2 -> mmHg
        PressureUnit: int
        /// Algorithm used to compute feel like temperature, 0 -> humidex, 1 -> heat-index
        FeelLikeAlgo: int
    }
    type User = {
        Mail: string
        Administrative: Administrative
    }

    type StationData = {
        Devices: Device seq
        User: User
        Status: string
        TimeExec: float
        TimeServer: int
    }

    type StationRequest = {
        DeviceId: string
        Favorites: bool
    }

    type MeasureRequest = {
        DeviceId: string
        ModuleId: string option
        Scale: Scale seq
        Type: Measurement seq
        DateBegin: int option
        DateEnd: int option
        Limit: int option
        Optimize: bool option
        RealTime: bool option
    }
