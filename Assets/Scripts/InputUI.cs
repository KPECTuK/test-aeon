using UnityEngine;

public class InputUI : IInput
{
	public bool CheckARSButtonToGame(IProvider provider)
	{
		return provider.Get<ScreenMain>().GetARSButtonValGame();
	}

	public bool CheckARSButtonToScore(IProvider provider)
	{
		return provider.Get<ScreenMain>().GetARSButtonValScore();
	}

	public bool IsForward()
	{
		return false;
	}

	public bool IsBackward()
	{
		return false;
	}

	public bool IsLeft()
	{
		return false;
	}

	public bool IsRight()
	{
		return false;
	}

	public bool IsQuit()
	{
		return Input.GetKey(KeyCode.Escape);
	}

	public bool ModifyVector(ref Vector3 vector)
	{
		return false;
	}
}
