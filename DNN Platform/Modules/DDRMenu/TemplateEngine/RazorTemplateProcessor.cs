using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.WebPages;
using DotNetNuke.Web.DDRMenu.DNNCommon;
using DotNetNuke.Web.Razor;

namespace DotNetNuke.Web.DDRMenu.TemplateEngine
{
    public class RazorTemplateProcessor : ITemplateProcessor
    {
        protected HttpContext HttpContext { get; }
        protected DNNContext DNNContext { get; }
        protected IPathResolver PathResolver { get; }

        public RazorTemplateProcessor(
            HttpContext context, 
            DNNContext dnnContext, 
            IPathResolver pathResolver)
        {
            HttpContext = context;
            DNNContext = dnnContext;
            PathResolver = pathResolver;
        }

        public bool LoadDefinition(TemplateDefinition baseDefinition)
        {
            var virtualPath = baseDefinition.TemplateVirtualPath;

            if (!virtualPath.EndsWith(".cshtml", StringComparison.InvariantCultureIgnoreCase) &&
                !virtualPath.EndsWith(".vbhtml", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return true;
        }

        public void Render(object source, HtmlTextWriter htmlWriter, TemplateDefinition liveDefinition)
        {
            if (!(string.IsNullOrEmpty(liveDefinition?.TemplateVirtualPath)))
            {
                PathResolver.Initialize(liveDefinition.Folder);

                dynamic model = new ExpandoObject();
                model.Source = source;
                model.ControlID = DNNContext.HostControl.ClientID;
                model.Options = ConvertToJson(liveDefinition.ClientOptions);
                model.DNNPath = PathResolver.Resolve("/", RelativeTo.Dnn);
                model.ManifestPath = PathResolver.Resolve("/", RelativeTo.Manifest);
                model.PortalPath = PathResolver.Resolve("/", RelativeTo.Portal);
                model.SkinPath = PathResolver.Resolve("/", RelativeTo.Skin);

                var modelDictionary = model as IDictionary<string, object>;
                liveDefinition.TemplateArguments.ForEach(a => modelDictionary.Add(a.Name, a.Value));

                htmlWriter.Write(RenderTemplate(liveDefinition.TemplateVirtualPath, model));
            }
        }

        protected virtual WebPageBase CreateWebPageFromVirtualPath(string virtualPath)
        {
            return WebPageBase.CreateInstanceFromVirtualPath(virtualPath);
        }

        private StringWriter RenderTemplate(string virtualPath, dynamic model)
        {
            var page = CreateWebPageFromVirtualPath(virtualPath);
            var httpContext = new HttpContextWrapper(HttpContext);
            var pageContext = new WebPageContext(httpContext, page, model);

            var writer = new StringWriter();

            if (page is WebPage)
            {
                page.ExecutePageHierarchy(pageContext, writer);
            }
            else
            {
                var razorEngine = new RazorEngine(virtualPath, null, null);
                razorEngine.Render<dynamic>(writer, model);
            }

            return writer;
        }

        protected static string ConvertToJson(List<ClientOption> options)
        {
            var result = new StringBuilder();
            result.Append("{");

            if (options != null)
            {
                foreach (var option in options)
                {
                    if (option is ClientNumber)
                    {
                        result.AppendFormat("{0}:{1},", option.Name, Utilities.ConvertToJs(Convert.ToDecimal(option.Value)));
                    }
                    else if (option is ClientString)
                    {
                        result.AppendFormat("{0}:{1},", option.Name, Utilities.ConvertToJs(option.Value));
                    }
                    else if (option is ClientBoolean)
                    {
                        result.AppendFormat(
                            "{0}:{1},", option.Name, Utilities.ConvertToJs(Convert.ToBoolean(option.Value.ToLowerInvariant())));
                    }
                    else
                    {
                        result.AppendFormat("{0}:{1},", option.Name, option.Value);
                    }
                }
                if (options.Count > 0)
                {
                    result.Remove(result.Length - 1, 1);
                }
            }

            result.Append("}");
            return result.ToString();
        }
    }
}