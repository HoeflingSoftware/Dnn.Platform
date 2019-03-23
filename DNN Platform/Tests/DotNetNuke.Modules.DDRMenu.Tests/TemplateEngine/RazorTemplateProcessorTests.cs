using DotNetNuke.Tests.Instance.Utilities;
using DotNetNuke.Web.DDRMenu.DNNCommon;
using DotNetNuke.Web.DDRMenu.TemplateEngine;
using DotNetNuke.Web.Razor;
using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.WebPages;

namespace DotNetNuke.Modules.DDRMenu.TemplateEngine.Tests
{
    [TestFixture]
    public class RazorTemplateProcessorTests
    {
        private RazorTemplateProcessor _processor;

        [SetUp]
        public void Setup()
        {
            UnitTestHelper.SetHttpContextWithSimulatedRequest("host", "application", "c:/helloworld", "index");

            var control = new Mock<Control>();
            control.Setup(x => x.ClientID).Returns(Guid.NewGuid().ToString());
            var dnnContext = new DNNContext(control.Object);

            var pathResolver = new Mock<IPathResolver>();
                        
            _processor = new RazorTemplateProcessor_TestHarness(HttpContext.Current, dnnContext, pathResolver.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _processor = null;
            UnitTestHelper.ClearHttpContext();
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

        [Test]
        public void Render_WebPage_Valid_Test()
        {
            (_processor as RazorTemplateProcessor_TestHarness).MockPage = new Mock<WebPage>().Object;

            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            HtmlTextWriter htmlWriter = new HtmlTextWriter(writer);

            TemplateDefinition definition = new TemplateDefinition();
            definition.TemplateVirtualPath = "Default.cshtml";

            _processor.Render(null, htmlWriter, definition);
        }
        
        private class RazorTemplateProcessor_TestHarness : RazorTemplateProcessor
        {
            public WebPageBase MockPage { get; set; }
            public RazorTemplateProcessor_TestHarness(
                HttpContext httpContext, 
                DNNContext dnnContext,
                IPathResolver pathResolver) 
                : base(httpContext, dnnContext, pathResolver)
            {
            }

            protected override WebPageBase CreateWebPageFromVirtualPath(string virtualPath)
            {
                return MockPage;
            }
        }
    }
}
