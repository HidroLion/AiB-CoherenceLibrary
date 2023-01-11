namespace BreathLib.Core
{
	/// <summary>
	/// Flags. These are used to enable/disable detection features based on compatibility. See <link>BreathDetection.Flags</link> for more information.
	/// </summary>
	public enum SupportedPlatforms
	{
		/// <summary>
		/// No platform is supported.
		/// </summary>
		None = 0,
		/// <summary>
		/// Game can be built for web platform. This platform will assume compatibility for Chrome, Firefox, Edge, and Safari.
		/// </summary>
		WebGL = 1 << 1,
		/// <summary>
		/// Game can be built for Mobile platform, supporting iOS. If on, assume flat-screen compatibility.
		/// </summary>
		iOS = 1 << 2,
		/// <summary>
		/// Game can be built for Mobile platform, supporting Android. If on, assume flat-screen compatibility.
		/// </summary>
		Android = 1 << 3,
		/// <summary>
		/// Game can be built for Desktop platform, supporting Windows. If on, assume flat-screen compatibility for both x32 and x64.
		/// </summary>
		Windows = 1 << 4,
		/// <summary>
		/// Game can be built for Desktop platform, supporting MacOS. If on, assume flat-screen compatibility.
		/// </summary>
		MacOS = 1 << 5,
		/// <summary>
		/// Game can be built for Desktop platform, supporting Linux. If on, assume flat-screen compatibility.
		/// </summary>
		Linux = 1 << 6,
		/// <summary>
		/// Game can be built for Oculus platform mobile platform (Android).
		/// </summary>
		OculusStandalone = 1 << 7,
		/// <summary>
		/// Game can be built for Oculus platform desktop platform (Windows only supported by oculus).
		/// </summary>
		OculusRift = 1 << 8,
		/// <summary>
		/// Game can be built for SteamVR platform desktop platform. If on, assume Steam for Windows compatibility.
		/// </summary>
		SteamVR = 1 << 9,

		/// <summary>
        /// All VR platforms are supported.
        /// </summary>
        VR = OculusStandalone | OculusRift | SteamVR,

		/// <summary>
        /// All mobile platforms are supported.
        /// </summary>
		Mobile = iOS | Android,

		/// <summary>
		/// All desktop platforms are supported.
		/// </summary>
		Desktop = Windows | MacOS | Linux,

		/// <summary>
        /// All flat-screen platforms are supported.
        /// </summary>
		FlatScreen = Mobile | Desktop | WebGL,

		/// <summary>
        /// All platforms with keyboard and mouse are supported.
        /// </summary>
		KeyboardMouse = Desktop | WebGL,

		/// <summary>
		/// All platforms are supported.
		/// </summary>
		All = WebGL | iOS | Android | Windows | MacOS | Linux | OculusStandalone | OculusRift | SteamVR,
	}
}
