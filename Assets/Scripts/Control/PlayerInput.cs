using UnityEngine;

public class PlayerInput : IPlayerInput
{
	public float Horizontal => Input.GetAxis("Horizontal");
	public Vector3 MousePosition => Input.mousePosition;
	public bool MouseLeftClicked => Input.GetMouseButton(0);
}
