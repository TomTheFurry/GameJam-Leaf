using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoInactive : MonoBehaviour
{
    public float CountDown;

    private void Update()
    {
        CountDown -= Time.deltaTime;
        if (CountDown <= 0)
            gameObject.SetActive(false);
    }
}
