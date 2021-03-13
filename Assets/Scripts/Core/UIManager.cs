using UnityEngine;

public class UIManager : MonoBehaviour
{
	public float PlayerHealth = 3;
	public float PlayerScore { get; set; }

    public delegate void PlayerHealthIsZero();
    public static PlayerHealthIsZero PlayerHealthZero;

    private void OnEnable()
    {
        Player.HealthIsChanged += OnPlayerHealthChange;
    }
    private void OnDisable()
    {
        Player.HealthIsChanged -= OnPlayerHealthChange;
    }
    private void Update()
    {
        UpdatePlayerScore();
    }

    private void UpdatePlayerScore()
    {
        PlayerScore += SpawnManager.CurrentSpeed * Time.deltaTime;
    }

    private void OnPlayerHealthChange()
    {
        PlayerHealth -= 1;
        if(PlayerHealth == 0)
        {
            PlayerHealthZero?.Invoke();
        }
    }
}
