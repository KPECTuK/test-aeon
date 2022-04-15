using System;
using System.Collections;

// ! link: Main <> Game

public class SensorMainToGame : ISensor
{
	public bool Check(IResolver resolver)
	{
		return resolver.Get<IInputState>().CheckARSButtonToGame();
	}

	public IEnumerator GetNextTransition(IResolver resolver)
	{
		return new CombineParallel(new[]
		{
			resolver.Get<ScreenMain>().GetFadeOut(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
			resolver.Get<ScreenBack>().GetFadeOut(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
		});
	}

	public ISensor[] GetNextSensors()
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
	private readonly DateTime _timeTemp = DateTime.UtcNow;

	public bool Check(IResolver resolver)
	{
		// test (random)
		return DateTime.UtcNow - _timeTemp > TimeSpan.FromSeconds(10.0) && UnityEngine.Random.value > .7f;
	}

	public IEnumerator GetNextTransition(IResolver resolver)
	{
		return new CombineParallel(new[]
		{
			resolver.Get<ScreenScoreWin>().GetFadeIn(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
			resolver.Get<ScreenBack>().GetFadeIn(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
		});
	}

	public ISensor[] GetNextSensors()
	{
		return new ISensor[]
		{
			new SensorWinToMain(),
		};
	}
}

public class SensorWinToMain : ISensor
{
	public bool Check(IResolver resolver)
	{
		return resolver.Get<IInputState>().IsEscPress();
	}

	public IEnumerator GetNextTransition(IResolver resolver)
	{
		return new CombineSeq(new[]
		{
			resolver.Get<ScreenScoreWin>().GetFadeOut(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
			resolver.Get<ScreenMain>().GetFadeIn(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
		});
	}

	public ISensor[] GetNextSensors()
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
	private readonly DateTime _timeTemp = DateTime.UtcNow;

	public bool Check(IResolver resolver)
	{
		// test (random)
		return DateTime.UtcNow - _timeTemp > TimeSpan.FromSeconds(10.0) && UnityEngine.Random.value > .7f;
	}

	public IEnumerator GetNextTransition(IResolver resolver)
	{
		return new CombineParallel(new[]
		{
			resolver.Get<ScreenScoreLose>().GetFadeIn(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
			resolver.Get<ScreenBack>().GetFadeIn(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
		});
	}

	public ISensor[] GetNextSensors()
	{
		return new ISensor[]
		{
			new SensorLoseToMain(),
		};
	}
}

public class SensorLoseToMain : ISensor
{
	public bool Check(IResolver resolver)
	{
		return resolver.Get<IInputState>().IsEscPress();
	}

	public IEnumerator GetNextTransition(IResolver resolver)
	{
		return new CombineSeq(new[]
		{
			resolver.Get<ScreenScoreLose>().GetFadeOut(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
			resolver.Get<ScreenMain>().GetFadeIn(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
		});
	}

	public ISensor[] GetNextSensors()
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
	public bool Check(IResolver resolver)
	{
		return resolver.Get<IInputState>().CheckARSButtonToScore();
	}

	public IEnumerator GetNextTransition(IResolver resolver)
	{
		return new CombineSeq(new[]
		{
			resolver.Get<ScreenMain>().GetFadeOut(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
			resolver.Get<ScreenScorePrevious>().GetFadeIn(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
		});
	}

	public ISensor[] GetNextSensors()
	{
		return new ISensor[] { new SensorScorePToMain() };
	}
}

public class SensorScorePToMain : ISensor
{
	public bool Check(IResolver resolver)
	{
		return resolver.Get<IInputState>().IsEscPress();
	}

	public IEnumerator GetNextTransition(IResolver resolver)
	{
		return new CombineSeq(new[]
		{
			resolver.Get<ScreenScorePrevious>().GetFadeOut(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
			resolver.Get<ScreenMain>().GetFadeIn(TimeSpan.FromSeconds(ScreensFsm.TRANSITION_TIME_DEFAULT_SEC)),
		});
	}

	public ISensor[] GetNextSensors()
	{
		return new ISensor[]
		{
			new SensorMainToGame(),
			new SensorMainToScore(),
		};
	}
}
