namespace Shared.Contracts

open Shared.Models

type AuthenticationContract = { Login: Credentials -> Async<string> }
