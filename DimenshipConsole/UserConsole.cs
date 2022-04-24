using System;
using System.Collections.Generic;
using System.Linq;
using DimenshipBase;
using DimenshipBase.SubSystems;
using DimenshipConsole.Commands;

namespace DimenshipConsole;

public class UserConsole
{
    private readonly SystemRunner _runner;
    private string _inputSoFar = string.Empty;
    private int _ore;
    private int _ice;
    private int _organic;
    private readonly List<ICommand> _commands;

    private bool _running = true;
    private string _lastCommand = string.Empty;

    public UserConsole(SystemRunner r)
    {
        _runner = r;
        r.SafeExecute(s=>s.GetSubState<NotificationSubSystem>().InfoAction += OnInfo);
        r.SafeExecute(s=>s.GetSubState<NotificationSubSystem>().PushNotificationAction += OnPush);
        _commands = new List<ICommand>()
        {
            new ListCommand(),
            new BuildCommand()
        };
    }

    private void OnPush(string obj)
    {
        Console.WriteLine($"\r!! {obj}");
        Prompt();
    }

    private void OnInfo(string obj)
    {
        Console.WriteLine($"\r{obj}");
        Prompt();
    }

    private void Prompt()
    {
        // get resources
        _runner.SafeExecute(UpdateResources);
        
        Console.Write($"\rM:{_ore} I:{_ice} O:{_organic} > {_inputSoFar}");
    }

    private void UpdateResources(DimenshipSystem system)
    {
        var  s = system.GetSubState<ItemStorageSubSystem>();
        _ore = s.Check(Category.resource.Path("ore"));
        _ice = s.Check(Category.resource.Path("ice"));
        _organic = s.Check(Category.resource.Path("organic"));
    }

    public void Run()
    {
         _runner.Start();
         do
         {
             Prompt();
             var key = Console.ReadKey();
             if (key.Key == ConsoleKey.Enter)
             {
                 _lastCommand = _inputSoFar;
                 Console.WriteLine($"Executing {_inputSoFar}");
                 ProcessCommand(_inputSoFar);
                 
                 _inputSoFar = string.Empty;
                 Console.WriteLine();
             }
             else
             {
                 if (key.Key == ConsoleKey.Backspace)
                     _inputSoFar = _inputSoFar.Length > 1 ? _inputSoFar.Substring(0, _inputSoFar.Length - 1) : string.Empty;
                 else if (key.Key == ConsoleKey.UpArrow)
                     _inputSoFar = _lastCommand;
                 else if(!char.IsControl(key.KeyChar))
                     _inputSoFar += key.KeyChar;
             }
         } while (_running);
    }

    private void ProcessCommand(string s)
    {
        if (s == "exit")
        {
            _runner.Stop();
            _running = false;
            return;
        }

        var args = s.Split(' ',',').Where(x=>!string.IsNullOrWhiteSpace(x)).ToList();
        var cmd = _commands.FirstOrDefault(x => x.Command == args[0]);

        if (cmd == null)
        {
            Console.WriteLine("unrecognized command. List of available commands:\n"+string.Join("\n",_commands.Select(x=>x.Command)));
            return;
        }
        _runner.SafeExecute(sys=>
        {
            var res = cmd.Execute(sys, args.Skip(1).ToArray());
            if(res == false)
                Console.WriteLine($"Failed to execute command. Please use the following format:\n{cmd.HelpLine}");
        });
    }
}