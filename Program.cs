﻿// See https://aka.ms/new-console-template for more information


using ATM;
using CommandLine;



string searchPattern = @".\*.*"; // replace with your actual input

Helper.Output(Environment.NewLine + DateTime.Now.ToString("F"));


return Parser.Default.ParseArguments<InitOptions, TestOptions>(args)
    .MapResult(
      (IVerb opts) => opts.Start(),
      errs => 1);