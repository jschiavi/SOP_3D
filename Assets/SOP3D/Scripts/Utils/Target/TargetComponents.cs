using UnityEngine;
using Sop.Utils;
using System;

namespace Sop.Utils
{
    public class TargetComponents : MonoBehaviour
    {

        public Material m_HitMaterial;

        public VRInteractiveItem m_BullsEye;
        public Renderer m_BullsEyeRenderer;

        public VRInteractiveItem m_Inner;
        public Renderer m_InnerRenderer;

        public VRInteractiveItem m_Mid;
        public Renderer m_MidRenderer;

        public VRInteractiveItem m_Outer;
        public Renderer m_OuterRenderer;

        public VRInteractiveItem m_Outermost;
        public Renderer m_OutermostRenderer;

        public event Action BullsEyeHit;
        public event Action BullsEyeMiss;

        public event Action InnerHit;
        public event Action InnerMiss;

        public event Action MidHit;
        public event Action MidMiss;

        public event Action OuterHit;
        public event Action OuterMiss;

        public event Action OutermostHit;
        public event Action OutermostMiss;

        Material m_BullsEyeMaterial;
        Material m_InnerMaterial;
        Material m_MidMaterial;
        Material m_OuterMaterial;
        Material m_OutermostMaterial;

        void Awake()
        {
            m_BullsEyeMaterial = m_BullsEyeRenderer.material;
            m_InnerMaterial = m_InnerRenderer.material;
            m_MidMaterial = m_MidRenderer.material;
            m_OuterMaterial = m_OuterRenderer.material;
            m_OutermostMaterial = m_OutermostRenderer.material;
        }

        void Start()
        {

        }

        void OnEnable()
        {
            m_BullsEye.OnOver += HandleBullsEyeOver;
            m_BullsEye.OnOut += HandleBullsEyeOut;

            m_Inner.OnOver += HandleInnerOver;
            m_Inner.OnOut += HandleInnerOut;

            m_Mid.OnOver += HandleMidOver;
            m_Mid.OnOut += HandleMidOut;

            m_Outer.OnOver += HandleOuterOver;
            m_Outer.OnOut += HandleOuterOut;

            m_Outermost.OnOver += HandleOutermostOver;
            m_Outermost.OnOut += HandleOutermostOut;
        }

        void OnDisable()
        {
            m_BullsEye.OnOver -= HandleBullsEyeOver;
            m_BullsEye.OnOut -= HandleBullsEyeOut;

            m_Inner.OnOver -= HandleInnerOver;
            m_Inner.OnOut -= HandleInnerOut;

            m_Mid.OnOver -= HandleMidOver;
            m_Mid.OnOut -= HandleMidOut;

            m_Outer.OnOver -= HandleOuterOver;
            m_Outer.OnOut -= HandleOuterOut;

            m_Outermost.OnOver -= HandleOutermostOver;
            m_Outermost.OnOut -= HandleOutermostOut;
        }

        void Update()
        {

        }

        void HandleBullsEyeOver()
        {
            m_BullsEyeRenderer.material = m_HitMaterial;
            if (BullsEyeHit != null)
                BullsEyeHit();
        }

        void HandleInnerOver()
        {
            m_InnerRenderer.material = m_HitMaterial;

            if (InnerHit != null)
                InnerHit();
        }

        void HandleMidOver()
        {
            m_MidRenderer.material = m_HitMaterial;

            if (MidHit != null)
                MidHit();
        }

        void HandleOuterOver()
        {
            m_OuterRenderer.material = m_HitMaterial;

            if (OuterHit != null)
                OuterHit();
        }

        void HandleOutermostOver()
        {
            m_OutermostRenderer.material = m_HitMaterial;
            if (OutermostHit != null)
                OutermostHit();
        }

        void HandleBullsEyeOut()
        {
            m_BullsEyeRenderer.material = m_BullsEyeMaterial;
            if (BullsEyeMiss != null)
                BullsEyeMiss();
        }

        void HandleInnerOut()
        {
            m_InnerRenderer.material = m_InnerMaterial;
            if (InnerMiss != null)
                InnerMiss();
        }

        void HandleMidOut()
        {
            m_MidRenderer.material = m_MidMaterial;
            if (MidMiss != null)
                MidMiss();
        }

        void HandleOuterOut()
        {
            m_OuterRenderer.material = m_OuterMaterial;
            if (OuterMiss != null)
                OuterMiss();
        }

        void HandleOutermostOut()
        {
            m_OutermostRenderer.material = m_OutermostMaterial;
            if (OutermostMiss != null)
                OutermostMiss();
        }

        public void Toggle()
        {
            m_BullsEyeRenderer.enabled = !m_BullsEyeRenderer.enabled;
            m_InnerRenderer.enabled = !m_InnerRenderer.enabled;
            m_MidRenderer.enabled = !m_MidRenderer.enabled;
            m_OuterRenderer.enabled = !m_OuterRenderer.enabled;
            m_OutermostRenderer.enabled = !m_OutermostRenderer.enabled;
        }

        public void Show()
        {
            m_BullsEyeRenderer.enabled = true;
            m_InnerRenderer.enabled = true;
            m_MidRenderer.enabled = true;
            m_OuterRenderer.enabled = true;
            m_OutermostRenderer.enabled = true;
        }

        public void Hide()
        {
            m_BullsEyeRenderer.enabled = false;
            m_InnerRenderer.enabled = false;
            m_MidRenderer.enabled = false;
            m_OuterRenderer.enabled = false;
            m_OutermostRenderer.enabled = false;
        }
    }
}

