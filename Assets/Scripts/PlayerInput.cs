using UnityEngine;

public class PlayerInput : IPlayerInput
{
	public float Horizontal => Input.GetAxis("Horizontal");

	public Vector3 MousePosition => Input.mousePosition;
}
