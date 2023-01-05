using PLATEAU.Network;
using UnityEditor;
using UnityEditor.PackageManager;

namespace LandscapeDesignTool.Editor
{
    /// <summary>
    /// Packages�t�H���_�ɓ����Ă��� �i�ς܂��Â���c�[�� �� tarball �`���ŏo�͂��܂��B
    /// �f�v���C�ŗ��p���܂��B
    /// </summary>
    public static class PackagePacker
    {
        [MenuItem("PLATEAU/�i�ς܂��Â���/�J���Ҍ���/Package��tarball�ɏo��")]
        public static void Pack()
        {
            var destDir = EditorUtility.SaveFolderPanel("�o�͐�", "", "");
            if (string.IsNullOrEmpty(destDir)) return;
            UnityEditor.PackageManager.Client.Pack("Packages/com.synesthesias.landscape-design-tool", destDir);
        }
    }
}
