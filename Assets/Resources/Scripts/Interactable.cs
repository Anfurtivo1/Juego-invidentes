using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Interactable : MonoBehaviour
{
    [Header("ID del objeto para alternativo")]
    public string interactableID; // Ej: "Cajon1"

    [Header("Sonidos")]
    public AudioClip defaultClip;
    public AudioClip alternateClip; // por ejemplo rebuscar

    [Header("Bloquear movimiento mientras suena")]
    public bool blockMovementDuringAudio = false;

    [Header("Dar objeto al interactuar")]
    public string itemToGive;
    public bool giveOnAlternate = false;

    [Header("Requiere mantener click (ej. generador reparando)")]
    public bool holdToPlay = false;
    public string requiredItemForHold;

    private AudioSource audioSource;
    private Inventory playerInventory;
    private SimpleFirstPersonController playerController;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.playOnAwake = false;
    }

    public void Interact(Inventory inventory, SimpleFirstPersonController controller)
    {
        playerInventory = inventory;
        playerController = controller;

        if (holdToPlay && inventory.HasItem(requiredItemForHold))
        {
            // Ejemplo: generador que requiere mantener click
            StartCoroutine(HoldInteraction());
            return;
        }

        // Determinar si el alternativo está desbloqueado
        bool alternateUnlocked = !string.IsNullOrEmpty(interactableID) &&
                                 InteractionFlagManager.Instance != null &&
                                 InteractionFlagManager.Instance.IsAlternateUnlocked(interactableID);

        AudioClip clip = (alternateUnlocked && alternateClip != null) ? alternateClip : defaultClip;

        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();

            if (blockMovementDuringAudio && playerController != null)
            {
                playerController.canMove = false;
                Invoke(nameof(EndBlock), clip.length);
            }
        }

        // Dar objeto si corresponde
        if (!string.IsNullOrEmpty(itemToGive))
        {
            if ((giveOnAlternate && alternateUnlocked) || !giveOnAlternate)
                inventory.AddItem(itemToGive);
        }
    }

    private IEnumerator HoldInteraction()
    {
        playerController.canMove = false;
        audioSource.clip = alternateClip != null ? alternateClip : defaultClip;
        audioSource.time = 0f;

        while (true)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                if (!audioSource.isPlaying)
                    audioSource.Play();
            }
            else
            {
                if (audioSource.isPlaying)
                    audioSource.Pause();
            }

            if (!audioSource.isPlaying && audioSource.time >= audioSource.clip.length)
                break;

            yield return null;
        }

        playerController.canMove = true;
    }

    private void EndBlock()
    {
        if (playerController != null)
            playerController.canMove = true;
    }
}