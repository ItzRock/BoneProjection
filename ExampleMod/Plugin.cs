using HarmonyLib;
using UnityEngine;
using Zorro.Settings;
using Landfall.Modding;
using System.Reflection;
using UnityEngine.Localization;
using Landfall.Haste;
using Unity.Mathematics;
using TiedBones;
namespace PeterBrokenWorlds;

[LandfallPlugin]
public class PeterBrokenWorlds {
    public static Harmony harmony;
    public static string GUID = "AnthonyStai.PeterBrokenWorlds";
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

    static PeterBrokenWorlds() {
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

[HarmonyPatch(typeof(PlayerAnimationHandler))]
public class PlayerPatches {
    [HarmonyPatch(nameof(PlayerAnimationHandler.Awake))]
    [HarmonyPostfix]
    private static void AwakePostFix(PlayerAnimationHandler __instance) {
        if (!PeterBrokenWorlds.Enabled) return;
        GameObject clone = GameObject.Instantiate(PeterBrokenWorlds.prefab);
        clone.transform.localScale = Vector3.one * PeterBrokenWorlds.modelScale;
        PeterBrokenWorlds.currentClone = clone;
        clone.transform.parent = __instance.gameObject.transform.Find("Visual/");
        clone.transform.localPosition = Vector3.zero;
        clone.transform.localEulerAngles = Vector3.zero;
        GameObject zoeModel = __instance.gameObject.transform.Find("Visual/Courier_Retake").gameObject;
        zoeModel.transform.Find("Courier/Meshes").gameObject.SetActive(false);
        zoeModel.transform.localScale = Vector3.one * PeterBrokenWorlds.zoeScale;

        CharacterBoneReference targetBones = new CharacterBoneReference();
        Transform root1 = clone.transform.Find("Griffin/mixamorig:Hips");

        // Spine & Head
        targetBones.hips = root1;
        targetBones.spine1 = root1.Find("mixamorig:Spine");
        targetBones.spine2 = root1.Find("mixamorig:Spine/mixamorig:Spine1");
        targetBones.spine3 = root1.Find("mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2");
        targetBones.neck = root1.Find("mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck");
        targetBones.head = root1.Find("mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head");
        targetBones.head_top = root1.Find("mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head/mixamorig:HeadTop_End");

        // Left Leg
        targetBones.upLegL = root1.Find("mixamorig:LeftUpLeg");
        targetBones.lowerLegL = root1.Find("mixamorig:LeftUpLeg/mixamorig:LeftLeg");
        targetBones.footL = root1.Find("mixamorig:LeftUpLeg/mixamorig:LeftLeg/mixamorig:LeftFoot");

        // Right Leg
        targetBones.upLegR = root1.Find("mixamorig:RightUpLeg");
        targetBones.lowerLegR = root1.Find("mixamorig:RightUpLeg/mixamorig:RightLeg");
        targetBones.footR = root1.Find("mixamorig:RightUpLeg/mixamorig:RightLeg/mixamorig:RightFoot");

        // Left Arm
        targetBones.shoulderL = root1.Find("mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder");
        targetBones.upperArmL = targetBones.shoulderL.Find("mixamorig:LeftArm");
        targetBones.armL = targetBones.upperArmL.Find("mixamorig:LeftForeArm");
        targetBones.handL = targetBones.armL.Find("mixamorig:LeftHand");

        // Right Arm
        targetBones.shoulderR = root1.Find("mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder");
        targetBones.upperArmR = targetBones.shoulderR.Find("mixamorig:RightArm");
        targetBones.armR = targetBones.upperArmR.Find("mixamorig:RightForeArm");
        targetBones.handR = targetBones.armR.Find("mixamorig:RightHand");


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
        //sourceBones.shoulderL = root2.Find("Spine_1/Spine_2/Spine_3/Shoulder_L");
        // Right Arm
        sourceBones.upperArmR = root2.Find("Spine_1/Spine_2/Spine_3/Shoulder_R/Arm_R");
        sourceBones.armR = root2.Find("Spine_1/Spine_2/Spine_3/Shoulder_R/Arm_R/Elbow_R");
        sourceBones.handR = root2.Find("Spine_1/Spine_2/Spine_3/Shoulder_R/Arm_R/Elbow_R/Hand_R");
        //sourceBones.shoulderR = root2.Find("Spine_1/Spine_2/Spine_3/Shoulder_R");

        TiedBones.TiedBones tiedBones = clone.GetComponent<TiedBones.TiedBones>();
        tiedBones.source = sourceBones;
        tiedBones.target = targetBones;
        tiedBones.Setup();
        PeterBrokenWorlds.curZoe = zoeModel;
        PeterBrokenWorlds.curModel = clone;
    }
}

[HasteSetting]
public class ModelSwapSettingPT : OffOnSetting, IExposedSetting {
    public override void ApplyValue() {
        PeterBrokenWorlds.Enabled = base.Value == OffOnMode.ON;
    }
    public string GetCategory() => "PeterMod";
    public override OffOnMode GetDefaultValue() {
        return OffOnMode.ON;
    }
    public LocalizedString GetDisplayName() => new UnlocalizedString("Enable Peter Model?");
    public override List<LocalizedString> GetLocalizedChoices() {
        return new List<LocalizedString>
        {
            new LocalizedString("Settings", "DisabledGraphicOption"),
            new LocalizedString("Settings", "EnabledGraphicOption")
        };
    }
}
[HasteSetting]
public class PTModelScale : FloatSetting, IExposedSetting {
    public override void ApplyValue() {
        PeterBrokenWorlds.modelScale = base.Value;
        if (PeterBrokenWorlds.curModel != null) PeterBrokenWorlds.curModel.transform.localScale = Vector3.one * PeterBrokenWorlds.modelScale;
    }
    public string GetCategory() => "PeterMod";
    public override float GetDefaultValue() => 1f;
    public LocalizedString GetDisplayName() => new UnlocalizedString("Model Scale");

    public override float2 GetMinMaxValue() => new float2(1, 2);
}
[HasteSetting]
public class ZoeModelScale : FloatSetting, IExposedSetting {
    public override void ApplyValue() {
        PeterBrokenWorlds.zoeScale = base.Value;
        if (PeterBrokenWorlds.curZoe != null) PeterBrokenWorlds.curZoe.transform.localScale = Vector3.one * PeterBrokenWorlds.zoeScale;
    }
    public string GetCategory() => "PeterMod";
    public override float GetDefaultValue() => 1.2f;
    public LocalizedString GetDisplayName() => new UnlocalizedString("Reference Model Scale");

    public override float2 GetMinMaxValue() => new float2(1, 2);
}