using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Dog : MonoBehaviour
{
    #region Properties
    public int WasteCollected { get; set; }
	public int ObstaclesCollected { get; set; }
	public BoxCollider DogCollider { get; set; }
	public Queue<GameObject> FocusQueue { get; set; }
	public Rigidbody Rb { get; set; }
	#endregion

	#region Field Declarations
	public static float zPos = 2.0f;
	public Player Player;
	public Vector3 targetPos;
	public float MaxVel = 3.0f;

	private GameObject currentObject;
	private BoxCollider currentObjectCollider;
	private bool isProcessingObject;
	private readonly float avoidObstacleBuffer = 0.1f;
	
	private float toVel = 2.5f;
	private float maxForce = 20.0f;
	private float gain = 5.0f;
	private GameObject playerGameObject;

    #endregion

    #region Delegate Declarations
    public delegate void ProcessObjectSignal();
	public ProcessObjectSignal processNextObjectSignal;
    #endregion

    #region Start UP
    private void Awake()
	{
		FocusQueue = new Queue<GameObject>();

		DogCollider = GetComponent<BoxCollider>();
		DogCollider.isTrigger = true;

		Rb = GetComponent<Rigidbody>();
		Rb.useGravity = false;
		Rb.constraints = RigidbodyConstraints.FreezeRotation;

		targetPos = transform.position;
	}
	private void OnEnable()
	{
		processNextObjectSignal += ProcessObject;
	}
	private void Start()
	{
		if (Player != null)
		{
			playerGameObject = Player.gameObject;
		}
	}
	#endregion

	#region Shut Down
	private void OnDisable()
	{
		processNextObjectSignal -= ProcessObject;
	}
    #endregion

    #region Update Methods
    private void FixedUpdate()
	{
		if (Player != null)
		{
			if (Player.mouseLeftClicked)
			{
				targetPos = Player.dragPos;
			}
			else if (Player.horizontal != 0 && Player.InTension && Player.dogDir != Mathf.Sign(Player.horizontal))
			{
				targetPos = playerGameObject.transform.position;
			}
		}

		if (targetPos.x > Obstacle.BoundXMax)
		{
			targetPos.x = Obstacle.BoundXMax;
		}
		if (targetPos.x < Obstacle.BoundXMin)
		{
			targetPos.x = Obstacle.BoundXMin;
		}

		Vector3 dist = targetPos - transform.position;
		dist.y = 0;
		dist.z = 0;
		Vector3 targetVel = Vector3.ClampMagnitude(toVel * dist, MaxVel);
		Vector3 error = targetVel - Rb.velocity;
		Vector3 force = Vector3.ClampMagnitude(gain * error, maxForce);
		Rb.AddForce(force);
	}
	private void Update()
	{
		if(currentObject != null)
		{
			if (currentObject.transform.position.z + (currentObjectCollider.size.z * 0.5f) <
			transform.position.z - (DogCollider.bounds.size.z * 0.5f))
			{
				isProcessingObject = false;
				processNextObjectSignal?.Invoke();
			}
		}
	}
	#endregion

	#region Process Object Methods
	public void AddObjectToFocusQueue(GameObject objectToAdd)
	{
		FocusQueue.Enqueue(objectToAdd);
		if (!isProcessingObject)
		{
			processNextObjectSignal?.Invoke();
			isProcessingObject = true;
		}
	}
	private void ProcessObject()
	{
		isProcessingObject = true;

		// Break out of process object loop if the focus queue is empty.
		if(FocusQueue.Count == 0)
		{
			isProcessingObject = false;
			return;
		}

		currentObject = FocusQueue.Dequeue();
		currentObjectCollider = currentObject.GetComponent<BoxCollider>();

		if (currentObject.transform.CompareTag("Obstacle") ||
		currentObject.transform.CompareTag("Pole"))
		{
			StartCoroutine(AvoidObstacleRoutine());
		}
		else if (currentObject.transform.CompareTag("Waste"))
		{
			StartCoroutine(SeekWasteRoutine());
		}
	}
    #endregion

    #region Avoid Obstacle Methods
    private IEnumerator AvoidObstacleRoutine()
	{
		bool isAvoidingObject = false;
		while (currentObject.transform.CompareTag("Obstacle") || 
			currentObject.transform.CompareTag("Pole"))
		{
			float oMin = currentObjectCollider.bounds.min.x;
			float oMax = currentObjectCollider.bounds.max.x;
			float dMin = DogCollider.bounds.min.x;
			float dMax = DogCollider.bounds.max.x;

			if ((dMin > oMax) || (dMax < oMin))
			{
				isAvoidingObject = false;
				targetPos = transform.position;
			}
			else
			{
				if (!isAvoidingObject)
				{
					isAvoidingObject = true;
					bool avoidLeft;
					float width = DogCollider.bounds.size.x;

					float leftAvoidPosX = oMin - (width * 0.5f) - avoidObstacleBuffer;
					float rightAvoidPosX = oMax + (width * 0.5f) + avoidObstacleBuffer;

					if (rightAvoidPosX >= Obstacle.BoundXMax)
					{
						avoidLeft = true;
					}
					else if (leftAvoidPosX <= Obstacle.BoundXMin)
					{
						avoidLeft = false;
					}
					else
					{
						avoidLeft = (Random.value > 0.5f);
					}
					targetPos = avoidLeft ? Vector3.right * leftAvoidPosX : Vector3.right * rightAvoidPosX;
				}
			}
			yield return null;
		}
	}
    #endregion

    #region Seek Waste Methods
    private IEnumerator SeekWasteRoutine()
	{
		while (currentObject.transform.CompareTag("Waste"))
		{
			// Check how far away waste is
			float wasteZDistanceFromDog = Mathf.Abs(currentObjectCollider.bounds.min.z - DogCollider.bounds.max.z);
			float wasteZTimeToReachDog = wasteZDistanceFromDog / Mathf.Abs(currentObject.gameObject.GetComponent<Rigidbody>().velocity.z);
			float wasteXDistanceFromDog = Mathf.Abs(currentObject.transform.position.x - transform.position.x);
			float dogXTimeToReachWaste = wasteXDistanceFromDog / MaxVel;

			// If dog can get to waste in time, seek waste
			if (dogXTimeToReachWaste < wasteZTimeToReachDog)
			{
				targetPos = currentObject.transform.position;
			}
			yield return null;
		}
		
	}
    #endregion

    #region Other Methods

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.CompareTag("Waste"))
		{
			WasteCollected += 1;
			other.gameObject.SetActive(false);
			isProcessingObject = false;
			processNextObjectSignal?.Invoke();
		}
		else if (other.transform.CompareTag("Obstacle") ||
			other.transform.CompareTag("Pole"))
		{
			ObstaclesCollected += 1;
		}
	}
    #endregion
}
