using System;
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
                Volume = 0.1,
                Weight = 0.2,
                GlyphName = "Component/PCB/Test",
                BaselineBuildTime = 60 // 1 minute
            };
            var xml = rec.ToXmlString();
            Console.WriteLine(xml);
            Assert.Pass();
        }
    }
}