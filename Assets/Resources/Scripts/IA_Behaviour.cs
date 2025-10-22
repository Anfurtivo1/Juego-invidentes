using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class IA_Behaviour : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private Vector3 randomPos;

    // Límites del área
    public Vector3 minBounds = new Vector3(-22.8f, 1, -23f);
    public Vector3 maxBounds = new Vector3(22.8f, 1, 23f);

    public bool waiting = false;
    public bool movingToRandom = false;

    public AudioSource audioSource;
    public AudioClip sonidoMatar; // sonido

    EnemySpawner enemy_Spawner;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemy_Spawner = FindAnyObjectByType<EnemySpawner>();
    }

    void Update()
    {
        if (!waiting)
        {
            if (!movingToRandom)
            {
                //Debug.Log("Distancia: " + Vector3.Distance(transform.position, player.position));

                if (Vector3.Distance(transform.position, player.position) < 3f)
                    StartCoroutine(WaitAndCheckPlayer());
                else
                    agent.SetDestination(player.position);
            }

            else
            {
                if (agent.isStopped)
                {
                    Debug.Log("Cuántas veces entra aquí???");
                    agent.SetDestination(randomPos);
                }

                else
                {
                    if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                        StartCoroutine(DisappearAndRespawn());
                }
            }
        }
    }

    IEnumerator WaitAndCheckPlayer()
    {

        if (enemy_Spawner.contadorVoces == 8)
        {
            StartCoroutine(enemy_Spawner.MatarJugador());
        }

        agent.isStopped = true;
        waiting = true;

        // Guardamos la posición inicial del jugador
        Vector3 initialPlayerPos = player.position;

        // Espera 5 segundos
        yield return new WaitForSeconds(5f);

        // Comprueba si el jugador se ha movido
        float distanceMoved = Vector3.Distance(initialPlayerPos, player.position);

        waiting = false;
        if (distanceMoved < 0.1f) // El jugador no se movió
        {

            Debug.Log("Pos me voy");// Ir a una posición aleatoria dentro del área
            randomPos = GetRandomNavMeshPosition();
            agent.isStopped = false;
            agent.SetDestination(randomPos);
            movingToRandom = true;
        }
        else
        {
            // El jugador se movió, seguir persiguiendo
            Debug.Log("Pos te persigo y te voy a matar");

            StartCoroutine(enemy_Spawner.MatarJugador());


            agent.isStopped = false;
        }
    }

    IEnumerator DisappearAndRespawn()
    {
        movingToRandom = false;
        agent.isStopped = true;

        yield return new WaitForSeconds(Random.Range(1f, 5f));
        // Desaparecer
        Destroy(gameObject);
    }

    Vector3 GetRandomNavMeshPosition()
    {
        waiting = false;

        for (int i = 0; i < 30; i++) // hasta 30 intentos de encontrar un punto válido
        {
            Vector3 randomPoint = new Vector3(
                Random.Range(minBounds.x, maxBounds.x), 0, maxBounds.z);

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 10f, NavMesh.AllAreas))
                return hit.position;

            Debug.Log("Posible posicion: "+hit.position);
        }
        Debug.Log("Posicion final al no encontrar otra: "+transform.position);
        // Si no encuentra un punto válido, usa la posición actual
        return transform.position;

    }
}

