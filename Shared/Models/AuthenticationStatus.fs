namespace Shared.Models

type AuthenticationStatus =
  | Unauthenticated
  | TokenExpired
  | Authenticated of Set<Role>
