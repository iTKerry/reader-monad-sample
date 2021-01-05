namespace ReaderSample

module Main =
    
    open System
    open Services
    open ReaderBuilder

    let program : Reader<Services, _> =
        reader {
            let getConsole services = services.Console 
            let getLogger services = services.Logger
            let getConsoleAndLogger services = services.Console, services.Logger

            let! str1, str2 = readFromConsole() |> Reader.withEnv getConsoleAndLogger 
            let! result = compareTwoStrings str1 str2 |> Reader.withEnv getLogger 
            do! writeToConsole result |> Reader.withEnv getConsole
        }
        
    let defaultConsole =
        { new IConsole with
            member __.ReadLn()    = Console.ReadLine()
            member __.WriteLn str = printfn "%s" str }
        
    let defaultLogger =
        { new ILogger with
            member __.Debug str   = printfn "Debug: %s" str
            member __.Info str    = printfn "Info: %s"  str
            member __.Error str   = printfn "Error: %s" str }
        
    let services =
        { Console = defaultConsole
          Logger = defaultLogger }
        
    [<EntryPoint>]
    let main _ =
        Reader.run services program
        0