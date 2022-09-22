using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafCollision : MonoBehaviour
{
    public bool isPlayer;
    bool end;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "LandExtra" || other.gameObject.name == "Terrain")
        {
            if (isPlayer)
                GameFlow.GameOver();

            if (!end && !isPlayer)
            {
                GameObject delObj = transform.parent.parent.gameObject;

                transform.SetParent(null);

                GameObject.Destroy(delObj);

                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<Rigidbody>().useGravity = true;

                end = true;
            }
            

        }
    }
}
