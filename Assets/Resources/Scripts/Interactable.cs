using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;  // Necesario para reiniciar la escena

[RequireComponent(typeof(AudioSource))]
public class Interactable : MonoBehaviour
{
    [Header("Identificador (opcional)")]
    public string interactableID; // Por si lo necesitas para debug

    [Header("Sonidos")]
    public AudioClip defaultClip;
    public AudioClip alternateClip; // Por ejemplo, rebuscar con energía

    [Header("Bloquear movimiento mientras suena")]
    public bool blockMovementDuringAudio = false;

    [Header("Dar objeto al interactuar")]
    public string itemToGive;
    public bool giveOnAlternate = false;

    [Header("Requiere mantener click (ej. generador reparando)")]
    public bool holdToPlay = false;
    public string requiredItemForHold; // Objeto que debe tener el jugador para activar un "hold"

    [Header("Requiere ítem para alternativo")]
    public string requiredItemForAlternate; // Ej: "reparar1"

    private AudioSource audioSource;
    private Inventory playerInventory;
    private SimpleFirstPersonController playerController;

    private float currentTime = 0f; // Guarda el tiempo del audio cuando se pausa

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.playOnAwake = false;
    }

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

    private void PlayAudioInteractionAtPoint(Vector3 position)
    {
        bool canDoAlternate = !string.IsNullOrEmpty(requiredItemForAlternate) &&
                              playerInventory != null &&
                              playerInventory.HasItem(requiredItemForAlternate);

        AudioClip clip = (canDoAlternate && alternateClip != null) ? alternateClip : defaultClip;

        if (clip != null)
        {
            GameObject tempAudio = new GameObject("TempAudio");
            tempAudio.transform.position = position;

            AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
            tempSource.clip = clip;
            tempSource.spatialBlend = 1f;

            AudioSource originalSource = GetComponent<AudioSource>();
            if (originalSource != null)
            {
                tempSource.volume = originalSource.volume;
                tempSource.pitch = originalSource.pitch;
                tempSource.minDistance = originalSource.minDistance;
                tempSource.maxDistance = originalSource.maxDistance;
                tempSource.rolloffMode = originalSource.rolloffMode;
            }
            else
            {
                tempSource.volume = 1f;
            }

            tempSource.Play();
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
            if ((giveOnAlternate && canDoAlternate) || !giveOnAlternate)
                playerInventory.AddItem(itemToGive);
        }
    }

    private IEnumerator HoldInteractionAtPoint(Vector3 position)
    {
        AudioClip clip = alternateClip != null ? alternateClip : defaultClip;
        if (clip == null) yield break;

        GameObject tempAudio = new GameObject("TempHoldAudio");
        tempAudio.transform.position = position;

        AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
        tempSource.clip = clip;
        tempSource.spatialBlend = 1f;

        bool isPlaying = false;

        while (true)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                // Si no está sonando y ya se había pausado, reanudar desde donde se quedó
                if (!tempSource.isPlaying)
                {
                    tempSource.time = currentTime;
                    tempSource.Play();
                    isPlaying = true;
                }
            }
            else
            {
                if (tempSource.isPlaying)
                {
                    // Pausar y guardar el tiempo actual
                    currentTime = tempSource.time;
                    tempSource.Pause();
                    isPlaying = false;
                }
            }

            // Si el audio termina, destruir el objeto y reiniciar la escena
            if (!tempSource.isPlaying && tempSource.time >= tempSource.clip.length && isPlaying)
            {
                break;
            }

            yield return null;
        }

        Destroy(tempAudio);

        // Reiniciar la escena una vez que termine el audio
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void EndBlock()
    {
        if (playerController != null)
            playerController.UnfreezePlayer();
    }
}