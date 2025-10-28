using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class Interactable : MonoBehaviour
{
    [Header("Identificador (opcional)")]
    public string interactableID;

    [Header("Modo Toggle")]
    [Tooltip("Si está activo, alternará entre dos sonidos (default/alternate) cada vez que se interactúe.")]
    public bool isToggle = false;
    private bool toggleState = false;

    [Header("Sonidos")]
    public AudioClip defaultClip;
    public AudioClip alternateClip;

    [Header("Bloquear movimiento mientras suena")]
    public bool blockMovementDuringAudio = false;

    [Header("Dar objeto al interactuar")]
    public string itemToGive;
    public bool giveOnAlternate = false;

    [Header("Quitar objeto al interactuar")]
    public string itemToRemove;
    public bool removeOnAlternate = false;

    [Header("Requiere mantener click (ej. generador reparando)")]
    public bool holdToPlay = false;
    public string requiredItemForHold;

    [Header("Requiere ítem para alternativo")]
    public string requiredItemForAlternate;

    private AudioSource audioSource;
    private Inventory playerInventory;
    private SimpleFirstPersonController playerController;

    private float currentTime = 0f;
    private bool isPlayingSound = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.playOnAwake = false;
    }

    public void Interact(Inventory inventory, SimpleFirstPersonController controller, Vector3 hitPoint)
    {
        if (isPlayingSound)
            return;

        playerInventory = inventory;
        playerController = controller;

        // 🟢 Si está en modo "toggle", usamos el sistema especial
        if (isToggle)
        {
            PlayToggleSoundAtPoint(hitPoint);
            return;
        }

        if (holdToPlay && inventory.HasItem(requiredItemForHold))
        {
            StartCoroutine(HoldInteractionAtPoint(hitPoint));
            return;
        }

        PlayAudioInteractionAtPoint(hitPoint);
    }

    // 🔁 NUEVO: modo toggle entre dos sonidos
    private void PlayToggleSoundAtPoint(Vector3 position)
    {
        AudioClip clip = toggleState ? alternateClip : defaultClip;

        if (clip == null)
            return;

        isPlayingSound = true;

        GameObject tempAudio = new GameObject("TempToggleAudio");
        tempAudio.transform.position = position;

        AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
        tempSource.clip = clip;
        tempSource.spatialBlend = 1f;

        tempSource.volume = audioSource.volume;
        tempSource.minDistance = audioSource.minDistance;
        tempSource.maxDistance = audioSource.maxDistance;
        tempSource.rolloffMode = audioSource.rolloffMode;

        tempSource.Play();
        Destroy(tempAudio, clip.length);

        // Cambiamos el estado toggle
        toggleState = !toggleState;

        StartCoroutine(ResetAfterSound(clip.length));
    }

    private void PlayAudioInteractionAtPoint(Vector3 position)
    {
        bool canDoAlternate = !string.IsNullOrEmpty(requiredItemForAlternate) &&
                              playerInventory != null &&
                              playerInventory.HasItem(requiredItemForAlternate);

        AudioClip clip = (canDoAlternate && alternateClip != null) ? alternateClip : defaultClip;

        if (clip != null)
        {
            isPlayingSound = true;

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

            StartCoroutine(ResetAfterSound(clip.length));

            if (blockMovementDuringAudio && playerController != null)
            {
                playerController.FreezePlayer();
                Invoke(nameof(EndBlock), clip.length);
            }
        }

        //Dar objeto
        if (!string.IsNullOrEmpty(itemToGive))
        {
            if ((giveOnAlternate && canDoAlternate) || !giveOnAlternate)
                playerInventory.AddItem(itemToGive);
        }

        //Quitar objeto
        if (!string.IsNullOrEmpty(itemToRemove))
        {
            if ((removeOnAlternate && canDoAlternate) || (!removeOnAlternate && !canDoAlternate))
                playerInventory.RemoveItem(itemToRemove);
        }
    }

    private IEnumerator ResetAfterSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        isPlayingSound = false;
    }

    private IEnumerator HoldInteractionAtPoint(Vector3 position)
    {
        AudioClip clip = alternateClip != null ? alternateClip : defaultClip;
        if (clip == null) yield break;

        isPlayingSound = true;

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
                    currentTime = tempSource.time;
                    tempSource.Pause();
                    isPlaying = false;
                }
            }

            if (tempSource.time >= tempSource.clip.length - 0.05f)
                break;

            yield return null;
        }

        Destroy(tempAudio);
        isPlayingSound = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void EndBlock()
    {
        if (playerController != null)
            playerController.UnfreezePlayer();
    }
}