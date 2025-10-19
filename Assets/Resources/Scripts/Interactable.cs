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

    /// <summary>
    /// Interacción principal. Ahora recibe la posición del impacto del raycast.
    /// </summary>
    public void Interact(Inventory inventory, SimpleFirstPersonController controller, Vector3 hitPoint)
    {
        playerInventory = inventory;
        playerController = controller;

        if (holdToPlay && inventory.HasItem(requiredItemForHold))
        {
            StartCoroutine(HoldInteractionAtPoint(hitPoint));
            return;
        }

        PlayAudioInteractionAtPoint(hitPoint);
    }

    /// <summary>
    /// Reproduce el audio en un emisor temporal en la posición del impacto.
    /// </summary>
    private void PlayAudioInteractionAtPoint(Vector3 position)
    {
        bool hasRequiredItem = string.IsNullOrEmpty(requiredItemForHold) ||
                               (playerInventory != null && playerInventory.HasItem(requiredItemForHold));

        bool alternateUnlocked = hasRequiredItem &&
                                 !string.IsNullOrEmpty(interactableID) &&
                                 InteractionFlagManager.Instance != null &&
                                 InteractionFlagManager.Instance.IsAlternateUnlocked(interactableID);

        AudioClip clip = (alternateUnlocked && alternateClip != null) ? alternateClip : defaultClip;

        if (clip != null)
        {
            // Crear objeto temporal en la posición del click
            GameObject tempAudio = new GameObject("TempAudio");
            tempAudio.transform.position = position;

            AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
            tempSource.clip = clip;
            tempSource.spatialBlend = 1f; // 1 = 3D. Cambia a 0 si prefieres que el sonido no tenga posición.
            tempSource.Play();

            // Destruir después de que termine el sonido
            Destroy(tempAudio, clip.length);

            if (blockMovementDuringAudio && playerController != null)
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

    /// <summary>
    /// Versión adaptada para interacciones de "mantener pulsado" con sonido 3D en el punto de impacto.
    /// </summary>
    private IEnumerator HoldInteractionAtPoint(Vector3 position)
    {
        AudioClip clip = alternateClip != null ? alternateClip : defaultClip;
        if (clip == null) yield break;

        GameObject tempAudio = new GameObject("TempHoldAudio");
        tempAudio.transform.position = position;

        AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
        tempSource.clip = clip;
        tempSource.spatialBlend = 1f;

        while (true)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                if (!tempSource.isPlaying)
                    tempSource.Play();
            }
            else
            {
                if (tempSource.isPlaying)
                    tempSource.Pause();
            }

            if (!tempSource.isPlaying && tempSource.time >= tempSource.clip.length)
                break;

            yield return null;
        }

        Destroy(tempAudio);
    }

    private void EndBlock()
    {
        if (playerController != null)
            playerController.UnfreezePlayer();
    }
}