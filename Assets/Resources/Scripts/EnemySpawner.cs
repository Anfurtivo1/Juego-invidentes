using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{

    public GameObject enemy;

    // Límites del área
    public Vector3 minBounds = new Vector3(-22.8f, 1, -23f);
    public Vector3 maxBounds = new Vector3(22.8f, 1, 23f);

    // Sonidos
    public AudioSource audioSource;
    public AudioSource audioSourceMatar;
    public AudioClip preSpawnSound1; // sonido 2 segundos antes
    public AudioClip preSpawnSound2; // sonido 2 segundos antes
    public AudioClip preSpawnSound3; // sonido 2 segundos antes
    public AudioClip preSpawnSound4; // sonido 2 segundos antes
    public AudioClip preSpawnSound5; // sonido 2 segundos antes
    public AudioClip preSpawnSound6; // sonido 2 segundos antes
    public AudioClip preSpawnSound7; // sonido 2 segundos antes
    public AudioClip preSpawnSound8; // sonido 2 segundos antes

    public AudioClip sonidoMatar; // sonido 2 segundos antes
    //public AudioClip spawnSound;    // sonido cuando spawnea

    public float spawnInterval = 20f; // cada cuánto tiempo se genera un enemigo
    public float preSpawnDelay = 2f;  // cuántos segundos antes suena el aviso

    public int contadorVoces = 0;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private void Update()
    {

    }

    public IEnumerator MatarJugador()
    {
        Debug.Log("Voy a matarte");
        audioSourceMatar.clip = sonidoMatar;
        audioSourceMatar.Play();

        yield return new WaitForSeconds(audioSourceMatar.clip.length);

        //Cargar escena
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    IEnumerator SpawnRoutine()
    {
        // Espera inicial antes del primer spawn PONER A 60
        yield return new WaitForSeconds(5f);

        while (true)
        {
            // Sonido previo (2 segundos antes del spawn)
            yield return new WaitForSeconds(spawnInterval - preSpawnDelay);
            if (audioSource && preSpawnSound1)
            {
                contadorVoces = contadorVoces + 1;
                switch (contadorVoces)
                {
                    case 1:
                        audioSource.PlayOneShot(preSpawnSound1);
                        break;
                    case 2:
                        audioSource.PlayOneShot(preSpawnSound2);
                        break;
                    case 3:
                        audioSource.PlayOneShot(preSpawnSound3);
                        break;
                    case 4:
                        audioSource.PlayOneShot(preSpawnSound4);
                        break;
                    case 5:
                        audioSource.PlayOneShot(preSpawnSound5);
                        break;
                    case 6:
                        audioSource.PlayOneShot(preSpawnSound6);
                        break;
                    case 7:
                        audioSource.PlayOneShot(preSpawnSound7);
                        break;
                    case 8:
                        audioSource.PlayOneShot(preSpawnSound8);
                        break;
                    default:
                        break;
                }
            }


            // Espera los segundos restantes antes del spawn
            yield return new WaitForSeconds(preSpawnDelay);

            // Spawnea enemigo y reproduce el sonido del spawn
            //SpawnEnemy();

            // Spawnea el enemigo
            GameObject newEnemy = Instantiate(enemy, GetRandomNavMeshPosition(), Quaternion.identity);


            // Si el prefab tiene EnemyController, activar su sonido 3D
            EnemyController controller = newEnemy.GetComponent<EnemyController>();

            if (controller != null)
            {
                controller.PlaySpawnLoopSound();
            }

        }
    }

    public void SpawnEnemy()
    {
        Instantiate(enemy, GetRandomNavMeshPosition(), Quaternion.identity);
    }

    Vector3 GetRandomNavMeshPosition()
    {
        // Corrijo tu cálculo: antes usabas siempre maxBounds.z en lugar de un Random.Range
        Vector3 randomPoint = new Vector3(
            Random.Range(minBounds.x, maxBounds.x),
            0,
            Random.Range(minBounds.z, maxBounds.z)
        );
        return randomPoint;
    }
}
