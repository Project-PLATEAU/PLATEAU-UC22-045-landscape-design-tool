using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



namespace LandscapeDesignTool.Editor
{
#if UNITY_EDITOR
    public class LandscapeDesign :EditorWindow
    {

        int _regulationType;
        float _regulationHeight = 10;
        float _screenWidth = 80.0f;
        float _screenHeight = 80.0f;
        float _heightAreaHeight = 30.0f;
        float _heightAreaRadius = 100.0f;
        Color _areaColor = new Color(0, 1, 1, 0.5f);

        string _regulationAreaExportPath = "";


        bool _regulationAreaEdit = false;
        bool _isRecurationAreaEdit = false;
        AnyPolygonRegurationAreaHandler polygonHandler;


        List<Vector3> vertex = new List<Vector3>();

        // Start is called before the first frame update


        private readonly string[] _tabToggles = { "�K���G���A�쐬", "���]�K���쐬", "�����K���G���A�쐬", "ShapeFile�����o��" };
        private int _tabIndex;

        [MenuItem("PLATEAU/�i�ς܂��Â���/�i�όv��")]
        public static void ShowWindow()
        {
            TagAdder.ConfigureTags();
            EditorWindow.GetWindow(typeof(LandscapeDesign), true, "�i�όv����");
        }

        void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (_regulationAreaEdit)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                var ev = Event.current;
                if( ev.type == EventType.MouseDown)
                {
                    RaycastHit[] hits;
                    int layerMask = 1 << 31;
                    Vector3 mousePosition = Event.current.mousePosition;

                    Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
                    hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);
                    if( hits != null)
                    {
                        vertex.Add(hits[0].point);
                    }

                    int id = 100;
                    Handles.color = Color.blue;
                    polygonHandler.SetVertex(vertex);
                    /*
                    foreach ( Vector3 v in vertex)
                    {
                        Vector3 p = new Vector3(v.x, v.y + 5, v.z);
                        Debug.Log(p);
                        Handles.CubeHandleCap(id, p, Quaternion.Euler(Vector3.forward), 10.0f, EventType.Repaint);
                        id++;
                    }
                    */
                }
            }
        }

        private void OnGUI()
        {

            var style = new GUIStyle(EditorStyles.label);
            style.richText = true;


            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                _tabIndex = GUILayout.Toolbar(_tabIndex, _tabToggles, new GUIStyle(EditorStyles.toolbarButton), GUI.ToolbarButtonSize.FitToContents);
            }

            if (_tabIndex == 0)
            {
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("<size=15>�K���G���A�쐬</size>", style);
                EditorGUILayout.HelpBox("�K�����A�̍�����ݒ肵�^�C�v��I�����ċK���G���A�쐬���N���b�N���Ă�������", MessageType.Info);
                string[] options = { "���p�`", "�~" };
                _regulationHeight = EditorGUILayout.FloatField("����", _regulationHeight);
                _areaColor = EditorGUILayout.ColorField("�F�̐ݒ�", _areaColor);

                _regulationType = EditorGUILayout.Popup(_regulationType, options);
                /*
                if (GUILayout.Button("�K���G���A�쐬"))
                {

                    LDTTools.CheckLayers();
                    LDTTools.CheckTag("RegulationArea");

                    if (_regurationType == 0)
                    {
                        /  *
                        GameObject grp = GameObject.Find("AnyPolygonRegurationArea");
                        if (!grp)
                        {
                            grp = new GameObject();
                            grp.name = "AnyPolygonRegurationArea";
                            grp.layer = LayerMask.NameToLayer("RegulationArea");

                            AnyPolygonRegurationAreaHandler handler = grp.AddComponent<AnyPolygonRegurationAreaHandler>();
                            handler.areaHeight = _regurationHeight;
                        }
                        *  /


                        GameObject go = new GameObject();
                        go.layer = LayerMask.NameToLayer("RegulationArea");
                        go.name = LDTTools.GetNumberWithTag("RegulationArea", "�K���G���A");
                        go.tag = "RegulationArea";
                        Selection.activeObject = go;
                        AnyPolygonRegurationAreaHandler handler = go.AddComponent<AnyPolygonRegurationAreaHandler>();
                        Debug.Log(_regurationHeight);
                        handler.SetHeight(_regurationHeight);
                        handler.SetAreaColor(_areaColor);
                        _regurationAreaEdit = false;
                        Repaint();
                    }
                    else
                    {
                        /  *
                        GameObject grp = GameObject.Find("AnyCirclnRegurationArea");
                        if (!grp)
                        {
                            grp = new GameObject();
                            grp.name = "AnyCircleRegurationArea";
                            grp.layer = LayerMask.NameToLayer("RegulationArea");

                            AnyCircleRegurationAreaHandler handler = grp.AddComponent<AnyCircleRegurationAreaHandler>();
                            handler.areaHeight = _regurationHeight;



                        }
                        *  /
                        GameObject go = new GameObject();
                        go.layer = LayerMask.NameToLayer("RegulationArea");
                        go.name = LDTTools.GetNumberWithTag("RegulationArea", "�K���G���A");
                        go.tag = "RegulationArea";
                        Selection.activeObject = go;
                        AnyCircleRegurationAreaHandler handler = go.AddComponent<AnyCircleRegurationAreaHandler>();
                        Debug.Log(_regurationHeight);
                        handler.SetHeight(_regurationHeight);
                        handler.SetAreaColor(_areaColor);
                        _regurationAreaEdit = false;
                        Repaint();
                    }

                }
                */
                if (_regulationAreaEdit)
                {
                    GUI.color = Color.green;
                    if (GUILayout.Button("���_�쐬�����������p�`�𐶐�"))
                    {
                        _regulationAreaEdit = false;
                        /*
                        GameObject go = new GameObject();
                        go.layer = LayerMask.NameToLayer("RegulationArea");
                        go.name = LDTTools.GetNumberWithTag("RegulationArea", "�K���G���A");
                        go.tag = "RegulationArea";
                        Selection.activeObject = go;
                        AnyPolygonRegurationAreaHandler handler = go.AddComponent<AnyPolygonRegurationAreaHandler>();
                        Debug.Log(_regurationHeight);
                        handler.SetHeight(_regurationHeight);
                        handler.SetAreaColor(_areaColor);
                        handler.SetVertex(vertex);
                        */
                        vertex.Clear();
                        polygonHandler.GenMesh();

                        Repaint();
                    }


                    GUI.color = Color.white;
                    if (GUILayout.Button("���_���N���A"))
                    {
                        vertex.Clear();
                    }


                }
                else
                {
                    GUI.color = Color.white;
                    if (GUILayout.Button("���_�쐬"))
                    {
                        _regulationAreaEdit = true;
                        SceneView sceneView = SceneView.sceneViews[0] as SceneView;
                        // sceneView.Focus();
                        GameObject go = new GameObject();
                        go.layer = LayerMask.NameToLayer("RegulationArea");
                        go.name = LDTTools.GetNumberWithTag("RegulationArea", "�K���G���A");
                        go.tag = "RegulationArea";
                        Selection.activeObject = go;
                        polygonHandler = go.AddComponent<AnyPolygonRegurationAreaHandler>();
                        Debug.Log(_regulationHeight);
                        polygonHandler.SetHeight(_regulationHeight);
                        polygonHandler.SetAreaColor(_areaColor);

                        Repaint();
                    }

                }

                /*
                var ev = Event.current;
                RaycastHit hit;
                if (ev.type == EventType.KeyUp && ev.keyCode == KeyCode.LeftShift)
                {
                    Debug.Log("Shift");
                    Vector3 mousePosition = Event.current.mousePosition;

                    Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        Debug.Log(hit.point);
                    }
                }
                */
            }
            else if (_tabIndex == 1)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<size=15>���]�Ώۂ���̒��]�K���쐬</size>", style);
                EditorGUILayout.HelpBox("���]�Ώےn�_�ł̕��ƍ�����ݒ肵���]�K���쐬���N���b�N���Ă�������", MessageType.Info);

                _screenWidth = EditorGUILayout.FloatField("���]�Ώےn�_�ł̕�", _screenWidth);
                _screenHeight = EditorGUILayout.FloatField("���]�Ώےn�_�ł̍���", _screenHeight);

                if (GUILayout.Button("���]�K���쐬"))
                {
                    LDTTools.CheckLayers();

                    GameObject grp = GameObject.Find("RegurationArea");
                    if (!grp)
                    {
                        grp = new GameObject();
                        grp.name = "RegurationArea";
                        grp.layer = LayerMask.NameToLayer("RegulationArea");

                        RegurationAreaHandler handler = grp.AddComponent<RegurationAreaHandler>();
                        handler.screenHeight = _screenHeight;
                        handler.screenWidth = _screenWidth;

                    }

                }

            }
            else if (_tabIndex == 2)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<size=15>�����K���G���A�쐬</size>", style);
                EditorGUILayout.HelpBox("�����K�����A�̍������a��ݒ肵�^�C�v��I�����ċK���G���A�쐬���N���b�N���Ă�������", MessageType.Info);
                _heightAreaHeight = EditorGUILayout.FloatField("����", _heightAreaHeight);
                _heightAreaRadius = EditorGUILayout.FloatField("���a", _heightAreaRadius);

                if (GUILayout.Button("�����K���G���A�쐬"))
                {
                    GameObject grp = GameObject.Find("HeitRegurationAreaGroup");
                    if (!grp)
                    {
                        grp = new GameObject();
                        grp.name = "HeightRegurationArea";
                        grp.layer = LayerMask.NameToLayer("RegulationArea");

                        HeightRegurationAreaHandler handler = grp.AddComponent<HeightRegurationAreaHandler>();
                        handler.areaHeight = _heightAreaHeight;
                        handler.areaRadius = _heightAreaRadius;

                    }
                }
            }
            else if (_tabIndex == 3)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<size=15>�K���G���A�o��</size>", style);
                List<string> type = new List<string>();
                List<LDTShapeFileHandler> fields = new List<LDTShapeFileHandler>();
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.TextField("�G�N�X�|�[�g��", _regulationAreaExportPath);
                }
                if (GUILayout.Button("�G�N�X�|�[�g��I��"))
                {
                    var selectedPath = EditorUtility.SaveFilePanel("�ۑ���", "", "Shapefile", "shp");
                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        _regulationAreaExportPath = selectedPath;
                    }
                }
                List<LDTTools.AreaType> areaTypes = new List<LDTTools.AreaType>();

                if (GUILayout.Button("�K���G���A�o��"))
                {
                    List<List<Vector2>> contours = new List<List<Vector2>>();
                    GameObject grp = GameObject.Find("AnyPolygonRegurationArea");

                    GameObject[] objects = GameObject.FindGameObjectsWithTag("RegulationArea");
                    string[] types = new string[objects.Length];
                    Color[] cols = new Color[objects.Length];
                    float[] heights = new float[objects.Length];
                    Vector2[,] v2 = new Vector2[objects.Length, 2];
                    for (int i = 0; i < objects.Length; i++)
                    {
                        if (objects[i].GetComponent<AnyPolygonRegurationAreaHandler>())
                        {
                            List<Vector2> p = new List<Vector2>();
                            AnyPolygonRegurationAreaHandler obj = objects[i].GetComponent<AnyPolygonRegurationAreaHandler>();
                            types[i] = "PolygonArea";
                            heights[i] = obj.GetHeight();
                            cols[i] = obj.GetAreaColor();
                            v2[i, 0] = new Vector2(0, 0);
                            v2[i, 1] = new Vector2(0, 0);


                            List<Vector2> cnt = obj.GetVertexData();
                            contours.Add(cnt);

                        }
                    }
                    LDTTools.WriteShapeFile(_regulationAreaExportPath, "RegurationArea", types, cols, heights, v2, contours);

                    /*
                        List<int> instanceList = new List<int>();
                    if (grp)
                    {
                        int narea = grp.transform.childCount;
                        for (int i = 0; i < narea; i++)
                        {
                            GameObject go = grp.transform.GetChild(i).gameObject;
                            instanceList.Add(go.GetInstanceID());
                            ShapeItem handler = go.GetComponent<ShapeItem>();
                            if (handler)
                            {
                                List<Vector2> cnt = handler.Contours;
                                contours.Add(cnt);
                                type.Add("Polygon");
                                fields.Add(handler.fields);

                            }
                        }
                    }
                    grp = GameObject.Find("AnyCircleRegurationArea");
                    if (grp)
                    {
                        int narea = grp.transform.childCount;
                        for (int i = 0; i < narea; i++)
                        {
                            GameObject go = grp.transform.GetChild(i).gameObject;
                            ShapeItem handler = go.GetComponent<ShapeItem>();
                            if (handler)
                            {
                                List<Vector2> cnt = handler.Contours;
                                contours.Add(cnt);
                                type.Add("Circle");
                                fields.Add(handler.fields);
                            }
                        }
                    }
                    */

                }
            }
        }

    }


#endif
}