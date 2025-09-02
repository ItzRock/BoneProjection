using UnityEngine.Localization;
using Unity.Mathematics;
using Landfall.Haste;
using Zorro.Settings;
using UnityEngine;

namespace FailboatModel;
[HasteSetting]
public class ModelSwapSettingPT : OffOnSetting, IExposedSetting {
    public override void ApplyValue() {
        BoneProjectionModelSwap.Enabled = base.Value == OffOnMode.ON;
    }
    public string GetCategory() => BoneProjectionModelSwap.category;
    public override OffOnMode GetDefaultValue() {
        return OffOnMode.ON;
    }
    public LocalizedString GetDisplayName() => new UnlocalizedString("Enable Model?");
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
        BoneProjectionModelSwap.modelScale = base.Value;
        if (BoneProjectionModelSwap.curModel != null) BoneProjectionModelSwap.curModel.transform.localScale = Vector3.one * base.Value;
    }
    public string GetCategory() => BoneProjectionModelSwap.category;
    public override float GetDefaultValue() => 1f;
    public LocalizedString GetDisplayName() => new UnlocalizedString("Model Scale");

    public override float2 GetMinMaxValue() => new float2(1, 2);
}
[HasteSetting]
public class ZoeModelScale : FloatSetting, IExposedSetting {
    public override void ApplyValue() {
        BoneProjectionModelSwap.zoeScale = base.Value;
        if (BoneProjectionModelSwap.curZoe != null) BoneProjectionModelSwap.curZoe.transform.localScale = Vector3.one * base.Value;
    }
    public string GetCategory() => BoneProjectionModelSwap.category;
    public override float GetDefaultValue() => 1f;
    public LocalizedString GetDisplayName() => new UnlocalizedString("Reference Model Scale");

    public override float2 GetMinMaxValue() => new float2(1, 2);
}
[HasteSetting]
public class ArmHeight : FloatSetting, IExposedSetting {
    public override void ApplyValue() {
        BoneProjectionModelSwap.armHeight = base.Value;
        if (BoneProjectionModelSwap.currentProjector != null) BoneProjectionModelSwap.currentProjector.armsHeight = base.Value;    
    }
    public string GetCategory() => BoneProjectionModelSwap.category;
    public override float GetDefaultValue() => 0f;
    public LocalizedString GetDisplayName() => new UnlocalizedString("Arm Height");

    public override float2 GetMinMaxValue() => new float2(-2, 2);
}
[HasteSetting]
public class ArmDistance : FloatSetting, IExposedSetting {
    public override void ApplyValue() {
        BoneProjectionModelSwap.armHeight = base.Value;
        if (BoneProjectionModelSwap.currentProjector != null) BoneProjectionModelSwap.currentProjector.armsDistance = base.Value;
    }
    public string GetCategory() => BoneProjectionModelSwap.category;
    public override float GetDefaultValue() => 0f;
    public LocalizedString GetDisplayName() => new UnlocalizedString("Arm Distance");

    public override float2 GetMinMaxValue() => new float2(-2, 2);
}
