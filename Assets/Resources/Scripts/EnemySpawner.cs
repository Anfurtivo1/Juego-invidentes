using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;

    // Límites del área
    public Vector3 minBounds = new Vector3(-22.8f, 1, -23f);
    public Vector3 maxBounds = new Vector3(22.8f, 1, 23f);

    // Sonidos
    public AudioSource audioSource;
    public AudioClip preSpawnSound; // sonido 2 segundos antes
    //public AudioClip spawnSound;    // sonido cuando spawnea

    public float spawnInterval = 20f; // cada cuánto tiempo se genera un enemigo
    public float preSpawnDelay = 2f;  // cuántos segundos antes suena el aviso

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        // Espera inicial antes del primer spawn (5 segundos como en tu código original)
        yield return new WaitForSeconds(5f);

        while (true)
        {
            // Sonido previo (2 segundos antes del spawn)
            yield return new WaitForSeconds(spawnInterval - preSpawnDelay);
            if (audioSource && preSpawnSound)
            {
                audioSource.PlayOneShot(preSpawnSound);
            }
                

            // Espera los 2 segundos restantes antes del spawn
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
