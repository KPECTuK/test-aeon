using System;
using System.Collections;

// ! all intended to be stateless

// ! link: Main <> Game

public class SensorMainToGame : ISensor
{
	public bool Check(IProvider provider)
	{
		return provider.Get<IInput>().CheckARSButtonToGame(provider);
	}

	public IEnumerator GetNextTransition(IProvider provider)
	{
		return new CombineSeq(new[]
		{
			new CombineParallel(new[]
			{
				provider.Get<ScreenMain>().GetFadeOut(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
				provider.Get<ScreenBack>().GetFadeOut(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
			}),
			Get(provider)
		});
	}

	private IEnumerator Get(IProvider provider)
	{
		yield return null;
		// TODO: ? combine mode and input 
		var controller = provider.Get<GameController>();
		controller.DropAllModes();
		controller.PushMode<GameModeUI>(provider);
		controller.PushMode<GameModeResetControl>(provider);
		controller.PushMode<GameModeMain>(provider);
		controller.PushMode<GameModeResetPosition>(provider);
		controller.SetGameStart();
	}

	public ISensor[] GetNextSensors(IProvider provider)
	{
		return new ISensor[]
		{
			new SensorGameToLose(),
			new SensorGameToWin(),
		};
	}
}

// win branch

public class SensorGameToWin : ISensor
{
	// private readonly DateTime _timeTemp = DateTime.UtcNow;

	public bool Check(IProvider provider)
	{
		// test (random)
		// return
		// 	DateTime.UtcNow - _timeTemp > TimeSpan.FromSeconds(ScreensFsm.GAME_MAX_TIME_F) &&
		// 	UnityEngine.Random.value > .7f;

		return provider.Get<GameController>().IsCurrentWin();
	}

	public IEnumerator GetNextTransition(IProvider provider)
	{
		return new CombineParallel(new[]
		{
			provider.Get<ScreenScoreWin>().GetFadeIn(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
			provider.Get<ScreenBack>().GetFadeIn(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
		});
	}

	public ISensor[] GetNextSensors(IProvider provider)
	{
		return new ISensor[]
		{
			new SensorWinToMain(),
		};
	}
}

public class SensorWinToMain : ISensor
{
	public bool Check(IProvider provider)
	{
		return provider.Get<IInput>().IsQuit();
	}

	public IEnumerator GetNextTransition(IProvider provider)
	{
		return new CombineSeq(new[]
		{
			provider.Get<ScreenScoreWin>().GetFadeOut(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
			provider.Get<ScreenMain>().GetFadeIn(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
		});
	}

	public ISensor[] GetNextSensors(IProvider provider)
	{
		return new ISensor[]
		{
			new SensorMainToGame(),
			new SensorMainToScore(),
		};
	}
}

// lose branch

public class SensorGameToLose : ISensor
{
	// private readonly DateTime _timeTemp = DateTime.UtcNow;

	public bool Check(IProvider provider)
	{
		// test (random)
		// return
		// 	DateTime.UtcNow - _timeTemp > TimeSpan.FromSeconds(ScreensFsm.GAME_MAX_TIME_F) &&
		// 	UnityEngine.Random.value > .7f;

		return provider.Get<GameController>().IsCurrentLose();
	}

	public IEnumerator GetNextTransition(IProvider provider)
	{
		return new CombineParallel(new[]
		{
			provider.Get<ScreenScoreLose>().GetFadeIn(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
			provider.Get<ScreenBack>().GetFadeIn(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
		});
	}

	public ISensor[] GetNextSensors(IProvider provider)
	{
		return new ISensor[]
		{
			new SensorLoseToMain(),
		};
	}
}

public class SensorLoseToMain : ISensor
{
	public bool Check(IProvider provider)
	{
		return provider.Get<IInput>().IsQuit();
	}

	public IEnumerator GetNextTransition(IProvider provider)
	{
		return new CombineSeq(new[]
		{
			provider.Get<ScreenScoreLose>().GetFadeOut(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
			provider.Get<ScreenMain>().GetFadeIn(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
		});
	}

	public ISensor[] GetNextSensors(IProvider provider)
	{
		return new ISensor[]
		{
			new SensorMainToGame(),
			new SensorMainToScore(),
		};
	}
}

// ! link: Main <> ScorePrevious

public class SensorMainToScore : ISensor
{
	public bool Check(IProvider provider)
	{
		return provider.Get<IInput>().CheckARSButtonToScore(provider);
	}

	public IEnumerator GetNextTransition(IProvider provider)
	{
		return new CombineSeq(new[]
		{
			provider.Get<ScreenMain>().GetFadeOut(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
			SetScores(provider),
			provider.Get<ScreenScorePrevious>().GetFadeIn(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
		});
	}

	private IEnumerator SetScores(IProvider provider)
	{
		yield return null;
		provider.Get<ScreenScorePrevious>().SetResults(provider.Get<GameController>());
	}

	public ISensor[] GetNextSensors(IProvider provider)
	{
		return new ISensor[]
		{
			new SensorScorePToMain()
		};
	}
}

public class SensorScorePToMain : ISensor
{
	public bool Check(IProvider provider)
	{
		return provider.Get<IInput>().IsQuit();
	}

	public IEnumerator GetNextTransition(IProvider provider)
	{
		return new CombineSeq(new[]
		{
			provider.Get<ScreenScorePrevious>().GetFadeOut(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
			provider.Get<ScreenMain>().GetFadeIn(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
		});
	}

	public ISensor[] GetNextSensors(IProvider provider)
	{
		return new ISensor[]
		{
			new SensorMainToGame(),
			new SensorMainToScore(),
		};
	}
}
