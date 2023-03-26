namespace Server

module Routes =

  open Giraffe

  open Server.Options
  open Server.Remoting
  open Server.Services.DevelopmentService

  let endpoints handlers = subRoute "/api" (choose handlers)

  let router (config: Config) : HttpHandler =
    choose
      [
        route "/" >=> text "Server Is Up and Running"

        authenticationApi config
        authenticate >=> taskApi config

        endpoints
          [
#if DEBUG
            GET >=> route "/claims" >=> authenticate >=> claims
#endif

            GET >=> route "/resource" >=> authenticate >=> resource
          ]

        setStatusCode 404
      ]
