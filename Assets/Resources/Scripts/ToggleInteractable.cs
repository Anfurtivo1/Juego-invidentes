using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ToggleInteractable : MonoBehaviour
{
    [Header("Sonidos")]
    public AudioClip onClip;
    public AudioClip offClip;

    [Header("Opciones")]
    [Tooltip("Bloquea interacción mientras suena el audio (opcional)")]
    public bool blockDuringAudio = false;

    private bool isOn = false;
    private bool isPlayingSound = false;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound
    }

    /// <summary>
    /// Método público que se llama desde PlayerInteraction (igual que Interactable).
    /// </summary>
    public void Interact()
    {
        // Si el sonido está en curso, no permitir nuevo toggle
        if (isPlayingSound)
            return;

        // Cambiar estado
        isOn = !isOn;

        // Elegir clip según el nuevo estado
        AudioClip clipToPlay = isOn ? onClip : offClip;

        if (clipToPlay != null)
        {
            isPlayingSound = true;

            // Crear objeto temporal para reproducir el sonido
            GameObject tempAudio = new GameObject("TempToggleAudio");
            tempAudio.transform.position = transform.position;

            AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
            tempSource.clip = clipToPlay;
            tempSource.spatialBlend = 1f;
            tempSource.volume = audioSource.volume;
            tempSource.minDistance = audioSource.minDistance;
            tempSource.maxDistance = audioSource.maxDistance;
            tempSource.rolloffMode = audioSource.rolloffMode;

            tempSource.Play();
            Destroy(tempAudio, clipToPlay.length);

            // Esperar a que termine el clip
            StartCoroutine(ResetAfterSound(clipToPlay.length));
        }

        // Aquí podrías añadir efectos visuales o activar/desactivar objetos:
        // e.g. luz.SetActive(isOn);
    }

    private System.Collections.IEnumerator ResetAfterSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        isPlayingSound = false;
    }

    /// <summary>
    /// Devuelve si el toggle está activado (por si lo necesitas en otros scripts)
    /// </summary>
    public bool IsOn()
    {
        return isOn;
    }
}