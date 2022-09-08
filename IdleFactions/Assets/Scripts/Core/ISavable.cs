using Newtonsoft.Json;

namespace IdleFactions
{
	public interface ISavable
	{
		void Save(JsonTextWriter writer);
	}
}