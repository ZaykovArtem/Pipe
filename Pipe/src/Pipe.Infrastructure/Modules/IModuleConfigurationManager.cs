namespace Pipe.Infrastructure.Modules
{
	public interface IModuleConfigurationManager
	{
		IEnumerable<ModuleInfo> GetModules();
	}
}
