namespace Server

module Options =

  open FsConfig
  open Giraffe
  open Microsoft.Extensions.Logging
  open System

  type Environment =
    | Development
    | Testing
    | Staging
    | Production

  [<Convention("APP")>]
  type Config =
    {
      [<CustomName("ASPNETCORE_ENVIRONMENT")>]
      Environment: Environment
      AllowedHosts: List<string>
      ListeningUrls: List<string>
      AllowedOrigins: List<string>
      DatabaseConnectionString: string
      JwtSecret: string
      JwtPepper: string
      JwtIssuer: string
      JwtAudience: string
      JwtExpirationTime: int
      JwtRefreshExpirationTime: int
    }

  let authenticationFailure: HttpHandler = setStatusCode 401

  let authenticate: HttpHandler = requiresAuthentication authenticationFailure

  let errorHandler (ex: Exception) (logger: ILogger) : HttpHandler =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request")

    clearResponse >=> setStatusCode 500
