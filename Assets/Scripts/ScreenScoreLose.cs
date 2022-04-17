using System;
using System.Collections;
using UnityEngine;

public class ScreenScoreLose : MonoBehaviour, IScreen
{
	[SerializeField]
	private CanvasGroup _target;

	public IEnumerator GetFadeIn(TimeSpan duration)
	{
		return _target.GetCoroutineAlphaFadeIn((float)duration.TotalSeconds);
	}

	public IEnumerator GetFadeOut(TimeSpan duration)
	{
		return _target.GetCoroutineAlphaFadeOut((float)duration.TotalSeconds);
	}
}
