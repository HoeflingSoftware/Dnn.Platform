namespace DotNetNuke.Web.DDRMenu.DNNCommon
{
    public interface IPathResolver
    {
        IPathResolver Initialize(string manifestFolder);
        string Resolve(string path, params RelativeTo[] roots);
    }
}