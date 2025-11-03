using UnityEngine;
using UnityEditor;
using System.IO;

public class BatchSetAssetBundleName : EditorWindow
{
    private string rootPath = "Assets/Bundles/Dependencies"; // 依赖根目录

    [MenuItem("YOOSSET/Batch Set AssetBundleName")]
    public static void ShowWindow()
    {
        GetWindow<BatchSetAssetBundleName>("批量设置AssetBundleName");
    }

    private void OnGUI()
    {
        GUILayout.Label("批量设置 AssetBundleName", EditorStyles.boldLabel);

        rootPath = EditorGUILayout.TextField("依赖根目录", rootPath);

        if (GUILayout.Button("执行批量设置"))
        {
            SetBundleNames(rootPath);
        }
    }

    private static void SetBundleNames(string path)
    {
        if (!Directory.Exists(path))
        {
            Debug.LogError($"目录不存在: {path}");
            return;
        }

        foreach (var typeFolder in Directory.GetDirectories(path))
        {
            string bundleName = Path.GetFileName(typeFolder).ToLower() + "_bundle";

            foreach (var file in Directory.GetFiles(typeFolder, "*.*", SearchOption.AllDirectories))
            {
                if (file.EndsWith(".meta")) continue;

                string assetPath = file.Replace("\\", "/");
                AssetImporter importer = AssetImporter.GetAtPath(assetPath);
                if (importer != null)
                    importer.assetBundleName = bundleName;
            }

            Debug.Log($"✅ {bundleName} 设置完成");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✅ 批量设置 AssetBundleName 完成！");
    }
}