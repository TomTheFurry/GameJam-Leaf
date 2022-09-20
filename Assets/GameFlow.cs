using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFlow : MonoBehaviour
{
    public Transform playerLeaf;

    [Header("Time")]
    public float targetTimeSecond = 60;
    public Image uiTimeBar;

    float time;


    [Header("Time")]
    public RectTransform rulerPointer;
    public float startY = 0;
    public float endY =-100;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        //display the time bar
        uiTimeBar.fillAmount = (targetTimeSecond - time) / targetTimeSecond;

        //display player`s leaf Height
        rulerPointer.anchoredPosition = new Vector3(rulerPointer.anchoredPosition.x, Mathf.LerpUnclamped(1030, 50, (playerLeaf.position.y - startY) / (endY - startY)));

    }
}
