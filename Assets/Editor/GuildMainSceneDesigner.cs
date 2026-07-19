using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class GuildMainSceneDesigner
{
    private const string ScenePath = "Assets/01. Scenes/02. Game.unity";

    public static string Apply()
    {
        Directory.CreateDirectory("Assets/Resources/UI");
        if (File.Exists("image/background_1.png"))
            File.Copy("image/background_1.png", "Assets/Resources/UI/background_1.png", true);
        File.Copy("Assets/08. Fonts/Pretendard-Bold SDF.asset", "Assets/Resources/UI/Pretendard-Bold SDF.asset", true);
        if (File.Exists("Assets/Resources/UI/background_1.png"))
            AssetDatabase.ImportAsset("Assets/Resources/UI/background_1.png", ImportAssetOptions.ForceUpdate);
        AssetDatabase.ImportAsset("Assets/Resources/UI/Pretendard-Bold SDF.asset", ImportAssetOptions.ForceUpdate);
        if (File.Exists("Assets/Resources/UI/background_1.png"))
            ConfigureSprite("Assets/Resources/UI/background_1.png", false);

        var scene = EditorSceneManager.OpenScene(ScenePath);
        var canvas = Object.FindFirstObjectByType<Canvas>();
        var scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = .5f;

        string[] legacyNames =
        {
            "ReferenceLayout", "ReceptionGameRoot", "ResumeScrollButton", "Panel_Hud",
            "Panel_Visitor", "Panel_Quest", "Panel_Insurance", "Panel_Item", "Panel_Decision"
        };
        foreach (var name in legacyNames)
        {
            var child = canvas.transform.Find(name);
            if (child != null) Object.DestroyImmediate(child.gameObject);
        }

        var flow = canvas.GetComponent<GuildReceptionFlow>();
        if (flow == null) flow = canvas.gameObject.AddComponent<GuildReceptionFlow>();
        var serializedFlow = new SerializedObject(flow);
        var portraits = serializedFlow.FindProperty("visitorPortraits");
        portraits.arraySize = 6;
        for (var i = 0; i < portraits.arraySize; i++)
        {
            var path = $"Assets/03. Images/Customer/Customer_{i + 1}.png";
            portraits.GetArrayElementAtIndex(i).objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
        serializedFlow.ApplyModifiedPropertiesWithoutUndo();
        flow.BuildEditorLayout();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        return "Rebuilt 02. Game for the plan.md investigation and party-assignment loop.";
    }

    private static void ConfigureSprite(string path, bool alpha)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = alpha;
        importer.mipmapEnabled = false;
        importer.SaveAndReimport();
    }
}
