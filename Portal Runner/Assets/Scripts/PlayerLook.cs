using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{

    public Transform playerCamera;
    public Vector2 sensitivity;
    private Vector2 XYRotation;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouseInput = new Vector2{
            x = Input.GetAxis("Mouse X"),
            y = Input.GetAxis("Mouse Y")
        };

        XYRotation.x -= mouseInput.y * sensitivity.y;
        XYRotation.y += mouseInput.x * sensitivity.x;

        XYRotation.x = Mathf.Clamp(XYRotation.x, -90f, 90f);

        transform.eulerAngles = new Vector3(0f, XYRotation.y, 0f);
        playerCamera.localEulerAngles = new Vector3(XYRotation.x, 0f, 0f);
    }
}
