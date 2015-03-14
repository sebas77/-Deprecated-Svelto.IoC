namespace Svelto.Ticker
{
    public interface ITickable
    {
        void Tick(float deltaSec);
    }

    public interface ILateTickable
    {
        void LateTick(float deltaSec);
    }

	public interface IPhysicallyTickable
	{
		void PhysicsTick(float deltaSec);
	}
}
