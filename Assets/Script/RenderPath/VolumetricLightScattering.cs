using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class VolumetricLightScatteringSettings
{
    [Header("Properties")]
    [Range(0.1f, 1f)]
    public float resolutionScale = 0.5f;

    [Range(0.0f, 1.0f)]
    public float intensity = 0.25f;

    [Range(0.0f, 1.0f)]
    public float blurWidth = 0.85f;

}
public class VolumetricLightScattering : ScriptableRendererFeature
{
    class LightScatteringPass : ScriptableRenderPass
    {

        private readonly RenderTargetHandle occluders = RenderTargetHandle.CameraTarget;

        private readonly float resolutionScale;
        private readonly float intensity;
        private readonly float blurWidth;
        private readonly Material occludersMaterial;

        public LightScatteringPass(VolumetricLightScatteringSettings settings)
        {
            occludersMaterial = new Material(Shader.Find("Hidden/KSC/UnlitColor"));

            occluders.Init("_OccludersMap");
            resolutionScale = settings.resolutionScale;
            intensity = settings.intensity;
            blurWidth = settings.blurWidth;
        }


        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // 1
            RenderTextureDescriptor cameraTextureDescriptor =
                renderingData.cameraData.cameraTargetDescriptor;

            // 2
            cameraTextureDescriptor.depthBufferBits = 0;

            // 3
            cameraTextureDescriptor.width = Mathf.RoundToInt(
                cameraTextureDescriptor.width * resolutionScale);
            cameraTextureDescriptor.height = Mathf.RoundToInt(
                cameraTextureDescriptor.height * resolutionScale);

            // 4
            cmd.GetTemporaryRT(occluders.id, cameraTextureDescriptor,
                FilterMode.Bilinear);

            // 5
            ConfigureTarget(occluders.Identifier());
        }


        //https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {

        }

    
        public override void OnCameraCleanup(CommandBuffer cmd)
        {


        }
    }

    LightScatteringPass m_ScriptablePass;


    public VolumetricLightScatteringSettings settings = new VolumetricLightScatteringSettings();
    /// <inheritdoc/>
    public override void Create() // 기능이 처음 로드되면 호출, 모든 인스턴스를 만들고 구성하는데 사용합니다.
    {


        m_ScriptablePass = new LightScatteringPass(settings);
        m_ScriptablePass.renderPassEvent =
        RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        // 카메라 한 대당 한 번씩 모든 프레임을 호출합니다. 이 도구를 사용해서 SRP 인스턴스를 Render에게 주입합니다.
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


