using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class IA_Behaviour : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject player;
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
        player = GameObject.FindGameObjectWithTag("Player");
        enemy_Spawner = FindAnyObjectByType<EnemySpawner>();

        if (enemy_Spawner.contadorVoces == 8)
            StartCoroutine(player.GetComponent<SimpleFirstPersonController>().MatarJugador());
    }

    void Update()
    {
        if (!waiting)
        {
            if (!movingToRandom)
            {
                //Debug.Log("Distancia: " + Vector3.Distance(transform.position, player.position));

                if (Vector3.Distance(transform.position, player.transform.position) < 3f)
                    StartCoroutine(WaitAndCheckPlayer());
                else
                    agent.SetDestination(player.transform.position);
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
        agent.isStopped = true;
        waiting = true;

        float waitTime = 5f;
        float elapsed = 0f;

        var controller = player.GetComponent<SimpleFirstPersonController>();

        while (elapsed < waitTime)
        {
            // Si el jugador se mueve en cualquier momento
            if (controller.isMoving)
            {
                Debug.Log("Comamos polvorones Jacobo");
                StartCoroutine(controller.MatarJugador());
                yield break; // Salimos de la corrutina
            }

            elapsed += Time.deltaTime;
            yield return null; // Espera al siguiente frame
        }

        // Si llegamos aquí, el jugador NO se movió en 5 segundos
        waiting = false;

        Debug.Log("Pos me voy");
        randomPos = GetRandomNavMeshPosition();
        agent.isStopped = false;
        agent.SetDestination(randomPos);
        movingToRandom = true;
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

            Debug.Log("Posible posicion: " + hit.position);
        }
        Debug.Log("Posicion final al no encontrar otra: " + transform.position);
        // Si no encuentra un punto válido, usa la posición actual
        return transform.position;

    }
}

