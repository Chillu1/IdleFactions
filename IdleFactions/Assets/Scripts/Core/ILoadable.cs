using Newtonsoft.Json.Linq;

namespace IdleFactions
{
	public interface ILoadable
	{
		void Load(JObject data);
	}
}