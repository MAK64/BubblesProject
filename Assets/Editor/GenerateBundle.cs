using UnityEditor;
using UnityEngine;
using System.Collections;

/// <summary>
/// Генерирует бандль из выбранных файлов и сохраняет по указанному пути
/// </summary>
public class GenerateBundle : MonoBehaviour {

    public class ExportAssetBundles
    {
        [MenuItem("Assets/Build AssetBundle")]
        static void ExportResource()
        {
            string path = EditorUtility.SaveFilePanel("Save bundle", Application.dataPath + "/Resources/", "MainResouces", "unity3d");
            if (path.Length != 0)
            {
                Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
                BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path,
                                  BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.UncompressedAssetBundle);
                Selection.objects = selection;
            }
        }
    }
}
