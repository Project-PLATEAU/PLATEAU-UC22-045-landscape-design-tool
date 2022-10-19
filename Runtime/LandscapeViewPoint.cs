using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class LandscapeViewPoint : MonoBehaviour
{
    public string ViewpointDescription = "視点場";
    public float viewpointFOV = 60.0f;
    public float EyeHeight = 1.6f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LandscapeViewPoint))]
    public class LandScapeViewPointEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            Selection.activeGameObject.GetComponent<LandscapeViewPoint>().ViewpointDescription =
                EditorGUILayout.TextField("視点場名",
                    Selection.activeGameObject.GetComponent<LandscapeViewPoint>().ViewpointDescription);
            Selection.activeGameObject.GetComponent<LandscapeViewPoint>().viewpointFOV =
                EditorGUILayout.FloatField("視野角",
                    Selection.activeGameObject.GetComponent<LandscapeViewPoint>().viewpointFOV);
            Selection.activeGameObject.GetComponent<LandscapeViewPoint>().EyeHeight =
                EditorGUILayout.FloatField("視点高",
                    Selection.activeGameObject.GetComponent<LandscapeViewPoint>().EyeHeight);



            if (GUILayout.Button("視点場の位置設定"))
            {
                sceneView.Focus();
            }


        }

        public void OnSceneGUI()
        {

            var ev = Event.current;

            RaycastHit hit;
            if (ev.type == EventType.KeyUp && ev.keyCode==KeyCode.LeftShift)
            {

                Vector3 mousePosition = Event.current.mousePosition;
                Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    Debug.Log(hit.collider.name);
                    Debug.Log(hit.point.ToString());
                    Selection.activeGameObject.transform.position = hit.point;
                }
                ev.Use();
            }
        }

    }

#endif
}