using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public float moveTime; //time to get to full speed walk
    public float gravityStr; 
    public float jumpStr; 
    public float walkSpeed;
    public float runSpeed;

    private bool isSprint = false;
    private bool isGround = false;
    private bool isWallRun = false;
    private float momentumX = 1.0f; //x in ylog_10 x graph
    private float momentumLog = 0.0f; //output of graph
    public float momentumMax;
    public float momentumGain; //rate of momentum gain
    public float sprintSlowDown; //rate of momentum loss
    public float sprintFlatBoost; //imediete sprint speed increase
    public float wallJumpScale;
    public float wallJumpDecay; 
    private Vector3 isWallJump = new Vector3(1f,1f,1f);
    private float wallJumpMoveTime;
    public float wallJumpMoveTimeSet;
    public float wallRunGravityPercent;

    public DebugLogs log;
    public PlayerWalls wall;

    private CharacterController controller;
    private Vector3 currentMoveVelocity;
    private Vector3 wallJumpVelocity;
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

        if(wallJumpMoveTime>0){
            wallJumpMoveTime -= Time.deltaTime;
            playerInput.x *= isWallJump.x;
            playerInput.z *= isWallJump.z;
        }

        if(playerInput.magnitude > 1f){
            playerInput.Normalize();
        }

        Vector3 moveVector = transform.TransformDirection(playerInput);
        isSprint = Input.GetKey(KeyCode.LeftShift) && isGround;
        if(isSprint && momentumX < momentumMax){
            momentumX += momentumGain * Time.deltaTime;
        }else if (momentumX > 1.0f){
            if(isGround){
            momentumX -= momentumGain * Time.deltaTime * sprintSlowDown;
            }
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
            isGround = true;
        }else{
            isGround = false;
        }

        //finds if wallRun
        //*eventually set wallRun to false is direction is swapped and slightly push player towards wall
        isWallRun = false;
        if(!isGround){
            if(wall.GetXWall() != 0 && (rb.velocity.z > 4f || rb.velocity.z < -4f)){
                isWallRun = true;
            }else if(wall.GetZWall() != 0 && (rb.velocity.x > 4f || rb.velocity.x < -4f)){
                isWallRun = true;
            }
        }

        //Jump code
        if(isGround){
            currentForceVelocity.y = -2;

            if(Input.GetKey(KeyCode.Space)){
                currentForceVelocity.y = jumpStr + (momentumLog/4);
                if(isSprint){
                    currentForceVelocity.y += 1;
                }
            }
        }else if(wall.GetXWall() != 0.0f || wall.GetZWall() != 0.0f){ //Wall jump *Make wall jump push in run dirrection during wall run

            if(wallJumpMoveTime <= 0){
                isWallJump = new Vector3(1f,1f,1f);
                if(wall.GetXWall() != 0){
                    isWallJump.z = 0f;
                }
                if(wall.GetZWall() != 0){
                    isWallJump.x = 0f;
                }
            }

            if(isWallRun){ //wall Run
                currentForceVelocity.y -= gravityStr * Time.deltaTime * wallRunGravityPercent;
            }else{
                currentForceVelocity.y -= gravityStr * Time.deltaTime;
            }
            if(Input.GetKeyDown(KeyCode.Space)){


                currentForceVelocity.y = jumpStr + (momentumLog/4);
                if(isSprint){
                    currentForceVelocity.y += 1;
                }
                wallJumpVelocity = new Vector3(wall.GetXWall(), 0.0f, wall.GetZWall()) * -wallJumpScale;
                wallJumpMoveTime = wallJumpMoveTimeSet;

                momentumX = 1f; //*improve wall jump sprint later

            }

        }else{
            currentForceVelocity.y -= gravityStr * Time.deltaTime;
        }

        if(wallJumpVelocity.x > .1f || wallJumpVelocity.x < -.1f || wallJumpVelocity.z > .1f || wallJumpVelocity.z < -.1f){
            wallJumpVelocity *= 1 - wallJumpDecay;
        } else{
            wallJumpVelocity = new Vector3(0f,0f,0f);
        }
        //controller.Move(currentForceVelocity * Time.deltaTime);
        finalVelocity = currentMoveVelocity + currentForceVelocity + wallJumpVelocity;
        Debug.Log(currentForceVelocity.y);
        rb.velocity = finalVelocity;

        //DEBUGS
        log.DebugText("isGround", isGround.ToString());
        log.DebugText("velocity", (finalVelocity).ToString());
        log.DebugText("momentumX", momentumX.ToString());
        log.DebugText("momentumLog", momentumLog.ToString());
        log.DebugText("isSprint", isSprint.ToString());
        log.DebugText("wallJumpTime", wallJumpMoveTime.ToString());
        log.DebugText("wallRun", isWallRun.ToString());


       
    }
}
