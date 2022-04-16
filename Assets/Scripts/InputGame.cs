using UnityEngine;

public class InputGame : IInput
{
	public bool CheckARSButtonToGame(IProvider provider)
	{
		return false;
	}

	public bool CheckARSButtonToScore(IProvider provider)
	{
		return false;
	}

	public bool IsForward()
	{
		return Input.GetKey(KeyCode.DownArrow);
	}

	public bool IsBackward()
	{
		return Input.GetKey(KeyCode.UpArrow);
	}

	public bool IsLeft()
	{
		return Input.GetKey(KeyCode.RightArrow);
	}

	public bool IsRight()
	{
		return Input.GetKey(KeyCode.LeftArrow);
	}

	public bool IsQuit()
	{
		return false;
	}

	public bool ModifyVector(ref Vector3 vector)
	{
		var accelerated = false;
		if(IsForward())
		{
			vector += Vector3.forward;
			accelerated = true;
		}
		if(IsBackward())
		{
			vector += Vector3.back;
			accelerated = true;
		}
		if(IsLeft())
		{
			vector += Vector3.left;
			accelerated = true;
		}
		if(IsRight())
		{
			vector += Vector3.right;
			accelerated = true;
		}
		vector = vector.normalized;
		return accelerated;
	}
}
