using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LandscapeDesignTool.Editor
{
    public class LandscapeDesign : EditorWindow
    {

        int _regurationType;
        float _regurationHeight;
        float _screenWidth = 80.0f;
        float _screenHeight = 80.0f;
        float _heightAreaHeight = 30.0f;
        float _heightAreaRadius = 100.0f;

        string _regurationAreaFileName = "";

        // Start is called before the first frame update

        private readonly string[] _tabToggles = { "�K���G���A�쐬", "���]�K���쐬", "�����K���G���A�쐬", "ShapeFile�����o��" };
        private int _tabIndex;

        [MenuItem("Sandbox/�i�ς܂��Â���/�i�όv��")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(LandscapeDesign), true, "�i�όv����");
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
                _regurationHeight = EditorGUILayout.FloatField("����", 10);

                _regurationType = EditorGUILayout.Popup(_regurationType, options);
                if (GUILayout.Button("�K���G���A�쐬"))
                {

                    LDTTools.CheckLayers();
                    if (_regurationType == 0)
                    {
                        GameObject grp = GameObject.Find("AnyPolygonRegurationArea");
                        if (!grp)
                        {
                            grp = new GameObject();
                            grp.name = "AnyPolygonRegurationArea";
                            grp.layer = LayerMask.NameToLayer("RegulationArea");

                            AnyPolygonRegurationAreaHandler handler = grp.AddComponent<AnyPolygonRegurationAreaHandler>();
                            handler.areaHeight = _regurationHeight;
                        }
                    }
                    else
                    {
                        GameObject grp = GameObject.Find("AnyCirclnRegurationArea");
                        if (!grp)
                        {
                            grp = new GameObject();
                            grp.name = "AnyCircleRegurationArea";
                            grp.layer = LayerMask.NameToLayer("RegulationArea");

                            AnyCircleRegurationAreaHandler handler = grp.AddComponent<AnyCircleRegurationAreaHandler>();
                            handler.areaHeight = _regurationHeight;



                        }
                    }

                }
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
                _regurationAreaFileName = EditorGUILayout.TextField("�t�@�C����", _regurationAreaFileName);
                if (GUILayout.Button("�K���G���A�o��"))
                {
                    List<List<Vector2>> contours = new List<List<Vector2>>();
                    GameObject grp = GameObject.Find("AnyPolygonRegurationArea");
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
                            }
                        }
                    }

                    LDTTools.WriteShapeFile(_regurationAreaFileName, "RArea", contours);
                }
            }
        }
    }
}
