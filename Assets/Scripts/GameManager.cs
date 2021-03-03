using UnityEngine;

public class GameManager : ScriptableObject
{
	public bool GameOver { get; set; }

	public void OnEnable()
	{
		Player.healthEmpty += OnPlayerHealthEmpty;
	}

	public void OnDisable()
	{
		Player.healthEmpty -= OnPlayerHealthEmpty;
	}

	public void OnPlayerHealthEmpty()
	{
		GameOver = true;
	}
}
