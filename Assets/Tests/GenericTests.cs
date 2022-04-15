using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class GenericTests
{
	[Test]
	public void TestCombineSeq()
	{
		var seq = new CombineSeq(new[] { Get1(), Get2(), Get3() });
		while(seq.MoveNext())
		{
			Debug.Log(seq.Current);
		}
	}

	[Test]
	public void TestCombineParallel()
	{
		var seq = new CombineParallel(new[] { Get1(), Get2(), Get3() });
		while(seq.MoveNext())
		{
			Debug.Log(seq.Current);
		}
	}

	private IEnumerator Get1()
	{
		yield return 1;
		yield return 2;
		yield return 3;
		yield return 4;
	}

	private IEnumerator Get2()
	{
		yield return 5;
		yield return 6;
		yield return 7;
		yield return 8;
		yield return 9;
		yield return 10;
	}

	private IEnumerator Get3()
	{
		yield return 11;
		yield return 12;
		yield return 13;
		yield return 14;
		yield return 15;
	}
}
