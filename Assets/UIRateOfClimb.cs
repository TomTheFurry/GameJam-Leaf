using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRateOfClimb : MonoBehaviour
{
    public Transform objective;//the leaf
    public Text text;
    public float velocityFullRedThreshold = -3;
    public float velocityFullGreenThreshold = -0.1f;

    float velocity;
    float objectiveLastPositionY;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        velocity = (objective.position.y - objectiveLastPositionY) / Time.deltaTime;
        objectiveLastPositionY = objective.position.y;

        text.text = "Rate of Climb :\n" + velocity;
        text.color = Color.LerpUnclamped(Color.green, Color.red, (velocity - velocityFullGreenThreshold) / (velocityFullRedThreshold - velocityFullGreenThreshold));

    }
}
