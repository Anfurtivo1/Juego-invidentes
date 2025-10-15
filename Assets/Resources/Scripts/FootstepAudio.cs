using UnityEngine;
using System.Collections.Generic;

public class FootstepAudio : MonoBehaviour
{
    [System.Serializable]
    public class SurfaceFootstep
    {
        public string tag;
        public List<AudioClip> clips;
    }

    [Header("Configuraci�n superficies")]
    public List<SurfaceFootstep> surfaces;
    public List<AudioClip> defaultClips;

    [Header("AudioSource (opcional)")]
    [Tooltip("Si est� vac�o, se crear� uno autom�ticamente en este GameObject.")]
    public AudioSource footstepSource;

    // Devuelve un clip aleatorio adecuado (o de default). NO reproduce nada.
    public AudioClip GetRandomFootstepClip()
    {
        // Raycast hacia abajo; si falla, usamos default
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 2f))
        {
            string surfaceTag = hit.collider.tag;
            List<AudioClip> clips = GetClipsForTag(surfaceTag);
            if (clips != null && clips.Count > 0)
                return clips[Random.Range(0, clips.Count)];
        }

        // fallback
        if (defaultClips != null && defaultClips.Count > 0)
            return defaultClips[Random.Range(0, defaultClips.Count)];

        return null;
    }

    private List<AudioClip> GetClipsForTag(string tag)
    {
        foreach (var s in surfaces)
        {
            if (!string.IsNullOrEmpty(s.tag) && s.tag == tag)
                return s.clips;
        }
        return null;
    }

    // Asegura que haya un AudioSource v�lido y lo devuelve (cre�ndolo si hace falta).
    public AudioSource EnsureAudioSource()
    {
        if (footstepSource == null)
        {
            footstepSource = GetComponent<AudioSource>();
            if (footstepSource == null)
            {
                footstepSource = gameObject.AddComponent<AudioSource>();
                footstepSource.spatialBlend = 1f; // 3D
                footstepSource.playOnAwake = false;
                footstepSource.dopplerLevel = 0f;
            }
        }
        return footstepSource;
    }

    // Devuelve la referencia (puede ser null)
    public AudioSource GetAttachedAudioSource()
    {
        return footstepSource != null ? footstepSource : GetComponent<AudioSource>();
    }
}