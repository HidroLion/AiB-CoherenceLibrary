using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BreathLib.Core;

public class MakeTableForBeathPattern : MonoBehaviour
{
	public TextAsset jsonFile;

	public GameObject point;

	public float interval = 0.125f;

	private void Start()
	{
		Debug.Log(PatternLibrary.TableForPattern(jsonFile, interval));
		TableForPattern(PatternLibrary.GetBreathPattern(jsonFile), interval);
	}

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
					go = Instantiate(point);
					go.transform.position = new Vector3(time*2, bs[index].Value + 1.2f * index, 0);
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
