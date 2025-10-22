using UnityEngine;
using UnityEngine.InputSystem;

public class RandomArmario : MonoBehaviour
{
    [Header("Lista de armarios")]
    public GameObject[] armarios;

    [Header("Sonido del armario activo")]
    public AudioClip sonidoArmario;
    public float volumen = 1f;

    private GameObject armarioActivo;
    private AudioSource tempSource;

    void Start()
    {
        if (armarios == null || armarios.Length == 0)
        {
            Debug.LogWarning("No se han asignado armarios en el inspector.");
            return;
        }

        // Desactivar todos
        foreach (GameObject armario in armarios)
        {
            if (armario != null)
                armario.SetActive(false);
        }

        // Elegir uno al azar y activarlo
        int indexAleatorio = Random.Range(0, armarios.Length);
        armarioActivo = armarios[indexAleatorio];
        if (armarioActivo != null)
            armarioActivo.SetActive(true);
    }

    void Update()
    {
        // Si el jugador pulsa ESPACIO → reproducir sonido del armario activo
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ReproducirSonidoArmario();
        }
    }

    void ReproducirSonidoArmario()
    {
        if (armarioActivo == null || sonidoArmario == null)
            return;

        if (tempSource != null && tempSource.isPlaying)
            return; // Evita solapamientos

        tempSource = armarioActivo.AddComponent<AudioSource>();
        tempSource.clip = sonidoArmario;
        tempSource.spatialBlend = 1f; // 3D sound
        tempSource.volume = volumen;
        tempSource.minDistance = 1f;
        tempSource.maxDistance = 15f;
        tempSource.rolloffMode = AudioRolloffMode.Linear;
        tempSource.Play();

        Destroy(tempSource, sonidoArmario.length + 0.1f);
    }
}