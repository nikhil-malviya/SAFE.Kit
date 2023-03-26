namespace Server.Services

module TaskService =

  open System

  open Shared.Contracts
  open Shared.Models

  let taskService (addTask, deleteTask, getTasks, updateTaskStatus) : TaskContract =
    {
      GetTasks =
        fun () ->
          async {
            let! tasks = getTasks() |> Async.AwaitTask

            return tasks
          }

      AddTask =
        fun (task: Task) ->
          async {
            let! count = addTask(task) |> Async.AwaitTask

            return count
          }

      UpdateTaskStatus =
        fun (id: Guid, isDone: bool) ->
          async {
            let! count = updateTaskStatus(id, isDone) |> Async.AwaitTask

            return count
          }

      DeleteTask =
        fun (id: Guid) ->
          async {
            let! count = deleteTask(id) |> Async.AwaitTask

            return count
          }
    }
