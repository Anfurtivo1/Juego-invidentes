using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class IntroAudioFreeze : MonoBehaviour
{
    public SimpleFirstPersonController playerController; // Referencia al jugador
    public AudioClip introClip; // Audio que debe reproducirse al inicio
    private AudioSource audioSource;

    void Start()
    {
        if (playerController == null)
        {
            Debug.LogError("No se ha asignado el SimpleFirstPersonController.");
            return;
        }

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = introClip;
        audioSource.spatialBlend = 0f; // 2D, no afecta la posición
        audioSource.volume = 1f;

        // Congelar jugador al inicio
        playerController.FreezePlayer();

        if (introClip != null)
            StartCoroutine(PlayIntroAudio());
        else
            playerController.UnfreezePlayer(); // Si no hay audio, desbloquear de inmediato
    }

    private IEnumerator PlayIntroAudio()
    {
        audioSource.Play();

        // Espera hasta que termine el audio
        yield return new WaitUntil(() => !audioSource.isPlaying);

        // Descongelar al jugador
        playerController.NormalPlayer();
    }
}