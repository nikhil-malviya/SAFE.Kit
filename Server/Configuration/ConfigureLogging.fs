namespace Server.Configuration

module ConfigureLogging =

  open Microsoft.Extensions.Logging

  open Server.Options

  let configureLogging (config: Config) (logging: ILoggingBuilder) =
    let logLevel =
      match config.Environment with
      | Development -> LogLevel.Information
      | _ -> LogLevel.Error

    logging.AddFilter(fun level -> level = logLevel).AddConsole().AddDebug()
    |> ignore
