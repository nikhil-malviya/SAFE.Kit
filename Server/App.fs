namespace Server

module App =

  open FsConfig
  open Microsoft.AspNetCore.Hosting
  open Microsoft.Extensions.Hosting
  open System.IO

  open Server.Configuration.ConfigureApp
  open Server.Configuration.ConfigureLogging
  open Server.Configuration.ConfigureServices
  open Server.Options

  let config =
    match EnvConfig.Get<Config>() with
    | Ok config -> config
    | Error error ->
      match error with
      | NotFound variable -> failwithf "Environment variable '%s' not found" variable
      | BadValue (variable, value) -> failwithf "Environment variable '%s' has invalid value '%s'" variable value
      | NotSupported message -> failwith message

  [<EntryPoint>]
  let main args =
    let webRoot = Path.Combine(Directory.GetCurrentDirectory(), "Public")
    let listeningUrls = config.ListeningUrls |> List.toArray

    Host
      .CreateDefaultBuilder(args)
      .ConfigureWebHostDefaults(fun webHostBuilder ->
        webHostBuilder
          .UseUrls(listeningUrls)
          .UseWebRoot(webRoot)
          .Configure(configureApp config)
          .ConfigureServices(configureServices config)
          .ConfigureLogging(configureLogging config)
        |> ignore
      )
      .Build()
      .Run()

    0
