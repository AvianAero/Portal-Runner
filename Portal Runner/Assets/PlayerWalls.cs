using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalls : MonoBehaviour
{

    private Vector3 wallDistance;
    float xWall;
    float zWall;
    public DebugLogs log;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        xWall = XWallCheck();
        zWall = ZWallCheck();
        log.DebugText("walls", ("X : " + xWall + " | Y : " + zWall).ToString());
    }

   public  float GetXWall(){
        return xWall;
    }

    public float GetZWall(){
        return zWall;
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
