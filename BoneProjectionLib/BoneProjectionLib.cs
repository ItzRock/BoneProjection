using UnityEngine;
namespace BoneProjectionLib {
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

    public class BoneProjector : MonoBehaviour {
        public CharacterBoneReference source;
        public CharacterBoneReference target;
        private Transform[] sourceBones;
        private Transform[] targetBones;
        private bool ready = false;

        public float armsHeight = 0;
        public float armsDistance = 0f;
        public bool doRotation = true;
        public bool doScale = true;
        public bool doPosition = true;

        public void SourceZoe(Transform ZoeHip) {
            Debug.Log($"IS HIP NULL? {ZoeHip == null}");
            CharacterBoneReference sourceBones = new CharacterBoneReference();
            Transform root2 = ZoeHip;
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
            sourceBones.shoulderL = root2.Find("Spine_1/Spine_2/Spine_3/Shoulder_L");
            // Right Arm
            sourceBones.upperArmR = root2.Find("Spine_1/Spine_2/Spine_3/Shoulder_R/Arm_R");
            sourceBones.armR = root2.Find("Spine_1/Spine_2/Spine_3/Shoulder_R/Arm_R/Elbow_R");
            sourceBones.handR = root2.Find("Spine_1/Spine_2/Spine_3/Shoulder_R/Arm_R/Elbow_R/Hand_R");
            sourceBones.shoulderR = root2.Find("Spine_1/Spine_2/Spine_3/Shoulder_R");

            source = sourceBones;
        }
        public void TargetMixamo(Transform rig) {
            CharacterBoneReference targetBones = new CharacterBoneReference();
            Transform root1 = rig.Find("mixamorig:Hips");

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

            target = targetBones;
        }
        public void Setup() {
            sourceBones = source.GetBoneArray();
            targetBones = target.GetBoneArray();
            ready = true;
        }

        private void LateUpdate() {
            if (!ready) return;
            for (int i = 0; i < targetBones.Length; i++) {
                if (sourceBones[i] != null && targetBones[i] != null) {
                    if (doPosition) targetBones[i].position = sourceBones[i].position;
                    // I'm sure theres a better way to do this
                    if (i == 13 || i == 14) targetBones[i].position = new Vector3(sourceBones[i].position.x, sourceBones[i].position.y + armsHeight, sourceBones[i].position.z);
                    if (i == 15 || i == 17 || i == 19) targetBones[i].position = (targetBones[6].right * armsDistance) + targetBones[i].position;
                    if (i == 16 | i == 18 || i == 20) targetBones[i].position = (-targetBones[6].right * armsDistance) + targetBones[i].position;

                    if (doScale) targetBones[i].localScale = sourceBones[i].localScale;
                    if (doRotation) targetBones[i].rotation = sourceBones[i].rotation;
                }
            }
        }
    }
}