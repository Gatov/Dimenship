using System;
using System.Collections.Generic;
using DimenshipBase;
using DimenshipBase.SubSystems;
using System.Linq;

namespace DimenshipConsole.Commands;

public class ListCommand : ICommand
{
    public string Command => "ls";
    public string HelpLine => $"ls category [tag] [and tag]\n categories:" + string.Join(",",Enum.GetNames(typeof(Category)));
    public bool Execute(ISystemStateSet system, string[] args)
    {
        if (args.Length < 1) return false;
        switch (args[0])
        {
            case nameof(Category.component): PrintComponents(system, args[0], args.Skip(1));
                break;
            case nameof(Category.resource): PrintComponents(system, args[0], args.Skip(1));
                break;
            case nameof(Category.recipe): PrintRecipes(system, args[0], args.Skip(1));
                break;
            default: return false;
        }

        return true;
    }

    private void PrintComponents(ISystemStateSet system, string cat, IEnumerable<string> tagse)
    {
        var tags = tagse.ToList();
        var storage = system.GetSubState<ItemStorageSubSystem>();
        var sd = system.GetSubState<StaticDataSubSystem>();
        var all = storage.GetItemsOfCategory(cat);
        var list = from item in all
            let cls = sd.GetItemClass(item.ClassId)
            where tags.All(cls.HasTag)
            select new {Class = cls, Item = item};

        foreach (var it in list)
        {
//            string s = $"{it.Class.Name,6}";
            Console.WriteLine($"{it.Class.Name,12} | {it.Item.Count,5} | {it.Item.ClassId}");
        }
    }
    private void PrintRecipes(ISystemStateSet system, string cat, IEnumerable<string> tagse)
    {
        var tags = tagse.ToList();
        var research = system.GetSubState<ResearchSubSystem>();
        var sd = system.GetSubState<StaticDataSubSystem>();
        var all = research.GetAllKnownRecipes();
        var list = from rec in all
            let cls = sd.GetRecipe(rec.Id)
            where tags.All(cls.HasTag)
            select new {Class = cls, Recipe = rec};

        foreach (var it in list)
        {
            Console.WriteLine($"{it.Class.Name,15} | {it.Recipe.BaselineBuildTime,6} | {it.Recipe.Id,30} | {it.Recipe.Item}");
        }
    }
}