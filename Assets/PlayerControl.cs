using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public Rigidbody[] fourSides; // 0 = front, 1 = back, 2 = left, 3 = right
    public ArticulationBody mainTransform;
    public InputActionReference inputMove;
    public InputActionReference inputRotate;
    public float force = 10f;
    public float rotationForce = 10f;

    [Header("Particle and animation")]
    public PlayerLeafParticle leafParticle;

    void Start()
    {
        if (fourSides.Length != 4)
        {
            Debug.LogError("fourSides array must have 4 elements");
        }
    }

    private void Update()
    {
        var value = inputMove.action.ReadValue<Vector2>();
        value.x = -value.x;
        
        var rotation = inputRotate.action.ReadValue<float>();
        Debug.Log("Rotation: " + rotation);
        // Fix the rotation to be relative to the camera
        // Basically, if target is upside down, rotate the other way

        // First, calculate the difference between cam and target rotation
        var camRotation = transform.rotation;
        var targetRotation = mainTransform.transform.rotation;
        var diff = targetRotation * Quaternion.Inverse(camRotation);

        // Then compute what the up vector would be after the rotation
        var up = diff * Vector3.up;
        // If the up vector is pointing down, then the target is upside down
        var isUpsideDown = up.y < 0;
        // If the target is upside down, then we need to invert the rotation
        if (isUpsideDown)
        {
            rotation = -rotation;
            //value = -value;
        }


        if (value.y > 0)
        {
            fourSides[0].AddRelativeForce(Vector3.up * force, ForceMode.Force);
            fourSides[1].AddRelativeForce(Vector3.down * force, ForceMode.Force);

            leafParticle.SetLeafParticleOn(0);
        }
        else if (value.y < 0)
        {
            fourSides[0].AddRelativeForce(Vector3.down * force, ForceMode.Force);
            fourSides[1].AddRelativeForce(Vector3.up * force, ForceMode.Force);

            leafParticle.SetLeafParticleOn(2);
        }
        
        if (value.x > 0)
        {
            fourSides[2].AddRelativeForce(Vector3.up * force, ForceMode.Force);
            fourSides[3].AddRelativeForce(Vector3.down * force, ForceMode.Force);

            leafParticle.SetLeafParticleOn(1);
        }
        else if (value.x < 0)
        {
            fourSides[2].AddRelativeForce(Vector3.down * force, ForceMode.Force);
            fourSides[3].AddRelativeForce(Vector3.up * force, ForceMode.Force);

            leafParticle.SetLeafParticleOn(3);
        }

        // Finally do the rotation
        if (rotation > 0) // Clockwise
        {
            mainTransform.AddRelativeTorque(Vector3.up * rotationForce);

        }
        else if (rotation < 0)
        {
            mainTransform.AddRelativeTorque(Vector3.down * rotationForce);
        }
    }
}
