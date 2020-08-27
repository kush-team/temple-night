using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
  Vector3 _CharacterDirection, realativedir;
     Quaternion _CharacterRotation, rot;
     Animator _CharacterAnim;
     Rigidbody _CharacterRigidbody;
     NetworkEntity _netWorkEntity;
     [SerializeField]
     float TurnSpeed = 10, speed = 2;
     bool _IsWalking;
     bool _IsHitting;
     bool _isRunning;
     bool _isJumping;
     float jumpHeight = 2.5f;     
     float groundDistance = 0.5f;
     float distance = 0.00f;
     [SerializeField]
     public Camera VCam;


     private GameObject proximatePlayer;
 

     private bool coolingdown = false;
     private float cooldownCounter = 3f;

     private bool coolingdownFreeze = false;
     private float cooldownCounterFreeze = 3f;
 
     void Start()
     {
         _CharacterAnim = GetComponent<Animator>();
         _CharacterRigidbody = GetComponent<Rigidbody>(); 
         _netWorkEntity = GetComponent<NetworkEntity>(); 
     }
 
     void Update()
     {
        if (!_netWorkEntity.isDead())
            CharacterActions();
     }
 
     void FixedUpdate()
     {
        if (!_netWorkEntity.isDead() && !coolingdownFreeze)
            CharacterMovement();
     }
 
     void CharacterMovement()
     {

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        _CharacterDirection.Set(h, 0, v);
        realativedir = VCam.transform.TransformDirection(_CharacterDirection);
        realativedir.y = 0f;
        realativedir.Normalize();

        bool _IsHorizontalChange = !Mathf.Approximately(h, 0f);
        bool _IsVerticalChange = !Mathf.Approximately(v, 0);
        _IsWalking = _IsHorizontalChange || _IsVerticalChange;

        Vector3 _DesairdForward = Vector3.RotateTowards(this.transform.forward, realativedir, TurnSpeed * Time.deltaTime, 0);
        _CharacterRotation = Quaternion.LookRotation(_DesairdForward); 

        rot = VCam.transform.rotation;
        rot.x = 0;
        rot.z = 0;
        transform.rotation = rot;


        Vector3 destination = _CharacterRigidbody.position + realativedir * speed * Time.deltaTime;
        distance = Vector3.Distance(destination, _CharacterRigidbody.position);

        _CharacterRigidbody.MovePosition(destination);

        Network.Move(transform.rotation.eulerAngles, destination, _IsHitting, _isRunning, _isJumping);

        _CharacterAnim.SetFloat("Distance", distance);
        _CharacterAnim.SetBool("Runnig", _isRunning);

     }
 
     void CharacterActions()
     {
 
        if ((Input.GetKey(KeyCode.F) ||  Input.GetKey("joystick 1 button 3")) && this.IsGrounded())
        {
            if (!coolingdown) 
            {
                _CharacterAnim.SetTrigger("HItting");
                _IsHitting = true;
                coolingdown = true;
                cooldownCounter = 3f;

                if (proximatePlayer)
                {
                    Network.Hit(proximatePlayer.GetComponent<NetworkEntity>().id);
                } 

            }
        } 
        else 
        {
            _IsHitting = false;
        }

        cooldownCounter -= Time.deltaTime;

        if (cooldownCounter <= 0) 
        {
            coolingdown = false;
        }


        cooldownCounterFreeze -= Time.deltaTime;

        if (cooldownCounterFreeze <= 0) 
        {
            coolingdownFreeze = false;
        }        

        if (Input.GetKey(KeyCode.LeftShift) ||  Input.GetKey("joystick 1 button 7"))
        {
            speed = 4;
            _isRunning = true;
        }
        else
        {
            speed = 2;
            _isRunning = false;
        }        

        if (Input.GetButtonDown("Jump") && this.IsGrounded())
        {
            _CharacterRigidbody.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -1f * Physics.gravity.y), ForceMode.VelocityChange);
            _CharacterAnim.SetTrigger("Jumping");
            _isJumping = true;
        }
        else
        {
            if (this.IsGrounded()) 
            {
                _isJumping = false;
            }
        } 
     }    




    public void Hit()
    {
        _CharacterAnim.SetTrigger("Hitted");
        coolingdownFreeze = true;
        cooldownCounterFreeze = 3f;        
    }


    public void Die()
    {
        _CharacterRigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ |  RigidbodyConstraints.FreezeRotationX;
        _CharacterAnim.SetTrigger("Die");
        _CharacterAnim.SetBool("Runnig", false);
        _CharacterAnim.SetFloat("Distance", 0f);

    }

    public void OnCollisionEnter(Collision collision)
    {
        if (_netWorkEntity.isBoss())
        {
            GameObject player = collision.gameObject;
            if (player && player.GetComponent<NetworkEntity>())
            {
                proximatePlayer = player;
            }
            else
            {
                proximatePlayer = null;
            }
        }

    }


    private bool IsGrounded() 
    {
       return Physics.Raycast(transform.position, -Vector3.up, groundDistance + 0.1f);
    }      
}
