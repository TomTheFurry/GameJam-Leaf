using UnityEngine;
using UnityEditor;
using System.Collections;

[InitializeOnLoad]
public class buttonsForEditor : Editor {

	public static int SelectedTool{

		get {
			return EditorPrefs.GetInt( "SelectedEditorTool", 0 );
		}

		set {
			if( value == SelectedTool ){
				return;
			}

			EditorPrefs.SetInt( "SelectedEditorTool", value );

		}
	}

	static void drawCreateButtons (){
		Handles.BeginGUI ();

		GUILayout.BeginArea (new Rect (5, 5, 60, 360));
		{
			GUILayout.BeginVertical ();

			GUI.Box(new Rect (0, 0, 60, 33), "Create Blocks", EditorStyles.textArea);

			if (GUI.Button(new Rect (0, 40, 60, 50), "Aero", EditorStyles.miniButton)){
				createAeroBlock.CreateBlock();
			}

			if (GUI.Button(new Rect (0, 92, 60, 50), "Hub", EditorStyles.miniButton)){
				createAeroBlock.CreateHub ();
			}

			GUILayout.EndVertical ();
		}
		GUILayout.EndArea ();

		Handles.EndGUI ();
	}
		

	static void DrawToolsMenu(Rect position){

		Handles.BeginGUI ();

		GUILayout.BeginArea (new Rect (0, position.height - 35, position.width, 20), EditorStyles.toolbar);
		{
			string[] buttonLabels = new string[] { "Hide Tools", "Edit Mode", "Join Mode"};

			SelectedTool = GUILayout.SelectionGrid (
				SelectedTool, 
				buttonLabels, 
				3,
				EditorStyles.toolbarButton,
				GUILayout.Width (300));
		}
		GUILayout.EndArea ();

		if(SelectedTool != 0){

			drawCreateButtons ();
		}

		Handles.EndGUI ();
	}

	static buttonsForEditor()
	{
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
		SceneView.onSceneGUIDelegate += OnSceneGUI;

	}

	void OnDestroy()
	{
		SceneView.onSceneGUIDelegate -= OnSceneGUI;

	}



	static void OnSceneGUI( SceneView sceneView )
	{

		DrawToolsMenu( sceneView.position );
	}

}
