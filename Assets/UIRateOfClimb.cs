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
    public GameObject pullUpText;

    float velocityY;
    float objectiveLastPositionY;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        velocityY = (objective.position.y - objectiveLastPositionY) / Time.deltaTime;
        objectiveLastPositionY = objective.position.y;

        text.text = "Rate of Climb\n" + velocityY.ToString("0");
        text.color = Color.LerpUnclamped(Color.green, Color.red, (velocityY - velocityFullGreenThreshold) / (velocityFullRedThreshold - velocityFullGreenThreshold));

        if(velocityY < velocityFullRedThreshold && !pullUpText.activeSelf)
            pullUpText.SetActive(true);
        else if (velocityY > velocityFullRedThreshold && pullUpText.activeSelf)
            pullUpText.SetActive(false);


    }
}
