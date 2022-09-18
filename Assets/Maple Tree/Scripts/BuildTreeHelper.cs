using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTreeHelper : MonoBehaviour
{
    [Header("Branch")]
    public GameObject branchPrefab;
    public float bSize = 0.7f;
    public float bSizeElasticity;
    public float bRotateYElasticity;

    [Header("Small Branch")]
    public GameObject smallBranchPrefab;
    public float sbSize = 4f;
    public float sbSizeElasticity;
    public float sbRotateYElasticity;

    [Header("Leaf")]
    public GameObject leafPrefab;
    public float lSize = 4f;
    public float lSizeElasticity;
    public float lRotateYElasticity;

    // Start is called before the first frame update
    void Start()
    {
        CreateBranch(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateBranch(GameObject sourceBranch)
    {
        Transform[] go = sourceBranch.GetComponentsInChildren<Transform>();
        Transform[] slot = new Transform[go.Length-1];
        Array.Copy(go,1, slot,0, go.Length-1);

        foreach (Transform t in slot)
        {
            GameObject branchInst = Instantiate(branchPrefab,t);
            float randomResult = UnityEngine.Random.Range(-bSizeElasticity, bSizeElasticity);
            branchInst.transform.localScale = new Vector3(bSize + randomResult, bSize + randomResult, bSize + randomResult);

            randomResult = UnityEngine.Random.Range(-bRotateYElasticity, bRotateYElasticity);
            branchInst.transform.Rotate(0, randomResult, 0);

            CreateSmallBranch(branchInst);
        }
    }

    void CreateSmallBranch(GameObject sourceBranch)
    {
        Transform[] go = sourceBranch.GetComponentsInChildren<Transform>();
        Transform[] slot = new Transform[go.Length - 1];
        Array.Copy(go, 1, slot, 0, go.Length - 1);

        foreach (Transform t in slot)
        {
            GameObject branchInst = Instantiate(smallBranchPrefab, t);
            float randomResult = UnityEngine.Random.Range(-sbSizeElasticity, sbSizeElasticity);
            branchInst.transform.localScale = new Vector3(sbSize + randomResult, sbSize + randomResult, sbSize + randomResult);

            randomResult = UnityEngine.Random.Range(-sbRotateYElasticity, sbRotateYElasticity);
            branchInst.transform.Rotate(0, randomResult, 0);

            CreateLeaf(branchInst);
        }
    }
    void CreateLeaf(GameObject sourceBranch)
    {
        Transform[] go = sourceBranch.GetComponentsInChildren<Transform>();
        Transform[] slot = new Transform[go.Length - 1];
        Array.Copy(go, 1, slot, 0, go.Length - 1);

        foreach (Transform t in slot)
        {
            GameObject branchInst = Instantiate(leafPrefab, t);
            float randomResult = UnityEngine.Random.Range(-lSizeElasticity, lSizeElasticity);
            branchInst.transform.localScale = new Vector3(lSize + randomResult, lSize + randomResult, lSize + randomResult);

            randomResult = UnityEngine.Random.Range(-lRotateYElasticity, lRotateYElasticity);
            branchInst.transform.Rotate(180, randomResult, 0);

        }
    }

}
