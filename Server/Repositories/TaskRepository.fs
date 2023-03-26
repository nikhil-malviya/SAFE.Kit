namespace Server.Repositories

module TaskRepository =

  open Npgsql.FSharp
  open System

  open Shared.Models

  let getTasks (connectionString: string) (userId: Guid) () =
    connectionString
    |> Sql.connect
    |> Sql.query
         "SELECT task_id, description, is_done FROM app.tasks WHERE user_id = @user_id AND is_deleted = false ORDER BY created_at DESC"
    |> Sql.parameters [ "@user_id", Sql.uuid userId ]
    |> Sql.executeAsync(fun read ->
      {
        Id = read.uuid "task_id"
        Description = read.text "description"
        IsDone = read.bool "is_done"
      }
    )

  let addTask (connectionString: string) (userId: Guid) (task: Task) =
    connectionString
    |> Sql.connect
    |> Sql.query
         "INSERT INTO app.tasks (task_id, description, is_done, user_id, is_deleted) VALUES (@task_id, @description, @is_done, @user_id, @is_deleted)"
    |> Sql.parameters
         [
           "@task_id", Sql.uuid task.Id
           "@description", Sql.string task.Description
           "@is_done", Sql.bool task.IsDone
           "@user_id", Sql.uuid userId
           "@is_deleted", Sql.bool false
         ]
    |> Sql.executeNonQueryAsync

  let updateTaskStatus (connectionString: string) (userId: Guid) (id: Guid, isDone: bool) =
    connectionString
    |> Sql.connect
    |> Sql.query
         "UPDATE app.tasks SET is_done = @is_done WHERE task_id = @task_id AND user_id = @user_id AND is_deleted = false"
    |> Sql.parameters
         [
           "@task_id", Sql.uuid id
           "@is_done", Sql.bool isDone
           "@user_id", Sql.uuid userId
         ]
    |> Sql.executeNonQueryAsync

  let deleteTask (connectionString: string) (userId: Guid) (id: Guid) =
    connectionString
    |> Sql.connect
    |> Sql.query
         "UPDATE app.tasks SET is_deleted = true WHERE task_id = @task_id AND user_id = @user_id AND is_deleted = false"
    |> Sql.parameters [ "@task_id", Sql.uuid id; "@user_id", Sql.uuid userId ]
    |> Sql.executeNonQueryAsync
