using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenMain : MonoBehaviour, IScreen
{
	[SerializeField]
	private CanvasGroup _target;
	[SerializeField]
	private Button _buttonGame;
	[SerializeField]
	private Button _buttonScore;

	private bool _buttonStateGame;
	private bool _buttonStateScore;

	private void Awake()
	{
		_buttonGame.onClick.AddListener(() => { _buttonStateGame = true; });
		_buttonScore.onClick.AddListener(() => { _buttonStateScore = true; });
	}

	public bool GetARSButtonValGame()
	{
		var result = _buttonStateGame;
		_buttonStateGame = false;
		return result;
	}

	public bool GetARSButtonValScore()
	{
		var result = _buttonStateScore;
		_buttonStateScore = false;
		return result;
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
