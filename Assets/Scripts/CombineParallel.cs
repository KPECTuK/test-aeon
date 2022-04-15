using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CombineParallel : IEnumerator
{
	private readonly List<IEnumerator> _group;
	private readonly bool[] _result; // debug

	public CombineParallel(IEnumerable<IEnumerator> group)
	{
		_group = new List<IEnumerator>(group);
		_result = new bool[_group.Count];
		for(var index = 0; index < _result.Length; index++)
		{
			_result[index] = true;
		}
	}

	public bool MoveNext()
	{
		var result = false;
		for(var index = 0; index < _group.Count; index++)
		{
			if(_result[index])
			{
				_result[index] = _group[index].MoveNext();
			}
			result = result || _result[index];
		}

		return result;
	}

	public void Reset() { }
	public object Current => string.Join(" ", _result.Select(_ => _ ? "+" : "-"));
}
