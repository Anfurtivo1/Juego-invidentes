using UnityEngine;
using UnityEngine.AI;

public class IA_Behaviour : MonoBehaviour
{
    private NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position);
    }
}
