namespace Server.Repositories.Entities

open Shared.Models
open System

type UserEntity =
  {
    Id: Guid
    Username: string
    FirstName: string
    LastName: option<string>
    PasswordHash: string
    Roles: Set<Role>
  }
