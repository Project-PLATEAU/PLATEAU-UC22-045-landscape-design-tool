




// TODO�@���b�N�A���Ƃŏ���

// using System;
// using UnityEditor;
// using UnityEngine;
//
// namespace LandscapeDesignTool.Editor
// {
//     
//     [Obsolete]
//     public class LandscapeDesignWindow : EditorWindow
//     {
//         [MenuItem("PLATEAU/�i�ς܂��Â���/�i�όv��")]
//         public static void ShowWindow()
//         {
//             EditorWindow.GetWindow(typeof(LandscapeDesignWindow), true, "�i�όv��");
//         }
//
//         private void OnGUI()
//         {
//             DrawAddRegulationAreaPanel();
//             //DrawEditRegulationAreaPanel();
//             //DrawSimulateDesignPanel();
//             //DrawLoadShapeFilePanel();
//         }
//
//         private void DrawAddRegulationAreaPanel()
//         {
//             using (new EditorGUILayout.HorizontalScope())
//             {
//                 GUILayout.Label("�K���^�C�v�I��");
//                 string[] regulationAreaTypes =
//                 {
//                     "�K���G���A",
//                     "���]�K��"
//                 };
//                 EditorGUILayout.Popup(0, regulationAreaTypes);
//             }
//
//             GUILayout.Space(80);
//
//             GUILayout.Button("�ǉ�");
//
//         }
//
//         private void DrawEditRegulationAreaPanel()
//         {
//             EditorGUILayout.Toggle("��ʕ\��", true);
//             using (new EditorGUILayout.HorizontalScope())
//             {
//                 GUILayout.Label("�`��w����@");
//                 string[] shapeType =
//                 {
//                     "���p�`�I��",
//                     "�~�`�I��"
//                 };
//                 EditorGUILayout.Popup(0, shapeType);
//             }
//
//             GUILayout.Button("�`��ҏW");
//             EditorGUILayout.FloatField("��������", 50);
//             EditorGUILayout.FloatField("���a", 50);
//
//             GUILayout.Label("�s�s���f���ւ̔��f", EditorStyles.boldLabel);
//             using (new EditorGUILayout.HorizontalScope())
//             {
//                 GUILayout.Button("���������ŃN���b�s���O");
//                 GUILayout.Button("�N���b�s���O������");
//             }
//         }
//
//         private void DrawSimulateDesignPanel()
//         {
//             GUILayout.Label("���z����I�����Ă�������");
//             GUILayout.Button("�ӏ��ύX");
//         }
//
//         private void DrawLoadShapeFilePanel()
//         {
//             GUILayout.Label("�t�@�C���I��", EditorStyles.boldLabel);
//             using (new EditorGUILayout.HorizontalScope())
//             {
//                 GUILayout.TextField("");
//
//                 var skin = new GUIStyle(GUI.skin.button)
//                 {
//                     fixedWidth = 150f
//                 };
//                 GUILayout.Button("shp�t�@�C����I��...", skin);
//             }
//
//             GUILayout.Space(10f);
//
//             GUILayout.Label("�ǂݍ��ݐݒ�", EditorStyles.boldLabel);
//
//             using (new EditorGUILayout.HorizontalScope())
//             {
//                 GUILayout.Label("���������̑���");
//                 string[] shapeType =
//                 {
//                     "����"
//                 };
//                 EditorGUILayout.Popup(0, shapeType);
//             }
//
//             GUILayout.Space(10f);
//
//             GUILayout.Button("�ǂݍ���");
//         }
//     }
// }
