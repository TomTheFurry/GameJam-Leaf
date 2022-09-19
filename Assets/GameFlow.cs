using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFlow : MonoBehaviour
{
    public float targetTimeSecond = 60;
    public Image uiTimeBar;

    float time;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        uiTimeBar.fillAmount = (targetTimeSecond - time) / targetTimeSecond;
    }
}
