using DotNetNuke.Web.DDRMenu.TemplateEngine;
using NUnit.Framework;

namespace DotNetNuke.Modules.DDRMenu.TemplateEngine.Tests
{
    [TestFixture]
    public class RazorTemplateProcessorTests
    {
        private RazorTemplateProcessor _processor;

        [SetUp]
        public void Setup()
        {
            _processor = new RazorTemplateProcessor(null);
        }

        [TearDown]
        public void TearDown()
        {
            _processor = null;
        }

        [Test]
        public void LoadDefinition_cshtml_Test()
        {
            TemplateDefinition definition = new TemplateDefinition();
            definition.TemplateVirtualPath = "index.cshtml";

            bool actual = _processor.LoadDefinition(definition);

            Assert.IsTrue(actual);
        }

        [Test]
        public void LoadDefinition_vbhtml_Test()
        {
            TemplateDefinition definition = new TemplateDefinition();
            definition.TemplateVirtualPath = "index.vbhtml";

            bool actual = _processor.LoadDefinition(definition);

            Assert.IsTrue(actual);
        }

        [Test]
        public void LoadDefinition_Invalid_Test()
        {
            TemplateDefinition definition = new TemplateDefinition();
            definition.TemplateVirtualPath = "index.html";

            bool actual = _processor.LoadDefinition(definition);

            Assert.IsFalse(actual);
        }
    }
}
