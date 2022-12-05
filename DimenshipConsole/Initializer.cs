using System;
using System.IO;
using DimenshipBase;
using DimenshipBase.SubSystems;

namespace DimenshipConsole;

public class Initializer
{
    private const string StaticDataFile = @"json database.txt";
    private const string StaticDataFolder = @"Data";
    public static DimenshipSystem CreateSystem()
    {
        DimenshipSystem system = new DimenshipSystem();
        FacilitySubSystem facilities = new FacilitySubSystem();
        StaticDataSubSystem staticData = new StaticDataSubSystem();
        ProcessSubSystem process = new ProcessSubSystem();
        ItemStorageSubSystem store = new ItemStorageSubSystem();
        ResearchSubSystem research = new ResearchSubSystem();
        string file = Path.Combine(Environment.CurrentDirectory, StaticDataFolder, StaticDataFile);
        var buf = File.ReadAllBytes(file);

        staticData.DeserializeJSON(buf);
        process.Initialize(system);
        facilities.Initialize(system);
        research.Initialize(system);

        var fbFactory = staticData.GetFacilityClass(Category.factory.Path("basic"));
        var fCount = 0;
        facilities.AddFacility(new Facility(++fCount) { Name = "Starter Factory", ClassId = fbFactory.Id });

        store.Store(Category.resource.Path("ore"), 1000);
        store.Store(Category.resource.Path("ice"), 1000);
        store.Store(Category.resource.Path("organic"), 1000);
        
        research.AddRecipe(Category.recipe.Path("component","chassis","wheel","mk1"));
        research.AddRecipe(Category.recipe.Path("component","chassis","hover"));
        research.AddRecipe(Category.recipe.Path("component","chassis","legs"));
        research.AddRecipe(Category.recipe.Path("component","sensor","radar"));

        system.AddSubsystem(new NotificationSubSystem());
        system.AddSubsystem(facilities);
        system.AddSubsystem(staticData);
        system.AddSubsystem(process);
        system.AddSubsystem(store);
        system.AddSubsystem(research);
        return system;
    }
}