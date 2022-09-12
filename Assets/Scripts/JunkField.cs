using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JunkField : MonoBehaviour
{
    public int junkCount = 0;
    public float spawnCooldown = 0.1f;
    public Transform container;
    public GameObject sampleObj;

    // the area of 'life' area
    public BoxCollider lifeArea;
    // the bounds of speed
    public Bounds speedBound;

    private void SpawnObj()
    {
        Vector3 pos = new Vector3(
            Random.Range(lifeArea.bounds.min.x, lifeArea.bounds.max.x),
            Random.Range(lifeArea.bounds.min.y, lifeArea.bounds.max.y),
            Random.Range(lifeArea.bounds.min.z, lifeArea.bounds.max.z));
        
        
        Quaternion quat = Quaternion.Euler(
            Random.Range(0, 360),
            Random.Range(0, 360),
            Random.Range(0, 360));

        GameObject obj = Instantiate(sampleObj, container);
        obj.transform.position = pos;
        obj.transform.rotation = quat;
        obj.SetActive(true);
    }

    private float previousSpawnTime = 0;

    private void LateUpdate()
    {
        if (Time.time - previousSpawnTime > spawnCooldown && container.childCount < junkCount)
        {
            previousSpawnTime = Time.time;
            SpawnObj();
        }
    }
}