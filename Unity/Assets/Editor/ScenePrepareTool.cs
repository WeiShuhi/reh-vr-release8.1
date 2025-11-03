using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

public class ScenePrepareTool : EditorWindow
{
    private string bundlesScenePath = "Assets/Bundles/Scenes";
    private string bundlesDepPath = "Assets/Bundles/Dependencies";

    [MenuItem("Tools/Scene/Prepare Scene For Bundles")]
    public static void ShowWindow()
    {
        GetWindow<ScenePrepareTool>("Prepare Scene");
    }

    private void OnGUI()
    {
        GUILayout.Label("打包前场景清理与导出", EditorStyles.boldLabel);

        if (GUILayout.Button("清理当前场景并导出到 Bundles"))
        {
            PrepareScene();
        }
    }

    private void PrepareScene()
    {
        Scene scene = EditorSceneManager.GetActiveScene();

        // 自动保存未保存场景
        if (scene.isDirty)
        {
            Debug.Log("场景有修改，自动保存中...");
            EditorSceneManager.SaveScene(scene);
        }

        CleanMissingScripts();
        CleanDebugObjects();

        CopySceneToBundles(scene);
        CopyDependencies(scene);

        AssetDatabase.Refresh();
        Debug.Log("✅ 场景清理与导出完成！");
    }

    private void CleanMissingScripts()
    {
        int count = 0;
        var allObjs = GameObject.FindObjectsOfType<GameObject>();
        foreach (var go in allObjs)
        {
            var components = go.GetComponents<Component>();
            foreach (var c in components)
            {
                if (c == null)
                {
                    count++;
                    GameObject.DestroyImmediate(c);
                }
            }
        }

        Debug.Log($"✅ 清理 Missing Script 完成，共移除 {count} 个组件");
    }

    private void CleanDebugObjects()
    {
        int removed = 0;
        var allObjs = GameObject.FindObjectsOfType<GameObject>();
        foreach (var go in allObjs)
        {
            if (go.name.StartsWith("Debug") || go.tag == "EditorOnly")
            {
                GameObject.DestroyImmediate(go);
                removed++;
            }
        }

        Debug.Log($"✅ 清理调试对象完成，共移除 {removed} 个对象");
    }

    private void CopySceneToBundles(Scene scene)
    {
        Directory.CreateDirectory(bundlesScenePath);
        string sceneName = Path.GetFileName(scene.path);
        string targetPath = Path.Combine(bundlesScenePath, sceneName);

        // 如果文件已存在，跳过
        if (File.Exists(targetPath))
        {
            Debug.Log($"跳过已存在场景: {targetPath}");
            return;
        }

        FileUtil.CopyFileOrDirectory(scene.path, targetPath);
        Debug.Log($"✅ 场景已复制到 Bundles: {targetPath}");
    }


    private void CopyDependencies(Scene scene)
    {
        Directory.CreateDirectory(bundlesDepPath);
        string depSceneFolder = Path.Combine(bundlesDepPath, Path.GetFileNameWithoutExtension(scene.path));
        if (!Directory.Exists(depSceneFolder))
            Directory.CreateDirectory(depSceneFolder);

        var deps = AssetDatabase.GetDependencies(scene.path, true)
            .Where(p => !p.Contains("/Editor/") && !p.Contains(".editor/") && !p.EndsWith(".cs"))
            .Distinct()
            .ToArray();

        int count = 0;
        foreach (var dep in deps)
        {
            string fileName = Path.GetFileName(dep);
            string typeFolder = GetAssetTypeFolder(dep);
            string targetDir = Path.Combine(depSceneFolder, typeFolder);
            string newPath = Path.Combine(targetDir, fileName);

            CopyFileSafe(dep, newPath);
            count++;
        }

        Debug.Log($"✅ 已导出 {count} 个依赖文件到 {depSceneFolder}");
    }

    private void CopyFileSafe(string sourcePath, string targetPath)
    {
        // 确保目标目录存在
        string targetDir = Path.GetDirectoryName(targetPath);
        if (!Directory.Exists(targetDir))
            Directory.CreateDirectory(targetDir);

        // 已存在则跳过
        if (File.Exists(targetPath))
        {
            Debug.Log($"跳过已存在文件: {targetPath}");
            return;
        }

        try
        {
            File.Copy(sourcePath, targetPath, false);
        }
        catch (IOException ex)
        {
            Debug.LogError($"复制失败: {sourcePath} -> {targetPath}\n{ex.Message}");
        }
    }

    private string GetAssetTypeFolder(string assetPath)
    {
        var type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
        if (type == typeof(GameObject))
            return "Prefabs";
        if (type == typeof(Material))
            return "Materials";
        if (type == typeof(Texture2D))
            return "Textures";
        if (type == typeof(AudioClip))
            return "Audio";
        if (type == typeof(Shader))
            return "Shaders";
        return "Others";
    }
}
