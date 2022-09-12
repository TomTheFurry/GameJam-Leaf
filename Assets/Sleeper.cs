using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ArticulationBody))]
public class Sleeper : MonoBehaviour
{
    private int sleepCount = 0;

    private void FixedUpdate()
    {
        ArticulationBody body = GetComponent<ArticulationBody>();
        if (body.velocity.magnitude < 0.01f)
        {
            sleepCount++;
            if (sleepCount > 100) Destroy(gameObject.transform.parent.gameObject);
        } else
        {
            sleepCount = 0;
        }
    }
}
