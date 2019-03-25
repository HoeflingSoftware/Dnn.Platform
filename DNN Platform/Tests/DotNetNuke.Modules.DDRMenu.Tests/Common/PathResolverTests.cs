using DotNetNuke.Tests.Instance.Utilities;
using DotNetNuke.Web.DDRMenu.DNNCommon;
using NUnit.Framework;

namespace DotNetNuke.Modules.DDRMenu.Common.Tests
{
    [TestFixture]
    public class PathResolverTests
    {
        [Test]
        public void Constructor_ManifestFolder_Test()
        {
            UnitTestHelper.SetHttpContextWithSimulatedRequest("host", "application", "path", "page");
            var pathResolver = new PathResolver("somemanifestfolder");
        }

        [Test]
        public void Constructor_Default_Test()
        {
            UnitTestHelper.SetHttpContextWithSimulatedRequest("host", "application", "path", "page");
            var pathResolver = new PathResolver();
        }

        [Test]
        public void Initialize_ManifestFolder_Test()
        {
            var pathResolver = new PathResolver(null, null);
            pathResolver.Initialize("somemanifestfolder");
        }
    }
}
