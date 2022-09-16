using System;
using System.Collections.Generic;

namespace IdleFactions
{
	public class ResourceRates
	{
		public double[] Rates { get; }

		//If in idle-mode, don't use a queue and use a longer interval
		//OR make a snapshot-based rates. Saving a snapshot of the resources every X seconds, then calculating rate.
		private double _interval = 1; //TODO Changeable interval in settings 
		private double _timer;

		private readonly double[] _currentSums;
		private readonly Queue<double[]> _sumsQueue;

		private const int MaxEntries = 5;

		public ResourceRates()
		{
			Rates = new double[ResourceTypeHelper.ResourceTypes.Length];
			_currentSums = new double[ResourceTypeHelper.ResourceTypes.Length];
			_sumsQueue = new Queue<double[]>(MaxEntries);
		}

		public void Update(double dt)
		{
			_timer += dt;
			if (_timer < _interval)
				return;

			_timer = 0;
			ResetCurrentSums();
			UpdateRates();
		}

		public void ChangeResource(ResourceType type, double amount)
		{
			_currentSums[(int)type] += amount;
		}

		private void UpdateRates()
		{
			Array.Clear(Rates, 0, Rates.Length);
			foreach (double[] sums in _sumsQueue)
				for (int i = 0; i < sums.Length; i++)
					Rates[i] += sums[i];
			for (int i = 0; i < Rates.Length; i++)
				Rates[i] /= _sumsQueue.Count;
		}

		private void ResetCurrentSums()
		{
			_sumsQueue.Enqueue((double[])_currentSums.Clone());
			Array.Clear(_currentSums, 0, _currentSums.Length);
			if (_sumsQueue.Count > MaxEntries)
				_sumsQueue.Dequeue();
		}
	}
}