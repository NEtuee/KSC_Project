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
    public override void Create() // ����� ó�� �ε�Ǹ� ȣ��, ��� �ν��Ͻ��� ����� �����ϴµ� ����մϴ�.
    {


        m_ScriptablePass = new LightScatteringPass(settings);
        m_ScriptablePass.renderPassEvent =
        RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        // ī�޶� �� ��� �� ���� ��� �������� ȣ���մϴ�. �� ������ ����ؼ� SRP �ν��Ͻ��� Render���� �����մϴ�.
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


