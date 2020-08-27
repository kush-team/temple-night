using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggleable : MonoBehaviour
{
	private bool toggle = false;


	private Animator animator;

	private bool _IsPlayerNear;


    private bool coolingdown = false;
    private float cooldownCounter = 3f;


	public void Start()
	{
		animator = this.GetComponent<Animator>();
	}


	public void Update()
	{

		if (_IsPlayerNear) 
		{
	        if (Input.GetKey(KeyCode.E) ||  Input.GetKey("joystick 1 button 2"))
	        {
	            if (_IsPlayerNear)
	            {
	                if (!coolingdown) 
	                {
						Network.Toggle(this.name, this.toggle);
	                } 
	            }
	        }
		}

        cooldownCounter -= Time.deltaTime;
        if (cooldownCounter <= 0) 
        {
            coolingdown = false;
        }		
	
	}


	public void FixedUpdate()
	{
	
	}

	public void SetToggle(bool toggle)
	{
		this.toggle = toggle;
		animator.SetBool("open", toggle);
        coolingdown = true;
        cooldownCounter = 3f;		
	}


	public void OnCollisionEnter (Collision collision)
	{
		if (collision.gameObject.name == "Me") 
		{
			_IsPlayerNear = true;
		}
    }

    public void OnCollisionExit (Collision collision) 
    {
		if (collision.gameObject.name == "Me") 
		{
			_IsPlayerNear = false;
		}    	
    }
}
