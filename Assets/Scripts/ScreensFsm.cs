using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreensFsm : MonoBehaviour, IResolver, IInputState
{
	public const double TRANSITION_TIME_DEFAULT_SEC = 0.3;

	[SerializeField]
	private ScreenMain _scrMain;
	[SerializeField]
	private ScreenScorePrevious _scrScorePrevious;
	[SerializeField]
	private ScreenScoreWin _scrScoreWin;
	[SerializeField]
	private ScreenScoreLose _scrScoreLose;
	[SerializeField]
	private ScreenBack _scrBack;

	private IEnumerator _transition;
	private ISensor[] _neighbors;

	private bool _pressEsc;

	private Dictionary<Type, object> _repo;

	public T Get<T>() where T : class
	{
		return _repo.TryGetValue(typeof(T), out var result)
			? result as T
			: throw new Exception($"type not found: {typeof(T)}");
	}

	private void Awake()
	{
		_repo = new Dictionary<Type, object>
		{
			{ typeof(IInputState), this },
			{ typeof(ScreenMain), _scrMain },
			{ typeof(ScreenScorePrevious), _scrScorePrevious },
			{ typeof(ScreenScoreWin), _scrScoreWin },
			{ typeof(ScreenScoreLose), _scrScoreLose },
			{ typeof(ScreenBack), _scrBack },
		};

		// app launch

		_transition = new CombineParallel(new[]
		{
			Get<ScreenMain>().GetFadeIn(TimeSpan.FromSeconds(TRANSITION_TIME_DEFAULT_SEC)),
			Get<ScreenBack>().GetFadeIn(TimeSpan.FromSeconds(TRANSITION_TIME_DEFAULT_SEC)),
		});

		_neighbors = new ISensor[]
		{
			new SensorMainToGame(),
			new SensorMainToScore(),
		};
	}

	private void Update()
	{
		if(_transition?.MoveNext() ?? false)
		{
			return;
		}

		_transition = null;

		for(var index = 0; index < _neighbors.Length; index++)
		{
			if(_neighbors[index].Check(this))
			{
				_transition = _neighbors[index].GetNextTransition(this);
				_neighbors = _neighbors[index].GetNextSensors();

				break;
			}
		}
	}

	public bool CheckARSButtonToGame()
	{
		return _scrMain.GetARSButtonValGame();
	}

	public bool CheckARSButtonToScore()
	{
		return _scrMain.GetARSButtonValScore();
	}

	public bool IsEscPress()
	{
		// TODO: scan input depending on state -> separate input

		_pressEsc = Input.GetKey(KeyCode.Escape);

		var result = _pressEsc;
		_pressEsc = false;
		return result;
	}
}
