using System.Collections;
using System.Collections.Generic;

public class CombineSeq : IEnumerator
{
	private int _index = -1;
	private IEnumerator _current;
	private readonly List<IEnumerator> _group;

	public CombineSeq(IEnumerable<IEnumerator> group)
	{
		_group = new List<IEnumerator>(group);
	}

	public bool MoveNext()
	{
		var result = false;
		while(!result)
		{
			result = _current?.MoveNext() ?? false;
			if(!result)
			{
				if(++_index == _group.Count)
				{
					break;
				}
				_current = _group[_index];
			}
		}
		return result;
	}

	public void Reset() { }
	public object Current => _current?.Current;
}
