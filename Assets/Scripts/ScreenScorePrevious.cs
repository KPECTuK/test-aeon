using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modules.Engine.Runtime.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class ScreenScorePrevious : MonoBehaviour, IScreen
{
	[SerializeField]
	private CanvasGroup _target;
	[SerializeField]
	private Text[] _rows;

	public void SetResults(IEnumerable<DescGame> results)
	{
		using(var enResults = results.Reverse().GetEnumerator())
		{
			using(var enRows = _rows.Cast<Text>().GetEnumerator())
			{
				while(enRows.MoveNext())
				{
					enRows.Current.text = enResults.MoveNext() 
						? new StringBuilder("Игра ")
							.Append(enResults.Current.Result ? "<color=lime>выиграна</color>" : "<color=orange>проиграна</color>")
							.Append(", потрачено: ")
							.Append(enResults.Current.Duration.TotalSeconds.ToString("##.##"))
							.Append(" сек")
							.ToString()
						
						: "";
				}
			}
		}
	}

	public IEnumerator GetFadeIn(TimeSpan duration)
	{
		return _target.GetCoroutineAlphaFadeIn((float)duration.TotalSeconds);
	}

	public IEnumerator GetFadeOut(TimeSpan duration)
	{
		return _target.GetCoroutineAlphaFadeOut((float)duration.TotalSeconds);
	}
}
