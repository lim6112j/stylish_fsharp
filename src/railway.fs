namespace Stylish

open System

module Railway =

    type Outcome<'TSuccess, 'TFailure> =
        | Success of 'TSuccess
        | Failure of 'TFailure

    let adapt func input =
        match input with
        | Success x -> func x
        | Failure f -> Failure f

    let passThrough func input =
        match input with
        | Success x -> func x |> Success
        | Failure f -> Failure f

    let checkString (s: string) =
        if isNull (s) then
            raise <| ArgumentNullException("Must not be null")
        elif String.IsNullOrEmpty(s) then
            raise <| ArgumentException("Must not be empty")
        elif String.IsNullOrWhiteSpace(s) then
            raise <| ArgumentException("Must not be white space")
        else
            s

    let notEmpty (s: string) =
        if isNull (s) then
            Failure "Must not be null"
        elif String.IsNullOrEmpty(s) then
            Failure "Must not be empty"
        elif String.IsNullOrWhiteSpace(s) then
            Failure "Must not be white space"
        else
            Success s

    let mixedCase (s: string) =
        let hasUpper = s |> Seq.exists (Char.IsUpper)
        let hasLower = s |> Seq.exists (Char.IsLower)

        if hasUpper && hasLower then
            Success s
        else
            Failure "Must contain mixed case"

    let containsAny (cs: string) (s: string) =
        if s.IndexOfAny(cs.ToCharArray()) > -1 then
            Success s
        else
            Failure(sprintf "Must contain at least on of %A" cs)

    let tidy (s: string) = s.Trim()

    let save (s: string) =
        let dbSave s : unit =
            printfn "Saving password '%s'" s
            raise <| Exception "Dummy exception"

        let log m = printfn "Logging error: %s" m

        try
            dbSave s |> Success
        with e ->
            log e.Message
            Failure "Sorry, there was an internal error your password"

    let validateAndSave password =
        let mixedCase' = adapt mixedCase
        let containsAny' = adapt (containsAny "-_!?")
        let tidy' = passThrough tidy
        let save' = adapt save

        password |> notEmpty |> mixedCase' |> containsAny' |> tidy' |> save'
