using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public AudioSource audioSource3D; // el AudioSource 3D del enemigo
    public AudioClip spawnLoopSound;  // sonido 3D en loop

    void Start()
    {
        // Puedes dejarlo vacío si no quieres que suene nada al instanciar
    }

    public void PlaySpawnLoopSound()
    {
        if (audioSource3D && spawnLoopSound)
        {
            audioSource3D.clip = spawnLoopSound;//
            audioSource3D.loop = true;
            audioSource3D.spatialBlend = 1f; // asegura que sea 3D
            audioSource3D.Play();

        }
    }

    public void StopSpawnLoopSound()
    {
        if (audioSource3D)
            audioSource3D.Stop();
    }
}
