namespace Server

module Remoting =

  open Fable.Remoting.Giraffe
  open Fable.Remoting.Server
  open Giraffe
  open Microsoft.AspNetCore.Http
  open System
  open System.IdentityModel.Tokens.Jwt

  open Server.Options
  open Server.Repositories.TaskRepository
  open Server.Repositories.UserRepository
  open Server.Services.CryptographyService.Password
  open Server.Services.CryptographyService.Token
  open Server.Services.TaskService
  open Server.Services.UserService

  let routeBuilder (typeName: string) (methodName: string) =
    $"""/api/%s{typeName.Replace("Contract", String.Empty)}/%s{methodName}"""

  let getUserId (context: HttpContext) =
    match context.User.FindFirst(JwtRegisteredClaimNames.Sub) with
    | claim when not <| isNull claim ->
      match Guid.TryParse(claim.Value) with
      | true, userId -> userId
      | false, _ -> Guid.Empty
    | _ -> Guid.Empty

  let taskServiceRoot (config: Config) (context: HttpContext) =
    let userId = getUserId context
    let connectionString = config.DatabaseConnectionString

    taskService(
      addTask connectionString userId,
      deleteTask connectionString userId,
      getTasks connectionString userId,
      updateTaskStatus connectionString userId
    )

  let taskApi (config: Config) : HttpHandler =
    Remoting.createApi()
    |> Remoting.withErrorHandler(fun ex routeInfo -> raise ex)
    |> Remoting.withRouteBuilder(routeBuilder)
    |> Remoting.fromContext(taskServiceRoot config)
    |> Remoting.buildHttpHandler

  let userServiceRoot (config: Config) =
    let connectionString = config.DatabaseConnectionString
    let secret = config.JwtSecret
    let audience = config.JwtAudience
    let issuer = config.JwtIssuer
    let expiration = config.JwtExpirationTime
    let pepper = config.JwtPepper

    userService(getUser connectionString, verifyPassword pepper, generateToken secret audience issuer expiration)

  let authenticationApi (config: Config) : HttpHandler =
    Remoting.createApi()
    |> Remoting.withErrorHandler(fun ex routeInfo -> raise ex)
    |> Remoting.withRouteBuilder(routeBuilder)
    |> Remoting.fromValue(userServiceRoot config)
    |> Remoting.buildHttpHandler
