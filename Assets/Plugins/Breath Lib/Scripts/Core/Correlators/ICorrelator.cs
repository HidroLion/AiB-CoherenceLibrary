namespace BreathLib.Core
{
	public interface ICorrelator
	{
		public float Correlation { get; }
		public bool IsRunning { get; }
		public void Begin(bool syncWithHistory = false);
		public void Stop();
	}
}