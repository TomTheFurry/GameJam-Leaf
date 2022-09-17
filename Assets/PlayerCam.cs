using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    public Vector2 camAngle = new Vector2(90, 0); // x = yaw (left/right), y = pitch (up/down)
    public float camDistance = 10;
    public Transform target;

    public InputActionReference camMovement;
    public InputActionReference camZoom;

    private void OnInputCamMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        input.y = -input.y; // invert y axis
        input *= 0.1f; // scale down input
        camAngle += input;
        // clamp pitch to [-90, 90]
        camAngle.y = Mathf.Clamp(camAngle.y, -90, 90);
        // wrap yaw to [-180, 180]
        camAngle.x = Mathf.Repeat(camAngle.x, 360);
    }
    private void OnInputCamZoom(InputAction.CallbackContext context)
    {
        float input = -context.ReadValue<Vector2>().y;
        camDistance += input / 100f;
        camDistance = Mathf.Clamp(camDistance, 1, 100);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            camMovement.action.Enable();
            camZoom.action.Enable();
        }
        else
        {
            camMovement.action.Disable();
            camZoom.action.Disable();
        }
    }

    void Start()
    {
        camMovement.action.performed += OnInputCamMove;
        camZoom.action.performed += OnInputCamZoom;

        // Disable mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDestroy()
    {
        camMovement.action.performed -= OnInputCamMove;
        camZoom.action.performed -= OnInputCamZoom;

        // Enable mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    

    private void OnPreRender()
    {
        float targetYaw = 0;//target.eulerAngles.y;
        transform.position = target.position + Quaternion.Euler(camAngle.y, camAngle.x + targetYaw, 0) * Vector3.back * camDistance;
        transform.LookAt(target);
    }
}
