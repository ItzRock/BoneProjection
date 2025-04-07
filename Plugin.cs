using HarmonyLib;
using UnityEngine;
using Zorro.Settings;
using Landfall.Modding;
using System.Reflection;
using UnityEngine.Localization;
using Landfall.Haste;
using Unity.Mathematics;
namespace BoneProjection;

[LandfallPlugin]
public class BoneProjectionMod {
    public static Harmony harmony;
    public static string GUID = "AnthonyStai.BoneProjection";
    public static GameObject prefab;
    public static GameObject curZoe;
    public static GameObject curModel;
    public static bool Enabled = true;
    public static float modelScale = 1f;
    public static float zoeScale = 1.2f;
    public static GameObject currentClone;
    public static string AssemblyDirectory {
        get {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }

    static BoneProjectionMod() {
        Debug.Log($"Loading {GUID} running in {AssemblyDirectory}");
        Debug.Log("Loading assetbundles");

        AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(AssemblyDirectory, "your.assetbundle"));
        if (assetBundle == null) {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }

        prefab = assetBundle.LoadAsset<GameObject>("Model");
        prefab.transform.localPosition = Vector3.zero;
        assetBundle.Unload(false);

        harmony = new(GUID);
        harmony.PatchAll();
    }
}

[HarmonyPatch(typeof(PlayerAnimationHandler))]
public class PlayerPatches {
    [HarmonyPatch(nameof(PlayerAnimationHandler.Awake))]
    [HarmonyPostfix]
    private static void AwakePostFix(PlayerAnimationHandler __instance) {
        if (!BoneProjectionMod.Enabled) return;
        GameObject clone = GameObject.Instantiate(BoneProjectionMod.prefab);
        CharacterBoneReference bones = new CharacterBoneReference();
        // Fill out bone based off clone
        
        clone.transform.localScale = Vector3.one * BoneProjectionMod.modelScale;
        BoneProjectionMod.currentClone = clone;
        clone.transform.parent = __instance.gameObject.transform.Find("Visual/");
        clone.transform.localPosition = Vector3.zero;
        clone.transform.localEulerAngles = Vector3.zero;
        GameObject zoeModel = __instance.gameObject.transform.Find("Visual/Courier_Retake").gameObject;
        zoeModel.transform.Find("Courier/Meshes").gameObject.SetActive(false);
        zoeModel.transform.localScale = Vector3.one * BoneProjectionMod.zoeScale;

        CharacterBoneReference sourceBones = new CharacterBoneReference();
        Transform root2 = zoeModel.transform.Find("Courier/Armature/Hip");
        root2.gameObject.SetActive(false);
        // Spine & head
        sourceBones.spine1 = root2.Find("Spine_1");
        sourceBones.spine2 = root2.Find("Spine_1/Spine_2");
        sourceBones.spine3 = root2.Find("Spine_1/Spine_2/Spine_3");
        sourceBones.neck = root2.Find("Spine_1/Spine_2/Spine_3/Neck");
        sourceBones.head = root2.Find("Spine_1/Spine_2/Spine_3/Neck/Head");
        sourceBones.head_top = root2.Find("Spine_1/Spine_2/Spine_3/Neck/Head/Top");

        // Hips base
        sourceBones.hips = root2;

        // Left Leg
        sourceBones.upLegL = root2.Find("Hip_L/Leg_L");
        sourceBones.lowerLegL = root2.Find("Hip_L/Leg_L/Knee_L");
        sourceBones.footL = root2.Find("Hip_L/Leg_L/Knee_L/Foot_L");

        // Right Leg
        sourceBones.upLegR = root2.Find("Hip_R/Leg_R");
        sourceBones.lowerLegR = root2.Find("Hip_R/Leg_R/Knee_R");
        sourceBones.footR = root2.Find("Hip_R/Leg_R/Knee_R/Foot_R");

        // Left Arm
        sourceBones.upperArmL = root2.Find("Spine_1/Spine_2/Spine_3/Shoulder_L/Arm_L");
        sourceBones.armL = root2.Find("Spine_1/Spine_2/Spine_3/Shoulder_L/Arm_L/Elbow_L");
        sourceBones.handL = root2.Find("Spine_1/Spine_2/Spine_3/Shoulder_L/Arm_L/Elbow_L/Hand_L");

        // Right Arm
        sourceBones.upperArmR = root2.Find("Spine_1/Spine_2/Spine_3/Shoulder_R/Arm_R");
        sourceBones.armR = root2.Find("Spine_1/Spine_2/Spine_3/Shoulder_R/Arm_R/Elbow_R");
        sourceBones.handR = root2.Find("Spine_1/Spine_2/Spine_3/Shoulder_R/Arm_R/Elbow_R/Hand_R");


        TiedBones tiedBones = clone.AddComponent<TiedBones>();
        tiedBones.target = bones;
        tiedBones.source = sourceBones;

        tiedBones.Setup();
        BoneProjectionMod.curZoe = zoeModel;
        BoneProjectionMod.curModel = clone;
    }
}

[HasteSetting]
public class ModelSwapSettingFB : OffOnSetting, IExposedSetting {
    public override void ApplyValue() {
        BoneProjectionMod.Enabled = base.Value == OffOnMode.ON;
    }
    public string GetCategory() => "BoneProjection";
    public override OffOnMode GetDefaultValue() {
        return OffOnMode.ON;
    }
    public LocalizedString GetDisplayName() => new UnlocalizedString("Enable Custom Model?");
    public override List<LocalizedString> GetLocalizedChoices() {
        return new List<LocalizedString>
        {
            new LocalizedString("Settings", "DisabledGraphicOption"),
            new LocalizedString("Settings", "EnabledGraphicOption")
        };
    }
}
[HasteSetting]
public class FBModelScale : FloatSetting, IExposedSetting {
    public override void ApplyValue() {
        BoneProjectionMod.modelScale = base.Value;
        if (BoneProjectionMod.curModel != null) BoneProjectionMod.curModel.transform.localScale = Vector3.one * BoneProjectionMod.modelScale;
    }
    public string GetCategory() => "BoneProjection";
    public override float GetDefaultValue() => 1f;
    public LocalizedString GetDisplayName() => new UnlocalizedString("Model Scale");

    public override float2 GetMinMaxValue() => new float2(1, 2);
}
[HasteSetting]
public class ZoeModelScale : FloatSetting, IExposedSetting {
    public override void ApplyValue() {
        BoneProjectionMod.zoeScale = base.Value;
        if (BoneProjectionMod.curZoe != null) BoneProjectionMod.curZoe.transform.localScale = Vector3.one * BoneProjectionMod.zoeScale;
    }
    public string GetCategory() => "BoneProjection";
    public override float GetDefaultValue() => 1.2f;
    public LocalizedString GetDisplayName() => new UnlocalizedString("Reference Model Scale");

    public override float2 GetMinMaxValue() => new float2(1, 2);
}


// I personally would add all of the bones that your model has and then find a similar bone that zoe has.
[System.Serializable]
public class CharacterBoneReference {
    public Transform head;
    public Transform head_top;
    public Transform neck;
    public Transform spine3;
    public Transform spine2;
    public Transform spine1;
    public Transform hips;
    public Transform upLegL;
    public Transform upLegR;
    public Transform lowerLegL;
    public Transform lowerLegR;
    public Transform footL;
    public Transform footR;
    public Transform upperArmL;
    public Transform upperArmR;
    public Transform armL;
    public Transform armR;
    public Transform handL;
    public Transform handR;

    public Transform[] GetBoneArray() {
        return new Transform[] {
            head, head_top, neck,
            spine3, spine2, spine1, hips,
            upLegL, upLegR, lowerLegL, lowerLegR,
            footL, footR,
            upperArmL, upperArmR,
            armL, armR, handL, handR
        };
    }
}

public class TiedBones : MonoBehaviour {
    public CharacterBoneReference source;
    public CharacterBoneReference target;

    private Transform[] sourceBones;
    private Transform[] targetBones;
    private bool ready = false;
    public void Setup() {
        sourceBones = source.GetBoneArray();
        targetBones = target.GetBoneArray();
        ready = true;
    }

    private void LateUpdate() {
        if (!ready) return;
        for (int i = 0; i < targetBones.Length; i++) {
            if (sourceBones[i] != null && targetBones[i] != null) {
                targetBones[i].position = sourceBones[i].position;
                targetBones[i].localScale = sourceBones[i].localScale;
                targetBones[i].rotation = sourceBones[i].rotation;
            }
        }
    }
}
