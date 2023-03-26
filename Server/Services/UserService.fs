namespace Server.Services

module UserService =

  open System

  open Server.Repositories.Entities
  open Shared.Contracts
  open Shared.Models

  let userService (getUser, verifyPassword, generateToken) : AuthenticationContract =
    {
      Login =
        fun (credentials: Credentials) ->
          async {
            let! users = getUser credentials.Username |> Async.AwaitTask

            let user = users |> List.tryExactlyOne

            let token =
              match user with
              | Some user when
                verifyPassword credentials.Password user.PasswordHash
                ->
                generateToken user.Id user.Username user.FirstName user.Roles
              | _ -> String.Empty

            return token
          }
    }
