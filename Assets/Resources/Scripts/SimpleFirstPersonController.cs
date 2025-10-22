using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class SimpleFirstPersonController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;

    [Header("Mirar con el ratón (solo horizontal)")]
    public float mouseSensitivity = 1f;
    public Transform cameraTransform;

    [Header("Sonidos de pasos")]
    [Tooltip("Tiempo extra que debe pasar DESPUÉS de que termine el clip hasta el siguiente paso.")]
    public float footstepInterval = 0.15f;

    private CharacterController controller;
    private Vector3 velocity;
    private float rotationY = 0f;

    private float originalMoveSpeed;
    private float originalMouseSensitivity;

    private bool isFrozen = false;

    private Coroutine footstepCoroutine;
    private bool isMoving;

    private FootstepAudio footstepAudio;

    // 👇 Nueva variable para comprobar desplazamiento real
    private Vector3 lastPosition;
    private float minMoveDistance = 0.01f; // margen de movimiento mínimo real

    void Start()
    {
        Debug.Log("AudioListener active: " + AudioListener.pause);
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalMoveSpeed = moveSpeed;
        originalMouseSensitivity = mouseSensitivity;

        footstepAudio = GetComponent<FootstepAudio>();
        if (footstepAudio == null)
            Debug.LogWarning("No FootstepAudio attached to Player. Añádelo para escuchar pasos.");

        lastPosition = transform.position;
    }

    void Update()
    {
        if (Keyboard.current == null || Mouse.current == null)
            return;

        HandleLook();
        HandleMovement();
        ApplyGravity();
    }

    void HandleLook()
    {
        if (isFrozen) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity * Time.deltaTime * 100f;
        rotationY += mouseDelta.x;
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }

    void HandleMovement()
    {
        Vector3 move = Vector3.zero;

        if (!isFrozen)
        {
            if (Keyboard.current.wKey.isPressed) move += transform.forward;
            if (Keyboard.current.sKey.isPressed) move -= transform.forward;
            if (Keyboard.current.aKey.isPressed) move -= transform.right;
            if (Keyboard.current.dKey.isPressed) move += transform.right;
        }

        move.Normalize();
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 👇 Comprobar si realmente se movió (posición actual vs anterior)
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        bool reallyMoved = distanceMoved > minMoveDistance;

        bool currentlyMoving = move.magnitude > 0 && controller.isGrounded && !isFrozen && reallyMoved;

        if (currentlyMoving && !isMoving)
        {
            isMoving = true;
            if (footstepCoroutine == null)
                footstepCoroutine = StartCoroutine(FootstepRoutine());
        }
        else if (!currentlyMoving && isMoving)
        {
            isMoving = false;
        }

        lastPosition = transform.position; // actualizar para el siguiente frame
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    IEnumerator FootstepRoutine()
    {
        while (isMoving)
        {
            if (footstepAudio != null)
            {
                AudioClip clip = footstepAudio.GetRandomFootstepClip();
                AudioSource src = footstepAudio.EnsureAudioSource();

                if (clip != null && src != null)
                {
                    src.clip = clip;
                    src.Play();
                    yield return new WaitUntil(() => !src.isPlaying);
                }
            }

            float timer = 0f;
            while (timer < footstepInterval)
            {
                if (!isMoving) break;
                timer += Time.deltaTime;
                yield return null;
            }
        }

        footstepCoroutine = null;
    }

    // Métodos públicos
    public void FreezePlayer()
    {
        moveSpeed = 0f;
        mouseSensitivity = 0f;
    }

    public void UnfreezePlayer()
    {
        moveSpeed = originalMoveSpeed;
        mouseSensitivity = originalMouseSensitivity;
    }

    public void NormalPlayer()
    {
        moveSpeed = 3f;
        mouseSensitivity = 0.3f;
    }
}