using System.Collections.Generic;
using BreathLib.Core;
using BreathLib.SerializableInterface;
using UnityEngine;

public class BreathStreamVisualizer : MonoBehaviour
{
	[SerializeField, Tooltip("The breath stream that is being correlated to a target pattern")]
	private SerializableInterface<IBreathStreamContainer> _breathStream = new(false);
	private BreathStream BreathStream => _breathStream.Value.BreathStream;

	public GameObject PlotPointPrefab;
	public GameObject BarPrefab;
	public int maxPlotSamples = 1000;
	public float ypadding = 1f;
	public float speed = 1f;

	private Queue<GameObject> _plotPoints = new Queue<GameObject>();
	private void FixedUpdate()
	{
		if (BreathStream == null)
		{
			Debug.LogWarning("No breath stream to visualize");
			return;
		}

		if (BreathStream.TotalSamples <= 0) return;

		for (int i = 0; i < BreathStream.Last.Length; i++)
		{
			
			if (BreathStream.Last[i].HasValue)
			{
				CreatePoint(i);
				CreateBar(i);
			}
		}
		
		while (_plotPoints.Count >= maxPlotSamples)
		{
			Destroy(_plotPoints.Dequeue());
		}

		foreach (var point in _plotPoints)
		{
			point.transform.localPosition += Vector3.left * speed * Time.fixedDeltaTime;
		}
	}

	private void CreatePoint(int index)
	{
		if (PlotPointPrefab == null)
			return;

		var point = Instantiate(PlotPointPrefab, transform);
		point.transform.localPosition = new Vector3(0, BreathStream.Last[index].Value + index * ypadding, 0);
		_plotPoints.Enqueue(point);
	}

	private void CreateBar(int index)
	{
		if (BarPrefab == null)
			return;

		var bar = Instantiate(BarPrefab, transform);
		bar.transform.localPosition = new Vector3(0, index * ypadding, 0);
		bar.transform.localScale = new Vector3(
			bar.transform.localScale.x,
			bar.transform.localScale.y * Mathf.Lerp(0.09f, 1f, BreathStream.Last[index].Value),
			bar.transform.localScale.z
		);
		_plotPoints.Enqueue(bar);
	}
}