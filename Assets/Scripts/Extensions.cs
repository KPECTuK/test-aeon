using System.Collections;
using UnityEngine;

public static class Extensions
{
	public static IEnumerator GetCoroutineAlphaFadeIn(this CanvasGroup target, float durationSec, float threshold = 1f)
	{
		return target.GetCoroutineAlphaTo(durationSec, threshold);
	}

	public static IEnumerator GetCoroutineAlphaFadeOut(this CanvasGroup target, float durationSec, float threshold = 0f)
	{
		return target.GetCoroutineAlphaTo(durationSec, threshold);
	}

	public static IEnumerator GetCoroutineAlphaTo(this CanvasGroup target, float durationSec, float threshold)
	{
		var initial = target.alpha;
		var value = initial;
		var speed = durationSec.GetSpeedOn(value, threshold);
		var debug = 0;

		// disable interaction on transition
		target.interactable = false;

		if(durationSec > 0f)
		{
			while(value > threshold && speed < 0f || value < threshold && speed > 0f)
			{
				yield return new WaitForEndOfFrame();
				value += speed * Time.deltaTime;
				target.alpha = value;

				if(debug++ > 100)
				{
					break;
				}
			}
		}

		// enable interaction if been turned visible
		target.interactable = speed > 0f;
		target.alpha = threshold;
	}

	public static float GetSpeedOn(this float duration, float from, float to)
	{
		var sign = Mathf.Sign(to) * Mathf.Sign(from);
		return duration > 0f
			? sign * (to - from) / duration
			: 1f;
	}
}
