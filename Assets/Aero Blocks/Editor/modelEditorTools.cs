using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(model))]
public class modelEditorTools : Editor {

	model selectedModel;

	void OnEnable(){
		selectedModel = (model)target;
	}

	void OnDisable(){
		selectedModel = null;
	}

	public override void OnInspectorGUI(){
		DrawDefaultInspector ();
		if (GUILayout.Button ("Make model zero"))
			zeroModelPosition ();
	}


	plateMesh plateToTheLeft(plateMesh sender){

		for (int j = 0; j < sender.joinedPlates.Count; j++) {
			if (!sender.joinedPlates [j].originalPlateJoinedRight) {
				return sender.joinedPlates [j].otherPlate;
			}
		}

		return null;
	}


	void zeroModelPosition(){
		if(selectedModel != null){

			bool found = false;

			plateMesh leftmostPlate = selectedModel.GetComponentInChildren<plateMesh>();

			int safetyCount = selectedModel.transform.childCount * 2;

			while(!found){
				plateMesh pm = plateToTheLeft (leftmostPlate);
				if (pm == null)
					found = true;
				else
					leftmostPlate = pm;
				safetyCount--;
				if (safetyCount == 0)
					break;
			}

			if(!found){
				Debug.LogWarning ("Your model may be too complicated to centre in the current version");
			}

			Vector3 leftFaceCentre = leftmostPlate.transform.TransformPoint (meshEditTools.leftFaceCentreLocalCoords (leftmostPlate));

			Vector3 offSet = selectedModel.transform.position - leftFaceCentre;

			selectedModel.transform.position = leftFaceCentre;

			leftmostPlate.transform.position += offSet;

			meshEditTools.UpdateConnectedPlatePosition (leftmostPlate, leftmostPlate);


		}
	}
}
