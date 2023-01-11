using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BreathLib.Core;

// Look at Pattern.cs  & BreathSample.cs to observe how pattern and BreathSample is structured.
public class AutoPatternMaker : MonoBehaviour
{
	public TextAsset jsonFile;

	//public GameObject point;

	/* To Choose which Text Component to display Breath Phase.
	*/
	[Header("Breath Phase Text Component")]
	public TextMeshProUGUI patternText;


	private void Start()
	{
		//Debug.Log(PatternLibrary.TableForPattern(jsonFile, interval));
		//TableForPattern(PatternLibrary.GetBreathPattern(jsonFile), interval);
		TextForPattern(PatternLibrary.GetBreathPattern(jsonFile));
	}

	public void TextForPattern(Pattern pattern) {
		string finalstring = "";
		// Equal to how many pairs of keyframes have same time duration. 
		int offset = 0;

		// Add Name and Total Time
		finalstring += "Breath Type: " + pattern.name + "\n";
		finalstring += "Total Time for a full cycle: " + pattern.length + "sec\n \n";
		finalstring += "|| "+ pattern.name + " Steps ||\n";

		// Goes through each keyframe. MUST FIX TO DEAL WITH KEYFRAMES OVERLAPPING IN TIME.
		for(int index = 0; index < pattern.keyframes.Length; ++index){
			string phasestring = "" + (index+1-offset) + ". ";
			float duration;
			
			// Breath Sample of current keyframe.
			BreathSample bs = pattern.GetTargetAtNormalizedTime(pattern.keyframes[index].time);

			//Getting Duration (how long a keyframe is for)
			if (index < pattern.keyframes.Length-1){
				// If next keyframe is not during the same time.
				if(pattern.keyframes[index+1].time != pattern.keyframes[index].time){					
					duration = pattern.keyframes[index+1].time - pattern.keyframes[index].time; 
				} else {
					++offset;
					continue; 
				}
			} else {
				duration = 1.0f - pattern.keyframes[index].time; 
			}
			duration *= pattern.length;

			// Get Breath Type
			if(bs.No == 1){
				phasestring += "Hold Breath ";
			} else {
				if (bs.In == 1){
					phasestring += "Inhale ";
				} else if (bs.Out == 1){
					phasestring += "Exhale ";
				} else {
					phasestring += "Inhale and Exhale ";
				}

				if(bs.Volume == 0.0){
					phasestring += "silently ";
				} else if(bs.Volume <= 0.5){
					phasestring += "slightly audibly ";
				} else if(bs.Volume > 0.5){
					phasestring += "very audibly ";
				}

				if(bs.Nasal == 1){
					phasestring += "through the Nose ";
				} else if(bs.Mouth == 1){
					phasestring += "through the Mouth ";
				}

				if(bs.Pitch <= 0.5){
					phasestring += "with Low Pitch ";
				} else if(bs.Pitch > 0.5){
					phasestring += "with High Pitch ";
				}

			}
			
			
			phasestring += "for " + duration + " seconds";
			Debug.Log("For frame #" + index + ":");
			Debug.Log("Keyframe: " + pattern.keyframes[index]);
			Debug.Log("Breath Sample: " + pattern.GetTargetAtNormalizedTime(pattern.keyframes[index].time));
			//BreathSample bs = pattern.keyframes[index];
			

			finalstring += phasestring + "\n";
		}

		patternText.text = finalstring;

	}
}

	/*
	public void TableForPattern(Pattern pattern, float interval = 0.125f)
	{
		for (float time = 0; time <= 1; time += interval)
		{
			BreathSample bs = pattern.GetTargetAtNormalizedTime(time);
		   
			GameObject go;
			for (int index = 0; index < bs.Length; index++)
			{
				if (bs[index].HasValue)
				{
					//go = Instantiate(point);
					//go.transform.position = new Vector3(time*2, bs[index].Value + 1.2f * index, 0);
				}
			   
			}

			// GameObject go = Instantiate(point);
			// go.transform.position = new Vector3(time*2, pattern.GetTargetAtNormalizedTime(time).In.Value + ,);
			// go = Instantiate(point);
			// go.transform.position = new Vector3(time*2, pattern.GetTargetAtNormalizedTime(time).No.Value + 1.2f * 1, 0);
			// go = Instantiate(point);
			// go.transform.position = new Vector3(time*2, pattern.GetTargetAtNormalizedTime(time).Mouth.Value + 1.2f * 2, 0);
			// go = Instantiate(point);
			// go.transform.position = new Vector3(time*2, pattern.GetTargetAtNormalizedTime(time).Pitch.Value + 1.2f * 3, 0);
			// go = Instantiate(point);
			// go.transform.position = new Vector3(time*2, pattern.GetTargetAtNormalizedTime(time).Volume.Value + 1.2f * 4, 0);
		}
	}

	public GameObject dot;
	public float progress;
	public float lengthOfGraph;

	private Vector3 origin;

	private void Awake()
	{
		if (dot != null)
			origin = dot.transform.position;
	}

	private void Update()
	{
		if (dot != null)
			dot.transform.position = origin + new Vector3(progress * lengthOfGraph, 0, 0);
	}
}
*/