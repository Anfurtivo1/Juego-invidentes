using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class SimpleFirstPersonController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;

    [Header("Mirar con el ratón")]
    public float mouseSensitivity = 1f;
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;
    private float rotationX = 0f; // Vertical
    private float rotationY = 0f; // Horizontal

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Keyboard.current == null || Mouse.current == null)
            return;

        HandleLook();
        HandleMovement();
    }

    void HandleLook()
    {
        // Leemos movimiento del ratón
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity * Time.deltaTime * 100f;

        //// ✅ Vertical (mirar arriba/abajo)
        //rotationX += mouseDelta.y; // ← invertido respecto al anterior
        //rotationX = Mathf.Clamp(rotationX, -80f, 80f);
        //cameraTransform.localRotation = Quaternion.Euler(-rotationX, 0f, 0f);

        // ✅ Horizontal (mirar izquierda/derecha)
        rotationY += mouseDelta.x;
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }

    void HandleMovement()
    {
        Vector3 move = Vector3.zero;

        if (Keyboard.current.wKey.isPressed) move += transform.forward;
        if (Keyboard.current.sKey.isPressed) move -= transform.forward;
        if (Keyboard.current.aKey.isPressed) move -= transform.right;
        if (Keyboard.current.dKey.isPressed) move += transform.right;

        move.Normalize();

        controller.Move(move * moveSpeed * Time.deltaTime);

        // Gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }
}
