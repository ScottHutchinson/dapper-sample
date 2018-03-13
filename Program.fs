// open System.Configuration
open System.Data.SqlClient
open System.IO
open Microsoft.Extensions.Configuration
open DapperFSharp

// [<Literal>]
// let ConnStr = "Data Source=localhost; Initial Catalog=AdventureWorks2017; Integrated Security=True"
let builder = 
    let ret = new ConfigurationBuilder()
    FileConfigurationExtensions.SetBasePath(ret, Directory.GetCurrentDirectory()) |> ignore
    JsonConfigurationExtensions.AddJsonFile(ret, "appSettings.json")

let config = builder.Build ()
let ConnStr = config.Item("App:Connection:Value") // config.GetSection("App").Get<AppSettings>()
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
    if argv.Length = 0 then
        printfn "You must pass a User ID as a command line argument."
    else
        let userId = int argv.[0]
        let user = getUser userId db
        printfn "User's Job Title: %s" user.JobTitle
    0 // return an integer exit code
