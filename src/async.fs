namespace Stylish

module Outcome =
    type Outcome =
        | OK of filename: string
        | Failed of filename: string

    let isOk =
        function
        | OK _ -> true
        | Failed _ -> false

    let fileName =
        function
        | OK fn
        | Failed fn -> fn

module Download =
    open System
    open System.IO
    open System.Net
    open System.Text.RegularExpressions
    open FSharp.Data

    let private absoluteUri (pageUri: Uri) (filePath: string) =
        if filePath.StartsWith("http:") || filePath.StartsWith("https:") then
            Uri(filePath)
        else
            let sep = '/'
            filePath.TrimStart(sep) |> (sprintf "%O%c%s" pageUri sep) |> Uri

    let private getLinks (pageUri: Uri) (filePattern: string) =
        Log.cyan "Getting names..."
        let re = Regex(filePattern)
        let html = HtmlDocument.Load(pageUri.AbsoluteUri)

        let links =
            html.Descendants [ "a" ]
            |> Seq.choose (fun node -> node.TryGetAttribute("href") |> Option.map (fun att -> att.Value()))
            |> Seq.filter (re.IsMatch)
            |> Seq.map (absoluteUri pageUri)
            |> Seq.distinct
            |> Array.ofSeq

        links

    let private tryDownload (localPath: string) (fileUri: Uri) =
        let fileName = fileUri.Segments |> Array.last
        Log.yellow (sprintf "%s - starting download" fileName)
        let filePath = Path.Combine(localPath, fileName)
        use client = new WebClient()

        try
            client.DownloadFile(fileUri, filePath)
            Log.green (sprintf "%s - download complete" fileName)
            Outcome.OK fileName
        with e ->
            Log.red (sprintf "%s - error: %s" fileName e.Message)
            Outcome.Failed fileName
    let GetFiles
        (pageUri: Uri) (filePattern: string) (localPath: string) =
            let links = getLinks pageUri filePattern
            let downloaded, failed =
                links
                |> Array.map (tryDownload localPath)
                |> Array.partition Outcome.isOk
            downloaded |> Array.map Outcome.fileName,
            failed |> Array.map Outcome.fileName
