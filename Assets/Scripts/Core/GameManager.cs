using UnityEngine;
using RopeMinikit;

public class GameManager : MonoBehaviour
{
    public bool IsGameOver { get; set; }

    private void OnEnable()
    {
        UIManager.PlayerHealthZero += OnPlayerHealthEmpty;
        Rope.CollidedWithPoleObject += OnPoleCollision;
    }
    private void OnDisable()
    {
        UIManager.PlayerHealthZero -= OnPlayerHealthEmpty;
        Rope.CollidedWithPoleObject -= OnPoleCollision;
    }

    private void OnPlayerHealthEmpty()
    {
        IsGameOver = true;
    }
    private void OnPoleCollision()
    {
        IsGameOver = true;
    }
}
