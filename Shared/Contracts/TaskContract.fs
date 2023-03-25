namespace Shared.Contracts

open System

open Shared.Models

type TaskContract =
  {
    GetTasks: unit -> Async<List<Task>>
    AddTask: Task -> Async<int>
    UpdateTaskStatus: Guid * bool -> Async<int>
    DeleteTask: Guid -> Async<int>
  }
