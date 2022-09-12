using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindChanger : MonoBehaviour
{
    public Vector3 maxWindSpeed = new Vector3(10, 1, 10);
    public float perlinSpeed = 1f;
    public Vector3 currentWind = Vector3.zero;

    private static double randDoubleFullRange()
    {
        return Random.Range(-1000000f, 1000000f);
    }

    // Use 3 perlin noise to generate a wind speed and direction
    private double perlinOffsetX;
    private double perlinOffsetY;
    private double perlinOffsetZ;

    private void Start()
    {
        perlinOffsetX = randDoubleFullRange();
        perlinOffsetY = randDoubleFullRange();
        perlinOffsetZ = randDoubleFullRange();
    }

    private Vector3 getWindSpeed()
    {
        float x = Mathf.PerlinNoise((float)(perlinOffsetX + Time.time * perlinSpeed), 0);
        float y = Mathf.PerlinNoise((float)(perlinOffsetY + Time.time * perlinSpeed), 0);
        float z = Mathf.PerlinNoise((float)(perlinOffsetZ + Time.time * perlinSpeed), 0);
        x = (x - 0.5f) * 2f;
        y = (y - 0.5f) * 2f;
        z = (z - 0.5f) * 2f;
        Vector3 windSpeed = new Vector3(x, y, z);
        windSpeed.Scale(maxWindSpeed);
        return windSpeed;
    }
    
    void FixedUpdate()
    {
        currentWind = getWindSpeed();
        plateMesh.wind = currentWind;
    }
}
