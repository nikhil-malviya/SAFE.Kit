namespace Shared.Models

open System

type User =
  {
    Id: Guid
    Username: string
    FirstName: string
    LastName: option<string>
  }
