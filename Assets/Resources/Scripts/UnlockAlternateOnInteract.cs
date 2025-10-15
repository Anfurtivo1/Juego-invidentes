using UnityEngine;

public class UnlockAlternateOnInteract : MonoBehaviour
{
    public string interactableIDToUnlock; // ej: "Cajon1"

    public void Interact()
    {
        if (!string.IsNullOrEmpty(interactableIDToUnlock) && InteractionFlagManager.Instance != null)
        {
            InteractionFlagManager.Instance.UnlockAlternate(interactableIDToUnlock);
        }
    }
}