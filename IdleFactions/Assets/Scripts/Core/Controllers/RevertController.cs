using System.Collections.Generic;

namespace IdleFactions
{
	public class RevertController : IRevertController
	{
		private const int MaxReverts = 3;

		private readonly LinkedList<IRevertible> _revertStack;
		private float _timer;

		private const double RevertTime = 10;

		public RevertController()
		{
			_revertStack = new LinkedList<IRevertible>();
		}

		public void Update(float dt)
		{
			if (_revertStack.Count == 0)
				return;

			_timer += dt;

			if (_timer > RevertTime)
			{
				_timer = 0;
				_revertStack.RemoveFirst();
			}
		}

		public void AddAction(IRevertible action)
		{
			if (_revertStack.Count >= MaxReverts)
				_revertStack.RemoveFirst();

			_revertStack.AddLast(action);
		}

		public void RevertLastAction()
		{
			if (_revertStack.Count == 0)
				return;

			_revertStack.Last.Value.Revert();
		}
	}
}