using System;
using System.Collections.Generic;
using DimenshipBase;
using DimenshipBase.FungibleItems;
using NUnit.Framework;

namespace DimenshipBaseTests
{
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
    }
}