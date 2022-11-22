using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LandscapeDesignTool
{
    public class AnyCircleRegurationAreaHandler : MonoBehaviour
    {

        const int CircleDiv = 180;
        public float areaHeight;
        public Color AreaColor;
        Vector3 AreaCenter;
        Vector3 AreaRadiusPoint;
        bool _isValid = false;
        Material _areaMaterial;

        [SerializeField] List<Vector3> vertex = new List<Vector3>();
        public List<Vector2> _Contours;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        private void OnDrawGizmosSelected()
        {
            if (_isValid)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(AreaCenter, new Vector3(10, 10, 10));
                Gizmos.DrawCube(AreaRadiusPoint, new Vector3(10, 10, 10));

                Gizmos.color = Color.red;

                Gizmos.DrawLine(AreaCenter, AreaRadiusPoint);
            }
        }

        public void GenMesh()
        {


            GameObject go = new GameObject("Upper");
            go.layer = LayerMask.NameToLayer("RegulationArea");
            var mr = go.AddComponent<MeshRenderer>();
            Material material = LDTTools.MakeMaterial(AreaColor);
            _areaMaterial = material;

            var mf = go.AddComponent<MeshFilter>();

            Mesh mesh = new Mesh();
            Vector3[] v = new Vector3[CircleDiv+1];
            int[] triangles = new int[(CircleDiv) * 3];
            int i = 0;
            foreach( Vector3 v0 in vertex)
            {
                v[i] = new Vector3(v0.x, v0.y+areaHeight, v0.z);
                Debug.Log(v0.ToString());
                i++;
            }

            int n=0;
            for( int j = 0; j<CircleDiv-1; j++)
            {
                triangles[n++] = 0;
                triangles[n++] = j + 1;
                triangles[n++] = j + 2;
            }
            triangles[n++] = 0;
            triangles[n++] = CircleDiv;
            triangles[n++] = 1;



            mesh.vertices = v;
            mesh.triangles = triangles;

            mr.sharedMaterial = material;
            mf.mesh = mesh;

            MeshCollider meshCollider = go.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;

            mesh.RecalculateBounds();

            GameObject rootNode = new GameObject("RegurationCircleArea");
            rootNode.layer = LayerMask.NameToLayer("RegulationArea");
            ShapeItem si = rootNode.AddComponent<ShapeItem>();
            si.material = material;
            si.height = areaHeight;
            si.oldHeight = areaHeight;

            rootNode.transform.parent = gameObject.transform;
            go.transform.parent = rootNode.transform;
            GenerateSide(rootNode);

        }

        void GenerateSide(GameObject parent)
        {

            Vector3[] v = new Vector3[CircleDiv * 2];
            int[] triangles = new int[CircleDiv * 2 * 3];


            int n = 0;
            for ( int i=0; i<CircleDiv; i++)
            {
                Vector3 v0 = vertex[i + 1];
                v[n++] = new Vector3(v0.x, v0.y, v0.z);
                v[n++] = new Vector3(v0.x, v0.y+areaHeight, v0.z);
            }

            n = 0;
            for( int i=0; i<v.Length-2; i+=2)
            {
                triangles[n++] = i+2;
                triangles[n++] = i+1;
                triangles[n++] = i;

                triangles[n++] = i + 2;
                triangles[n++] = i + 3;
                triangles[n++] = i + 1;
            }

            GameObject go = new GameObject("Side");
            go.layer = LayerMask.NameToLayer("RegulationArea");
            var mf = go.AddComponent<MeshFilter>();
            var mesh = new Mesh();
            mf.mesh = mesh;
            mesh.vertices = v;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();

            var mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterial = _areaMaterial;

            go.transform.parent = parent.transform;


        }

        public void ClearPoint()
        {
            _isValid = false;
            AreaRadiusPoint = Vector3.zero;
            AreaCenter = Vector3.zero;
            vertex.Clear();
        }

        public void SetCenter(Vector3 c)
        {
            AreaCenter = c;
            AreaRadiusPoint = c;
            _isValid = true;
        }

        public void SetArcRadius(Vector3 r)
        {
            AreaRadiusPoint = r;
        }

        public void FinishArc()
        {

            vertex.Clear();
            vertex.Add(AreaCenter);
            float length = Vector3.Distance(AreaRadiusPoint, AreaCenter);
            float angle = 360.0f / (float)CircleDiv;
            Debug.Log("angle " + angle);
            RaycastHit hit;

            for (int i = 0; i < CircleDiv; i++)
            {
                float d = angle * i *Mathf.Deg2Rad;
                float x = Mathf.Sin(d)*length;
                float y = Mathf.Cos(d)*length;
                Debug.Log(d+" "+x+" "+y);
                Vector3 p = new Vector3(x+AreaCenter.x, 5000, y+AreaCenter.z);
                if (Physics.Raycast(p, new Vector3(0, -1, 0), out hit, Mathf.Infinity))
                {
                    // p.y = hit.collider.transform.position.y;
                    Vector3 v0 = new Vector3(p.x, hit.point.y, p.z);
                    vertex.Add(v0);
                    Debug.Log("hit "+ hit.point.y);
                    
                }
                else
                {
                    vertex.Add(p);
                    Debug.Log("no hit");

                }
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(AnyCircleRegurationAreaHandler))]
        [CanEditMultipleObjects]

        public class AnyCircleRegurationAreEditor : Editor
        {

            Color _areaColor = new Color(0, 1, 1, 0.5f);
            float _height;
            bool _pointing = false;
            Color _overlayColor = new Color(1, 0, 0, 0.5f);
            bool isBuildSelecting = false;

            private void Awake()
            {
                _height = Selection.activeGameObject.GetComponent<AnyCircleRegurationAreaHandler>().areaHeight;
            }

            public override void OnInspectorGUI()
            {


                SceneView sceneView = SceneView.lastActiveSceneView;

                EditorGUILayout.HelpBox("中心と半径を設定して任意規制領域を生成します", MessageType.Info);

                _height = EditorGUILayout.FloatField("高さ(m)", _height);
                _areaColor = EditorGUILayout.ColorField("色の設定", _areaColor);

                EditorGUILayout.Space();
                if (_pointing == false)
                {
                    GUI.color = Color.white;
                    if (GUILayout.Button("円による任意規制エリアの作成"))
                    {
                        sceneView.Focus();
                        _pointing = true;
                        isBuildSelecting = false;

                        sceneView.rotation = Quaternion.Euler(90, 0, 0);
                        sceneView.orthographic = true;
                        sceneView.size = 300.0f;
                    }
                }
                else
                {
                    GUI.color = Color.green;
                    if (GUILayout.Button("完了し円柱を生成"))
                    {
                        _pointing = false;
                        sceneView.orthographic = false;
                        _keydown = false;

                        Selection.activeGameObject.GetComponent<AnyCircleRegurationAreaHandler>().AreaColor = _areaColor;
                        Selection.activeGameObject.GetComponent<AnyCircleRegurationAreaHandler>().areaHeight = _height;
                        Selection.activeGameObject.GetComponent<AnyCircleRegurationAreaHandler>().GenMesh();
                    }
                    GUI.color = Color.white;
                    if (GUILayout.Button("中心と半径をクリア"))
                    {
                        Selection.activeGameObject.GetComponent<AnyCircleRegurationAreaHandler>().ClearPoint();
                        _pointing = false;
                    }

                }
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("建物のカラーを変更します", MessageType.Info);
                _overlayColor = EditorGUILayout.ColorField("色の設定", _overlayColor);
                if (isBuildSelecting == false)
                {
                    GUI.color = Color.white;
                    if (GUILayout.Button("建物のカラーを変更"))
                    {
                        sceneView.Focus();
                        isBuildSelecting = true;
                        _pointing = false;
                    }
                }
                else
                {
                    GUI.color = Color.green;
                    if (GUILayout.Button("カラー変更を終了"))
                    {
                        isBuildSelecting = false;

                    }
                }


            }

            bool _keydown = false;

            private void OnSceneGUI()
            {
                SceneView sceneView = SceneView.lastActiveSceneView;
                if (_pointing)
                {
                    var ev = Event.current;
                    RaycastHit hit;

                    if (ev.type == EventType.KeyDown && ev.keyCode == KeyCode.LeftShift)
                    {
                        if (_keydown == false)
                        {
                            _keydown = true;
                            Vector3 mousePosition = Event.current.mousePosition;

                            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
                            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                            {
                                Selection.activeGameObject.GetComponent<AnyCircleRegurationAreaHandler>().SetCenter(hit.point);
                            }
                        }
                    }
                    if (ev.type == EventType.KeyUp && ev.keyCode == KeyCode.LeftShift)
                    {
                        _keydown = false;
                        Selection.activeGameObject.GetComponent<AnyCircleRegurationAreaHandler>().FinishArc();
                    }

                    if (ev.type == EventType.MouseMove)
                    {
                        if (_keydown == true)
                        {
                            Vector3 mousePosition = Event.current.mousePosition;

                            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
                            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                            {
                                Selection.activeGameObject.GetComponent<AnyCircleRegurationAreaHandler>().SetArcRadius(hit.point);
                            }
                        }

                    }
                }


                if (isBuildSelecting)
                {
                    GameObject hitTarget = null;
                    var ev = Event.current;
                    if (ev.type == EventType.KeyUp && ev.keyCode == KeyCode.LeftShift)
                    {

                        Vector3 mousePosition = Event.current.mousePosition;
                        RaycastHit[] hits;
                        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
                        hits = Physics.RaycastAll(ray, Mathf.Infinity);

                        float mindistance = float.MaxValue;

                        if (hits.Length > 0)
                        {
                            for (int i = 0; i < hits.Length; i++)
                            {
                                RaycastHit hit = hits[i];

                                int layer = LayerMask.NameToLayer("Building");

                                if (hit.collider.gameObject.layer == layer)
                                {
                                    if (hit.distance < mindistance)
                                    {
                                        hitTarget = hit.collider.gameObject;
                                        mindistance = hit.distance;
                                        Debug.Log("hit " + hit.collider.name + " " + mindistance + " " + hit.point.ToString());
                                    }
                                }
                            }
                        }
                    }

                    if (hitTarget != null)
                    {
                        Bounds box = hitTarget.GetComponent<Renderer>().bounds;
                        float x = (box.min.x + box.max.x) / 2.0f;
                        float z = (box.min.z + box.max.z) / 2.0f;
                        Vector3 bottomCenter = new Vector3(x, box.min.y, z);
                        Debug.Log(bottomCenter);

                        RaycastHit[] hits;

                        Physics.queriesHitBackfaces = true;
                        hits = Physics.RaycastAll(bottomCenter, new Vector3(0, 1, 0), Mathf.Infinity);

                        bool isRegurationArea = false;
                        Debug.Log(hits.Length);
                        if (hits.Length > 0)
                        {
                            for (int i = 0; i < hits.Length; i++)
                            {
                                RaycastHit hit = hits[i];
                                Debug.Log(hit.collider.gameObject.name);
                                int layer = LayerMask.NameToLayer("RegulationArea");
                                if (hit.collider.gameObject.layer == layer)
                                {
                                    isRegurationArea = true;
                                }
                            }
                        }


                        if (isRegurationArea)
                        {
                            Material material = LDTTools.MakeMaterial(_overlayColor);
                            int nmat;
                            List<Material> oldmat = new List<Material>();
                            hitTarget.GetComponent<Renderer>().GetSharedMaterials(oldmat);
                            nmat = oldmat.Count;
                            Material[] matArray = new Material[nmat + 1];
                            for (int i = 0; i < nmat; i++)
                            {
                                matArray[i] = oldmat[i];
                            }
                            matArray[nmat] = material;

                            hitTarget.GetComponent<Renderer>().materials = matArray;
                        }
                    }
                }
            }
        }
#endif
    }
}