using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Movement : MonoBehaviour
{

    public float speed = 3f;
    public float rSpeed = 400f;
    private float momentum = 1f;
    public float momentumCap = 5f;
    private float roatoate = 0f;
    private float rotyateCam = 0f;
    public float jump = 5f;
    private bool isGrounded;
    public Rigidbody rb;
    public float wallPush = 3f;
    
    public Transform camera;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
            //Get Movement in relation to rotation
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector3 movement = new Vector3(h, 0f, v);
            movement = movement.normalized * speed * Time.deltaTime;
            movement = transform.worldToLocalMatrix.inverse * movement;
            //end Movement

            //Increase/Decrease momentum
            if(Input.GetAxis("Vertical") == 1 && momentum < momentumCap && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))){
                momentum *= 1.005f;
            }else if(Input.GetAxis("Vertical") <= 0 && momentum > 1f){
                momentum *= .995f;
            }
            //momentum end


            //camera rotation code
            roatoate=Input.GetAxis("Mouse X");

            float Rotation;
            if(camera.eulerAngles.x <= 180f)
            {
                Rotation = camera.eulerAngles.x;
            }
            else
            {
                Rotation = camera.eulerAngles.x - 360f;
            }
            rotyateCam=Input.GetAxis("Mouse Y") * Time.deltaTime * -rSpeed;
            if(Rotation > 85f && rotyateCam > 0){
                rotyateCam = 0f;
            }else if(Rotation < -85f && rotyateCam < 0){
                rotyateCam = 0f;
            }
            //Camera rotation end

            //jump code
            GroundCheck();
            if(Input.GetKeyDown(KeyCode.Space) && isGrounded){
                rb.velocity = new Vector3(0, jump + momentum / 4f, 0);
            }
            float xWall = XWallCheck();
            float zWall = ZWallCheck();
            if(Input.GetKeyDown(KeyCode.Space) && !isGrounded && (xWall != 0 || zWall != 0)){
                rb.velocity = new Vector3(-xWall * wallPush, jump + momentum / 4f, -zWall * wallPush);
            }
            //jump end

            Debug.Log("Shmovemnt: " + movement);
            Debug.Log("Momentum:"  + momentum);
            Debug.Log("Rotate: " + roatoate);
            //Debug.Log("Rotate Cam: " + rotyateCam + " : " + Rotation);
            //Debug.Log("Grounded: " + isGrounded);
            //Debug.Log("Wall:" + xWall + " : " + zWall);
            rb.velocity += (movement * Time.deltaTime * speed * momentum);
            transform.Rotate(0f, roatoate * Time.deltaTime * rSpeed, 0f);
            camera.transform.Rotate(rotyateCam, 0f, 0f);
    }

    void GroundCheck()
    {
        RaycastHit hit;
        float distance = 1.3f;
        Vector3 dir = new Vector3(0, -1, 0);

        if(Physics.Raycast(transform.position, dir, out hit, distance))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    float XWallCheck()
    {
        RaycastHit hit;
        float distance = 1f;
        Vector3 dir = new Vector3(1, 0, 0);

        if(Physics.Raycast(transform.position, dir, out hit, distance))
        {
            return 1;
        }else if(Physics.Raycast(transform.position, -dir, out hit, distance))
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    float ZWallCheck()
    {
        RaycastHit hit;
        float distance = 1f;
        Vector3 dir = new Vector3(0, 0, 1);

        if(Physics.Raycast(transform.position, dir, out hit, distance))
        {
            return 1;
        }else if(Physics.Raycast(transform.position, -dir, out hit, distance))
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

}
