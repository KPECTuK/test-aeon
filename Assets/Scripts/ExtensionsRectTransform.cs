using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Engine.Runtime.Extensions
{
	// Название класса не соответствует действительности
	// Extension для определенного класса нужно держать в static ClassNameExtension, а не валить в кучу
	// статик методы вынести в соответствующие утилки 
	public static class ExtensionsRectTransform
	{
		public static T GetOrCreateComponent<T>(this GameObject go) where T : Component
		{
			return go.TryGetComponent<T>(out var component)
				? component
				: go.AddComponent<T>();
		}

		public static T GetOrCreateComponent<T>(this Component source) where T : Component
		{
			var go = source.gameObject;
			return go.TryGetComponent<T>(out var component)
				? component
				: go.AddComponent<T>();
		}

		public static Component GetOrCreateComponent(this GameObject go, Type componentType)
		{
			return go.TryGetComponent(componentType, out var component)
				? component
				: go.AddComponent(componentType);
		}

		public static Component GetOrCreateComponent(this Component source, Type componentType)
		{
			var go = source.gameObject;
			return go.TryGetComponent(componentType, out var component)
				? component
				: go.AddComponent(componentType);
		}

		/// <summary>
		/// includeHidden is disabled temporarily
		/// </summary>
		public static T SearchComponent<T>(this Component source, Predicate<T> predicate, bool includeHidden = true) where T : Component
		{
			predicate = predicate ?? DefaultPredicate;

			if(!source)
			{
				return null;
			}

			var component = source.GetComponent<T>();
			if(component != null && predicate(component))
			{
				return component;
			}

			for(var index = 0; index < source.transform.childCount; index++)
			{
				component = source.transform.GetChild(index).SearchComponent(predicate);
				if(component is null)
				{
					continue;
				}
				return component;
			}

			return null;
		}

		// public static unsafe T[] FilterComponents<T>(this Component source, Predicate<T> filter)
		// {
		// 	// TODO: tests
		//
		// 	var transform = source.transform;
		// 	var total = transform.childCount;
		// 	var buffer = stackalloc int[total];
		// 	var current = 0;
		//
		// 	filter = filter ?? DefaultPredicate;
		// 	for(var index = 0; index < total; index++)
		// 	{
		// 		var child = transform.GetChild(index).GetComponent<T>();
		// 		if(!ReferenceEquals(null, child) && filter(child))
		// 		{
		// 			buffer[current] = index;
		// 			current++;
		// 		}
		// 	}
		//
		// 	if(current == 0)
		// 	{
		// 		return Array.Empty<T>();
		// 	}
		//
		// 	var result = new T[current];
		// 	for(var index = 0; index < current; index++)
		// 	{
		// 		result[index] = transform.GetChild(buffer[index]).GetComponent<T>();
		// 	}
		//
		// 	return result;
		// }

		public static T[] CollectComponents<T>(this Component source, Predicate<T> filter) where T : Component
		{
			filter = filter ?? DefaultPredicate;
			var result = new List<T>();
			CollectComponents(source, filter, result);
			return result.ToArray();
		}

		private static void CollectComponents<T>(Component source, Predicate<T> filter, List<T> result) where T : Component
		{
			var components = source.GetComponents<T>();
			foreach(var component in components)
			{
				if(filter(component))
				{
					result.Add(component);
				}
			}

			foreach(Component child in source.transform)
			{
				CollectComponents(child, filter, result);
			}
		}

		private static bool DefaultPredicate<T>(T value)
		{
			return true;
		}

		//

		public static IEnumerator GetCoroutineAlphaFadeIn(this CanvasGroup target, float durationSec, float threshold = 1f)
		{
			return target.GetCoroutineAlphaTo(durationSec, threshold);
		}

		public static IEnumerator GetCoroutineAlphaFadeOut(this CanvasGroup target, float durationSec, float threshold = 0f)
		{
			return target.GetCoroutineAlphaTo(durationSec, threshold);
		}

		public static IEnumerator GetCoroutineAlphaFadeIn(this Image target, float durationSec, float threshold = 1f)
		{
			return target.GetCoroutineAlphaTo(durationSec, threshold);
		}

		public static IEnumerator GetCoroutineAlphaFadeOut(this Image target, float durationSec, float threshold = 0f)
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
					value += speed * UnityEngine.Time.deltaTime;
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

		public static IEnumerator GetCoroutineAlphaTo(this Image target, float durationSec, float threshold)
		{
			var initial = target.color;
			var value = initial.a;
			var speed = durationSec.GetSpeedOn(value, threshold);
			var debug = 0;
			if(durationSec > 0f)
			{
				while(value > threshold && speed < 0f || value < threshold && speed > 0f)
				{
					yield return new WaitForEndOfFrame();
					value += speed * UnityEngine.Time.deltaTime;
					target.color = new Color(initial.r, initial.g, initial.b, value);

					if(debug++ > 100)
					{
						break;
					}
				}
			}

			target.color = new Color(initial.r, initial.g, initial.b, threshold);
		}

		/// <summary>
		/// wrong probably
		/// </summary>
		public static IEnumerator GetCoroutineScaleTo(this RectTransform target, float durationSec, float threshold)
		{
			var value = 1f;
			var initial = target.GetSize();
			var speed = durationSec.GetSpeedOn(value, threshold);
			var debug = 0;
			if(durationSec > 0f)
			{
				while(value > threshold && speed < 0f || value < threshold && speed > 0f)
				{
					yield return new WaitForEndOfFrame();
					value += speed * UnityEngine.Time.deltaTime;
					target.SetSize(initial * value);

					if(debug++ > 100)
					{
						break;
					}
				}
			}

			target.SetSize(initial * threshold);
		}

		/// <summary>
		/// wrong
		/// </summary>
		public static IEnumerator GetCoroutineMoveTo(this RectTransform target, float durationSec, Vector2 to)
		{
			var debug = 0;
			var initial = target.anchoredPosition;
			var direction = initial - to;
			var value = direction.magnitude;
			var speed = value / durationSec;
			if(durationSec > 0f)
			{
				while(value > 0f && speed > 0f)
				{
					yield return new WaitForEndOfFrame();

					value -= speed * UnityEngine.Time.deltaTime;
					target.anchoredPosition = to + direction.normalized * value;

					if(debug++ > 100)
					{
						break;
					}
				}
			}

			target.anchoredPosition = to;
		}

		public static float GetSpeedOn(this float duration, float from, float to)
		{
			var sign = Mathf.Sign(to) * Mathf.Sign(from);
			return duration > 0f
				? sign * (to - from) / duration
				: 1f;
		}

		// math

		public static void RotateZ(this RectTransform target, float degrees)
		{
			target.transform.rotation *= Quaternion.Euler(0f, 0f, degrees);
		}

		public static void SetDefaultScale(this RectTransform target)
		{
			target.localScale = new Vector3(1, 1, 1);
		}

		public static void SetPivotAndAnchors(this RectTransform target, Vector2 aVec)
		{
			target.pivot = aVec;
			target.anchorMin = aVec;
			target.anchorMax = aVec;
		}

		public static Vector2 GetSize(this RectTransform target)
		{
			return target.rect.size;
		}

		public static float GetWidth(this RectTransform target)
		{
			return target.rect.width;
		}

		public static float GetHeight(this RectTransform target)
		{
			return target.rect.height;
		}

		public static void SetPositionOfPivot(this RectTransform target, Vector2 newPos)
		{
			target.localPosition = new Vector3(newPos.x, newPos.y, target.localPosition.z);
		}

		public static void SetLeftBottomPosition(this RectTransform target, Vector2 newPos)
		{
			target.localPosition = new Vector3(newPos.x + target.pivot.x * target.rect.width, newPos.y + target.pivot.y * target.rect.height, target.localPosition.z);
		}

		public static void SetLeftTopPosition(this RectTransform target, Vector2 newPos)
		{
			target.localPosition = new Vector3(newPos.x + target.pivot.x * target.rect.width, newPos.y - (1f - target.pivot.y) * target.rect.height, target.localPosition.z);
		}

		public static void SetRightBottomPosition(this RectTransform target, Vector2 newPos)
		{
			target.localPosition = new Vector3(newPos.x - (1f - target.pivot.x) * target.rect.width, newPos.y + target.pivot.y * target.rect.height, target.localPosition.z);
		}

		public static void SetRightTopPosition(this RectTransform target, Vector2 newPos)
		{
			target.localPosition = new Vector3(newPos.x - (1f - target.pivot.x) * target.rect.width, newPos.y - (1f - target.pivot.y) * target.rect.height, target.localPosition.z);
		}

		public static void SetSize(this RectTransform target, Vector2 newSize)
		{
			var oldSize = target.rect.size;
			var deltaSize = newSize - oldSize;
			var pivot = target.pivot;
			target.offsetMin -= new Vector2(deltaSize.x * pivot.x, deltaSize.y * pivot.y);
			target.offsetMax += new Vector2(deltaSize.x * (1f - target.pivot.x), deltaSize.y * (1f - pivot.y));
		}

		public static void SetWidth(this RectTransform target, float newSize)
		{
			SetSize(target, new Vector2(newSize, target.rect.size.y));
		}

		public static void SetHeight(this RectTransform target, float newSize)
		{
			SetSize(target, new Vector2(target.rect.size.x, newSize));
		}

		//

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 UpRight(this Rect source)
		{
			var halfWidth = source.width * .5f;
			var halfHeight = source.height * .5f;
			return new Vector2(
				source.center.x + halfWidth,
				source.center.y + halfHeight);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 UpLeft(this Rect source)
		{
			var halfWidth = source.width * .5f;
			var halfHeight = source.height * .5f;
			return new Vector2(
				source.center.x - halfWidth,
				source.center.y + halfHeight);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 DownRight(this Rect source)
		{
			var halfWidth = source.width * .5f;
			var halfHeight = source.height * .5f;
			return new Vector2(
				source.center.x + halfWidth,
				source.center.y - halfHeight);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 DownLeft(this Rect source)
		{
			var halfWidth = source.width * .5f;
			var halfHeight = source.height * .5f;
			return new Vector2(
				source.center.x - halfWidth,
				source.center.y - halfHeight);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 DownCenter(this Rect source)
		{
			var halfHeight = source.height * .5f;
			return new Vector2(
				source.center.x,
				source.center.y - halfHeight);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 UpCenter(this Rect source)
		{
			var halfHeight = source.height * .5f;
			return new Vector2(
				source.center.x,
				source.center.y + halfHeight);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 LeftCenter(this Rect source)
		{
			var halfWidth = source.width * .5f;
			return new Vector2(
				source.center.x - halfWidth,
				source.center.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 RightCenter(this Rect source)
		{
			var halfWidth = source.width * .5f;
			return new Vector2(
				source.center.x + halfWidth,
				source.center.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Rect ToWorld(this RectTransform source)
		{
			//! see: RectTransform target GetLocalCorners\GetWorldCorners
			var rect = source.rect;
			var x = rect.x;
			var y = rect.y;
			var xMax = rect.xMax;
			var yMax = rect.yMax;
			var connerWorldMin = new Vector3(x, y, 0.0f);
			var connerWorldMax = new Vector3(xMax, yMax, 0.0f);

			var mtx = source.transform.localToWorldMatrix;
			connerWorldMin = mtx.MultiplyPoint(connerWorldMin);
			connerWorldMax = mtx.MultiplyPoint(connerWorldMax);

			return new Rect(connerWorldMin, connerWorldMax - connerWorldMin);
		}

	// [MethodImpl(MethodImplOptions.AggressiveInlining)]
	// 	public static Vector3 ScreenToWorld<TStateCamera>(this Vector2 source, IContext context) where TStateCamera : StateCameraBase
	// 	{
	// 		return context
	// 			.Resolve<IServiceCameras>()
	// 			.GetCamera<TStateCamera>(context)
	// 			.Camera
	// 			.ScreenToWorldPoint(source);
	// 	}
	//
	// 	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	// 	public static Vector3 ScreenToWorld(this Vector2 source, IContext context)
	// 	{
	// 		return context
	// 			.Resolve<IServiceCameras>()
	// 			.GetOrtoWidestCamera(context)
	// 			.Camera
	// 			.ScreenToWorldPoint(source);
	// 	}
	// 	
	// 	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	// 	public static Rect ToScreen<TStateCamera>(this RectTransform source, IContext context) where TStateCamera : StateCameraBase
	// 	{
	// 		//! see: RectTransform target GetLocalCorners\GetWorldCorners
	// 		var rect = source.rect;
	// 		var x = rect.x;
	// 		var y = rect.y;
	// 		var xMax = rect.xMax;
	// 		var yMax = rect.yMax;
	// 		var connerWorldMin = new Vector3(x, y, 0.0f);
	// 		var connerWorldMax = new Vector3(xMax, yMax, 0.0f);
	//
	// 		var mtxLTW = source.transform.localToWorldMatrix;
	// 		var component = context.Resolve<IServiceCameras>().GetCamera<TStateCamera>(context).Camera;
	// 		var connerScreenMin = (Vector2)component.WorldToScreenPoint(mtxLTW.MultiplyPoint(connerWorldMin));
	// 		var connerScreenMax = (Vector2)component.WorldToScreenPoint(mtxLTW.MultiplyPoint(connerWorldMax));
	//
	// 		return new Rect(connerScreenMin, connerScreenMax - connerScreenMin);
	// 	}
	//
	// 	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	// 	public static Rect ToScreen(this RectTransform source, IContext context)
	// 	{
	// 		//! see: RectTransform target GetLocalCorners\GetWorldCorners
	// 		var rect = source.rect;
	// 		var x = rect.x;
	// 		var y = rect.y;
	// 		var xMax = rect.xMax;
	// 		var yMax = rect.yMax;
	// 		var connerWorldMin = new Vector3(x, y, 0.0f);
	// 		var connerWorldMax = new Vector3(xMax, yMax, 0.0f);
	//
	// 		var mtxLTW = source.transform.localToWorldMatrix;
	// 		var component = context.Resolve<IServiceCameras>().GetOrtoWidestCamera(context).Camera;
	// 		var connerScreenMin = (Vector2)component.WorldToScreenPoint(mtxLTW.MultiplyPoint(connerWorldMin));
	// 		var connerScreenMax = (Vector2)component.WorldToScreenPoint(mtxLTW.MultiplyPoint(connerWorldMax));
	//
	// 		return new Rect(connerScreenMin, connerScreenMax - connerScreenMin);
	// 	}
	//
	// 	public static Vector2 ScreenToLocal<TStateCamera>(this UIGameComponent target, IContext context, Vector2 screen) where TStateCamera : StateCameraBase
	// 	{
	// 		var component = context.Resolve<IServiceCameras>().GetCamera<TStateCamera>(context).Camera;
	// 		//var world = component.ScreenToWorldPoint(screen);
	// 		//return target.Transform.InverseTransformPoint(world);
	//
	// 		//https://forum.unity.com/threads/world-position-to-local-recttransform-position.445256/
	// 		Vector2 pos;
	// 		if(target.Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
	// 		{
	// 			RectTransformUtility.ScreenPointToLocalPointInRectangle(target.RectTransform, screen, null, out pos);
	// 		}
	// 		else
	// 		{
	// 			RectTransformUtility.ScreenPointToLocalPointInRectangle(target.RectTransform, screen, component, out pos);
	// 		}
	//
	// 		return pos;
	// 	}
	//
	// 	public static Vector3 ScreenToWorld<TStateCamera>(this UIGameComponent source, IContext context, Vector2 screen) where TStateCamera : StateCameraBase
	// 	{
	// 		var component = context.Resolve<IServiceCameras>().GetCamera<TStateCamera>(context).Camera;
	// 		//var world = component.ScreenToWorldPoint(screen);
	// 		//return target.Transform.InverseTransformPoint(world);
	//
	// 		//https://forum.unity.com/threads/world-position-to-local-recttransform-position.445256/
	// 		Vector3 pos;
	// 		if(source.Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
	// 		{
	// 			RectTransformUtility.ScreenPointToWorldPointInRectangle(source.RectTransform, screen, null, out pos);
	// 		}
	// 		else
	// 		{
	// 			RectTransformUtility.ScreenPointToWorldPointInRectangle(source.RectTransform, screen, component, out pos);
	// 		}
	//
	// 		return pos;
	// 	}
	//
	// 	// TODO: convert one rect transform local space to another rect transform local space
	//
	// 	/// <summary>
	// 	/// wrong
	// 	/// </summary>
	// 	public static Rect ToWorldSpace(this RectTransform transform, bool isClassicScreenSpace)
	// 	{
	// 		// https://answers.unity.com/questions/1013011/convert-recttransform-rect-to-screen-space.html?page=2&pageSize=5&sort=votes
	//
	// 		throw new NotImplementedException();
	//
	// 		//var position = transform.position;
	// 		//var anchoredPosition = transform.anchoredPosition;
	// 		//var size = Vector2.Scale(transform.rect.size, transform.lossyScale);
	// 		//var x = position.x + anchoredPosition.x;
	// 		//var y = isClassicScreenSpace
	// 		//	? Screen.height - position.y - anchoredPosition.y
	// 		//	: position.y + anchoredPosition.y;
	//
	// 		//return new Rect(x, y, size.x, size.y);
	// 	}
	//
	// 	/// <summary>
	// 	/// wrong
	// 	/// </summary>
	// 	public static Rect ToWorldSpace(this RectTransform transform)
	// 	{
	// 		// https://answers.unity.com/questions/1013011/convert-recttransform-rect-to-screen-space.html?page=2&pageSize=5&sort=votes
	//
	// 		throw new NotImplementedException();
	//
	// 		//var position = transform.position;
	// 		//var pivot = transform.pivot;
	// 		//var size = Vector2.Scale(transform.rect.size, transform.lossyScale);
	//
	// 		//var rect = new Rect(position.x, Screen.height - position.y, size.x, size.y);
	// 		//rect.x -= pivot.x * size.x;
	// 		//rect.y -= (1.0f - pivot.y) * size.y;
	// 		//return rect;
	// 	}
	//
	// 	/// <summary>
	// 	/// wrong
	// 	/// </summary>
	// 	/// <returns></returns>
	// 	public static (bool positionIsInRect, Vector2 positionInRect) GetScreenPositionInRect(
	// 		RectTransform rectTransform,
	// 		Vector2 inputScreenPosition)
	// 	{
	// 		// https://answers.unity.com/questions/1013011/convert-recttransform-rect-to-screen-space.html?page=2&pageSize=5&sort=votes
	//
	// 		throw new NotImplementedException();
	//
	// 		//// top left corner
	// 		//var corners = new Vector3[4];
	// 		//rectTransform.GetWorldCorners(corners);
	// 		//var topLeft = corners[1];
	//
	// 		//// rectTransform.position can be in world coordinate and in screen coordinates
	// 		//var canvas = rectTransform.GetComponentInParent<Canvas>();
	// 		//var topLeftScreen = canvas.renderMode == RenderMode.ScreenSpaceOverlay
	// 		//	? topLeft
	// 		//	: canvas.worldCamera.WorldToScreenPoint(topLeft);
	//
	// 		//// LeftRight TopBottom position in
	// 		//var positionInRect = new Vector2(
	// 		//	inputScreenPosition.x - topLeftScreen.x,
	// 		//	Screen.height - (inputScreenPosition.y + (Screen.height - topLeftScreen.y)));
	//
	// 		//// if out of rect return null
	// 		//if (positionInRect.x < 0 || positionInRect.x >= rectTransform.rect.width) return default;
	// 		//if (positionInRect.y < 0 || positionInRect.y >= rectTransform.rect.height) return default;
	//
	// 		//return (true, positionInRect);
	// 	}
	}
}
