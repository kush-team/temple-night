using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
  Vector3 _CharacterDirection, realativedir;
     Quaternion _CharacterRotation, rot;
     Animator _CharacterAnim;
     Rigidbody _CharacterRigidbody;
     [SerializeField]
     float TurnSpeed = 2, speed = 2;
     bool _IsWalking;
     bool _isRunning;
     bool _isJumping;
     float jumpHeight = 2.5f;     
     float groundDistance = 0.1f;
     [SerializeField]
     public Camera VCam;

 
 
     void Start()
     {
         _CharacterAnim = GetComponent<Animator>();
         _CharacterRigidbody = GetComponent<Rigidbody>(); 
     }
 
     void Update()
     {
         CharacterActions();
     }
 
     void FixedUpdate()
     {
         CharacterMovement();
     }
 
     void CharacterMovement()
     {
         //movement input's
         float h = Input.GetAxis("Horizontal");
         float v = Input.GetAxis("Vertical");
         _CharacterDirection.Set(h, 0, v);
 
         // convert the character dir to cam dir
         realativedir = VCam.transform.TransformDirection(_CharacterDirection);
         realativedir.y = 0f;
         realativedir.Normalize();
 
         //bool movement check
         bool _IsHorizontalChange = !Mathf.Approximately(h, 0f);
         bool _IsVerticalChange = !Mathf.Approximately(v, 0);
         _IsWalking = _IsHorizontalChange || _IsVerticalChange;


         Vector3 _DesairdForward = Vector3.RotateTowards(this.transform.forward, realativedir, TurnSpeed * Time.deltaTime, 0);
         _CharacterRotation = Quaternion.LookRotation(_DesairdForward); 
 
         //rotate character with cam
         rot = VCam.transform.rotation;
         rot.x = 0;
         rot.z = 0;
         transform.rotation = rot;
         
 
         //RigidBody Direction && Rotation
         Vector3 destination = _CharacterRigidbody.position + realativedir * speed * Time.deltaTime;
         _CharacterRigidbody.MovePosition(destination);

         Network.Move(transform.rotation.eulerAngles, transform.position, _IsWalking, _isRunning, _isJumping);

        _CharacterAnim.SetBool("Walking", _IsWalking);
        _CharacterAnim.SetBool("Runnig", _isRunning);
        _CharacterAnim.SetBool("Idle", !_IsWalking);
 
     }
 
     void CharacterActions()
     {

        if (Input.GetKeyUp (KeyCode.E)) 
        {

        }   

        if (Input.GetKey(KeyCode.LeftShift))
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
            _isJumping = true;
            _CharacterRigidbody.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -0.8f * Physics.gravity.y), ForceMode.VelocityChange);
        }
        else
        {
            if (this.IsGrounded()) 
            {
                _isJumping = false;
            }
        } 
     }    

    private bool IsGrounded() {
       return Physics.Raycast(transform.position, -Vector3.up, groundDistance + 0.1f);
    }      
}
