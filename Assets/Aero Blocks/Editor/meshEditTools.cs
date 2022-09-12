using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
[CustomEditor (typeof(plateMesh))]
[CanEditMultipleObjects]
public class meshEditTools : Editor
{

	public static plateMesh thisPlate;
	static joinedPlate joiningPlates;

	Vector3 offsetLeft;
	Vector2 screenPointLeft;
	Vector2 mouseStartLeft;

	// Values that are stored and used to check if a plateMesh value has changed
	float sweepStore;
	float thicknessToChordStore;
	float controlFlapDeflectionStore;
	float flapChordRatioStore;
	float widthStore;
	float rightChordStore;
	float leftChordStore;

	public static bool showMAC;

	static List<plateMesh> selectedPlatesList;
	static bool initialised;
	static int selectionCount;

	int foundPos;



	void OnEnable ()
	{
		// Initialise selection list
		if(selectedPlatesList == null){
			selectedPlatesList = new List<plateMesh> ();
		}

		if (Selection.gameObjects.Count () == 1) {
			Initialise ();
			selectionCount = 1;
		}

		constructMesh ((plateMesh)target);

		Undo.undoRedoPerformed -= undoStuff;
		Undo.undoRedoPerformed += undoStuff;
	}


	void undoStuff(){
		
		plateMesh[] allPlates = FindObjectsOfType<plateMesh> ();

		for(int i = 0; i < allPlates.Count(); i++){
			constructMesh (allPlates [i]);
		}

	}

	void Initialise ()
	{
		selectedPlatesList = new List<plateMesh> ();
		selectedPlatesList.Add (Selection.gameObjects [0].GetComponent<plateMesh> ());

			
		thisPlate = selectedPlatesList [0];
		sweepStore = thisPlate.sweepAngle;
		thicknessToChordStore = thisPlate.thicknessToChordRatio;
		widthStore = thisPlate.width;
		controlFlapDeflectionStore = thisPlate.controlFlapDeflection;
		rightChordStore = thisPlate.rightChord;
		leftChordStore = thisPlate.leftChord;
		flapChordRatioStore = thisPlate.controlFlapChordRatio;

	}
		

	void OnDisable ()
	{
		Undo.undoRedoPerformed -= undoStuff;
		checkForMissingOrDeletedPlates ();
	}

	void checkForMissingOrDeletedPlates(){
		if (selectedPlatesList.Count == 1) {
			thisPlate = selectedPlatesList [0];
			int numberOfJoints = thisPlate.joinedPlates.Count;
			if (numberOfJoints != 0) {
				for (int i = 0; i < numberOfJoints; i++) {
					removeDeleted (thisPlate.joinedPlates [i].otherPlate);
				}
			}
		}
	}

	public static void removeDeleted(plateMesh pl){
		int numberOfJoints = pl.joinedPlates.Count;
		if (numberOfJoints != 0){
			for(int i = 0; i < numberOfJoints; i++){
				if (pl.joinedPlates[i].otherPlate == null){
					pl.joinedPlates.RemoveAt (i);
					numberOfJoints = pl.joinedPlates.Count;
					i = 0;
				}
			}
		}
	}

	static List<GameObject> getSelectionGameObjects(){
		List<GameObject> gos = new List<GameObject> (selectedPlatesList.Count);
		for(int i = 0; i < selectedPlatesList.Count; i++){
			gos.Add (selectedPlatesList [i].gameObject);
		}
		return gos;
	}



	///	------------------------------------------------------------------------------------------------------------------------------
	/// 
	/// SELECTION TRACKING
	/// 
	/// ------------------------------------------------------------------------------------------------------------------------------
	/// 
	/// 


	int countPlatesInSelection(){
		int count = 0;
		for (int i = 0; i < Selection.gameObjects.Count(); i++){
			if (Selection.gameObjects [i].GetComponent<plateMesh> () != null)
				count++;
		}
		return count;
	}

	// This function allows us to keep the order of selection of plateMesh objects. I think anyway
	void storeSelection(){
		// Check if amount of selected objects is different to the stored list of selected objects
		int differenceInSelection = selectedPlatesList.Count - countPlatesInSelection();

		// If there is a difference then the list  needs updating
		if (differenceInSelection != 0) {

			// > 0 means the list is longer than the actual selection count
			if (differenceInSelection > 0) {

				// So we need to remove things from the list
				List<plateMesh> elementsToBeRemoved = new List<plateMesh> ();

				// Iterate through the list
				for (int j = 0; j < selectedPlatesList.Count; j++) {

					bool found = false;

					// For each item in the list check if it is also in the selection
					for (int k = 0; k < Selection.gameObjects.Count (); k++) {
						if (selectedPlatesList [j] == Selection.gameObjects [k].GetComponent<plateMesh> ())
							found = true;
					}

					// If it isn't in the selection it must have been deselected or deleted or something so remove it from the list
					if (!found) {
						elementsToBeRemoved.Add (selectedPlatesList [j]);
						break;
					}
				}
				// Remove all elements no longer in the selection
				selectedPlatesList.RemoveAll (x => elementsToBeRemoved.Contains (x));

			} else {	// Else the selection count is > the list length

				// So we need to add more elements to the list
				List<plateMesh> elementsToBeAdded = new List<plateMesh> ();

				// Iterate through all selected gameobjects (NOTE: this may cause repeated calls every update if the user selects
				// a gameobject that doesn't have a plateMesh component, as then the list will always be < selection count
				for (int j = 0; j < Selection.gameObjects.Count (); j++) {

					bool found = false;

					// For each item in the selection check if it is in the list
					for (int k = 0; k < selectedPlatesList.Count; k++) {
						if (selectedPlatesList [k] == Selection.gameObjects [j].GetComponent<plateMesh> ()) {
							found = true;
							break;
						}
					}

					// If it isn't then it needs to be added to the list
					if (!found) {
						elementsToBeAdded.Add (Selection.gameObjects [j].GetComponent<plateMesh> ());
					}
				}
				// Add the newly selected elements to the list
				selectedPlatesList.AddRange (elementsToBeAdded);
			}
		}

		if (selectedPlatesList.Count > 0) {
			thisPlate = selectedPlatesList [0];
		}
	}

	void OnSceneGUI ()
	{

		///	-------------------------------------------------------------
		/// 
		/// HANDLES SELECTION CHANGES
		/// 
		///	-------------------------------------------------------------

		// This is how I stop multiple calls when the user has selected a gameobject that doesn't have a plateMesh component. Might mess things up somewhere but
		// it's better than eating all the CPU with calls every ~0.02s
		if (Selection.gameObjects.Count () != selectionCount) {
			storeSelection ();
			selectionCount = Selection.gameObjects.Count ();
		}


		///	-------------------------------------------------------------
		/// 
		/// TOOL SELECTION
		/// 
		/// -------------------------------------------------------------

		switch (buttonsForEditor.SelectedTool) {

		// No tool
		case 0:
			Tools.hidden = false;
			break;
	
		// Edit geometry
		case 1:
			Tools.hidden = false;
			// Draw the MAC indicators if the user has checked the draw MAC toggle

			drawMacCheck ();

			for (int i = 0; i < selectedPlatesList.Count; i++) {
				if (showMAC)
					calculateAndDrawMAC (selectedPlatesList [i]);
			}
			if (selectedPlatesList.Count == 1)
				UpdateVerticesFromHandles (selectedPlatesList [0]);
			break;

		// Join blocks together
		case 2:
			Tools.hidden = false;
			drawJoinButtons ();
			break;
		}


		///	----------------------------------------------------------------------------------------
		/// 
		/// UPDATE POSITION OF PLATES, AND THE MESH IF DIMENSIONS CHANGE
		/// 
		/// ----------------------------------------------------------------------------------------

		/// Things get tricky with updating the plate positions when the user can select multiple plates
		/// if the user selects a few parts of a model but not all the parts in the model
		/// when they move those parts, they will not drag the rest of the model with them.
		/// I cannot think of a way to solve this without risking a lot of overhead as every selected
		/// plate would call the UpdateConnectedPlatePosition() function

		// I used a for loop and breaks here because if one value has changed the whole mesh needs updating
		// there is no point in checking the rest of the values and updating the mesh every time
		if (selectedPlatesList.Count == 1) {
			for (int i = 0; i < 1; i++) {
				
				if (meshUpdateCheck (ref controlFlapDeflectionStore, thisPlate.controlFlapDeflection))
					break;
				
				if (meshUpdateCheck (ref flapChordRatioStore, thisPlate.controlFlapChordRatio))
					break;

				// Width is slightly different because the plate needs to move as well as update the mesh
				if (widthStore != thisPlate.width) {

					if (isInModel (selectedPlatesList [0])) {
						/*
						plateMesh[] pms = selectedPlatesList [0].transform.parent.GetComponentsInChildren<plateMesh> ();
						GameObject[] gos = new GameObject[pms.Count()];

						for(int j = 0; j < pms.Count(); j++){
							gos [j] = pms [j].gameObject;
							Debug.Log (gos [i].name);
						}
*/
						Undo.RegisterCompleteObjectUndo (selectedPlatesList [0].transform.parent.GetComponentsInChildren<plateMesh> (), "Edit plate mesh");

					} else {
						Undo.RegisterCompleteObjectUndo (selectedPlatesList [0], "Edit plate mesh");
					}

					float deltaWidth = thisPlate.width - widthStore;
					movePlateDeltaWidth (thisPlate, deltaWidth);
					constructMesh (thisPlate);
					widthStore = thisPlate.width;

					break;
				}

				if (meshUpdateCheck (ref leftChordStore, thisPlate.leftChord))
					break;
				
				if (meshUpdateCheck (ref rightChordStore, thisPlate.rightChord))
					break;
				
				if (meshUpdateCheck (ref thicknessToChordStore, thisPlate.thicknessToChordRatio))
					break;
				
				if (meshUpdateCheck (ref sweepStore, thisPlate.sweepAngle))
					break;
			}
			UpdateConnectedPlatePosition (thisPlate, thisPlate);
		}
	}

	[DrawGizmo(GizmoType.Selected | GizmoType.Active)]
	static void drawRoots(plateMesh pm, GizmoType gizmoType){
		
		Matrix4x4 cubeTransform = Matrix4x4.TRS (pm.transform.TransformPoint (leftFaceCentreLocalCoords (pm)), pm.transform.rotation, new Vector3 (0.0001f, pm.leftChord * pm.thicknessToChordRatio * 2f, pm.leftChord * 1.2f));
		Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

		Gizmos.matrix = cubeTransform;

		Gizmos.color = Color.yellow;
		Gizmos.DrawCube (Vector3.zero, Vector3.one);

		Gizmos.matrix = oldGizmosMatrix;
	}

	void movePlateDeltaWidth(plateMesh pm, float delta){
		delta *= 0.5f * pm.flip;
		pm.transform.position += pm.transform.TransformDirection (new Vector3 (delta, 0, 0));

	}

	// Check for change in value that merits reconstruction of the mesh
	// ref is used to update the store variable
	bool meshUpdateCheck(ref float storeVal, float checkVal){
		// If the value has changed
		if(storeVal != checkVal){
			if (isInModel (selectedPlatesList [0])) {
				/*
				plateMesh[] pms = selectedPlatesList [0].transform.parent.GetComponentsInChildren<plateMesh> ();
				GameObject[] gos = new GameObject[pms.Count()];

				for(int i = 0; i < pms.Count(); i++){
					gos [i] = pms [i].gameObject;
				}

				Undo.RegisterCompleteObjectUndo (gos, "Edit plate mesh");
				*/

				Undo.RegisterCompleteObjectUndo (selectedPlatesList [0], "Edit plate mesh");


			} else {
				Undo.RegisterCompleteObjectUndo (selectedPlatesList [0], "Edit plate mesh");
			}
			constructMesh (thisPlate);
			storeVal = checkVal;

			return true;
		} else {
			return false;
		}
	}


	// Draw the buttons used to join plates in the scene view
	void drawJoinButtons(){
		Handles.BeginGUI ();

		float sceneHeight = SceneView.currentDrawingSceneView.position.height;

		GUILayout.BeginArea (new Rect (5, 147, 100, sceneHeight));
		{
			GUILayout.BeginVertical ();
			if (GUI.Button(new Rect (0, 0, 60, 60), "R to L")){
				Undo.RecordObjects (getSelectionGameObjects().ToArray (), "Join selection");
				joinThem (true, false);
			}
			if (GUI.Button(new Rect (0, 62, 60, 60), "R to R")){
				Undo.RecordObjects (getSelectionGameObjects().ToArray (), "Join selection");
				joinThem (true, true);
			}
			if (GUI.Button(new Rect (0, 124, 60, 60), "L to L")){
				Undo.RecordObjects (getSelectionGameObjects().ToArray (), "Join selection");

				joinThem (false, false);
			}
			if (GUI.Button(new Rect (0, 186, 60, 60), "L to R")){
				//Undo.RecordObjects (getSelectionGameObjects().ToArray (), "Join selection");
				joinThem (false, true);
			}
			if (GUI.Button(new Rect (0, 248, 60, 60), "Mirror")){
				Undo.RegisterCompleteObjectUndo(FindObjectsOfType<plateMesh>().ToArray (), "Mirror selection");

				mirror ();

				Undo.FlushUndoRecordObjects ();
			}



			GUILayout.EndVertical ();
		}
		GUILayout.EndArea ();

		Handles.EndGUI ();
	}

	void drawMacCheck(){
		Handles.BeginGUI ();

		float sceneHeight = SceneView.currentDrawingSceneView.position.height;

		GUILayout.BeginArea (new Rect (5, 147, 100, sceneHeight));
		{
			GUILayout.BeginVertical ();

			showMAC = GUI.Toggle (new Rect (0, sceneHeight - 207, 100, 20), showMAC, "Show MAC");

			GUILayout.EndVertical ();
		}
		GUILayout.EndArea ();

		Handles.EndGUI ();
	}




	///	------------------------------------------------------------------------------------------------------------------------------
	/// 
	/// JOIN SELECTED PLATES
	/// 
	/// ------------------------------------------------------------------------------------------------------------------------------
	/// 
	/// Check which models the plates belong to and destroy all but one of the models as they are now combined
	/// Join the plates in selected order - note this may not work if selection is made by holding shift and selecting in the hierarchy
	/// ctrl should be fine however as that is individual selections


	static List<Transform> modelsToBeDestroyed;
	static List<Transform> partsNeedNewModel;	

	public static void joinThem (bool originalRight, bool otherRight)
	{
		if (selectedPlatesList.Count > 1) {
			Undo.RegisterCompleteObjectUndo (getSelectionGameObjects().ToArray (), "Join plates");

			modelCheck ();
			GameObject ModelGO = new GameObject ("Model");
			//int groupIndex = Undo.GetCurrentGroup ();

			ModelGO.tag = "Model";
			ModelGO.transform.position = selectedPlatesList [0].transform.TransformPoint(leftFaceCentreLocalCoords(selectedPlatesList[0]));
			model mod = Undo.AddComponent<model> (ModelGO);


			for (int i = 0; i < selectedPlatesList.Count - 1; i++) {

				joiningPlates = new joinedPlate ();
				joiningPlates.originalPlateJoinedRight = originalRight;
				joiningPlates.otherPlateJoinedRight = otherRight;
				joiningPlates.originalPlate = selectedPlatesList [i];
				joiningPlates.otherPlate = selectedPlatesList [i + 1];

				UpdatePlatePosition (selectedPlatesList [i], selectedPlatesList [i + 1]);

				createJoint ();

				selectedPlatesList [i + 1].transform.SetParent (ModelGO.transform);
				selectedPlatesList [i].transform.SetParent (ModelGO.transform);

				joiningPlates.originalPlate.joinedPlates.Add (joiningPlates);
				joiningPlates.otherPlate.joinedPlates.Add (joinedPlate.swapOverPlates (joiningPlates));
				mod.AddJoint (joiningPlates);

			}

			UpdateConnectedPlatePosition (selectedPlatesList [0], selectedPlatesList [0]);

			if (partsNeedNewModel != null) {
				for (int i = 0; i < partsNeedNewModel.Count; i++) {
					partsNeedNewModel [i].SetParent (ModelGO.transform);
				}
			}

			if (modelsToBeDestroyed != null) {
				for (int i = 0; i < modelsToBeDestroyed.Count; i++) {
					mod.joints.AddRange (modelsToBeDestroyed [i].GetComponent<model> ().joints);
					Editor.DestroyImmediate (modelsToBeDestroyed [i].gameObject);
				}
			}

			mod.joints = mod.joints.OrderBy (o => o.otherPlate.transform.position.x).ToList ();
			Undo.RegisterCreatedObjectUndo(ModelGO, "Create Model");

			Undo.FlushUndoRecordObjects ();
		} else {
			Debug.Log("Please select two or more Aero Blocks to join");
		}
	
	}


	static bool onlyPlateMeshSelected(){
		int count = Selection.objects.Count();
		if(count >= 2){
			for (int i = 0; i < count; i++) {
				if(Selection.transforms[i].GetComponent<plateMesh>() == null){
					Debug.Log ("Please only select aero blocks to join together");
					return false;
				}
			}
			return true;
		} else {
			return false;
		}
	}
		
	static void modelCheck(){
		modelsToBeDestroyed = new List<Transform> ();
		partsNeedNewModel = new List<Transform> ();
		for (int i = 0; i < Selection.objects.Count (); i++) {
			Transform par = Selection.transforms [i].parent;

			if ((par != null) && (par.tag == "Model")) {
				if (!(modelsToBeDestroyed.Contains (par))) {
					modelsToBeDestroyed.Add (par);
					Transform[] tr = par.GetComponentsInChildren<Transform> ();
					for (int j = 0; j < tr.Count (); j++) {
						partsNeedNewModel.Add (tr [j]);
					}
				}
			}
		}
	}
		
	static bool isInModel(plateMesh pm){
		Transform par = pm.transform.parent;

		if ((par != null) && (par.tag == "Model")) {
			return true;
		} else {
			return false;
		}
	}


	///	------------------------------------------------------------------------------------------------------------------------------
	/// 
	/// MIRROR PLATES
	/// 
	/// ------------------------------------------------------------------------------------------------------------------------------

	public static void mirror(){
		thisPlate = selectedPlatesList [0];
		if (isInModel (thisPlate)) {
			plateMesh[] pms = thisPlate.transform.parent.GetComponentsInChildren<plateMesh> ();
			for (int i = 0; i < pms.Count (); i++) {
				mirrorBlock (pms [i]);
			}
		} else {
			mirrorBlock (thisPlate);
		}
	}
		
	static void mirrorBlock(plateMesh pm){
		
		float flippedWidth = pm.flip * pm.width;
		pm.transform.position -= pm.transform.TransformDirection(new Vector3(flippedWidth, 0,0));
		pm.transform.position += new Vector3(0, Mathf.Sin(pm.transform.localEulerAngles.z * Mathf.Deg2Rad) * flippedWidth ,0);

		pm.flip *= -1;
		pm.transform.localEulerAngles = new Vector3 (pm.transform.localEulerAngles.x, -pm.transform.localEulerAngles.y, -pm.transform.localEulerAngles.z);

		constructMesh (pm);
		UpdateConnectedPlatePosition (pm, pm);
	}


	///	------------------------------------------------------------------------------------------------------------------------------
	/// 
	/// MESH CONSTRUCT AND UPDATE
	/// 
	/// ------------------------------------------------------------------------------------------------------------------------------
	/// 
	/// Rebuild the mesh from dimensions of plate and then update the plate's mesh


	static void constructMesh (plateMesh pm)
	{
		// This bit just gets a bit hairy, it's just to find the vert positions for the mesh from the dimensions
		// I think this could all be avoided if the verts were edited directly instead of the dimensions, however
		// that would just make things even more of a mess and you still need to show the dimensions to the user
		// and you need them for the aerodynamics so..

		float leftChord = pm.leftChord;
		float rightChord = pm.rightChord;

		float halfLeftChord = leftChord / 2;
		float leftThickness = halfLeftChord * pm.thicknessToChordRatio;
		float leftFlapStart = -halfLeftChord + pm.controlFlapChordRatio * leftChord;

		float halfRightChord = rightChord / 2;
		float rightThickness = halfRightChord * pm.thicknessToChordRatio;
		float rightFlapStart = -halfRightChord + pm.controlFlapChordRatio * rightChord;

		float sweepBackDistance = pm.width * Mathf.Tan (pm.sweepAngle * Mathf.Deg2Rad);

		// flap deflection
		float tanBeta = Mathf.Tan (-pm.controlFlapDeflection * Mathf.Deg2Rad);
		float leftEdge = leftChord * tanBeta * pm.controlFlapChordRatio;
		float rightEdge = rightChord * tanBeta * pm.controlFlapChordRatio;
		float alpha = (90 + pm.controlFlapDeflection) * Mathf.Deg2Rad;
		float cosAlpha = Mathf.Cos (alpha);
		float sinAlpha = Mathf.Sin (alpha);
		float leftZ = leftEdge * cosAlpha;
		float rightZ = rightEdge * cosAlpha;
		float leftY = leftEdge * sinAlpha;
		float rightY = rightEdge * sinAlpha;

		float flipVal = 0.5f * pm.width * pm.flip;

		leftThickness *= pm.flip;
		rightThickness *= pm.flip;

		pm.verts [0] = new Vector3 (-flipVal, -leftThickness, halfLeftChord);					// Bottom left edge
		pm.verts [1] = new Vector3 (-flipVal, -leftThickness, leftFlapStart);					//
		pm.verts [2] = new Vector3 (-flipVal, -leftThickness + leftY, -halfLeftChord + leftZ);	// flap vert

		pm.verts [3] = new Vector3 (-flipVal, leftThickness, halfLeftChord);					// Top left edge
		pm.verts [4] = new Vector3 (-flipVal, leftThickness, leftFlapStart);					//
		pm.verts [5] = new Vector3 (-flipVal, leftThickness + leftY, -halfLeftChord + leftZ);	// flap vert

		pm.verts [6] = new Vector3 (flipVal, -rightThickness, halfRightChord + sweepBackDistance);					// Bottom right edge
		pm.verts [7] = new Vector3 (flipVal, -rightThickness, rightFlapStart + sweepBackDistance);					//
		pm.verts [8] = new Vector3 (flipVal, -rightThickness + rightY, -halfRightChord + sweepBackDistance + rightZ);// flap vert

		pm.verts [9] = new Vector3 (flipVal, rightThickness, halfRightChord + sweepBackDistance);					// Top right edge
		pm.verts [10] = new Vector3 (flipVal, rightThickness, rightFlapStart + sweepBackDistance);					//
		pm.verts [11] = new Vector3 (flipVal, rightThickness + rightY, -halfRightChord + sweepBackDistance + rightZ);// flap vert

		UpdateMesh (pm);
		UpdateJointAnchor (pm);

	}

	static void UpdateMesh (plateMesh pm)
	{
		pm.mesh = pm.GetComponent<Mesh> ();
		pm.meshFilter = pm.GetComponent<MeshFilter> ();
		pm.meshCollider = pm.GetComponent<MeshCollider> ();

		if (pm.mesh == null)
			rebuildMesh (pm);
		else {
			List<Vector3> meshVerts = new List<Vector3> ();
			for (int j = 0; j < 3; j++) {
				for (int i = 0; i < pm.verts.Count; i++) {
					meshVerts.Add (pm.verts [i]);
				}
			}
			pm.mesh.vertices = meshVerts.ToArray ();
		}

		// I don't know a lot about meshes so this might all be pointless and wasteful
		Mesh newMesh = Mesh.Instantiate (pm.mesh) as Mesh;
		newMesh.RecalculateNormals ();
		newMesh.RecalculateBounds ();

		pm.meshFilter.mesh = newMesh;
	
		pm.meshCollider.sharedMesh = newMesh;
		pm.meshCollider.sharedMesh.RecalculateBounds ();

	}

	static void rebuildMesh(plateMesh pm){
		pm.mesh = new Mesh();

		List<Vector3> meshVerts = new List<Vector3>();

		for (int j = 0; j < 3; j++) {
			for (int i = 0; i < pm.verts.Count; i++) {
				meshVerts.Add (pm.verts [i]);
			}
		}


		pm.mesh.vertices = meshVerts.ToArray();


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

		pm.mesh.triangles = triangles;

	}

	static void UpdateJointAnchor (plateMesh pm)
	{
		if (pm.GetComponent<ConfigurableJoint> () != null) {
			int joints = pm.joinedPlates.Count ();
			// HERE
			if (joints >= 1) {
				for (int i = 0; i < joints; i++) {
					
					if (pm.joinedPlates [i].joint.connectedBody == pm.rigidBody) {
						pm.joinedPlates [i].joint.connectedAnchor = pm.joinedPlates [i].originalPlateJoinedRight ? rightFaceCentreLocalCoords (pm) : leftFaceCentreLocalCoords (pm);
					} else {
						pm.joinedPlates [i].joint.anchor = pm.joinedPlates [i].originalPlateJoinedRight ? rightFaceCentreLocalCoords (pm) : leftFaceCentreLocalCoords (pm);
					}
					pm.joinedPlates [i].joint.autoConfigureConnectedAnchor = true;
					pm.joinedPlates [i].joint.autoConfigureConnectedAnchor = false;
				}
			}
		}
	}


	static void UpdateVerticesFromHandles (plateMesh pm)
	{
		float leftChord = pm.leftChord;
		float rightChord = pm.rightChord;
		float width = pm.width;

		Vector3 arrowPos = pm.transform.TransformPoint (leftFaceCentreLocalCoords (pm));
		Handles.color = Handles.zAxisColor;
		leftChord = Handles.ScaleValueHandle (leftChord, arrowPos, pm.transform.rotation, -4 * HandleUtility.GetHandleSize (arrowPos), Handles.ArrowHandleCap, 1);

		arrowPos = pm.transform.TransformPoint (rightFaceCentreLocalCoords (pm));

		rightChord = Handles.ScaleValueHandle (rightChord, arrowPos, pm.transform.rotation * Quaternion.Euler (new Vector3 (0, 0, 0)), 4 * HandleUtility.GetHandleSize (arrowPos), Handles.ArrowHandleCap, 1);

		Handles.color = Handles.xAxisColor;
		width = Handles.ScaleValueHandle (width, arrowPos, pm.transform.rotation * Quaternion.Euler (new Vector3 (0, pm.flip * 90, 0)), 4 * HandleUtility.GetHandleSize (arrowPos), Handles.ArrowHandleCap, 1);

		leftChord = Mathf.Max (leftChord, 0.02f);
		rightChord = Mathf.Max (rightChord, 0.02f);
		width = Mathf.Max (width, 0.02f);

		pm.leftChord = leftChord;
		pm.rightChord = rightChord;
		pm.width = width;
	}


	///	------------------------------------------------------------------------------------------------------------------------------
	/// 
	/// SNAP PLATES
	/// 
	/// ------------------------------------------------------------------------------------------------------------------------------


	static void UpdatePlatePosition (plateMesh pm1, plateMesh pm2)
	{
		// This looks like a mess but it's pretty simple really..
		// If the other plate is joined on its right side then move the original plate
		pm1.transform.position = pm2.transform.TransformPoint (joiningPlates.otherPlateJoinedRight ? 
			rightFaceCentreLocalCoords(pm2) : leftFaceCentreLocalCoords(pm2));
		
		pm1.transform.position = pm1.transform.TransformPoint (joiningPlates.originalPlateJoinedRight ? 
			-rightFaceCentreLocalCoords(pm1) : -leftFaceCentreLocalCoords(pm1));
	}

	static void createJoint ()
	{
		
		ConfigurableJoint confJoint = Undo.AddComponent<ConfigurableJoint> (joiningPlates.originalPlate.gameObject); //joiningPlates.originalPlate.gameObject.AddComponent<ConfigurableJoint> ();
		confJoint.anchor = joiningPlates.originalPlateJoinedRight ? rightFaceCentreLocalCoords(joiningPlates.originalPlate) : leftFaceCentreLocalCoords(joiningPlates.originalPlate);
		confJoint.connectedBody = joiningPlates.otherPlate.GetComponent<Rigidbody> ();

		confJoint.xMotion = ConfigurableJointMotion.Locked;
		confJoint.yMotion = ConfigurableJointMotion.Locked;
		confJoint.zMotion = ConfigurableJointMotion.Locked;
		confJoint.angularXMotion = ConfigurableJointMotion.Locked;
		confJoint.angularYMotion = ConfigurableJointMotion.Locked;
		confJoint.angularZMotion = ConfigurableJointMotion.Locked;

		joiningPlates.joint = confJoint;
	}


	// This is designed to be called by one plateMesh that sends itself as pl and sender, the function will then spread out to the other plates, however I think it may hang if there's a closed loop of plates
	public static void UpdateConnectedPlatePosition (plateMesh pl, plateMesh sender)
	{
		
		int numberOfJoinedPlates = pl.joinedPlates.Count;
		if (numberOfJoinedPlates != 0) {
			List<joinedPlate> missingPlates = new List<joinedPlate> ();
			for (int i = 0; i < numberOfJoinedPlates; i++) {
				joinedPlate jp = pl.joinedPlates [i];


				if(jp.otherPlate == null || jp.originalPlate == null){
					missingPlates.Add (jp);
				} else if (!(Object.Equals (jp.otherPlate, sender))) {

					// Remember shorthand for an if/else statement is:
					// bool ? (do when bool is true) : (do when bool is false)

					jp.otherPlate.transform.position = jp.originalPlateJoinedRight ? jp.originalPlate.transform.TransformPoint (rightFaceCentreLocalCoords (jp.originalPlate))
						 : jp.originalPlate.transform.TransformPoint (leftFaceCentreLocalCoords (jp.originalPlate));
					
					jp.otherPlate.transform.position = jp.otherPlateJoinedRight ? jp.otherPlate.transform.TransformPoint (-rightFaceCentreLocalCoords (jp.otherPlate))
						 : jp.otherPlate.transform.TransformPoint (-leftFaceCentreLocalCoords (jp.otherPlate));

					jp.joint.autoConfigureConnectedAnchor = true;
					jp.joint.autoConfigureConnectedAnchor = false;

					UpdateConnectedPlatePosition (jp.otherPlate, jp.originalPlate);
				}
			}
			pl.joinedPlates.RemoveAll (x => missingPlates.Contains (x));
		}


	}

	static bool isInJoint(joinedPlate jp, plateMesh pm){
		if (Object.Equals (pm, jp.otherPlate) || Object.Equals (pm, jp.originalPlate))
			return true;
		return false;
	}
		
	public static Vector3 rightFaceCentreLocalCoords (plateMesh pm)
	{
		Vector3 centre = new Vector3 (0.5f * pm.width * pm.flip, 0, pm.width * Mathf.Tan (pm.sweepAngle * Mathf.Deg2Rad));
		return centre;
	}

	public static Vector3 leftFaceCentreLocalCoords (plateMesh pm)
	{
		Vector3 centre = new Vector3 (-0.5f * pm.width * pm.flip, 0, 0);
		return centre;
	}
		





	///	------------------------------------------------------------------------------------------------------------------------------
	/// 
	/// Mean Aerodynamic Chord DRAWING + CALCULATING
	/// 
	/// ------------------------------------------------------------------------------------------------------------------------------

	void calculateAndDrawMAC(plateMesh pm){

		float distHolder = 0.5f * pm.leftChord + pm.rightChord;

		Vector2 rootPointOne = new Vector2 (distHolder, -0.5f * pm.flip * pm.width);
		Vector2 rootPointTwo = new Vector2 (-distHolder, -0.5f * pm.flip * pm.width);

		float sweepDist = pm.width * Mathf.Tan (pm.sweepAngle * Mathf.Deg2Rad);
		distHolder = 0.5f * pm.rightChord + pm.leftChord;

		Vector2 tipPointOne = new Vector2 (distHolder + sweepDist, 0.5f * pm.flip * pm.width);
		Vector2 tipPointTwo = new Vector2 (-distHolder + sweepDist, 0.5f * pm.flip * pm.width);

		///	------------------------------------------------------------------------------------------------
		/// 
		/// Equations of lines:
		/// -------------------
		/// 
		///	Using line equations of A1x + B1y = C1 and A2x + B2y = C2
		/// 
		///	Define line 1 as line from rootPointOne to tipPointTwo
		///	Define line 2 as line from rootPointTwo to tipPointOne

		float A1 = tipPointTwo.y - rootPointOne.y;
		float B1 = rootPointOne.x - tipPointTwo.x;
		float C1 = (A1 * rootPointOne.x) + (B1 * tipPointTwo.y);

		float A2 = tipPointOne.y - rootPointTwo.y;
		float B2 = rootPointTwo.x - tipPointOne.x;
		float C2 = (A2 * rootPointTwo.x) + (B2 * tipPointOne.y);

		float det = (A1 * B2) - (A2 * B1);

		Vector2 macPoint;
		float macLength;
		float macHeight;

		if (det == 0) {
			// this means the lines are somehow parallel, should never happen but just in case
			macPoint = Vector2.zero;
		} else {
			macPoint = new Vector2 (
				( (B2 * C1) - (B1 * C2) ) / det,						// x coord
				( (A1 * C2) - (A2 * C1) ) / det - pm.flip * pm.width	// y coord
			);
		}

		macLength = lengthDueToTaper (pm.leftChord, pm.rightChord);
		macHeight = lengthDueToTaper (pm.leftChord * pm.thicknessToChordRatio, pm.rightChord * pm.thicknessToChordRatio);

		pm.macPoint = macPoint;

		Vector3 rp1 = pm.transform.TransformPoint (new Vector3 (rootPointOne.y, 0, rootPointOne.x));
		Vector3 rp2 = pm.transform.TransformPoint (new Vector3 (rootPointTwo.y, 0, rootPointTwo.x));


		Vector3 tp1 = pm.transform.TransformPoint (new Vector3 (tipPointOne.y, 0, tipPointOne.x));
		Vector3 tp2 = pm.transform.TransformPoint (new Vector3 (tipPointTwo.y, 0, tipPointTwo.x));

		Handles.color = Color.white;
		Handles.DrawLine (tp1, rp2);
		Handles.DrawLine (tp2, rp1);
		Handles.DrawLine (tp1, tp2);
		Handles.DrawLine (rp2, rp1);

		Matrix4x4 cubeTransform = Matrix4x4.TRS (pm.transform.TransformPoint (new Vector3 (macPoint.y, 0, macPoint.x)), pm.transform.rotation, new Vector3 (0.01f, macHeight * 1.01f, macLength));
		Matrix4x4 oldHandlesMatrix = Handles.matrix;

		Handles.matrix = cubeTransform;

		Handles.color = Color.green;
		Handles.DrawWireCube (Vector3.zero, Vector3.one);

		Handles.matrix = oldHandlesMatrix;

	}

	float lengthDueToTaper(float root, float tip){
		float taperRatio = tip / root;
		float val = root * (2f / 3f) * ((1 + taperRatio + (taperRatio * taperRatio)) / (1 + taperRatio));
		return val;
	}
		


}
