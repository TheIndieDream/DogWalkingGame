using UnityEngine;

public enum ObstacleType { Obstacle, Waste, Pole }

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Obstacle : MonoBehaviour
{
	public static float BoundXMax = 5.0f;
	public static float BoundXMin = -5.0f;

	public BoxCollider ObstacleCollider;
	public Rigidbody ObstacleRb;

	private Transform thisTransform;

	private void Awake()
	{
		thisTransform = transform;

		ObstacleCollider = GetComponent<BoxCollider>();
		ObstacleCollider.isTrigger = true;

		ObstacleRb = GetComponent<Rigidbody>();
		ObstacleRb.useGravity = false;
	}

	private void Update()
	{
		if(thisTransform.position.z < -2.0f)
		{
			ObstacleRb.velocity = Vector3.zero;
			gameObject.SetActive(false);
		}
	}
}
