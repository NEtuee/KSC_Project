using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RadialBlurBlitFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class FeatureSettings
    {
        // we're free to put whatever we want here, public fields will be exposed in the inspector
        public bool IsEnabled = true;
        public RenderPassEvent WhenToInsert = RenderPassEvent.AfterRendering;
        public Material MaterialToBlit;
    }

    // MUST be named "settings" (lowercase) to be shown in the Render Features inspector
    public FeatureSettings settings = new FeatureSettings();

    RenderTargetHandle renderTextureHandle;
    RadialBlurBlitRenderPass myRenderPass;

    public CameraManager cameraManager;

    private void Awake()
    {
        cameraManager = GameObject.Find("CameraManager").GetComponent<CameraManager>();
        if(cameraManager == null)
        {
            Debug.LogError("Not Set CameraManger");
        }
    }

    public override void Create()
    {
        myRenderPass = new RadialBlurBlitRenderPass(
            "My custom pass",
            settings.WhenToInsert,
            settings.MaterialToBlit
        );
    }
  
    // called every frame once per camera
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!settings.IsEnabled)
        {
            return;
        }

        if (renderingData.cameraData.camera != Camera.main)
        {
            return;
        }

        if (!(Application.isPlaying && cameraManager.CheckScreenEffectActive(name)))
        {
            return;
        }
        


        var cameraColorTargetIdent = renderer.cameraColorTarget;
        myRenderPass.Setup(cameraColorTargetIdent);

        // Ask the renderer to add our pass.
        // Could queue up multiple passes and/or pick passes to use
        renderer.EnqueuePass(myRenderPass);
    }
}
