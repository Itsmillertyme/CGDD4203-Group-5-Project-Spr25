using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor {

    //**PROPERTIES**
    SerializedProperty debugModeProperty;
    SerializedProperty updateDebugProperty;
    SerializedProperty triggerDebugProperty;
    SerializedProperty classicCameraProperty;
    SerializedProperty levelWidthProperty;
    SerializedProperty levelHeightProperty;

    //**UNITY METHODS**
    private void OnEnable() {
        // Link to the properties in GameManager
        debugModeProperty = serializedObject.FindProperty("debugMode");
        updateDebugProperty = serializedObject.FindProperty("updateDebug");
        triggerDebugProperty = serializedObject.FindProperty("triggerDebug");
        classicCameraProperty = serializedObject.FindProperty("classicCamera");
        levelWidthProperty = serializedObject.FindProperty("levelWidth");
        levelHeightProperty = serializedObject.FindProperty("levelHeight");
    }
    //
    public override void OnInspectorGUI() {

        //update serialized object
        serializedObject.Update();

        //Show other properties
        EditorGUILayout.PropertyField(serializedObject.FindProperty("walls"), new GUIContent("Walls"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("asteroidPrefabs"), new GUIContent("Asteroid Prefab"));
        //
        EditorGUILayout.PropertyField(classicCameraProperty, new GUIContent("Classic Camera"));
        EditorGUILayout.PropertyField(levelWidthProperty, new GUIContent("Level Width"));
        EditorGUILayout.PropertyField(levelHeightProperty, new GUIContent("Level Height"));

        //Add space and a header for debug options
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Ship Debug Options", EditorStyles.boldLabel);

        //Show debug mode option
        EditorGUILayout.PropertyField(debugModeProperty, new GUIContent("Debug Mode"));

        //Show various debug options if in debug mode
        if (debugModeProperty.boolValue) {
            EditorGUILayout.PropertyField(updateDebugProperty, new GUIContent("Update Debug"));
            EditorGUILayout.PropertyField(triggerDebugProperty, new GUIContent("Trigger Debug"));
        }

        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}
