using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Vector2 moveInput;

    // MoveƒAƒNƒVƒ‡ƒ“‚ðŽó‚¯Žæ‚é
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        Vector3 movement = new Vector3(
            moveInput.x,
            moveInput.y,
            0f
        );

        transform.position += movement * moveSpeed * Time.deltaTime;
    }
}