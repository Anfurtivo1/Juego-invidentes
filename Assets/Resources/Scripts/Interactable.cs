using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Interactable : MonoBehaviour
{
    [Header("ID del objeto para alternativo")]
    public string interactableID; // ej: "Cajon1"

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
    public string requiredItemForHold; // opcional: objeto que debe tener el jugador para activar alternativo

    private AudioSource audioSource;
    private Inventory playerInventory;
    private SimpleFirstPersonController playerController;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.playOnAwake = false;

        if (InteractionFlagManager.Instance != null)
        {
            InteractionFlagManager.Instance.UnlockAlternate(interactableID);
            Debug.Log(interactableID + " desbloqueado manualmente");
        }
    }

    public void Interact(Inventory inventory, SimpleFirstPersonController controller)
    {
        playerInventory = inventory;
        playerController = controller;

        if (holdToPlay && inventory.HasItem(requiredItemForHold))
        {
            // Generador: solo inicia si mantiene click
            StartCoroutine(HoldInteraction());
            return;
        }

        PlayAudioInteraction();
    }

    private void PlayAudioInteraction()
    {
        // Determinar si el alternativo est� desbloqueado y opcionalmente si tienes el item requerido
        bool hasRequiredItem = string.IsNullOrEmpty(requiredItemForHold) ||
                               (playerInventory != null && playerInventory.HasItem(requiredItemForHold));

        bool alternateUnlocked = hasRequiredItem &&
                                 !string.IsNullOrEmpty(interactableID) &&
                                 InteractionFlagManager.Instance != null &&
                                 InteractionFlagManager.Instance.IsAlternateUnlocked(interactableID);

        AudioClip clip = (alternateUnlocked && alternateClip != null) ? alternateClip : defaultClip;

        if (clip != null && playerController != null)
        {
            audioSource.clip = clip;
            audioSource.Play();

            if (blockMovementDuringAudio)
            {
                playerController.FreezePlayer();
                Invoke(nameof(EndBlock), clip.length);
            }
        }

        // Dar objeto si corresponde
        if (!string.IsNullOrEmpty(itemToGive))
        {
            if ((giveOnAlternate && alternateUnlocked) || !giveOnAlternate)
                playerInventory.AddItem(itemToGive);
        }
    }

    private IEnumerator HoldInteraction()
    {
        if (playerController != null)
            playerController.FreezePlayer();

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

        if (playerController != null)
            playerController.UnfreezePlayer();
    }

    private void EndBlock()
    {
        if (playerController != null)
            playerController.UnfreezePlayer();
    }
}