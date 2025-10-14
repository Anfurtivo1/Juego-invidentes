using UnityEngine;
using UnityEngine.InputSystem; // Nuevo sistema de Input

public class FirstPersonLook : MonoBehaviour
{
    [Header("Sensibilidad del ratón")]
    public float sensitivity = 1f;

    [Header("Referencia al input")]
    public DefaultInputActions controls;

    private Vector2 lookInput;
    private float rotationY = 0f;

    void Awake()
    {
        controls = new DefaultInputActions();

        // Escuchamos el input del ratón
        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += _ => lookInput = Vector2.zero;
    }

    void OnEnable()
    {
        controls.Player.Enable();
        Cursor.lockState = CursorLockMode.Locked; // Bloquea el cursor al centro
    }

    void OnDisable()
    {
        controls.Player.Disable();
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        // Solo rotamos en el eje Y (horizontal)
        rotationY += lookInput.x * sensitivity;
        transform.localRotation = Quaternion.Euler(0f, rotationY, 0f);
    }
}
