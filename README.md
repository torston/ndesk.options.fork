# NDesk.Options.Fork

## Build Status
#### Master Branch: [![Build status](https://ci.appveyor.com/api/projects/status/9rkhbjxrka6ra66a/branch/master?svg=true)](https://ci.appveyor.com/project/torston/ndesk-options-fork/branch/master) 
#### Tests: [![Test status](http://flauschig.ch/batch.php?type=tests&account=torston&slug=ndesk-options-fork)](https://ci.appveyor.com/api/projects/status/9rkhbjxrka6ra66a/branch/master)
#### NuGet: [![NuGet version](https://badge.fury.io/nu/Ndesk.Options.Fork.svg)](https://badge.fury.io/nu/Ndesk.Options.Fork)
## Description
This repository is the fork of NDesk.Options 0.2.1 (callback-based program option parser for C#).

Original project link: http://www.ndesk.org/Options

Original project documentation: http://www.ndesk.org/doc/ndesk-options/NDesk.Options/
## Quickstart
1) Create Option Set
```c#
var p = new OptionSet ()
```
2) Add argumets ( Important: if you need to get value in lambda you need `=` like: `g|game=`)
```c#
p.Add("s|status=", n => Console.WriteLine("Status is "+ n));
```
3) If it find your argument the lambda will called, othewise it will return it back
```c#
var unexpectedArguments = p.Parse(argsArray);

foreach(var arg in unexpectedArguments)
{
    Console.WriteLine($"Unknown argument: {arg}");
}
```
 
Output:
```
> program.exe -s Ready
Status is Ready
> program.exe --status "Loading"
Status is Loading
> program.exe --anotherArgument 12
Unknown argument: --anotherArgument
Unknown argument: 12
```
## Getting Deeper 
### Define options
```c#
 var p = new OptionSet ()
 
 // You can call: -n "Rick" or --name "Morty"
 p.Add("n|name=", n => Console.WriteLine(n));
 
 // You can call only with long argument: --name "Morty"
 p.Add("name=", n => Console.WriteLine("First Name: " + n));
 
 // If you use '=' parametr is required: -s "Sanchez"
  p.Add("s|surname=", a => Console.WriteLine("Last Name:" + a));
  
  // Bool options usage: -s
  p.Add("s|isSmart", s => Console.WriteLine(s != null));
  
  // Int options usage: -a 11
  p.Add("a|age=", (int a) => Console.WriteLine("Age: " + s));
 ```
### Parce options
```c#
var unexpectedArguments = p.Parse (args);

foreach(var arg in unexpectedArguments)
{
    Console.WriteLine($"Unknown argument: {arg}");
}
 ```
 
#### Command Line: 
```
program.exe --name "Morty" --surname "Smith" --sex "male" -a 13
 
First Name: Morty
Last Name: Smith
Age: 13
Unknown argument: --sex
Unknown argument: male
```
 
## Example
```c#
using System;
using System.Collections.Generic;
using NDesk.Options.Fork;

class Test {
    static int verbosity;

    public static void Main (string[] args)
    {
        bool show_help = false;
        List<string> names = new List<string> ();
        int repeat = 1;

        var p = new OptionSet () {
            { "n|name=", "the {NAME} of someone to greet.", v => names.Add (v) },
            { "r|repeat=", "the number of {TIMES} to repeat the greeting.", (int v) => repeat = v },
            { "v", "increase debug message verbosity", v => { if (v != null) ++verbosity; } },
            { "h|help",  "show this message and exit", v => show_help = v != null },
        };

        List<string> extra;
        try {
            extra = p.Parse (args);
        }
        catch (OptionException e) {
            Console.Write ("greet: ");
            Console.WriteLine (e.Message);
            Console.WriteLine ("Try `greet --help' for more information.");
            return;
        }

        if (show_help) {
            ShowHelp (p);
            return;
        }

        string message;
        if (extra.Count > 0) {
            message = string.Join (" ", extra.ToArray ());
            Debug ("Using new message: {0}", message);
        }
        else {
            message = "Hello {0}!";
            Debug ("Using default message: {0}", message);
        }

        foreach (string name in names) {
            for (int i = 0; i < repeat; ++i)
                Console.WriteLine (message, name);
        }
    }

    static void ShowHelp (OptionSet p)
    {
        Console.WriteLine ("Usage: greet [OPTIONS]+ message");
        Console.WriteLine ("Greet a list of individuals with an optional message.");
        Console.WriteLine ("If no message is specified, a generic greeting is used.");
        Console.WriteLine ();
        Console.WriteLine ("Options:");
        p.WriteOptionDescriptions (Console.Out);
    }

    static void Debug (string format, params object[] args)
    {
        if (verbosity > 0) {
            Console.Write ("# ");
            Console.WriteLine (format, args);
        }
    }
}
```
## Usage:
```
$ mono greet.exe --help
Usage: greet [OPTIONS]+ message
Greet a list of individuals with an optional message.
If no message is specified, a generic greeting is used.

Options:
  -n, --name=NAME            the NAME of someone to greet.
  -r, --repeat=TIMES         the number of TIMES to repeat the greeting.
  -v                         increase debug message verbosity
  -h, --help                 show this message and exit

$ mono greet.exe -v- -n A -name=B --name=C /name D -nE
Hello A!
Hello B!
Hello C!
Hello D!
Hello E!

$ mono greet.exe -v -n E custom greeting for: {0}
# Using new message: custom greeting for: {0}
custom greeting for: E

$ mono greet.exe -r 3 -n A
Hello A!
Hello A!
Hello A!

$ mono greet.exe -r not-an-int
greet: Could not convert string `not-an-int' to type Int32 for option `-r'.
Try `greet --help' for more information.
```
