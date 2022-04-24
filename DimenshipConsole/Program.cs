// See https://aka.ms/new-console-template for more information

using DimenshipConsole;

var system = Initializer.CreateSystem();
var runner = new SystemRunner(system);
var ui = new UserConsole(runner);
ui.Run();