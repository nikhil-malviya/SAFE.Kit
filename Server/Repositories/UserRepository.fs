namespace Server.Repositories

module UserRepository =

  open Npgsql.FSharp
  open System

  open Server.Repositories.Entities
  open Shared.Models

  let getUser (connectionString: string) (username: string) =
    connectionString
    |> Sql.connect
    |> Sql.query
         "SELECT user_id, username, firstname, lastname, password_hash, roles FROM app.users WHERE username = @username AND is_deleted = false"
    |> Sql.parameters [ "@username", Sql.string username ]
    |> Sql.executeAsync(fun read ->
      {
        Id = read.uuid "user_id"
        Username = read.text "username"
        FirstName = read.text "firstname"
        LastName = read.textOrNone "lastname"
        PasswordHash = read.text "password_hash"
        Roles = (read.int16Array "roles") |> Set.ofArray |> Set.map(fun num -> enum(int num))
      }
    )
