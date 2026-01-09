using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MenuAccesible : MonoBehaviour
{
    // 0 = Empezar juego
    // 1 = Controles
    // 2 = Salir del juego
    private int opcionSeleccionada = 0;

    [Header("Audios")]
    public AudioClip audioEmpezarJuego;
    public AudioClip audioControles;
    public AudioClip audioSalirJuego;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Opción inicial
        opcionSeleccionada = 0;
        ReproducirAudioActual();
    }

    void Update()
    {
        // Subir opción
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            CambiarOpcion(-1);
        }

        // Bajar opción
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            CambiarOpcion(1);
        }

        // Confirmar
        if (
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetMouseButtonDown(0)
        )
        {
            EjecutarOpcion();
        }
    }

    void CambiarOpcion(int direccion)
    {
        opcionSeleccionada += direccion;

        // Menú circular (0–2)
        if (opcionSeleccionada < 0)
            opcionSeleccionada = 2;
        else if (opcionSeleccionada > 2)
            opcionSeleccionada = 0;

        ReproducirAudioActual();
    }

    void ReproducirAudioActual()
    {
        audioSource.Stop();

        switch (opcionSeleccionada)
        {
            case 0:
                audioSource.clip = audioEmpezarJuego;
                break;
            case 1:
                audioSource.clip = audioControles;
                break;
            case 2:
                audioSource.clip = audioSalirJuego;
                break;
        }

        audioSource.Play();
    }

    void EjecutarOpcion()
    {
        switch (opcionSeleccionada)
        {
            case 0:
                SceneManager.LoadScene("Mapa");
                break;

            case 1:
                SceneManager.LoadScene("Controles");
                break;

            case 2:
                Application.Quit();
                break;
        }
    }
}