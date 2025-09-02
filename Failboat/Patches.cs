using HarmonyLib;
using UnityEngine;
using BoneProjectionLib;

namespace FailboatModel;
// bound to this because it will always be on the player when they spawn.
[HarmonyPatch(typeof(PlayerSkinSetter))]
public class PlayerPatches {
    [HarmonyPatch(nameof(PlayerSkinSetter.Awake))]
    [HarmonyPostfix]
    private static void AwakePostFix(PlayerSkinSetter __instance) {
        Debug.Log("LOADING PLAYER MODEL");
        if (!BoneProjectionModelSwap.Enabled) return;
        // Setup Clone
        GameObject clone = GameObject.Instantiate(BoneProjectionModelSwap.prefab);
        clone.transform.localScale = Vector3.one * BoneProjectionModelSwap.modelScale;
        clone.transform.parent = __instance.transform;
        clone.transform.localPosition = Vector3.zero;
        clone.transform.localEulerAngles = Vector3.zero;

        // Setup Zoe Model
        GameObject zoeModel = __instance.gameObject;
       // zoeModel.transform.Find("Courier/Armature").gameObject.SetActive(false);
        zoeModel.transform.localScale = Vector3.one * BoneProjectionModelSwap.zoeScale;

        // Do the Projection
        BoneProjector boneProjector = clone.AddComponent<BoneProjector>();
        boneProjector.armsHeight = BoneProjectionModelSwap.armHeight;
        boneProjector.armsDistance = BoneProjectionModelSwap.armDistance;
        CharacterBoneReference bones = new CharacterBoneReference();

        Transform root = clone.transform.Find("Failboat");

        bones.hips = root.Find("spine/hip");

        bones.spine1 = root.Find("spine");
        bones.spine2 = root.Find("spine.002");
        bones.spine3 = root.Find("spine.003");

        bones.neck = root.Find("spine.003/neck");
        bones.head = root.Find("spine.003/neck/head");
        bones.head_top = root.Find("spine.003/neck/head/head_end");

        // Left Leg
        bones.upLegL = root.Find("spine/hip/thigh.L");
        bones.lowerLegL = root.Find("spine/hip/thigh.L/shin.L");
        bones.footL = null;

        // Right Leg
        bones.upLegR = root.Find("spine/hip/thigh.R");
        bones.lowerLegR = root.Find("spine/hip/thigh.R/shin.R");
        bones.footR = null;

        // Left Arm
        bones.upperArmL = root.Find("spine/spine.002/spine.003/upper_arm.L");
        bones.armL = root.Find("spine/spine.002/spine.003/upper_arm.L/forearm.L");
        bones.handL = root.Find("spine/spine.002/spine.003/upper_arm.L/forearm.L/hand.L");

        // Right Arm
        bones.upperArmR = root.Find("spine/spine.002/spine.003/upper_arm.R");
        bones.armR = root.Find("spine/spine.002/spine.003/upper_arm.R/forearm.R");
        bones.handR = root.Find("spine/spine.002/spine.003/upper_arm.R/forearm.R/hand.R");


        boneProjector.target = bones;
        boneProjector.SourceZoe(__instance.transform.Find("Courier/Armature/Hip"));
        boneProjector.Setup();
        BoneProjectionModelSwap.currentProjector = boneProjector;
        BoneProjectionModelSwap.curZoe = zoeModel;
        BoneProjectionModelSwap.curModel = clone;
    }
}