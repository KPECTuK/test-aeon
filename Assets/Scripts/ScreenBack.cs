using System;
using System.Collections;
using System.Collections.Generic;
using Modules.Engine.Runtime.Extensions;
using UnityEngine;

public class ScreenBack : MonoBehaviour, IScreen
{
	[SerializeField]
	private CanvasGroup _target;

	public IEnumerable<ISensor> GetExitSensors()
	{
		return new ISensor[] { };
	}

	public IEnumerator GetFadeIn(TimeSpan duration)
	{
		return _target.GetCoroutineAlphaFadeIn((float)duration.TotalSeconds);
	}

	public IEnumerator GetFadeOut(TimeSpan duration)
	{
		return _target.GetCoroutineAlphaFadeOut((float)duration.TotalSeconds);
	}
}
