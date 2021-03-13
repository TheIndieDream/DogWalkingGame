using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Player : MonoBehaviour
{
    #region Properties
    public bool IsDebuffed { get; set; }
    public Health Health { get; set; }
    public bool InTension { get; set; }
    #endregion

    #region Field Declarations
    public IPlayerInput PlayerInput;
    public Dog Dog;
    public UIManager UiManager;
    public Rigidbody Rb;
    public Vector3 targetPos;
    public float MaxVel = 3.0f;
    public float LeashLength = 1.0f;
    public bool mouseLeftClicked = false;
    public bool isDragging;
    public Vector3 dragPos;
    public Vector3 dragRate = new Vector3(0.5f, 0.0f, 0.0f);
    public float dogDir = 0;
    public float horizontal;

    private CapsuleCollider playerCollider;
    private GameObject dogGameObject;
    private float toVel = 2.5f;
    private float maxForce = 20.0f;
    private float gain = 5.0f;
    #endregion

    #region Delegate Declarations
    public delegate void HealthEmpty();
    public static HealthEmpty HealthIsEmpty;
    public delegate void HealthChange();
    public static HealthChange HealthIsChanged;
    #endregion

    #region Startup
    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        Rb.useGravity = false;
        Rb.constraints = RigidbodyConstraints.FreezeRotation;

        playerCollider = GetComponent<CapsuleCollider>();
        playerCollider.isTrigger = true;

        PlayerInput = new PlayerInput();

        Health = new Health(3);

        targetPos = transform.position;
    }
    private void Start()
    {
        if(Dog != null)
        {
            dogGameObject = Dog.gameObject;
            dragPos = dogGameObject.transform.position;
        }
    }
    #endregion

    #region Update Methods
    private void FixedUpdate()
    {
        if(Dog != null)
        {
            if (horizontal == 0 && Dog.targetPos != dogGameObject.transform.position &&
            InTension && -dogDir != Mathf.Sign(Dog.targetPos.x - dogGameObject.transform.position.x) &&
            !isDragging)
            {
                targetPos = dogGameObject.transform.position;
            }
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
        horizontal = PlayerInput.Horizontal;
        Vector3 mousePosition = PlayerInput.MousePosition;
        mouseLeftClicked = PlayerInput.MouseLeftClicked;

        if(Dog != null)
        {
            if (!isDragging && mouseLeftClicked)
            {
                StartCoroutine(ClickAndDragRoutine(mousePosition));
            }
        }

        ApplyDebuff();
        MoveX();
        UpdateDogDirection();
        CheckTension();
    }
    private void UpdateDogDirection()
    {
        if(Dog != null)
        {
            dogDir = Mathf.Sign(dogGameObject.transform.position.x - transform.position.x);
        }
    }

    private void ApplyDebuff()
    {
        if (IsDebuffed)
        {
            horizontal *= -1;
        }
    }

    private void CheckTension()
    {
        if (Dog != null)
        {
            if (mouseLeftClicked)
            {
                InTension = true;
            }
            else
            {
                if (Mathf.Abs(dogGameObject.transform.position.x - transform.position.x) > LeashLength)
                {
                    InTension = true;
                }
                else
                {
                    InTension = false;
                }
            }
        }
    }

    private void MoveX()
    {
        if (horizontal > 0)
        {
            targetPos = new Vector3(Obstacle.BoundXMax, 0.0f, 0.0f);
        }
        else if (horizontal < 0)
        {
            targetPos = new Vector3(Obstacle.BoundXMin, 0.0f, 0.0f);
        }
        else
        {
            targetPos = transform.position;
        }
    }
    #endregion

    #region Other Methods
    private IEnumerator ClickAndDragRoutine(Vector3 initMousePosition)
    {
        isDragging = true;
        Vector3 currentMousePosition;
        bool firstTug = true;
        while (mouseLeftClicked)
        {
            currentMousePosition = PlayerInput.MousePosition;
            float dragDir = Mathf.Sign(currentMousePosition.x - initMousePosition.x);
            if(dragDir != dogDir && currentMousePosition.x != initMousePosition.x) 
            {
                if (firstTug)
                {
                    firstTug = false;
                    
                    dragPos = transform.position;
                }
                if(Mathf.Abs(dragPos.x + dragRate.x * dragDir) < LeashLength)
                {
                    dragPos += dragRate * dragDir;
                }
                else
                {
                    dragPos = new Vector3(LeashLength * dragDir, 0.0f, 0.0f);
                }
            }
            yield return null;
        }
        isDragging = false;
    }
    private IEnumerator DebuffTimerRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        IsDebuffed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Waste"))
        {
            IsDebuffed = true;
            StartCoroutine(DebuffTimerRoutine());
        }
        else if (other.transform.CompareTag("Obstacle"))
        {
            Health.Current -= 1;
            HealthIsChanged?.Invoke();
        }
        else if (other.transform.CompareTag("Pole"))
        {
            Health.Current = 0;
        }

        if(Health.Current == 0)
        {
            HealthIsEmpty?.Invoke();
        }
    }
    #endregion
}
