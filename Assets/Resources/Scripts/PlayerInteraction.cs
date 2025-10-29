using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interacción")]
    public float interactRange = 3f;
    public LayerMask interactableMask;

    private Inventory inventory;
    private SimpleFirstPersonController controller;

    void Start()
    {
        inventory = GetComponent<Inventory>();
        controller = GetComponent<SimpleFirstPersonController>();

        if (controller == null)
            Debug.LogWarning("No se encontró SimpleFirstPersonController en el Player.");
        if (inventory == null)
            Debug.LogWarning("No se encontró Inventory en el Player.");
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (controller == null || controller.cameraTransform == null)
                return;

            // Raycast desde la cámara
            Ray ray = new Ray(controller.cameraTransform.position, controller.cameraTransform.forward);
            Debug.DrawRay(controller.cameraTransform.position, controller.cameraTransform.forward * interactRange, Color.red);

            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableMask))
            {
                // --- 1️⃣ Interactuable normal ---
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.Interact(inventory, controller, hit.point);
                    return;
                }

                // --- 2️⃣ ToggleInteractable (nuevo) ---
                ToggleInteractable toggle = hit.collider.GetComponent<ToggleInteractable>();
                if (toggle != null)
                {
                    toggle.Interact();
                    return;
                }
            }
        }
    }
}