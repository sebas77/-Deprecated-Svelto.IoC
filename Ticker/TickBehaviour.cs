using System.Collections.Generic;
using UnityEngine;

namespace Svelto.Ticker
{
	public class TickBehaviour:MonoBehaviour
	{
		internal void Add(ITickable tickable)
		{
			_ticked.Add(tickable);
		}
		
		internal void Remove(ITickable tickable)
		{
			_ticked.Remove(tickable);
		}

		internal void AddPhysical(IPhysicallyTickable tickable)
		{
			_physicallyticked.Add(tickable);
		}

		internal void RemovePhysical(IPhysicallyTickable tickable)
		{
			_physicallyticked.Remove(tickable);
		}

		internal void AddLate(ILateTickable tickable)
		{
			_lateTicked.Add(tickable);
		}

		internal void RemoveLate(ILateTickable tickable)
		{
			_lateTicked.Remove(tickable);
		}

		void Update()
		{
			foreach (ITickable tickable in _ticked)
				tickable.Tick(Time.deltaTime);
		}
		void LateUpdate()
		{
			foreach (ILateTickable tickable in _lateTicked)
				tickable.LateTick(Time.deltaTime);
		}
		void FixedUpdate()
		{
			foreach (IPhysicallyTickable tickable in _physicallyticked)
				tickable.PhysicsTick(Time.fixedDeltaTime);
		}

		private List<ITickable> _ticked = new List<ITickable>();
		private List<ILateTickable> _lateTicked = new List<ILateTickable>();
		private List<IPhysicallyTickable> _physicallyticked = new List<IPhysicallyTickable>();
	}
}
