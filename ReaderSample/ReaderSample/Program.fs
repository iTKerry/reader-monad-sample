﻿namespace ReaderSample

type ILogger = 
    abstract Debug : string -> unit 
    abstract Info  : string -> unit 
    abstract Error : string -> unit
  
type IConsole = 
    abstract ReadLn  : unit   -> string
    abstract WriteLn : string -> unit

type IServices = 
    inherit ILogger
    inherit IConsole

type ComparisonResult =
    | Bigger
    | Smaller
    | Equal
    
module Services =
    
    open ReaderBuilder
        
    let readFromConsole () = 
        reader {
            let! (console : #IConsole) = Reader.ask

            console.WriteLn "Enter the first value"
            let str1 = console.ReadLn ()
            
            console.WriteLn "Enter the second value"
            let str2 = console.ReadLn ()

            return str1, str2
        }
        
    let writeToConsole result = 
        reader {
            let! (console : #IConsole) = Reader.ask

            match result with
            | Bigger  -> console.WriteLn "The first value is bigger"
            | Smaller -> console.WriteLn "The first value is smaller"
            | Equal   -> console.WriteLn "The values are equal"
        }
        
    let compareTwoStrings str1 str2  =
        reader {
            let! (logger : #ILogger) = Reader.ask
            logger.Debug "compareTwoStrings: Starting"

            let compare = function
                | (s1, s2) when s1 < s2 -> Smaller
                | (s1, s2) when s1 > s2 -> Bigger
                | _                     -> Equal
            
            let result = compare (str1, str2)
            
            logger.Info (sprintf "compareTwoStrings: result=%A" result)
            logger.Debug "compareTwoStrings: Finished"
            return result 
        }

module Main =
    
    open Services
    open System
    open ReaderBuilder

    let program : Reader<IServices, _> =
        reader {
            let! str1, str2 = readFromConsole ()  
            let! result = compareTwoStrings str1 str2
            do! writeToConsole result 
        }
        
    let services = 
        { new IServices
            interface IConsole with 
                member __.ReadLn()    = Console.ReadLine()
                member __.WriteLn str = printfn "%s" str
            interface ILogger with
                member __.Debug str   = printfn "Debug: %s" str
                member __.Info str    = printfn "Info: %s" str
                member __.Error str   = printfn "Error: %s" str
        }
        
    [<EntryPoint>]
    let main _ =
        Reader.run services program
        0