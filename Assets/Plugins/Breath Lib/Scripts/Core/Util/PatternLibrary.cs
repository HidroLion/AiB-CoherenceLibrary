using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

using TextAsset = UnityEngine.TextAsset;
using Debug = UnityEngine.Debug;

namespace BreathLib.Core
{
	/// <summary>
	/// Helper for caching and processing breath pattern files.
	/// </summary>
	public static class PatternLibrary
	{
		/// <summary>
		/// Cache for breath patterns.
		/// </summary>
        // TODO - this should be changed to a non unity specific dictionary with a Text Asset utility class/function
		// Possibly use a (hash, filename) pair as the key?
		private static Dictionary<TextAsset, Pattern> LoadedBreathTypes = new Dictionary<TextAsset, Pattern>();

		/// <summary>
		/// Given a text file, returns the breath pattern that is defined in the file.
		/// </summary>
		/// <param name="asset">JSON text file that has the information for a breath pattern</param>
		/// <returns>A Pattern object with all of the data listed in the text file.</returns>
		public static Pattern GetBreathPattern(TextAsset asset)
		{
			if (LoadedBreathTypes.TryGetValue(asset, out Pattern value))
				return value;

			value = JsonConvert.DeserializeObject<Pattern>(asset.text);
			value.Initialize();

			if (!value.IsValid())
			{
				Debug.LogError("Loaded Breath Pattern is not valid: " + asset.name);
				return null;
			}

			LoadedBreathTypes.Add(asset, value);

			return value;
		}

		public static string TableForPattern(TextAsset patternToUser, float interval = 0.125f)
		{
			Pattern pattern = PatternLibrary.GetBreathPattern(patternToUser);
			return TableForPattern(pattern, interval);
		}

		public static string TableForPattern(Pattern pattern, float interval = 0.125f)
		{
			string result = "";

			for (float time = 0; time <= 1; time += interval)
			{
				//time = (float)Math.Round(time*100)/100;
				result += "Time: " + time + ", Time (sec): " + (time * pattern.length) + " {" + pattern.GetTargetAtNormalizedTime(time) + "}\n";
			}

			return result;
		}

		/// <summary>
		/// Gets an already loaded breath pattern by name.
		/// </summary>
		/// <param name="name">Breath pattern name.</param>
		/// <returns>Pattern that has been loaded in with a matching name.</returns>
		public static Pattern GetBreathType(string name)
		{
			return LoadedBreathTypes.First(x => x.Key.name == name).Value;
		}
	}
}
