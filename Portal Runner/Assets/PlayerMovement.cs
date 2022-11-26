using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public float moveTime;
    public float gravityStr;
    public float jumpStr;
    public float walkSpeed;
    public float runSpeed;

    private bool isSprint = false;
    private float momentumX = 1.0f;
    private float momentumLog = 0.0f;
    public float momentumMax;
    public float momentumGain;
    public float sprintSlowDown;
    public float sprintFlatBoost;

    public DebugLogs log;

    private CharacterController controller;
    private Vector3 currentMoveVelocity;
    private Vector3 moveDampVelocity;
    private Vector3 finalVelocity;

    private Vector3 currentForceVelocity;

    private Rigidbody rb;

    void Start()
    {
        //controller = GetComponent<CharacterController>();
        rb =  GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 playerInput = new Vector3{
            x = Input.GetAxisRaw("Horizontal"),
            y = 0f,
            z = Input.GetAxisRaw("Vertical")
        };

        if(playerInput.magnitude > 1f){
            playerInput.Normalize();
        }

        Vector3 moveVector = transform.TransformDirection(playerInput);
        isSprint = Input.GetKey(KeyCode.LeftShift);
        if(isSprint && momentumX < momentumMax){
            momentumX += momentumGain * Time.deltaTime;
        }else if (momentumX > 1.0f){
            momentumX -= momentumGain * Time.deltaTime * sprintSlowDown;
        } else {
            momentumX = 1.0f;
        }
        momentumLog = momentumMax * Mathf.Log(momentumX, 10);


        float currentSpeed = walkSpeed + momentumLog;
        if(isSprint){
            currentSpeed += sprintFlatBoost;
        }

        currentMoveVelocity = Vector3.SmoothDamp(
            currentMoveVelocity,
            moveVector * currentSpeed,
            ref moveDampVelocity,
            moveTime
        );

        
        //controller.Move(currentMoveVelocity * Time.deltaTime);

        Ray groundCheckRay = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(groundCheckRay, 1.1f)){
            log.DebugText("isGround", "true");
            currentForceVelocity.y = -2;

            if(Input.GetKey(KeyCode.Space)){
                currentForceVelocity.y = jumpStr + (momentumLog/4);
                if(isSprint){
                    currentForceVelocity.y += 1;
                }
            }
        }else{
            currentForceVelocity.y -= gravityStr * Time.deltaTime;
            log.DebugText("isGround", "false");
        }
        //controller.Move(currentForceVelocity * Time.deltaTime);

        finalVelocity = currentMoveVelocity + currentForceVelocity;

        rb.velocity = finalVelocity;

        //DEBUGS
        log.DebugText("velocity", (finalVelocity).ToString());
        log.DebugText("momentumX", momentumX.ToString());
        log.DebugText("momentumLog", momentumLog.ToString());
        log.DebugText("isSprint", isSprint.ToString());


       
    }
}
