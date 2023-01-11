using BreathLib.SerializableInterface;

namespace BreathLib.Core
{
	/// <summary>
	/// Interface between Breath Detection scripts and the Aggregate Breath script.
	/// </summary>
	public interface IBreathDetector
	{
		/// <summary>
		/// Gets the supported platforms. This is used to determine which detection features are available, as well as for warnings if the platform is not supported.
		/// </summary>
		public SupportedPlatforms SupportedPlatforms { get; }

		// TODO : Needed hardware

		public void Initialize(BreathStream stream) { }

		/// <summary>
		/// Returns the data values for the last detected breath (breath at this specific time).
		/// </summary>
		/// <returns></returns>
		public BreathSample GetBreathSample(BreathStream stream);
	}
}
