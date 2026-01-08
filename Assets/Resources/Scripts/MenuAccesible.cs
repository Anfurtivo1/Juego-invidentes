using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuAccesible : MonoBehaviour
{
    // 0 = Empezar juego
    // 1 = Salir del juego
    private int opcionSeleccionada = 0;

    [Header("Audios")]
    public AudioClip audioEmpezarJuego;
    public AudioClip audioSalirJuego;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Al iniciar el menú, siempre empieza en "Empezar juego"
        opcionSeleccionada = 0;
        ReproducirAudioActual();
    }

    void Update()
    {
        // Cambiar opción (W / Flecha arriba)
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            CambiarOpcion();
        }

        // Cambiar opción (S / Flecha abajo)
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            CambiarOpcion();
        }

        // Confirmar opción
        if (
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetMouseButtonDown(0)
        )
        {
            EjecutarOpcion();
        }
    }

    void CambiarOpcion()
    {
        // Solo hay 2 opciones, así que alternamos
        opcionSeleccionada = (opcionSeleccionada == 0) ? 1 : 0;
        ReproducirAudioActual();
    }

    void ReproducirAudioActual()
    {
        audioSource.Stop();

        if (opcionSeleccionada == 0)
        {
            audioSource.clip = audioEmpezarJuego;
        }
        else
        {
            audioSource.clip = audioSalirJuego;
        }

        audioSource.Play();
    }

    void EjecutarOpcion()
    {
        if (opcionSeleccionada == 0)
        {
            // Cargar la escena del juego
            SceneManager.LoadScene("Mapa");
        }
        else
        {
            // Salir del juego
            Application.Quit();
        }
    }
}
