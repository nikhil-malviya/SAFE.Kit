namespace Server.Configuration

module ConfigureServices =

  open Giraffe
  open Microsoft.AspNetCore.Authentication.JwtBearer
  open Microsoft.AspNetCore.HostFiltering
  open Microsoft.Extensions.DependencyInjection

  open Server.Options
  open Server.Services.CryptographyService.Token

  let configureServices (config: Config) (services: IServiceCollection) =
    let allowedHosts = config.AllowedHosts |> List.toArray
    let secret = config.JwtSecret
    let issuer = config.JwtIssuer
    let audience = config.JwtAudience

    services
      .Configure(fun (options: HostFilteringOptions) -> options.AllowedHosts <- allowedHosts)
      .AddCors()
      .AddGiraffe()
      .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(validateToken secret issuer audience)
    |> ignore
