using UnityEngine;

namespace BreathLib.Unity
{
	public static class MathStructExtensions
	{
		public static Vector3 OverwriteX(this Vector3 v, float x) => new Vector3(x, v.y, v.z);
		public static Vector3 OverwriteY(this Vector3 v, float y) => new Vector3(v.x, y, v.z);
		public static Vector3 OverwriteZ(this Vector3 v, float z) => new Vector3(v.x, v.y, z);
	}
}