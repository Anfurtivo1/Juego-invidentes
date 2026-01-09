using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class ChangeSceneAfterAudio : MonoBehaviour
{
    [Header("Escena a cargar al terminar el audio")]
    public string sceneToLoad;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource.clip == null)
        {
            Debug.LogWarning("No hay AudioClip asignado.");
            return;
        }

        audioSource.Play();
        Invoke(nameof(LoadScene), audioSource.clip.length);
    }

    void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}