﻿using HarmonyLib;
using UnityEngine;
using BoneProjectionLib;

namespace PeterBrokenWorlds;
// bound to this because it will always be on the player when they spawn.
[HarmonyPatch(typeof(PlayerAnimationHandler))]
public class PlayerPatches {
    [HarmonyPatch(nameof(PlayerAnimationHandler.Awake))]
    [HarmonyPostfix]
    private static void AwakePostFix(PlayerAnimationHandler __instance) {
        if (!BoneProjectionModelSwap.Enabled) return;
        // Setup Clone
        GameObject clone = GameObject.Instantiate(BoneProjectionModelSwap.prefab);
        clone.transform.localScale = Vector3.one * BoneProjectionModelSwap.modelScale;
        clone.transform.parent = __instance.gameObject.transform.Find("Visual/");
        clone.transform.localPosition = Vector3.zero;
        clone.transform.localEulerAngles = Vector3.zero;

        // Setup Zoe Model
        GameObject zoeModel = __instance.gameObject.transform.Find("Visual/Courier_Retake").gameObject;
        zoeModel.transform.Find("Courier/Meshes").gameObject.SetActive(false);
        zoeModel.transform.localScale = Vector3.one * BoneProjectionModelSwap.zoeScale;

        // Do the Projection
        BoneProjector boneProjector = clone.AddComponent<BoneProjector>();
        boneProjector.armsHeight = BoneProjectionModelSwap.armHeight;
        boneProjector.armsDistance = BoneProjectionModelSwap.armDistance;
        boneProjector.TargetMixamo(clone.transform.Find("Griffin"));
        boneProjector.SourceZoe(__instance.transform);
        boneProjector.source.shoulderL = null; // Shoulders don't look too pretty on peter so we'll remove them
        boneProjector.source.shoulderR = null; 
        boneProjector.source.hips.gameObject.SetActive(false); // To hide her bags and stuff, Everything still works.
        boneProjector.Setup();
        BoneProjectionModelSwap.currentProjector = boneProjector;
        BoneProjectionModelSwap.curZoe = zoeModel;
        BoneProjectionModelSwap.curModel = clone;
    }
}