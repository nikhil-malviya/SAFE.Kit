namespace Server.Configuration

module ConfigureApp =

  open Giraffe
  open Microsoft.AspNetCore.Builder
  open Microsoft.AspNetCore.Cors.Infrastructure

  open Server.Options
  open Server.Routes

  let corsPolicy (allowedOrigins: string[]) (builder: CorsPolicyBuilder) =
    builder
      .WithOrigins(allowedOrigins)
      .SetIsOriginAllowedToAllowWildcardSubdomains()
      .AllowAnyMethod()
      .AllowAnyHeader()
    |> ignore

  let configureApp (config: Config) (app: IApplicationBuilder) =
    let allowedOrigins = config.AllowedOrigins |> List.toArray

    (match config.Environment with
     | Development -> app.UseDeveloperExceptionPage()
     | _ -> app.UseGiraffeErrorHandler(errorHandler))
      .UseCors(
        corsPolicy allowedOrigins
      )
      .UseDefaultFiles()
      .UseStaticFiles()
      .UseAuthentication()
      .UseStatusCodePages()
      .UseGiraffe(
        router config
      )
