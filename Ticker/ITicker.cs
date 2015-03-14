using System;

namespace Svelto.Ticker
{
	public interface ITicker
	{
		void Add(ITickable tickable);
        void Remove(ITickable tickable);
        void AddPhysical(IPhysicallyTickable tickable);
        void RemovePhysical(IPhysicallyTickable tickable);
        void AddLate(ILateTickable tickable);
        void RemoveLate(ILateTickable tickable);
	}
}


