using System.Collections;
using System.Collections.Generic;
using LandscapeDesignTool.Editor.WindowTabs;
using UnityEngine;
using UnityEditor;



namespace LandscapeDesignTool.Editor
{
#if UNITY_EDITOR
    public class LandscapeDesign :EditorWindow
    {
        float _heightAreaHeight = 30.0f;
        float _heightAreaRadius = 100.0f;

        string _regulationAreaExportPath = "";
        

        // Start is called before the first frame update


        private readonly string[] _tabToggles = { "���_��쐬", "�K���G���A�쐬", "���]�K���쐬", "�����K���G���A�쐬", "ShapeFile�Ǎ�", "ShapeFile�����o��" };
        private int _tabIndex;
        private readonly TabViewPointGenerate _tabViewPointGenerate = new TabViewPointGenerate();
        private readonly TabRegulationAreaGenerate _tabRegulationAreaGenerate;
        private readonly TabShapefileLoad _tabShapefileLoad = new TabShapefileLoad();

        private readonly TabViewportRegulationGenerate _tabViewportRegulationGenerate =
            new TabViewportRegulationGenerate();

        public LandscapeDesign()
        {
            _tabRegulationAreaGenerate = new TabRegulationAreaGenerate(this);
        }

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
            _tabRegulationAreaGenerate.OnSceneGUI();
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
                _tabViewPointGenerate.Draw(style);    
            }
            else if (_tabIndex == 1)
            {
                _tabRegulationAreaGenerate.OnGUI(style);
                
            }
            else if (_tabIndex == 2)
            {
                _tabViewportRegulationGenerate.Draw(style);
            }
            else if (_tabIndex == 3)
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
            else if (_tabIndex == 4)
            {
                _tabShapefileLoad.Draw(style);
            }
            else if (_tabIndex == 5)
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