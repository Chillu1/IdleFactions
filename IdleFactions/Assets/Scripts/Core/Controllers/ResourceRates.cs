using System;

namespace IdleFactions
{
	public class ResourceRates
	{
		public double[] Rates { get; }

		private double _interval = 1; //TODO Changeable interval in settings 
		private double _timer;

		private readonly double[] _sums;

		public ResourceRates()
		{
			Rates = new double[ResourceTypeHelper.ResourceTypes.Length];
			_sums = new double[ResourceTypeHelper.ResourceTypes.Length];
		}

		public void Update(double dt)
		{
			_timer += dt;
			if (_timer < _interval)
				return;

			_timer = 0;
			Array.Copy(_sums, Rates, _sums.Length);
			ResetSums();
		}

		public void ChangeResource(ResourceType type, double amount)
		{
			_sums[(int)type] += amount;
		}

		public void ResetSums()
		{
			//Would be best to reset the oldest rates first, but not the biggest deal
			Array.Clear(_sums, 0, _sums.Length);
		}
	}
}