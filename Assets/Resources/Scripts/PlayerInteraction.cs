using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interacci�n")]
    public float interactRange = 3f;
    public LayerMask interactableMask;

    private Inventory inventory;
    private SimpleFirstPersonController controller;

    void Start()
    {
        inventory = GetComponent<Inventory>();
        controller = GetComponent<SimpleFirstPersonController>();
        if (controller == null)
            Debug.LogWarning("No se encontr� SimpleFirstPersonController en el Player.");
        if (inventory == null)
            Debug.LogWarning("No se encontr� Inventory en el Player.");
    }

    void Update()
    {
        // Detectar click izquierdo
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Intento interactuar");

            if (controller == null || controller.cameraTransform == null)
                return;

            // Raycast desde la c�mara
            Ray ray = new Ray(controller.cameraTransform.position, controller.cameraTransform.forward);
            Debug.DrawRay(controller.cameraTransform.position, controller.cameraTransform.forward * interactRange, Color.red);

            // Chequear colisiones solo con layers permitidos
            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableMask))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    Debug.Log("Hay algo con lo que interactuar: " + hit.collider.name);
                    interactable.Interact(inventory, controller);
                }
            }
        }
    }
}