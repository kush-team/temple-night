using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class NetPlayer : MonoBehaviour
{
    public Vector3 destination;
    public Vector3 rotation;
    public bool hitting;
    public bool running;
    public bool jumping;
    //public TextMeshPro Label;
    
    public string NickName;

    private Rigidbody _body;
    private Animator animator;
    private NetworkEntity _netWorkEntity;
    private float distance = 0.0f;


    void Start()
    {
        _body = GetComponent<Rigidbody>();
        _netWorkEntity = GetComponent<NetworkEntity>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate() 
    {
        if (destination != null && rotation != null && !_netWorkEntity.isDead()) 
        {
            Quaternion r = Quaternion.Euler(rotation);

            _body.MoveRotation(r);
            distance = Vector3.Distance(destination, transform.position);

            if (distance > 0.01f)
            {
                _body.MovePosition(destination);
            }

            animator.SetBool("Runnig", running);
            animator.SetFloat("Distance", distance);
            if (jumping)
                animator.SetTrigger("Jumping");

            if (hitting)
                animator.SetTrigger("HItting");

            //Label.text = NickName;
        }        
    }

    public void Hit()
    {
        animator.SetTrigger("Hitted");
    }


    public void Die()
    {
        _body.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ |  RigidbodyConstraints.FreezeRotationX;
        animator.SetBool("Runnig", false);
        animator.SetFloat("Distance", 0f);        
        animator.SetTrigger("Die");
    }    
}
