namespace Shared.Models

open System

type Task =
  {
    Id: Guid
    Description: string
    IsDone: bool
  }
