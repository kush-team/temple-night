using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class NetPlayer : MonoBehaviour
{
    public Vector3 destination;
    public Vector3 rotation;
    public bool walking;
    public bool running;
    public bool jumping;
    //public TextMeshPro Label;
    
    public string NickName;

    private Rigidbody _body;
    private Animator animator;
    private NetworkEntity _netWorkEntity;


    void Start()
    {
        _body = GetComponent<Rigidbody>();
        _netWorkEntity = GetComponent<NetworkEntity>();
        animator = GetComponent<Animator>();
        walking = false;
    }

    void Update()
    {

    }


    void FixedUpdate() 
    {
        if (destination != null && rotation != null && !_netWorkEntity.isDead()) 
        {
            Quaternion r = Quaternion.Euler(rotation);

            _body.MoveRotation(r);
            
            if (Vector3.Distance(destination, transform.position) > 0.01f)
            {
                _body.MovePosition(destination);
                if (jumping)
                {
                    animator.SetBool("Runnig", false);
                    animator.SetBool("Walking", false);
                    animator.SetBool("Idle", false);
                }
                else
                {
                    if (running)
                    {
                        animator.SetBool("Runnig", true);
                        animator.SetBool("Walking", false);
                        animator.SetBool("Idle", false);
                    }
                    else
                    {
                        animator.SetBool("Runnig", false);
                        animator.SetBool("Walking", walking);
                        animator.SetBool("Idle", !walking);
                    }
                }
            }

            //Label.text = NickName;
        }        
    }

    public void Hit()
    {
        
    }


    public void Die()
    {
        _body.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ |  RigidbodyConstraints.FreezeRotationX;
        animator.SetBool("Walking", false);
        animator.SetBool("Runnig", false);
        animator.SetBool("Idle", false);           
        animator.SetTrigger("Die");
    }    
}
