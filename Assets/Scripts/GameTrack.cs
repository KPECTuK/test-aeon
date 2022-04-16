using UnityEngine;

public class GameTrack : MonoBehaviour
{
	[SerializeField]
	private Collider _colliderTrack;
	[SerializeField]
	private Collider _colliderFinish;

	public bool IsTrack(Collider source)
	{
		return ReferenceEquals(source, _colliderTrack);
	}

	public bool IsFinish(Collider source)
	{
		return ReferenceEquals(source, _colliderFinish);
	}
}
