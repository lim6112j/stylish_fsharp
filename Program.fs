open System
open System.Threading
open Stylish

open System.Diagnostics
open Railway

[<EntryPoint>]
let main argv =
    // async programming

    // let uri = Uri @"https://minorplanetcenter.net/data"
    // let pattern = @"neam.*\.json\.gz$"
    // let localPath = @"/tmp/downloads"
    // let sw = Stopwatch()
    // sw.Start()
    // let downloaded, failed = Download.GetFiles uri pattern localPath
    // failed |> Array.iter (fun fn -> Log.red (sprintf "Failed: %s" fn))
    //
    // Log.cyan (
    //     sprintf
    //         "%i files downloaded in %0.1fs, %i failed, Press a key"
    //         downloaded.Length
    //         sw.Elapsed.TotalSeconds
    //         failed.Length
    // )

    // railway programming

    let demo () =
        null |> validateAndSave |> printfn "%A"
        "" |> validateAndSave |> printfn "%A"
        "The quick brown fox!" |> validateAndSave |> printfn "%A"

    demo ()

    Console.ReadKey() |> ignore
    0
