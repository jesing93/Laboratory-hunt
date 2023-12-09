using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Variables
    [Header("Camera sensibility")]
    public float sensX;
    public float sensY;
    private float rotDelay = 0.2f;
    private float xRotation;
    private float yRotation;
    private bool FPSMode;
    private bool isPaused = true;

    //Singletone
    public static CameraController instance;

    private void Awake()
    {
        instance = this;
        //isPaused = false; //TODO: Delete once gameflow is finished
    }

    private void Update()
    {
        float mouseX = 0;
        float mouseY = 0;

        //Get inputs
        if (!isPaused)
        {
            mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
            mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

            yRotation += mouseX;
            xRotation -= mouseY;
            if (FPSMode)
            {
                xRotation = Mathf.Clamp(xRotation, -30f, 23f);

                //Rotate the player horizontally instead of the camera
                //PlayerController.instance.Orientation.rotation = Quaternion.Euler(0, yRotation, 0);
            }
            else
            {
                xRotation = Mathf.Clamp(xRotation, 0f, 45f);
            }
            //Normalize yRotation
            if (yRotation > 360)
            {
                yRotation -= 360;
            }
            else if (yRotation < 0)
            {
                yRotation += 360;
            }

            //Rotate camera vertically and orientations smoothly
            Quaternion targetXRotation = Quaternion.Euler(xRotation, yRotation, 0);
            transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, targetXRotation, Time.deltaTime * 10);

            //Rotation smoothness
            Vector3 targetYRotation = new(0, yRotation, 0);
            //Rotate the player horizontally instead of the camera;
            PlayerController.instance.Orientation.DORotate(targetYRotation, rotDelay, RotateMode.Fast).SetId("CamRot");
        }
    }

    public void SwitchToTPS() {
        transform.localPosition = new Vector3(0.75f, 0.7f, -1.5f);
        rotDelay = 0.2f;
        FPSMode = false;
    }

    public void SwitchToFPS()
    {
        transform.localPosition = new Vector3(0.1f, 0f, 0.1f);
        rotDelay = 0f;
        FPSMode = true;
    }

    public void TogglePause(bool state)
    {
        isPaused = state;
    }
}
