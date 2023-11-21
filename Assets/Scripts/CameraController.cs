using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Variables
    [Header("Camera sensibility")]
    public float sensX;
    public float sensY;
    private float xRotation;
    private float yRotation;

    private void Update()
    {
        //Get inputs
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Rotate camera and orientations smoothly
        Quaternion targetXRotation = Quaternion.Euler(xRotation, yRotation, 0);
        transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, targetXRotation, Time.deltaTime * 10);

        //Quaternion targetYRotation = Quaternion.Euler(0, yRotation, 0);
        //PlayerController.instance.Orientation.rotation = Quaternion.Lerp(PlayerController.instance.Orientation.rotation, targetYRotation, Time.deltaTime * 5);

        //Rotate the player instead of the camera
        PlayerController.instance.Orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
