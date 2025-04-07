using UnityEngine;
namespace TiedBones;

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
    public Transform shoulderL;
    public Transform shoulderR;
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
            upperArmL, upperArmR,shoulderL,shoulderR,
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
