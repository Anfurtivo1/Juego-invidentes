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
    [HideInInspector] public bool canMove = true;

    private Coroutine footstepCoroutine;
    private bool isMoving;

    private FootstepAudio footstepAudio;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        footstepAudio = GetComponent<FootstepAudio>();
        if (footstepAudio == null)
            Debug.LogWarning("No FootstepAudio attached to Player. Añádelo para escuchar pasos.");
    }

    void Update()
    {
        if (Keyboard.current == null || Mouse.current == null)
            return;

        if (canMove)
        {
            HandleLook();
            HandleMovement();
        }
        else
        {
            ApplyGravity();
            // Si se bloquea movimiento, forzamos parada de pasos
            if (isMoving)
                StopMovingSteps();
        }
    }

    void HandleLook()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity * Time.deltaTime * 100f;
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

        bool currentlyMoving = move.magnitude > 0 && controller.isGrounded && canMove;

        if (currentlyMoving && !isMoving)
        {
            isMoving = true;
            if (footstepCoroutine == null)
                footstepCoroutine = StartCoroutine(FootstepRoutine());
        }
        else if (!currentlyMoving && isMoving)
        {
            // marcamos que dejó de moverse; la corrutina dejará de programar nuevos pasos
            isMoving = false;
            // no la paramos bruscamente; dejamos que el paso actual termine si está sonando
        }

        ApplyGravity();
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
        // Mientras isMoving sea true, reproducimos pasos en secuencia.
        while (true)
        {
            // Si dejó de moverse antes de empezar un nuevo paso, salimos.
            if (!isMoving)
                break;

            if (footstepAudio == null)
            {
                // Si no hay sistema de footsteps, evitamos bucle intenso
                yield return null;
                continue;
            }

            AudioClip clip = footstepAudio.GetRandomFootstepClip();
            AudioSource src = footstepAudio.EnsureAudioSource();

            if (clip == null || src == null)
            {
                // Si no hay clip ni audio source, esperamos un frame
                yield return null;
                continue;
            }

            // Opcional: si quieres variar pitch para más naturalidad, hazlo aquí:
            // src.pitch = Random.Range(0.95f, 1.05f);

            src.clip = clip;
            src.Play();

            // Esperamos hasta que termine de sonar (toma en cuenta pitch automáticamente: isPlaying seguirá true)
            yield return new WaitUntil(() => !src.isPlaying);

            // Si el jugador dejó de moverse mientras sonaba: no programamos siguiente paso
            if (!isMoving)
                break;

            // Esperamos el intervalo extra (en fracciones por frame; si deja de moverse durante el intervalo, salimos)
            float timer = 0f;
            while (timer < footstepInterval)
            {
                if (!isMoving)
                    break;
                timer += Time.deltaTime;
                yield return null;
            }

            // Si dejó de moverse, salimos; si no, se repite el bucle y suena siguiente clip.
        }

        // Limpiamos referencia
        footstepCoroutine = null;
    }

    void StopMovingSteps()
    {
        isMoving = false;
        // La corrutina saldrá por su propio flujo tras terminar el clip o al comprobar isMoving
    }

    // método público para forzar detener y cortar cualquier sonido de paso activo (si lo quieres)
    public void ForceStopFootstepsImmediate()
    {
        StopMovingSteps();
        if (footstepCoroutine != null)
        {
            StopCoroutine(footstepCoroutine);
            footstepCoroutine = null;
        }
        if (footstepAudio != null)
        {
            AudioSource s = footstepAudio.GetAttachedAudioSource();
            if (s != null && s.isPlaying)
                s.Stop();
        }
    }
}