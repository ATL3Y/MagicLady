using UnityEngine;
using UnityEditor;

public class CustomOutlineShaderGUI : ShaderGUI
{
    public override void OnGUI ( MaterialEditor materialEditor, MaterialProperty [ ] properties )
    {
        MaterialProperty outlineColor = ShaderGUI.FindProperty("_OutlineColor", properties);
        materialEditor.ShaderProperty ( outlineColor, outlineColor.displayName );
        base.OnGUI ( materialEditor, properties );
    }
}
