using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace DotNetNuke.Web.DDRMenu.DNNCommon
{
    public class PathResolver : IPathResolver
	{
        protected DNNContext DNNContext { get; }
        protected HttpContext HttpContext { get; }
        
		private string manifestFolder;

		public PathResolver()
		{
            HttpContext = HttpContext.Current;
            DNNContext = DNNContext.Current;
		}

        [Obsolete("Deprecated in 9.3.1, will be removed in 11.0.0, use \"Initialize(string manifestFolder)\" instead.")]
        public PathResolver(string manifestFolder) : this()
        {
            Initialize(manifestFolder);
        }

        public PathResolver(HttpContext httpContext, DNNContext dnnContext)
        {
            HttpContext = httpContext;
            DNNContext = dnnContext;
        }

        public IPathResolver Initialize(string manifestFolder)
        {
            this.manifestFolder = manifestFolder;
            return this;
        }

        public string Resolve(string path, params RelativeTo[] roots)
		{
			var mappings = new Dictionary<string, RelativeTo>
			               {
			               	{"[DDRMENU]", RelativeTo.Module},
			               	{"[MODULE]", RelativeTo.Module},
			               	{"[MANIFEST]", RelativeTo.Manifest},
			               	{"[PORTAL]", RelativeTo.Portal},
			               	{"[SKIN]", RelativeTo.Skin},
			               	{"[CONTAINER]", RelativeTo.Container},
			               	{"[DNN]", RelativeTo.Dnn}
			               };
			foreach (var key in mappings.Keys)
			{
				if (path.StartsWith(key, StringComparison.InvariantCultureIgnoreCase))
				{
					path = path.Substring(key.Length);
					roots = new[] {mappings[key]};
					break;
				}
			}

			if (path.StartsWith("/"))
			{
				path = path.Substring(1);
			}

			if (!path.StartsWith("~") && !path.Contains(":"))
			{
				foreach (var root in roots)
				{
					string resolvedPath = null;
					switch (root)
					{
						case RelativeTo.Container:
							var container = DNNContext.HostControl;
							while ((container != null) && !(container is UI.Containers.Container))
							{
								container = container.Parent;
							}
							var containerRoot = (container == null)
							                    	? DNNContext.SkinPath
// ReSharper disable PossibleNullReferenceException
													: Path.GetDirectoryName(((UI.Containers.Container)container).AppRelativeVirtualPath).Replace(
// ReSharper restore PossibleNullReferenceException
														'\\', '/') + "/";
							resolvedPath = Path.Combine(containerRoot, path);
							break;
						case RelativeTo.Dnn:
							resolvedPath = Path.Combine("~/", path);
							break;
						case RelativeTo.Manifest:
							if (!string.IsNullOrEmpty(manifestFolder))
							{
								resolvedPath = Path.Combine(manifestFolder + "/", path);
							}
							break;
						case RelativeTo.Module:
							resolvedPath = Path.Combine(DNNContext.ModuleFolder, path);
							break;
						case RelativeTo.Portal:
							resolvedPath = Path.Combine(DNNContext.PortalSettings.HomeDirectory, path);
							break;
						case RelativeTo.Skin:
							resolvedPath = Path.Combine(DNNContext.SkinPath, path);
							break;
					}

					if (!String.IsNullOrEmpty(resolvedPath))
					{
						var sep = resolvedPath.LastIndexOf('/');
						var dirName = resolvedPath.Substring(0, sep + 1);
						var fileName = resolvedPath.Substring(sep + 1);

						var mappedDir = HttpContext.Server.MapPath(dirName);
						mappedDir = mappedDir.Remove(mappedDir.Length - 1);
						if (Directory.Exists(mappedDir))
						{
							if (String.IsNullOrEmpty(fileName))
								return resolvedPath.Replace('\\', '/');
	
							var matches = Directory.GetFileSystemEntries(mappedDir, fileName);
							if (matches.Length > 0)
							{
								resolvedPath = (dirName + Path.GetFileName(matches[0])).Replace('\\', '/');
								return resolvedPath;
							}
						}
					}
				}
			}

			return path;
		}
    }
}