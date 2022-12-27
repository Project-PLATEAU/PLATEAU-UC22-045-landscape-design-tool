using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LandscapeDesignTool.Editor
{
    /// <summary>
    /// �s�s�v��c�[���𓮍삳����ɂ������ĕK�v�ȃ^�O�ݒ���s���܂��B
    /// </summary>
    public static class TagAdder
    {
        private static readonly Func<Transform, bool> predIsBuilding = trans => trans.name.Contains("_bldg_");
        private static readonly Func<Transform, bool> predIsGround = trans => trans.name.Contains("_dem_");
        private const string LayerNameBuilding = "Building";
        private const string LayerNameGround = "Ground";
        private const int LayerIdBuilding = 29;
        private const int LayerIdGround = 31;

        /// <summary>
        /// �v���W�F�N�g�ݒ�̃��C���[�ݒ�ŁABuilding, Ground ��ݒ肵�܂��B
        /// �V�[�����̖��O�� _bldg_ ���܂ރQ�[���I�u�W�F�N�g�̃^�O�� Building �ɐݒ肵�܂��B
        /// �V�[�����̖��O�� _dem_  ���܂ރQ�[���I�u�W�F�N�g�̃^�O�� Ground   �ɐݒ肵�܂��B
        /// </summary>
        public static void ConfigureTags()
        {
            EditorUtility.DisplayProgressBar("", "�^�O��ݒ蒆�ł�...", 30f);
            try
            {
                ConfigureLayerName();
                SetTagOfBuildingAndGround();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        
        private static void SetTagOfBuildingAndGround()
        {
            var targets = Search(trans => predIsBuilding(trans) || predIsGround(trans));
            foreach (var target in targets)
            {
                if (predIsBuilding(target))
                {
                    SetTagsRecursive(target, LayerIdBuilding);
                }else if (predIsGround(target))
                {
                    SetTagsRecursive(target, LayerIdGround);
                }
            }
        }

        /// <summary>
        /// �J���Ă���e�V�[�����̊e���[�g�I�u�W�F�N�g�ɂ��āA�����Ƃ��̎q��T�����܂��B
        /// <paramref name="pred"/> �ɍ��v������̂����X�g�ŕԂ��܂��B
        /// �������A pred�ɍ��v���� GameObject �̎q�͒T���Ώۂ���O���܂��B
        /// </summary>
        private static List<Transform> Search(Func<Transform, bool> pred)
        {
            int sceneCount = SceneManager.sceneCount;
            var ret = new List<Transform>();
            for (int i = 0; i < sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                var roots = scene.GetRootGameObjects();
                foreach (var root in roots)
                {
                    SearchRecursive(root.transform, ret, pred);
                }
            }

            return ret;
        }

        private static void SearchRecursive(Transform trans, List<Transform> ret, Func<Transform, bool> pred)
        {
            if (pred(trans))
            {
                ret.Add(trans);
                // �����ɍ��v�������̂̎q�܂ł͒T�����܂���B
                return;
            }
            int childCount = trans.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = trans.GetChild(i);
                SearchRecursive(child, ret, pred);
            }
        }

        private static void SetTagsRecursive(Transform trans, int layerId)
        {
            trans.gameObject.layer = layerId;
            int childCount = trans.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = trans.GetChild(i);
                SetTagsRecursive(child, layerId);
            }
        }

        /// <summary>
        /// �v���W�F�N�g�ݒ�̃��C���[����ύX���ABuilding, Ground �Ƃ������O�̃��C���[�����܂��B
        /// �Q�l: <see href="https://forum.unity.com/threads/adding-layer-by-script.41970/#post-2274824"/>
        /// </summary>
        private static void ConfigureLayerName()
        {
            SerializedObject tagManager =
                new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            SerializedProperty layers = tagManager.FindProperty("layers");
            if (layers == null || !layers.isArray)
            {
                Debug.LogWarning(
                    "Can't set up the layers.  It's possible the format of the layers and tags data has changed in this version of Unity.");
                Debug.LogWarning("Layers is null: " + (layers == null));
                return;
            }

            var layersToSet = new (int id, string name)[]
            {
                (LayerIdBuilding, LayerNameBuilding), (LayerIdGround, LayerNameGround)
            };
            foreach (var layerTuple in layersToSet)
            {
                SerializedProperty layerProperty = layers.GetArrayElementAtIndex(layerTuple.id);
                if (layerProperty.stringValue != layerTuple.name)
                {
                    layerProperty.stringValue = layerTuple.name;
                }
            }
        }
    }
}
