using HarmonyLib;
using UnityEngine;
using Landfall.Modding;
using System.Reflection;
namespace PeterBrokenWorlds; // Change this to your namespace

[LandfallPlugin]
public class BoneProjectionModelSwap {
    public static Harmony harmony;
    public static string GUID = "AnthonyStai.PeterBrokenWorlds"; // change this as well
    public static string category = "Peter Model"; // and this

    public static GameObject prefab;
    public static GameObject curZoe;
    public static GameObject curModel;
    public static BoneProjectionLib.BoneProjector currentProjector;

    public static bool Enabled = true;
    public static float modelScale = 1f;
    public static float zoeScale = 1.2f;
    public static float armHeight = 0f;

    public static string AssemblyDirectory {
        get {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }

    static BoneProjectionModelSwap() {
        Debug.Log($"Loading {GUID} running in {AssemblyDirectory}");
        Debug.Log("Loading assetbundles");

        AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(AssemblyDirectory, "peter.assetbundle"));
        if (assetBundle == null) {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }

        prefab = assetBundle.LoadAsset<GameObject>("Peter");
        prefab.transform.localPosition = Vector3.zero;
        assetBundle.Unload(false);

        harmony = new(GUID);
        harmony.PatchAll();
    }
}