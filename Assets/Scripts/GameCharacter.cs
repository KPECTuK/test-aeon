using System;
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
