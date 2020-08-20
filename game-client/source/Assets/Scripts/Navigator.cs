using UnityEngine;
using System.Collections;

public class Navigator : MonoBehaviour {

    private UnityEngine.AI.NavMeshAgent agent;
    private Animator animator;
    
	void Awake () {
	    agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
	    animator = GetComponent<Animator>();
	}

    void Update()
    {
        animator.SetBool("Walking", agent.remainingDistance > 0.01);
        animator.SetBool("Idle", agent.remainingDistance < 0.01);
    }
	
	public void NavigateTo (Vector3 position)
	{
	    agent.SetDestination(position);
	}
}
