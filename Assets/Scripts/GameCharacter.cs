using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacter : MonoBehaviour
{
	public const float A_FRICTION_DELTA_F = 10f;
	public const float A_FALL_F = 40f;
	public const float A_FALL_FRICTION_F = 3f;
	public const float A_FRICTION_F = 12f;
	public const float A_ANGULAR_F = .6f;

	[NonSerialized]
	public Vector3 Speed;
	[NonSerialized]
	public Vector3 SpeedAngularAxis;
	[NonSerialized]
	public float SpeedAngular;
	[NonSerialized]
	public Vector3 InitialPos;
	[NonSerialized]
	public readonly RaycastHit[] SurfaceHits = new RaycastHit[1];

	private void Awake()
	{
		InitialPos = transform.position;
	}
}

public class GameModeResetPosition : IGameMode
{
	public bool Update(IProvider provider)
	{
		var @char = provider.Get<GameCharacter>();
		@char.transform.position = @char.InitialPos;
		@char.Speed = Vector3.zero;
		provider.Get<InputProvider>().SetGame();
		return false;
	}
}

// TODO: prefer to calculate in vector form, need tools

public class GameModeMain : IGameMode
{
	public bool Update(IProvider provider)
	{
		var @char = provider.Get<GameCharacter>();
		var input = provider.Get<IInput>();

		// read input
		var acceleration = Vector3.zero;
		var accelerated = input.ModifyVector(ref acceleration);

		// or acceleration and friction, both the same time (accel greater then friction)

		if(accelerated)
		{
			// or user
			@char.Speed += GameCharacter.A_FRICTION_DELTA_F * Time.deltaTime * acceleration;
		}
		else
		{
			// or friction
			var frictionAcceleration = GameCharacter.A_FRICTION_F * Time.deltaTime;
			@char.Speed -= frictionAcceleration * @char.Speed.normalized;
			if(@char.Speed.magnitude < frictionAcceleration + frictionAcceleration)
			{
				@char.Speed = Vector3.zero;
			}
		}

		// surf transition
		var delta = @char.Speed * Time.deltaTime;

		// check
		@char.transform.position += delta;

		// surf rotation
		@char.SpeedAngularAxis = Vector3.Cross(Vector3.up, delta.normalized).normalized;
		@char.SpeedAngular = delta.magnitude * Mathf.Rad2Deg * 2f;
		@char.transform.GetChild(0).Rotate(@char.SpeedAngularAxis, @char.SpeedAngular, Space.World);

		// debug
		var pos = @char.transform.position;
		Debug.DrawLine(pos, pos + delta.normalized, Color.blue);
		Debug.DrawLine(pos, pos + @char.SpeedAngularAxis.normalized, Color.magenta);

		// sense
		var hits = Physics.RaycastNonAlloc(new Ray(pos + Vector3.up, Vector3.down), @char.SurfaceHits, 2f);
		return hits > 0 && provider.Get<GameTrack>().IsTrack(@char.SurfaceHits[0].collider);
	}
}

public class GameModeResetControl : IGameMode
{
	public bool Update(IProvider provider)
	{
		provider.Get<InputProvider>().SetUI();

		var game = provider.Get<GameController>();

		if(provider.Get<GameTrack>().IsFinish(provider.Get<GameCharacter>().SurfaceHits[0].collider))
		{
			game.SetGameStopWinning(provider);
			game.PushMode<GameModeInertia>(provider);
		}
		else
		{
			game.SetGameStopLoosing();
			game.PushMode<GameModeFalling>(provider);
		}

		return false;
	}
}

public class GameModeInertia : IGameMode
{
	public bool Update(IProvider provider)
	{
		var @char = provider.Get<GameCharacter>();

		// or friction
		var frictionAcceleration = GameCharacter.A_FRICTION_F * Time.deltaTime;
		@char.Speed -= frictionAcceleration * @char.Speed.normalized;
		if(@char.Speed.magnitude < frictionAcceleration + frictionAcceleration)
		{
			@char.Speed = Vector3.zero;
		}

		// surf transition
		var delta = @char.Speed * Time.deltaTime;

		// check
		@char.transform.position += delta;

		// surf rotation
		@char.SpeedAngularAxis = Vector3.Cross(Vector3.up, delta.normalized).normalized;
		@char.SpeedAngular = delta.magnitude * Mathf.Rad2Deg * 2f;
		@char.transform.GetChild(0).Rotate(@char.SpeedAngularAxis, @char.SpeedAngular, Space.World);

		// sense
		return @char.Speed.magnitude < frictionAcceleration;
	}
}

public class GameModeFalling : IGameMode
{
	public bool Update(IProvider provider)
	{
		// TODO: prefer to calculate in vector form, need tools
		var @char = provider.Get<GameCharacter>();

		// suf transition inertia
		var speedSurf = new Vector3(@char.Speed.x, 0f, @char.Speed.z);
		var frictionAcceleration = GameCharacter.A_FALL_FRICTION_F * Time.deltaTime;
		speedSurf -= frictionAcceleration * speedSurf.normalized;
		if(speedSurf.magnitude < frictionAcceleration + frictionAcceleration)
		{
			speedSurf = Vector3.zero;
		}

		// fall transition
		var speedFall = new Vector3(0f, @char.Speed.y, 0f);
		speedFall += GameCharacter.A_FALL_F * Time.deltaTime * Vector3.down;

		// check
		@char.Speed = speedSurf + speedFall;
		var delta = @char.Speed * Time.deltaTime;
		@char.transform.position += delta;

		// rotation inertia
		@char.SpeedAngular -= GameCharacter.A_ANGULAR_F;
		@char.SpeedAngular = @char.SpeedAngular < 0f ? 0f : @char.SpeedAngular;
		@char.transform.GetChild(0).Rotate(@char.SpeedAngularAxis, @char.SpeedAngular, Space.World);

		// sense
		return @char.transform.position.y > -3f;
	}
}

public class GameModeUI : IGameMode
{
	public bool Update(IProvider provider)
	{
		// can be animated during ui in background
		// never completes
		return !provider.Get<GameController>().IsGame();
	}
}

public class DescGame
{
	public DateTime Start;
	public DateTime Stop;
	public bool Result;
	public bool Complete;

	public TimeSpan Duration => Stop - Start;
}

public class GameController : IEnumerable<DescGame>
{
	private readonly Stack<IGameMode> _modes = new Stack<IGameMode>();
	private IGameMode _currentMode;
	private readonly List<DescGame> _results = new List<DescGame>();
	private DescGame _currentDesc;

	public void PushMode<T>(IProvider provider) where T : class, IGameMode
	{
		var mode = provider.Get<T>();
		if(!ReferenceEquals(null, mode))
		{
			_modes.Push(mode);
		}
	}

	public void DropAllModes()
	{
		_modes.Clear();
	}

	public void SetGameStart()
	{
		_currentMode = null;
		_currentDesc = new DescGame
		{
			Start = DateTime.UtcNow,
		};
	}

	public void SetGameStopWinning(IProvider provider)
	{
		_currentDesc.Stop = DateTime.UtcNow;
		_currentDesc.Complete = true;
		_currentDesc.Result = true;
		_results.Add(_currentDesc);

		provider.Get<ScreenScoreWin>().SetText(_currentDesc.Duration);
	}

	public void SetGameStopLoosing()
	{
		_currentDesc.Stop = DateTime.UtcNow;
		_currentDesc.Complete = true;
		_currentDesc.Result = false;
		_results.Add(_currentDesc);
	}

	public bool IsCurrentWin()
	{
		return (_currentDesc?.Complete ?? false) && _currentDesc.Result;
	}

	public bool IsCurrentLose()
	{
		return (_currentDesc?.Complete ?? false) && !_currentDesc.Result;
	}

	public bool IsGame()
	{
		return _currentDesc == null || !_currentDesc.Complete;
	}

	public void Update(IProvider provider)
	{
		// able to push mode from mode
		if(_currentMode == null)
		{
			if(_modes.Count > 0)
			{
				_currentMode = _modes.Pop();
			}
		}

		if(_currentMode != null)
		{
			if(!_currentMode.Update(provider))
			{
				_currentMode = null;
			}
		}
	}

	public IEnumerator<DescGame> GetEnumerator()
	{
		return _results.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
