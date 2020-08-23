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


    void Start()
    {
        _body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        walking = false;
    }

    void Update()
    {

    }

    void FixedUpdate() 
    {
        if (destination != null && rotation != null) 
        {
            Quaternion r = Quaternion.Euler(rotation);

            transform.rotation = r;
            transform.position = destination;

            if (jumping)
            {
                animator.SetBool("Runnig", false);
                animator.SetBool("Walking", false);
                animator.SetBool("Idle", false);
                animator.SetBool("Jumping", true);
            }
            else
            {
                animator.SetBool("Jumping", false);
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
            //Label.text = NickName;
        }        
    }
}
