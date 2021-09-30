using UnityEditor;
using UnityEngine;


public class TextureBlendGUI : ShaderGUI
{
    MaterialProperty MainTex = null;
    MaterialProperty MainColor = null;
    MaterialProperty MainSAM = null;
    MaterialProperty MainSmt = null;
    MaterialProperty MainNormal = null;
    MaterialProperty MainNormalInt = null;
    MaterialProperty MainTilingOffset = null;

    MaterialProperty Tex1 = null;
    MaterialProperty Tex1Color = null;
    MaterialProperty Tex1SAM = null;
    MaterialProperty Tex1Smt = null;
    MaterialProperty Tex1Normal = null;
    MaterialProperty Tex1NormalInt = null;
    MaterialProperty Tex1TilingOffset = null;

    MaterialProperty BlendNoise1 = null;
    MaterialProperty BlendNoise1contrast = null;
    MaterialProperty BlendNoise1TilingOffset = null;

    MaterialProperty Tex2 = null;
    MaterialProperty Tex2Color = null;
    MaterialProperty Tex2SAM = null;
    MaterialProperty Tex2Smt = null;
    MaterialProperty Tex2Normal = null;
    MaterialProperty Tex2NormalInt = null;
    MaterialProperty Tex2TilingOffset = null;

    MaterialProperty BlendNoise2 = null;
    MaterialProperty BlendNoise2contrast = null;
    MaterialProperty BlendNoise2TilingOffset = null;

    MaterialProperty Tex3 = null;
    MaterialProperty Tex3Color = null;
    MaterialProperty Tex3SAM = null;
    MaterialProperty Tex3Smt = null;
    MaterialProperty Tex3Normal = null;
    MaterialProperty Tex3NormalInt = null;
    MaterialProperty Tex3TilingOffset = null;

    MaterialProperty BlendNoise3 = null;
    MaterialProperty BlendNoise3contrast = null;
    MaterialProperty BlendNoise3TilingOffset = null;



    public class Styles
    {
        
        public static readonly GUIContent TexGUI = new GUIContent();
        public static GUIContent getTexGUI(string tex) 
        {
            TexGUI.text = tex;
            return TexGUI;
        }

        public static GUIContent getTexGUIcolor(string tex,Color col)
        {
            TexGUI.text = tex;
            GUI.contentColor = col;
            return TexGUI;
        }

    }
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        EditorGUI.BeginChangeCheck();

        MainTex = FindProperty("Main_Albedo_Black", properties); 
        MainColor = FindProperty("Main_Color", properties);
        MainSAM = FindProperty("Main_SAM", properties);
        MainSmt = FindProperty("Main_Smoothness", properties);
        MainNormal = FindProperty("Main_Normal", properties);
        MainNormalInt = FindProperty("Main_Normal_Intensity", properties);
        MainTilingOffset = FindProperty("Main_Tiling_Offset", properties);

        Tex1 = FindProperty("Tex1_Albedo_Red", properties);
        Tex1Color = FindProperty("Tex1_Color", properties);
        Tex1SAM = FindProperty("Tex1_SAM", properties);
        Tex1Smt = FindProperty("Tex1_Smoothness", properties);
        Tex1Normal = FindProperty("Tex1_Normal", properties);
        Tex1NormalInt = FindProperty("Tex1_Normal_Intensity", properties);
        Tex1TilingOffset = FindProperty("Tex1_Tiling_Offset", properties);

        BlendNoise1 = FindProperty("MAIN_TEX1_Blend_Noise_R", properties);
        BlendNoise1contrast = FindProperty("MAIN_TEX1_Blend_Contrast", properties);
        BlendNoise1TilingOffset = FindProperty("MAIN_TEX1_Blend_TilingOffset", properties);

        Tex2 = FindProperty("Tex2_Albedo_Green", properties);
        Tex2Color = FindProperty("Tex2_Color", properties);
        Tex2SAM = FindProperty("Tex2_SAM", properties);
        Tex2Smt = FindProperty("Tex2_Smoothness", properties);
        Tex2Normal = FindProperty("Tex2_Normal", properties);
        Tex2NormalInt = FindProperty("Tex2_Normal_Intensity", properties);
        Tex2TilingOffset = FindProperty("Tex2_Tiling_Offset", properties);

        BlendNoise2 = FindProperty("TEX2_Blend_Noise_G", properties);
        BlendNoise2contrast = FindProperty("TEX2_Blend_Contrast", properties);
        BlendNoise2TilingOffset = FindProperty("TEX2_Blend_TilingOffset", properties);

        Tex3 = FindProperty("Tex3_Albedo_Blue", properties);
        Tex3Color = FindProperty("Tex3_Color", properties);
        Tex3SAM = FindProperty("Tex3_SAM", properties);
        Tex3Smt = FindProperty("Tex3_Smoothness", properties);
        Tex3Normal = FindProperty("Tex3_Normal", properties);
        Tex3NormalInt = FindProperty("Tex3_Normal_Intensity", properties);
        Tex3TilingOffset = FindProperty("Tex3_Tiling_Offset", properties);

        BlendNoise3 = FindProperty("TEX3_Blend_Noise_B", properties);
        BlendNoise3contrast = FindProperty("TEX3_Blend_Contrast", properties);
        BlendNoise3TilingOffset = FindProperty("TEX3_Blend_TilingOffset", properties);


        //EditorGUILayout.LabelField("----------- Tex 2 -----------", EditorStyles.largeLabel, GUI.contentColor = Color.red);

        
        
        EditorGUILayout.LabelField(Styles.getTexGUI("----------- Main -----------"), EditorStyles.boldLabel);
        /// Main ///
        materialEditor.TexturePropertySingleLine(Styles.getTexGUI("MainTextrue"), MainTex, MainColor, MainTilingOffset);
       
        EditorGUILayout.Space(10);
        materialEditor.TexturePropertySingleLine(Styles.getTexGUI("Main Smt,AO,Metalic"), MainSAM);
        materialEditor.RangeProperty(MainSmt, "MainTex Smoothness");

        EditorGUILayout.Space(10);
        materialEditor.TexturePropertySingleLine(Styles.getTexGUI("Main Normal"), MainNormal, MainNormalInt);
        EditorGUILayout.Space(20);

        EditorGUILayout.LabelField(Styles.getTexGUIcolor(("----------- Tex 1 (R)-----------"),Color.red), EditorStyles.boldLabel);
        GUI.contentColor = Color.white;
        /// Tex 1 ///
        materialEditor.TexturePropertySingleLine(Styles.getTexGUI("Textrue 1"), Tex1, Tex1Color, Tex1TilingOffset);

        EditorGUILayout.Space(10);
        materialEditor.TexturePropertySingleLine(Styles.getTexGUI("Tex1 Smt,AO,Metalic"), Tex1SAM);
        materialEditor.RangeProperty(Tex1Smt, "Tex1Tex Smoothness");

        EditorGUILayout.Space(10);
        materialEditor.TexturePropertySingleLine(Styles.getTexGUI("Tex1 Normal"), Tex1Normal, Tex1NormalInt);

        EditorGUILayout.Space(20);



        EditorGUILayout.LabelField(Styles.getTexGUIcolor(("----------- Tex 2 (G)-----------"), Color.green), EditorStyles.boldLabel);

        GUI.contentColor = Color.white;
        /// Tex 2 ///
        materialEditor.TexturePropertySingleLine(Styles.getTexGUI("Textrue 2"), Tex2, Tex2Color, Tex2TilingOffset);

        EditorGUILayout.Space(10);
        materialEditor.TexturePropertySingleLine(Styles.getTexGUI("Tex2 Smt,AO,Metalic"), Tex2SAM);
        materialEditor.RangeProperty(Tex2Smt, "Tex2Tex Smoothness");

        EditorGUILayout.Space(10);
        materialEditor.TexturePropertySingleLine(Styles.getTexGUI("Tex2 Normal"), Tex2Normal, Tex2NormalInt);

        EditorGUILayout.Space(20);

        EditorGUILayout.LabelField(Styles.getTexGUIcolor(("----------- Tex 3 (B)-----------"), Color.blue), EditorStyles.boldLabel);
        GUI.contentColor = Color.white;
        /// Tex 3 ///
        materialEditor.TexturePropertySingleLine(Styles.getTexGUI("Textrue 3"), Tex3, Tex3Color, Tex3TilingOffset);

        EditorGUILayout.Space(10);
        materialEditor.TexturePropertySingleLine(Styles.getTexGUI("Tex3 Smt,AO,Metalic"), Tex3SAM);
        materialEditor.RangeProperty(Tex3Smt, "Tex3Tex Smoothness");

        EditorGUILayout.Space(10);
        materialEditor.TexturePropertySingleLine(Styles.getTexGUI("Tex3 Normal"), Tex3Normal, Tex3NormalInt);

        EditorGUILayout.Space(40);

        EditorGUILayout.LabelField(Styles.getTexGUIcolor(("----------- RGB BlendNoise -----------"), Color.white), EditorStyles.boldLabel);
        /// Blend Noise 1 ///
        materialEditor.TexturePropertySingleLine(Styles.getTexGUI("Blend Texture 1"), BlendNoise1, BlendNoise1TilingOffset);
        materialEditor.RangeProperty(BlendNoise1contrast, "BlendTexture1 Contrast");
        EditorGUILayout.Space(20);

        /// Blend Noise 2 ///
        materialEditor.TexturePropertySingleLine(Styles.getTexGUI("Blend Texture 2"), BlendNoise2, BlendNoise2TilingOffset);
        materialEditor.RangeProperty(BlendNoise2contrast, "BlendTexture2 Contrast");
        EditorGUILayout.Space(20);

        /// Blend Noise 3 ///
        materialEditor.TexturePropertySingleLine(Styles.getTexGUI("Blend Texture 3 "), BlendNoise3, BlendNoise3TilingOffset);
        materialEditor.RangeProperty(BlendNoise3contrast, "BlendTexture3 Contrast");

    }

}
