using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MinigameController : MonoBehaviour
{
    // ───────────── AUDIO SOURCES ─────────────
    public AudioSource generadorSource;
    public AudioSource monstruoSource;

    // ───────────── GENERADOR ─────────────
    public AudioClip empezarReparar;
    public AudioClip reparando;          // LOOP
    public AudioClip pararReparar;

    private bool estaReparando = false;
    private bool puedeReparar = true;

    private float progresoReparacion = 0f;
    private const float TIEMPO_GANAR = 50f;

    // ───────────── MONSTRUO ─────────────
    public AudioClip monstruoAviso;
    public AudioClip monstruoInvestigando; // largo
    public AudioClip monstruoScreamer;

    private bool monstruoLetal = false;
    private Coroutine rutinaMonstruo;
    private bool jugadorMuerto = false;

    void Start()
    {
        rutinaMonstruo = StartCoroutine(ControlMonstruo());
    }

    void Update()
    {
        if (jugadorMuerto) return;

        ControlReparacion();
        ContarProgreso();
    }

    // ───────────── REPARACIÓN ─────────────
    void ControlReparacion()
    {
        bool input = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);

        if (input && !estaReparando && puedeReparar)
        {
            IniciarReparacion();
        }
        else if (!input && estaReparando)
        {
            DetenerReparacion();
        }

        // Muerte si repara cuando el monstruo es letal
        if (estaReparando && monstruoLetal)
        {
            StartCoroutine(Muerte());
        }
    }

    void IniciarReparacion()
    {
        estaReparando = true;

        generadorSource.Stop();
        generadorSource.clip = empezarReparar;
        generadorSource.loop = false;
        generadorSource.Play();

        StartCoroutine(EsperarYReparar());
    }

    IEnumerator EsperarYReparar()
    {
        yield return new WaitForSeconds(empezarReparar.length);

        if (estaReparando && !jugadorMuerto)
        {
            generadorSource.clip = reparando;
            generadorSource.loop = true;
            generadorSource.Play();
        }
    }

    void DetenerReparacion()
    {
        estaReparando = false;

        generadorSource.Stop();
        generadorSource.loop = false;
        generadorSource.clip = pararReparar;
        generadorSource.Play();
    }

    void ContarProgreso()
    {
        if (estaReparando && !monstruoLetal)
        {
            progresoReparacion += Time.deltaTime;

            if (progresoReparacion >= TIEMPO_GANAR)
            {
                SceneManager.LoadScene("Victory");
            }
        }
    }

    // ───────────── MONSTRUO ─────────────
    IEnumerator ControlMonstruo()
    {
        while (true)
        {
            yield return new WaitForSeconds(8f);
            if (IntentarAparecer(25)) { yield return Aparicion(); continue; }

            yield return new WaitForSeconds(2f);
            if (IntentarAparecer(50)) { yield return Aparicion(); continue; }

            yield return new WaitForSeconds(2f);
            if (IntentarAparecer(75)) { yield return Aparicion(); continue; }

            yield return new WaitForSeconds(2f);
            yield return Aparicion(); // 100%
        }
    }

    bool IntentarAparecer(int probabilidad)
    {
        return Random.Range(0, 100) < probabilidad;
    }

    IEnumerator Aparicion()
    {
        // Aviso (jugador seguro)
        monstruoSource.clip = monstruoAviso;
        monstruoSource.Play();
        yield return new WaitForSeconds(monstruoAviso.length);

        // Investigando (jugador en peligro)
        monstruoLetal = true;
        monstruoSource.clip = monstruoInvestigando;
        monstruoSource.Play();
        yield return new WaitForSeconds(monstruoInvestigando.length);

        // El monstruo se va automáticamente aquí
        monstruoLetal = false;
    }

    // ───────────── MUERTE ─────────────
    IEnumerator Muerte()
    {
        jugadorMuerto = true;
        monstruoLetal = false;
        puedeReparar = false;
        estaReparando = false;

        generadorSource.Stop();
        monstruoSource.Stop();

        monstruoSource.clip = monstruoScreamer;
        monstruoSource.Play();

        yield return new WaitForSeconds(monstruoScreamer.length + 2f);

        SceneManager.LoadScene("TutorialGenerador");
    }
}
