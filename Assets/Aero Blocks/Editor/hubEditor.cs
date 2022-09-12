using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
[CustomEditor (typeof(bladeHub))]
public class hubEditor : Editor{


	bladeHub thisHub;
	Rigidbody hubRB;

	void OnEnable(){
		thisHub = (bladeHub)target;
		hubRB = thisHub.GetComponentInChildren<Rigidbody> ();
		if (!Application.isPlaying)
			Initialise ();
	}

	void Initialise(){
		if (thisHub.plate != null) {

			clearPlatesInList ();
			removeJoints ();
			populateList ();
			setPlatePositions ();
			joinPlatesTogether ();

		}
	}

	bool checkListIsCorrectLength(){
		
		if (thisHub.plates.Count == thisHub.numberOfBlades){
			for (int i = 0; i < thisHub.plates.Count; i++) {
				if (thisHub.plates [i] == null)
					return false;
			}
			return true;
		} else {
			return false;
		}

	}

	void clearPlatesInList(){

		if (thisHub.plates != null) {
			if (thisHub.plates.Count > 1) {
				if (thisHub.plate != thisHub.plates [0]) {
					if (thisHub.plates [0] != null)
						Editor.DestroyImmediate (thisHub.plates [0].gameObject);
				}
			}

			for (int i = 1; i < thisHub.plates.Count; i++) {
				if (thisHub.plates [i] != null)
					Editor.DestroyImmediate (thisHub.plates [i].gameObject);
			}
		}

	}

	void populateList(){
		thisHub.plates = new List<plateMesh> (thisHub.numberOfBlades);
		thisHub.plates.Add (thisHub.plate);
		for (int i = 1; i < thisHub.numberOfBlades; i++) {
			thisHub.plates.Add (Instantiate (thisHub.plate.gameObject).GetComponent<plateMesh>());
		}
	}

	void setPlatePositions(){

		float angle = Mathf.PI * 2f / thisHub.numberOfBlades;
		float xRadius = 0.5f * hubRB.transform.localScale.x;
		float zRadius = 0.5f * hubRB.transform.localScale.z;

		// If the blade is twisted it will have a slightly larger width due to its thickness, imagine a square - it would be wider corner to corner than edge to edge.
		float extraBitFromThickness = Mathf.Abs( 0.5f * thisHub.plate.thicknessToChordRatio * thisHub.plate.leftChord * Mathf.Sin (thisHub.plate.transform.localEulerAngles.x * Mathf.Deg2Rad));

		for (int i = 0; i < thisHub.plates.Count; i++) {

			float currentAngle = i * angle;
			float cosTheta = Mathf.Cos (currentAngle);
			float sinTheta = Mathf.Sin (currentAngle);

			float polarR;
			if (zRadius == xRadius) {
				polarR = Mathf.Max((thisHub.plate.leftChord + extraBitFromThickness) / (2 * Mathf.Tan (Mathf.PI / thisHub.plates.Count)),
				(xRadius * zRadius) / (Mathf.Sqrt (zRadius * zRadius * cosTheta * cosTheta + xRadius * xRadius * sinTheta * sinTheta)));

			} else {
				polarR = (xRadius * zRadius) /
				(Mathf.Sqrt (zRadius * zRadius * cosTheta * cosTheta + xRadius * xRadius * sinTheta * sinTheta));
			}

			float X = polarR * cosTheta;
			float Z = polarR * sinTheta;

			// Position
			thisHub.plates [i].transform.SetParent (thisHub.transform);
			Vector3 pointOnEdge = new Vector3 (X / thisHub.transform.localScale.x, 0, Z / thisHub.transform.localScale.z);
			thisHub.plates [i].transform.localPosition = pointOnEdge;

			// Rotation
			Quaternion rotation = Quaternion.LookRotation (pointOnEdge);
			rotation *= Quaternion.Euler (0, -90, 0);
			rotation *= thisHub.plate.transform.localRotation;
			thisHub.plates [i].transform.localRotation = rotation;

			// Put left face onto edge of hub
			thisHub.plates [i].transform.position = thisHub.plates [i].transform.TransformPoint (-meshEditTools.leftFaceCentreLocalCoords (thisHub.plates [i]) + Vector3.right * thisHub.distanceFromHub);

			FixedJoint[] pj = thisHub.plates [i].GetComponents<FixedJoint> ();

			if (pj.Count () == 0) {
				thisHub.plates [i].gameObject.AddComponent<FixedJoint> ().connectedBody = hubRB;
			} else {

				bool found = false;

				for (int j = 0; j < pj.Count (); j++) {
					if (pj [j].connectedBody == hubRB) {// already one
						found = true;
						break;
					}
				}
				if (!found)
					thisHub.plates [i].gameObject.AddComponent<FixedJoint> ().connectedBody = hubRB;
			}
		}
	}

	void joinPlatesTogether(){

		for (int i = 1; i < thisHub.plates.Count; i++) {
			thisHub.plate.gameObject.AddComponent<FixedJoint> ().connectedBody = thisHub.plates [i].rigidBody;
		}
	}

	void removeJoints(){
		FixedJoint[] pj = thisHub.plate.GetComponents<FixedJoint> ();

		if (pj.Count () != 0) {
			for (int j = 0; j < pj.Count (); j++) {
				if (pj [j].connectedBody != hubRB) {// already one
					Editor.DestroyImmediate(pj[j]);
				}
			}
		}
	}



	public override void OnInspectorGUI(){
		DrawDefaultInspector ();

		if(GUILayout.Button("Update")){
			clearPlatesInList ();
			Initialise ();
		}
	}
	
}
