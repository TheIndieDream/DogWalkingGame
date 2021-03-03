using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Player : MonoBehaviour
{
    public GameObject Dog { get; set; }
    public Health Health { get; set; }
    public IPlayerInput PlayerInput { get; set; }
    public bool IsDebuffed { get; set; }
    public float MoveSpeed { get; set; }
    private Rigidbody Rb { get; set; }

    private CapsuleCollider PlayerCollider { get; set; }

    public delegate void HealthEmpty();
    public static HealthEmpty healthEmpty;
    
    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        Rb.useGravity = false;
        Rb.constraints = RigidbodyConstraints.FreezeRotation;

        PlayerCollider = GetComponent<CapsuleCollider>();
        PlayerCollider.isTrigger = true;

        Health = new Health(3);
    }

    private void Update()
    {
        float horizontal = PlayerInput.Horizontal;
        Vector3 mousePosition = PlayerInput.MousePosition;

        if(Dog != null)
        {
            Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
            float dogLeashForce = Mathf.Sign(mousePosition.x - playerScreenPosition.x);
            Dog.GetComponent<Rigidbody>().AddForce(dogLeashForce, 0, 0);
        }
        
        if (IsDebuffed)
        {
            horizontal *= -1;
        }

        Rb.AddForce(horizontal * MoveSpeed, 0, 0);

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
        }
        else if (other.transform.CompareTag("Pole"))
        {
            Health.Current = 0;
        }

        if (Health.Current <= 0)
        {
            healthEmpty?.Invoke();
        }
    }

    private IEnumerator DebuffTimerRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        IsDebuffed = false;
    }
}
