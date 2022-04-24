using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using DimenshipBase;
using DimenshipBase.FungibleItems;
using NUnit.Framework;

namespace DimenshipBaseTests
{
    [TestFixture]
    public class SerializationTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestRecipeXml()
        {
            ComponentRecipe rec = new ComponentRecipe()
            {
                Id = "TestRecipeId",
                Description = "Test description",
                Name = "Test Recipe",
                Tags = "test component",
                GlyphName = "Component/PCB/Test",
                BaselineBuildTime = 60, // 1 minute
                RequiredFacility = "ElectronicAssembly",
                BaselineIngredientList = new List<Ingredient>()
                {
                    new() { Required = 2, ResourceId = "Resource/Basic/Ore" },
                    new() { Required = 3, ResourceId = "Resource/Basic/Organic" },
                }
                
            };
            var xml = (new List<ComponentRecipe>() { rec }).ToXmlString();
            Console.WriteLine(xml);
            Assert.Pass();
        }

        [Test]
        public void TestFacilityXml()
        {
            FacilityBaseClass fbFactory = new FacilityBaseClass()
            {
                Name = "Basic Factory",
                Description = "Basic Factory for simple components production",
                Id = "factory/basic",
                Tags = "factory production facility",
                GlyphName = "facilities/factory/basic",
                IdlePowerConsumption = 0.1
            };

            var xml = (new List<FacilityBaseClass>() { fbFactory }).ToXmlString();
            Console.WriteLine(xml);
            Assert.Pass();
        }

        [Test]
        public void StaticDataSerialization()
        {
            StaticDataSubSystem staticData = new StaticDataSubSystem();
            FacilityBaseClass fbFactory = new FacilityBaseClass()
            {
                Name = "Basic Factory",
                Description = "Basic Factory for simple components production",
                Id = Category.factory.Path("basic"),
                Tags = "factory production facility",
                GlyphName = "facilities/factory/basic",
                Functions = "Workshop Assembly Purifier",
                IdlePowerConsumption = 0.1
            };
            staticData.AddFacilityClass(fbFactory);
            var recipe = new ComponentRecipe()
            {
                Name = "Wheel Chassis MK1",
                Description = "Description of Wheel Chassis MK1",
                Tags = "chassis,wheel,component,bot",
                Id = Category.recipe.Path("components", "chassis", "wheel", "mk1"),
                GlyphName = Category.recipe.Path("components", "chassis", "wheel", "mk1"),
                RequiredFacility = Category.factory.Path("basic"),
                BaselineBuildTime = 120,
                BaselineIngredientList = new List<Ingredient>()
                {
                    new(){Required = 10, ResourceId = Category.resource.Path("metal"),},
                    new(){Required = 20, ResourceId = Category.resource.Path("composite"),}
                }
            };
            staticData.AddRecipe(recipe);
            staticData.AddItemClass(new ItemClassBase()
            {
                Id = Category.resource.Path("metal"),
                Description = "Metal description",
                Name = "Metal name",
                Tags = "basic,resource,metal",
                Volume = 1,
                Weight = 7.86,
                GlyphName = Category.resource.Path("metal"),
            });
            var buf = staticData.SerializeJSON();
            File.WriteAllBytes(@"C:\Temp\staticData.txt", buf);
        }

        //[Test]
        [TestCase("StaticDataRegression.json")]
        [TestCase("StaticDataTests.json")]
        [TestCase("StaticDataGenerated.json")]
         public void StaticDataDeserialization(string file)
        //public void StaticDataDeserialization()
        {
            //string file = "StaticDataTests.json";
            var resFileName = Path.Join(TestContext.CurrentContext.TestDirectory, "Data", file);
            var buf = File.ReadAllBytes(resFileName);
            StaticDataSubSystem sd = new StaticDataSubSystem();
            sd.DeserializeJSON(buf);
            var test = sd.SerializeJSON();
            Assert.NotNull(test);
            sd.GetItemClass(Category.resource.Path("ore"));
            sd.GetItemClass(Category.component.Path("chassis", "wheel", "mk1"));
            sd.GetRecipe(Category.recipe.Path("component", "chassis", "wheel", "mk1"));
            sd.GetFacilityClass(Category.factory.Path("basic"));
        }
    }
    
}