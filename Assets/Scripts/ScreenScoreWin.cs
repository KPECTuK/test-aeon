using System;
using System.Collections;
using Modules.Engine.Runtime.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class ScreenScoreWin : MonoBehaviour, IScreen
{
	[SerializeField]
	private CanvasGroup _target;
	[SerializeField]
	private Text _text;

	public void SetText(TimeSpan duration)
	{
		_text.text = $"Вы потратили {(int)duration.TotalSeconds} сек";
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
