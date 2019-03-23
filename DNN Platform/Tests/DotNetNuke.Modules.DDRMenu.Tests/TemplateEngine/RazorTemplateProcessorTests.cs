using DotNetNuke.Web.DDRMenu.TemplateEngine;
using NUnit.Framework;
using System.IO;
using System.Text;
using System.Web.UI;

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

        [Test]
        public void Render_Invalid_Null_VirtualPath_Test()
        {
            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            HtmlTextWriter htmlWriter = new HtmlTextWriter(writer);

            TemplateDefinition definition = new TemplateDefinition();
            definition.TemplateVirtualPath = null;

            _processor.Render(null, htmlWriter, definition);

            Assert.IsEmpty(builder.ToString());
        }

        [Test]
        public void Render_Invalid_Empty_VirtualPath_Test()
        {
            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            HtmlTextWriter htmlWriter = new HtmlTextWriter(writer);

            TemplateDefinition definition = new TemplateDefinition();
            definition.TemplateVirtualPath = string.Empty;

            _processor.Render(null, htmlWriter, definition);

            Assert.IsEmpty(builder.ToString());
        }

        [Test]
        public void Render_Invalid_Null_TemplateDefinition_Test()
        {
            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            HtmlTextWriter htmlWriter = new HtmlTextWriter(writer);

            _processor.Render(null, htmlWriter, null);

            Assert.IsEmpty(builder.ToString());
        }
    }
}
