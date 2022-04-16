using System;
using System.Collections;
using UnityEngine;

public interface IScreen
{
	IEnumerator GetFadeIn(TimeSpan duration);
	IEnumerator GetFadeOut(TimeSpan duration);
}

/// <summary> aka State </summary>
public interface ISensor
{
	bool Check(IProvider provider);
	IEnumerator GetNextTransition(IProvider provider);
	ISensor[] GetNextSensors(IProvider provider);
}

public interface IInput
{
	bool CheckARSButtonToGame(IProvider provider);
	bool CheckARSButtonToScore(IProvider provider);
	bool IsForward();
	bool IsBackward();
	bool IsLeft();
	bool IsRight();
	bool IsQuit();
	bool ModifyVector(ref Vector3 vector);
}

public interface IGameMode
{
	bool Update(IProvider provider);
}

public interface IProvider
{
	T Get<T>() where T : class;
}

public class ProviderCache<TComponent> : IProvider where TComponent : UnityEngine.Object
{
	private TComponent _cache;

	public T Get<T>() where T : class
	{
		if(ReferenceEquals(_cache, null))
		{
			_cache = UnityEngine.Object.FindObjectOfType<TComponent>();
		}

		return _cache as T;
	}
}
