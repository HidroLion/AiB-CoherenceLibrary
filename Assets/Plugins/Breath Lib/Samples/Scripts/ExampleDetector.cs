using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BreathLib.Core;

public class ExampleDetector : IBreathDetector
{
	public SupportedPlatforms SupportedPlatforms => throw new System.NotImplementedException();

	public BreathSample GetBreathSample(BreathStream stream)
	{
		throw new System.NotImplementedException();
	}
}
