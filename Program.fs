// open System.Configuration
open System.Data.SqlClient
open Microsoft.Extensions.Configuration
open DapperFSharp

// [<Literal>]
// let ConnStr = "Data Source=localhost; Initial Catalog=AdventureWorks2017; Integrated Security=True"
let builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json");

var config = builder.Build();

let appConfig = config.GetSection("App").Get<AppSettings>()
let ConnStr = "ConfigurationManager"
let db = new SqlConnection(ConnStr)

type User = { ID: int; JobTitle: string }

let getUser userId connection =
    connection
    |> dapperMapParameterizedQuery<User> 
        "SELECT BusinessEntityID AS ID, JobTitle From HumanResources.Employee WHERE BusinessEntityID = @UserId" 
        (Map ["UserId", userId])
    |> Seq.head

// Example command lines (last argument is userId):
//  dotnet run -c Release 4
//  dotnet run -c Release  --no-build --no-restore 5
[<EntryPoint>]
let main argv =
    let userId = int argv.[0]
    let user = getUser userId db
    printfn "User's Job Title: %s" user.JobTitle
    0 // return an integer exit code
