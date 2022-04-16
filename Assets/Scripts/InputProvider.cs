public class InputProvider : IProvider
{
	private IInput _input;
	private readonly IInput _inputGame = new InputGame();
	private readonly IInput _inputUI = new InputUI();

	public void SetGame()
	{
		_input = _inputGame;
	}

	public void SetUI()
	{
		_input = _inputUI;
	}

	public T Get<T>() where T : class
	{
		return _input as T;
	}
}
