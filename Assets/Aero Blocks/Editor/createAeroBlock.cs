using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[System.Serializable]
public class createAeroBlock : Editor {

	//public static GameObject hub;

	//[MenuItem("Tools/Aero Blocks/New Hub")]
	public static void CreateHub(){
		GameObject go = (GameObject)Instantiate (Resources.Load("Hub", typeof(GameObject)), Vector3.zero, Quaternion.identity);
		go.name = "Hub";
		Selection.activeObject = go;
		Undo.RegisterCreatedObjectUndo (go, "Create Hub Block");

	}

	//[MenuItem("Tools/Aero Blocks/New Block")]
	public static void CreateBlock(){

		GameObject main = new GameObject ("Aero Block");

		Undo.RegisterCreatedObjectUndo (main, "Create Aero Block");

		main.transform.position = Vector3.zero;

		plateMesh plate = main.AddComponent<plateMesh> ();

		plate.joinedPlates  = new List<joinedPlate>();

		plate.verts = new List<Vector3> () {
			new Vector3 (-0.5f, -plate.thicknessToChordRatio / 2, 0.5f),										// 0 - bottom left
			new Vector3 (-0.5f, -plate.thicknessToChordRatio / 2, -0.5f + plate.controlFlapChordRatio),			// 1 - bottom left
			new Vector3 (-0.5f, -plate.thicknessToChordRatio / 2, -0.5f),										// 2 - bottom left
			new Vector3 (-0.5f, plate.thicknessToChordRatio / 2, 0.5f),											// 3 - top left
			new Vector3 (-0.5f, plate.thicknessToChordRatio / 2, -0.5f + plate.controlFlapChordRatio),			// 4 - top left
			new Vector3 (-0.5f, plate.thicknessToChordRatio / 2, -0.5f),										// 5 - top left
			new Vector3 (0.5f, -plate.thicknessToChordRatio / 2, 0.5f),										// 6 - bottom right
			new Vector3 (0.5f, -plate.thicknessToChordRatio / 2, -0.5f + plate.controlFlapChordRatio),			// 7 - bottom right
			new Vector3 (0.5f, -plate.thicknessToChordRatio / 2, -0.5f),										// 8 - bottom right
			new Vector3 (0.5f, plate.thicknessToChordRatio / 2, 0.5f),											// 9 - top right
			new Vector3 (0.5f, plate.thicknessToChordRatio / 2, -0.5f + plate.controlFlapChordRatio),			// 10 - top right
			new Vector3 (0.5f, plate.thicknessToChordRatio / 2, -0.5f),										// 11 - top right
		};

		// create meshes
		plate.mesh = new Mesh();

		List<Vector3> meshVerts = new List<Vector3>();

		for (int j = 0; j < 3; j++) {
			for (int i = 0; i < plate.verts.Count; i++) {
				meshVerts.Add (plate.verts [i]);
			}
		}


		plate.mesh.vertices = meshVerts.ToArray();


		int[] triangles = {
			3, 1, 0, 	//  left face
			3, 4, 1, 	//
			4, 2, 1,	//
			4, 5, 2, 	//
			6, 7, 9, 	// right face
			7, 10, 9, 	//
			7, 8, 10, 	//
			8, 11, 10, 	//
			21, 16, 15, // top face
			21, 22, 16,	//
			22, 17, 16, //
			22, 23, 17, //
			12, 13, 18,	// bottom face
			13, 19, 18,	//
			13, 14, 19,	//
			14, 20, 19,	//
			30, 33, 24,	// front face
			33, 27, 24,	//
			35, 32, 26,	// back face
			29, 35, 26,	//
		};

		plate.mesh.triangles = triangles;

		main.AddComponent<MeshFilter> ().mesh = plate.mesh;

		main.AddComponent<MeshRenderer> ().material = Resources.Load("orange",typeof(Material)) as Material;

		plate.rigidBody = main.AddComponent<Rigidbody> ();

		plate.rigidBody.angularDrag = 0;

		plate.meshFilter = main.GetComponent<MeshFilter> ();

		plate.meshFilter.sharedMesh.RecalculateNormals ();
		plate.meshFilter.sharedMesh.RecalculateBounds ();
		plate.meshFilter.sharedMesh.RecalculateTangents ();

		plate.meshCollider = main.AddComponent<MeshCollider> ();
		plate.meshCollider.convex = true;

		Selection.activeObject = main;

	}

}
