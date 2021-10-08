using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

class VolumeLightPass : ScriptableRenderPass
{
    // used to label this pass in Unity's Frame Debug utility
    string profilerTag;

    Material materialToBlit;
    RenderTargetIdentifier cameraColorTargetIdent;
    RenderTargetHandle tempTexture;
    int _divideFactor;

    public VolumeLightPass(string profilerTag, int divideFactor,
        RenderPassEvent renderPassEvent, Material materialToBlit)
    {
        this.profilerTag = profilerTag;
        this._divideFactor = divideFactor;
        this.renderPassEvent = renderPassEvent;
        this.materialToBlit = materialToBlit;
        //this.volRenderer = GameObject.Find("MainCamera").GetComponent<VolumetricLightRenderer>();
    }

    // This isn't part of the ScriptableRenderPass class and is our own addition.
    // For this custom pass we need the camera's color target, so that gets passed in.
    public void Setup(RenderTargetIdentifier cameraColorTargetIdent)
    {
        this.cameraColorTargetIdent = cameraColorTargetIdent;
    }

    // called each frame before Execute, use it to set up things the pass will need
    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        cameraTextureDescriptor.width /= _divideFactor;
        cameraTextureDescriptor.height /= _divideFactor;
      // create a temporary render texture that matches the camera
      cmd.GetTemporaryRT(tempTexture.id, cameraTextureDescriptor);
    }

    // Execute is called for every eligible camera every frame. It's not called at the moment that
    // rendering is actually taking place, so don't directly execute rendering commands here.
    // Instead use the methods on ScriptableRenderContext to set up instructions.
    // RenderingData provides a bunch of (not very well documented) information about the scene
    // and what's being rendered.

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if(renderingData.lightData.mainLightIndex < 0)
            return;

      // fetch a command buffer to use
        CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
        cmd.Clear();

        // // the actual content of our custom render pass!
        // // we apply our material while blitting to a temporary texture
        
        cmd.Blit(cameraColorTargetIdent, tempTexture.Identifier(), materialToBlit, -1);
        //cmd.SetGlobalFloat("_check",1f);
        // var tex = (RenderTexture)Shader.GetGlobalTexture("_MainLightShadowmapTexture");
        // cmd.SetGlobalTexture("_testTex",tex);
        var mainLight = renderingData.lightData.visibleLights[renderingData.lightData.mainLightIndex].light;
        materialToBlit.SetVector("_CamPosition",renderingData.cameraData.camera.transform.position);
        materialToBlit.SetVector("_LightDirection",mainLight.transform.forward);
        // materialToBlit.SetMatrix("_InvProjectionMatrix",_camera.projectionMatrix.inverse);
        // materialToBlit.SetMatrix("_ViewToWorld",_camera.cameraToWorldMatrix);


        // // // ...then blit it back again 
        cmd.Blit(tempTexture.Identifier(), cameraColorTargetIdent);

        // don't forget to tell ScriptableRenderContext to actually execute the commands
        context.ExecuteCommandBuffer(cmd);

        // tidy up after ourselves
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    // called after Execute, use it to clean up anything allocated in Configure
    public override void FrameCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(tempTexture.id);
    }
}