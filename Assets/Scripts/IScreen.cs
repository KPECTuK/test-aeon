using System;
using System.Collections;
using System.Collections.Generic;

public interface IScreen
{
	IEnumerator GetFadeIn(TimeSpan duration);
	IEnumerator GetFadeOut(TimeSpan duration);
}

public interface IResolver
{
	T Get<T>() where T : class;
}

public interface ISensor
{
	bool Check(IResolver resolver);
	IEnumerator GetNextTransition(IResolver resolver);
	ISensor[] GetNextSensors();
}

public interface IState
{
	IEnumerable<ISensor> GetSensors();
	IEnumerator GetFadeTransition(IResolver resolver);
}

public interface IInputState
{
	bool CheckARSButtonToGame();
	bool CheckARSButtonToScore();
	bool IsEscPress();
}
