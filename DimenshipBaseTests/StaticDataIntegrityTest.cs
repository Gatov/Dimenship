using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DimenshipBase;
using DimenshipBase.FungibleItems;
using NUnit.Framework;

namespace DimenshipBaseTests;

[TestFixture]
public class StaticDataIntegrityTest
{
    
    [TestCase("StaticDataTests.json")]
    [TestCase("json database.txt")]
    public void RecipeIntegrityTest(string file)
    {
        var sd = Load(file);
        var recipes  = sd.GetAllRecipes();
        StringBuilder issues = new();

        bool recipeIsOk;

        void ReportIssue(ComponentRecipe componentRecipe, string issue)
        {
            if (recipeIsOk)
                issues.AppendLine($"Found issues in Recipe {componentRecipe.Name} ({componentRecipe.Id}):");
            recipeIsOk = false;
            issues.AppendLine("\t" + issue);
        }

        void CheckExistence<T>(Func<string,T> a, ComponentRecipe recipe, string type, string classId)
        {
            try
            {
                a(classId);
            }
            catch (KeyNotFoundException e)
            {
                ReportIssue(recipe, $"{type} id='{classId}' was not found in the static data");
            }
        }

        foreach (var rec in recipes)
        {
            recipeIsOk = true;
            CheckExistence(sd.GetFacilityClass, rec, "Facility",rec.RequiredFacility);
            CheckExistence(sd.GetItemClass, rec, "Resulting Item",rec.Item);
            if (rec.BaselineIngredientList?.Any() != true)
            {
                ReportIssue(rec, "Ingredients list is empty");
            }
            else
                rec.BaselineIngredientList.ForEach(r=>CheckExistence(sd.GetItemClass, rec, "Resource", r.ResourceId));
        }
        //if(issues.Length>0)
        //    Console.WriteLine(issues);
        Assert.IsEmpty(issues.ToString());
    }
    
    
    
    private StaticDataSubSystem Load(string file)
    {
        StaticDataSubSystem staticData = new StaticDataSubSystem();

        var resFileName = Path.IsPathRooted(file)?file: 
            Path.Join(TestContext.CurrentContext.TestDirectory, "Data", file);
        var buf = File.ReadAllBytes(resFileName);
        //StaticDataSubSystem sd = new StaticDataSubSystem();
        staticData.DeserializeJSON(buf);
        return staticData;
    }
}