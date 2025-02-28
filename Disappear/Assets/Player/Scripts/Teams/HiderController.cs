using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using WAG.Multiplayer;

namespace WAG.Player.Teams
{
    public class HiderController : MonoBehaviour
    {
        [SerializeField] private int HiderLife;
        [SerializeField] private float transparencyThreshold;

        public NGOPlayerSync sync { get; private set; }
        private List<Material> hiderMaterial = new List<Material>();
        private DecalProjector shadowProjector;
        private float shadowOpacity;
        private float maxRefractionSpeed;
        private float transparencyPercentage;
        private float oldTransparency = 0;
        private static readonly int Opacity = Shader.PropertyToID("_Opacity");

        public bool IsMine()
        {
            return sync.IsLocalPlayer;
        }

        protected void Awake()
        {
            sync = GetComponent<NGOPlayerSync>();
        }

        public void Init(int maxLife, float hiderMaxRefractionSpeed, float threshold)
        {
            HiderLife = maxLife;
            this.maxRefractionSpeed = hiderMaxRefractionSpeed;
            transparencyThreshold = threshold;
        }

        public void SetMaterials(SkinnedMeshRenderer[] renderers, DecalProjector proj)
        {
            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                Material newMat = renderer.material;
                hiderMaterial.Add(newMat);
            }

            shadowProjector = proj;
        }

        private void Update()
        {
            transparencyPercentage = Mathf.Floor(10 * sync.RPCVelocity.Value.magnitude / maxRefractionSpeed) / 10.0f;

            if (!(Mathf.Abs(transparencyPercentage - oldTransparency) > transparencyThreshold)) return;
            float currentTransparency = Mathf.Clamp(1 - transparencyPercentage, 0, 1);
            foreach (Material mat in hiderMaterial)
                mat.SetFloat(Opacity, currentTransparency);
            shadowProjector.fadeFactor = currentTransparency;
            oldTransparency = transparencyPercentage;
        }

    }
}