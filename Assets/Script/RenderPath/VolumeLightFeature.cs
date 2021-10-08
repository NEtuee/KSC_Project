using UnityEngine;
using UnityEngine.Rendering.Universal;

public class VolumeLightFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class MyFeatureSettings
    {
      // we're free to put whatever we want here, public fields will be exposed in the inspector
      public bool IsEnabled = true;
      public int divider = 1;
      public RenderPassEvent WhenToInsert = RenderPassEvent.AfterRendering;
      public Material MaterialToBlit;
    }

    // MUST be named "settings" (lowercase) to be shown in the Render Features inspector
    public MyFeatureSettings settings = new MyFeatureSettings();

    RenderTargetHandle renderTextureHandle;
    VolumeLightPass myRenderPass;

    public override void Create()
    {
      myRenderPass = new VolumeLightPass(
        "My custom pass",
        settings.divider,
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

      var cameraColorTargetIdent = renderer.cameraColorTarget;
      myRenderPass.Setup(cameraColorTargetIdent);

      // Ask the renderer to add our pass.
      // Could queue up multiple passes and/or pick passes to use
      renderer.EnqueuePass(myRenderPass);
    }
}