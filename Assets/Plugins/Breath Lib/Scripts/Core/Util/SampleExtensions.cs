using System;

namespace BreathLib.Core
{
	public static class SampleExtensions
	{
		/// <summary>
		/// Interpolation between two BreathSample objects, used as a helper function on top of base interpolation functions.
		/// </summary>
		/// <param name="start">BreathSample, weighted more with lower t. When t == 0, return is start.</param>
		/// <param name="end">BreathSample, weighted more with higher t. When t == 1, return is end.</param>
		/// <param name="t">Normalized value representing the point in the interpolation</param>
		/// <param name="interpolationMethod">The method of interpolation that will be used for all data values in the BreathSample</param>
		/// <returns>A new breath sample object with interpolated values from start to end with a transition point a t.</returns>
		public static BreathSample SampleInterp(this BreathSample start, BreathSample end, float t, Interpolators.Method interpolationMethod)
		{
			BreathSample result = new BreathSample();

			for (int i = 0; i < result.Length; i++)
			{
				if (start[i] != null && end[i] != null)
				{
					result[i] = Interpolators.Interp(start[i].Value, end[i].Value, t, interpolationMethod);
				}
			}

			return result;
		}

		/// <summary>
		/// Calculates the RMSE between two breath sample sets. "No" is used as a scaling factor for the RMSE bec a "No" value closer to "1" can mean that all other data in the breath sample is ambiguous/invalid.
		/// <para>RMSE = sqrt( sum(||y - y'||^2) / N ). y = data 1, y' = data 2, N is common values</para>
		/// </summary>
		/// <param name="a">BreathSample to be compared</param>
		/// <param name="b">BreathSample to be compared</param>
		/// <returns>A value [0,1] that represents the likelihood that the two breath sample are the same/similar.</returns>
		public static float SampleLikeness(this BreathSample a, BreathSample b)
		{
			// Total Squared Error.
			float SE = 0;
			int N = 0;
			// No breath scales different than other values:
			// If there should be no breath, the other values will not be used. Thus, we will scale the likeness if there is no breath.
			float noScalar = 1;
			float noDiff = 0;

			if (a.Yes != null && b.Yes != null)
			{
				noScalar = a.Yes.Value * b.Yes.Value;
				noDiff = Math.Abs(a.No.Value - b.No.Value);
			}
			else if (a.Yes == null && b.Yes == null)
			{
				noScalar = 1;
			}
			else
			{
				UnityEngine.Debug.LogWarningFormat("Correlator.TemporalLikeness(): One of the BreathSample objects has no No value, but the other one does. This will typically lead to unexpected likelihoods. Inputs: a: [{0}], b: [{1}]", a, b);
				if (a.Yes != null)
					noScalar = a.Yes.Value;

				else
					noScalar = b.Yes.Value;
			}

			for (int i = 1; i < a.Length; i++)
			{
				if (a[i] != null && b[i] != null)
				{
					SE += (float)Math.Pow(a[i].Value - b[i].Value, 2);
					N++;
				}
			}
			
			if (N == 0)
			{
				UnityEngine.Debug.LogWarning("Correlator.TemporalLikeness(): No comparisons for BreathSample could be made!");
				return 0;
			}

			float RMSE = (float)Math.Sqrt(SE / N) * noScalar;
			UnityEngine.Debug.Assert(RMSE >= 0 && RMSE <= 1, "Correlator.TemporalLikeness(): RMSE is not between 0 and 1!");

			float result = RMSE + (1 - noScalar) * noDiff;
			return 1 - result;
		}

		/// <summary>
        /// Returns a BreathSample that has the closest values to the BreathSample a.
        /// If a does not have a value, the first value in samples will be returned.
        /// </summary>
        /// <param name="a">Sample to compare to</param>
        /// <param name="samples">Samples to condense into one</param>
        /// <returns>A BreathSample that has the closest values, per values, to the BreathSample a.</returns>
		public static BreathSample MaxLikeness(this BreathSample a, params BreathSample[] samples)
		{
			if (samples.Length == 0)
				return new BreathSample();
			else if (samples.Length == 1)
				return samples[0];

			BreathSample min = samples[0];
			BreathSample max = samples[0];

			for (int i = 0; i < samples.Length; i++)
			{
				for (int data = 0; data < samples[i].Length; data++)
				{
					if (samples[i][data] != null)
					{
						if (min[data] == null || samples[i][data].Value < min[data].Value)
							min[data] = samples[i][data];

						if (max[data] == null || samples[i][data].Value > max[data].Value)
							max[data] = samples[i][data];
					}
				}
			}

			return new BreathSample(
				no: a.No >= min.No ? (a.No <= max.No ? a.No : max.No) : min.No,
				_in: a.In >= min.In ? (a.In <= max.In ? a.In : max.In) : min.In,
				nasal: a.Nasal >= min.Nasal ? (a.Nasal <= max.Nasal ? a.Nasal : max.Nasal) : min.Nasal,
				volume: a.Volume >= min.Volume ? (a.Volume <= max.Volume ? a.Volume : max.Volume) : min.Volume,
				pitch: a.Pitch >= min.Pitch ? (a.Pitch <= max.Pitch ? a.Pitch : max.Pitch) : min.Pitch
			);
		}
	}
}