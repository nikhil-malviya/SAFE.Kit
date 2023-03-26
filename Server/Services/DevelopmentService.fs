namespace Server.Services

module DevelopmentService =

  open FSharp.Control.Tasks
  open Giraffe
  open Microsoft.AspNetCore.Http
  open Microsoft.IdentityModel.JsonWebTokens
  open System.Security.Claims

#if DEBUG
  let claims: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
      task {
        let claims = ctx.User.Claims

        let listOfClaims =
          let _map (claim: Claim) =
            {|
              Type = claim.Type
              Value = claim.Value
            |}

          claims |> Seq.map _map

        return! json listOfClaims next ctx
      }
#endif

  let resource: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
      task {
        let name = ctx.User.FindFirst JwtRegisteredClaimNames.GivenName

        return! text $"User %s{name.Value} is authorized to access this resource" next ctx
      }
