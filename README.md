# SAFE.Kit

Full-stack web application using the [SAFE Stack](https://safe-stack.github.io/)

## Building the application

In order to build the application, you'll need to install the following:

- [.NET LTS](https://www.microsoft.com/net/download)
- [Node LTS](https://nodejs.org/en/download/)
- [Yarn](https://yarnpkg.com/)

Install dotnet tools

```bash
dotnet tool restore
```

Install packages for Server project

```bash
cd Server
dotnet restore
```

Install packages for Web project

```bash
cd Web
yarn
```

## Running the application

Start the Server project:

```bash
cd Server
dotnet run
```

Start the Web project:

```bash
cd Web
yarn start
```

Open a browser and enter the URL [http://localhost:3000](http://localhost:3000) to view the site

You can find more about the F# components used at the following places:

- [Giraffe](https://giraffe.wiki/)
- [Feliz](https://zaid-ajaj.github.io/Feliz/)
- [Fable](https://fable.io/docs/)
