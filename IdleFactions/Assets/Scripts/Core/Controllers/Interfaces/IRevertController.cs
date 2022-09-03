namespace IdleFactions
{
	public interface IRevertController
	{
		void AddAction(IRevertible action);
		void RevertLastAction();
		void Update(float dt);
	}
}