namespace Server.Services

module CryptographyService =

  module Password =

    open Microsoft.AspNetCore.Cryptography.KeyDerivation
    open System
    open System.Security.Cryptography

    type Iterations =
      | V1 = 1211372 // 2021

    let private defaultIterations = Iterations.V1
    let private defaultSaltLengthInBytes = 256 / 8
    let private defaultDerivedKeyLengthInBytes = 256 / 8

    let generateSalt (saltLengthInBytes: int) =
      let salt = Array.zeroCreate<byte> saltLengthInBytes
      use rng = RandomNumberGenerator.Create()
      rng.GetNonZeroBytes salt

      salt

    let generateHash
      (plaintext: string)
      (salt: byte[])
      (pepper: string)
      (iterations: Iterations)
      (derivedKeyLengthInBytes: int)
      =
      KeyDerivation.Pbkdf2(
        password = plaintext + pepper,
        salt = salt,
        prf = KeyDerivationPrf.HMACSHA512,
        iterationCount = (iterations |> int),
        numBytesRequested = derivedKeyLengthInBytes
      )

    // Hash Password using PBKDF2
    let hashPassword (pepper: string) (password: string) =
      let salt = generateSalt(defaultSaltLengthInBytes)

      let hash =
        generateHash password salt pepper defaultIterations defaultDerivedKeyLengthInBytes

      let hashString = Convert.ToBase64String hash
      let saltString = Convert.ToBase64String salt

      $"%s{hashString}:%s{saltString}:{defaultIterations}"

    let retrieveContents (passwordHash: string) =
      let tokens = passwordHash.Split(":")

      let hashString = tokens[0]
      let saltString = tokens[1]
      let iterationsString = tokens[2]

      let hash = Convert.FromBase64String hashString
      let salt = Convert.FromBase64String saltString

      let iterations =
        match Enum.TryParse<Iterations>(iterationsString) with
        | true, iterations -> iterations
        | _ -> defaultIterations

      (hash, salt, iterations)

    let verifyPassword (pepper: string) (password: string) (passwordHash: string) =
      let (hash, salt, iterations) = retrieveContents passwordHash

      hash = generateHash password salt pepper iterations defaultDerivedKeyLengthInBytes

  module Token =

    open Microsoft.AspNetCore.Authentication.JwtBearer
    open Microsoft.IdentityModel.JsonWebTokens
    open Microsoft.IdentityModel.Tokens
    open System
    open System.Security.Claims
    open System.Text

    open Shared.Models

    let signingCredentials (secret: string) =
      let securityKey = SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))

      SigningCredentials(key = securityKey, algorithm = SecurityAlgorithms.HmacSha256)

    // Generate Token using HMACSHA256
    let generateToken
      (secret: string)
      (audience: string)
      (issuer: string)
      (expiration: int)
      (userid: Guid)
      (username: string)
      (firstname: string)
      (roles: Set<Role>)
      =
      let currentTime = DateTime.UtcNow
      let notBefore = currentTime
      let expires = currentTime.AddSeconds(expiration)
      let signingCredentials = signingCredentials secret

      let claims =
        [
          Claim(JwtRegisteredClaimNames.Sub, userid.ToString())
          Claim(JwtRegisteredClaimNames.GivenName, firstname)
          Claim(JwtRegisteredClaimNames.Email, username)
          Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
          Claim(
            JwtRegisteredClaimNames.Iat,
            DateTimeOffset(currentTime).ToUnixTimeSeconds().ToString(),
            ClaimValueTypes.Integer64
          )
        ]

      let roleClaims =
        roles |> List.ofSeq |> List.map(fun role -> Claim("roles", role.ToString()))

      let subject = ClaimsIdentity(claims @ roleClaims)

      let tokenDescriptor =
        SecurityTokenDescriptor(
          Issuer = issuer,
          Audience = audience,
          NotBefore = notBefore,
          Expires = expires,
          Subject = subject,
          SigningCredentials = signingCredentials
        )

      JsonWebTokenHandler().CreateToken(tokenDescriptor)

    let validateToken (secret: string) (issuer: string) (audience: string) (options: JwtBearerOptions) =
      let tokenValidationParameters =
        TokenValidationParameters(
          RequireExpirationTime = true,
          ValidateLifetime = true,
          ClockSkew = TimeSpan.Zero,
          ValidateIssuer = true,
          ValidIssuer = issuer,
          ValidateAudience = true,
          ValidAudience = audience,
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
        )

      options.TokenValidationParameters <- tokenValidationParameters
      options.MapInboundClaims <- false
