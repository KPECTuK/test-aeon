using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreensFsm : MonoBehaviour, IProvider
{
	public const double TRANSITION_TIME_DEFAULT_SEC = 0.3;
	public const double GAME_MAX_TIME_F = 30f;

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

	private Dictionary<Type, object> _repo;

	public T Get<T>() where T : class
	{
		T result = null;
		if(_repo.TryGetValue(typeof(T), out var @object))
		{
			if(@object is IProvider cast)
			{
				result = cast.Get<T>();
			}
			
			if(result == null)
			{
				result = @object as T;
			}
		}

		if(result == null)
		{
			throw new Exception($"type not found: {typeof(T)}");
		}

		return result;
	}

	private void Awake()
	{
		// topmost (all got initialized)

		var providerInput = new ProviderInput();
		_repo = new Dictionary<Type, object>
		{
			{ typeof(IInput), providerInput },
			{ typeof(ProviderInput), providerInput },
			//
			{ typeof(GameModeResetChar), new GameModeResetChar() },
			{ typeof(GameModeSelectFinish), new GameModeSelectFinish() },
			{ typeof(GameModeDriveChar), new GameModeDriveChar() },
			{ typeof(GameModeInertiaChar), new GameModeInertiaChar() },
			{ typeof(GameModeFallChar), new GameModeFallChar() },
			{ typeof(GameModeUI), new GameModeUI() },
			//
			{ typeof(GameController), new GameController() },
			{ typeof(GameCharacter), new ProviderCache<GameCharacter>() },
			{ typeof(GameTrack), new ProviderCache<GameTrack>() },
			{ typeof(GameCamera), new ProviderCache<GameCamera>() },
			//
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

		providerInput.SetUI();
		Get<GameController>().PushMode<GameModeUI>(this);
	}

	private void Update()
	{
		Get<GameController>().Update(this);

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
				_neighbors = _neighbors[index].GetNextSensors(this);

				break;
			}
		}
	}
}
