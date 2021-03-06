﻿[<AutoOpen>]
module FunScript.Tests.Common

open FunScript
open Jint
open NUnit.Framework
open Microsoft.FSharp.Linq.QuotationEvaluation

[<JSEmit("test_log({0}.toString());")>]
let log (msg : obj) : unit = failwith "never"

let checkAreEqualWithComponents components expectedResult quote =
   let code = Compiler.Compiler.Compile(quote, components = components(*, shouldCompress = true*))
   try
      let engine = JintEngine().SetFunction("test_log", System.Action<string>(printfn "//[LOG] %s"))
      let result = engine.Run(code + "\nreturn null;")
      let message (ex: 'a) (re: 'b) = sprintf "%sExpected: %A%sBut was: %A" System.Environment.NewLine ex System.Environment.NewLine re
      Assert.That((result = expectedResult), (message expectedResult result))
   // Wrap xUnit exceptions to stop pauses.
   with ex ->
      //printfn "// Code:\n%s" code
      if ex.GetType().Namespace.StartsWith "FunScript" then raise ex
      else failwithf "Message: %s\n" ex.Message

let checkAreEqual expectedResult quote =
    checkAreEqualWithComponents [] expectedResult quote

/// Bootstrapping:
/// Generates code. Runs it through a JS interpreter. 
/// Checks the result against the compiled expression.
let check (quote:Quotations.Expr) =
   let expectedResult = quote.EvalUntyped()
   checkAreEqual expectedResult quote

// TODO:
// Add support for inheritance.
// Add support for TypeScript inheritance.
// Add support for TypeScript param arrays.
// Add support for TypeScript optional members/params.
// Add tests for union/array/list/seq/map/set equality/comparison.
// Add support for multiple constructors.
// Add support for method name overloading. DefineGlobal to take MethodBase? What about instances?
// Add support for type checks. How would you differentiate between ints and floats though?
// Add support for renaming reserved words.
// Add support for exceptions.
// Add support for events/observables.
// Add support for computation expressions.
// Add support for custom operators.
// Add support for tail recursive transformations into while loops.