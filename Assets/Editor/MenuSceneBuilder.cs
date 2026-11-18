using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

// Builds Assets/Scenes/MainMenu.unity from code: Tools > Nightline > Build Menu Scene.
// Re-run it after changing any screen's Build() method.
public static class MenuSceneBuilder
{
    const string ScenePath = "Assets/Scenes/MainMenu.unity";
    const string ThemePath = "Assets/UI/UITheme.asset";

    [MenuItem("Tools/Nightline/Import TMP Essentials")]
    public static void ImportTmp()
    {
        if (File.Exists("Assets/TextMesh Pro/Resources/TMP Settings.asset")) return;
        var pkg = Directory.GetFiles("Packages", "TMP Essential Resources.unitypackage", SearchOption.AllDirectories).FirstOrDefault()
                  ?? Directory.GetFiles("Library/PackageCache", "TMP Essential Resources.unitypackage", SearchOption.AllDirectories).FirstOrDefault();
        AssetDatabase.ImportPackage(pkg, false);
    }


    [MenuItem("Tools/Nightline/Build Menu Scene")]
    public static void Build()
    {
        var theme = MakeTheme();
        UIKit.T = theme;

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        var cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Palette.Bg;
        cam.transform.position = new Vector3(0, 0, -10);

        var es = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));

        var canvasGo = new GameObject("MenuCanvas", typeof(RectTransform)) { layer = 5 };
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = cam;
        canvas.planeDistance = 5;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand; // the whole 1920x1080 layout always fits
        canvasGo.AddComponent<GraphicRaycaster>();

        var bg = UIKit.Img(canvasGo.transform, "Background", Palette.Bg);
        bg.RT().Stretch();

        var menu = canvasGo.AddComponent<MenuManager>();
        menu.theme = theme;
        menu.title = Screen<TitleScreen>(canvasGo, "01 Title");
        menu.main = Screen<MainMenuScreen>(canvasGo, "02 Main Menu");
        menu.modeSelect = Screen<ModeSelectScreen>(canvasGo, "03 Mode Select");
        menu.trackSelect = Screen<TrackSelectScreen>(canvasGo, "04 Track Select");
        menu.garage = Screen<GarageScreen>(canvasGo, "05 Garage");
        menu.shop = Screen<ShopScreen>(canvasGo, "06 Dealership");
        menu.settings = Screen<SettingsScreen>(canvasGo, "07 Settings");
        menu.race = Screen<RaceScreen>(canvasGo, "08 Race + Pause");
        menu.results = Screen<ResultsScreen>(canvasGo, "09 Results");
        menu.credits = Screen<CreditsScreen>(canvasGo, "10 Credits");

        var dlg = UIKit.New("ConfirmDialog", canvasGo.transform).gameObject.AddComponent<ConfirmDialog>();
        dlg.Build();
        dlg.gameObject.SetActive(false);
        menu.dialog = dlg;
        var toast = UIKit.New("Toast", canvasGo.transform).gameObject.AddComponent<Toast>();
        toast.Build();
        toast.gameObject.SetActive(false);
        menu.toast = toast;

        // only keep the title screen visible in the editor
        foreach (Transform t in canvasGo.transform)
            if (t.GetComponent<UIScreen>() != null) t.gameObject.SetActive(t.name.StartsWith("01"));

        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        AssetDatabase.SaveAssets();
        Debug.Log("Menu scene built: " + ScenePath);
    }

    static T Screen<T>(GameObject canvas, string name) where T : UIScreen
    {
        var r = UIKit.New(name, canvas.transform).Stretch();
        r.gameObject.AddComponent<CanvasGroup>();
        var s = r.gameObject.AddComponent<T>();
        s.Build();
        return s;
    }

    static UITheme MakeTheme()
    {
        Directory.CreateDirectory("Assets/UI");
        var theme = AssetDatabase.LoadAssetAtPath<UITheme>(ThemePath);
        if (theme == null)
        {
            theme = ScriptableObject.CreateInstance<UITheme>();
            AssetDatabase.CreateAsset(theme, ThemePath);
        }
        theme.heading = FontAsset("Rajdhani-Bold");
        theme.body = FontAsset("Barlow-Regular");
        theme.bodyBold = FontAsset("Barlow-SemiBold");

        // Unity's own built-in UI sprites for the rounded boxes and circles
        theme.round = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        theme.circle = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        theme.icons = Folder("Icons");
        EditorUtility.SetDirty(theme);
        AssetDatabase.SaveAssets();
        return theme;
    }

    static Sprite[] Folder(string folder) =>
        AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/Art/" + folder })
            .Select(g => AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(g)))
            .OrderBy(s => s.name).ToArray();

    static TMP_FontAsset FontAsset(string file)
    {
        string path = "Assets/Fonts/" + file + " SDF.asset";
        var fa = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
        if (fa != null) return fa;

        var font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/" + file + ".ttf");
        fa = TMP_FontAsset.CreateFontAsset(font, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic, true);
        fa.name = file + " SDF";
        AssetDatabase.CreateAsset(fa, path);
        fa.atlasTextures[0].name = file + " Atlas";
        AssetDatabase.AddObjectToAsset(fa.atlasTextures[0], fa);
        fa.material.name = file + " Material";
        AssetDatabase.AddObjectToAsset(fa.material, fa);
        fa.TryAddCharacters(" !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~·×●›");
        EditorUtility.SetDirty(fa);
        AssetDatabase.SaveAssets();
        return fa;
    }
}

// Icons under Assets/Art are imported as sprites. The Material icons come in black,
// so they are turned white on import, which lets each Image tint them any colour.
public class ArtImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith("Assets/Art/")) return;
        var ti = (TextureImporter)assetImporter;
        ti.textureType = TextureImporterType.Sprite;
        ti.spriteImportMode = SpriteImportMode.Single;
        ti.mipmapEnabled = false;
        ti.alphaIsTransparency = true;
        ti.textureCompression = TextureImporterCompression.Uncompressed;
    }

    void OnPostprocessTexture(Texture2D tex)
    {
        if (!assetPath.StartsWith("Assets/Art/")) return;
        var px = tex.GetPixels();
        for (int i = 0; i < px.Length; i++) px[i] = new Color(1, 1, 1, px[i].a);
        tex.SetPixels(px);
        tex.Apply();
    }
}
