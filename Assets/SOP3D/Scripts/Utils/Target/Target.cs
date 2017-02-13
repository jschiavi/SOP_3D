//=============================================================================
// This file is part of the SOP3D program.
//
// For information about this application contact Terek Arce at
// tarce@cise.ufl.edu
//
// THIS CODE AND INFORMATION ARE PROVIDED ""AS IS"" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//=============================================================================


using UnityEngine;
using RealSpace3D;

namespace Sop.Utils
{

    public class Target : MonoBehaviour
    {

        public TargetComponents m_TargetComponents;
        public RealSpace3D_AudioSource m_Audio;

        bool m_BullsEye;
        bool m_Inner;
        bool m_Mid;
        bool m_Outer;
        bool m_Outermost;

        bool m_GUIEnabled;
        bool m_Measure;

        bool m_SoundLoaded;

        float m_FrameCount = 0f;

        public float FrameCount
        {
            get { return m_FrameCount; }
        }

        float m_BullsEyeCount = 0f;
        float m_InnerCount = 0f;
        float m_MidCount = 0f;
        float m_OuterCount = 0f;
        float m_OutermostCount = 0f;

        public float BullsEyeCount
        {
            get { return m_BullsEyeCount; }
        }

        public float InnerCount
        {
            get { return m_InnerCount; }
        }

        public float MidCount
        {
            get { return m_MidCount; }
        }

        public float OuterCount
        {
            get { return m_OuterCount; }
        }

        public float OutermostCount
        {
            get { return m_OutermostCount; }
        }

        float m_BullsEyeAccuracy;
        float m_InnerAccuracy;
        float m_MidAccuracy;
        float m_OuterAccuracy;
        float m_OutermostAccuracy;

        void Awake()
        {
        }

        void Start()
        {
            m_GUIEnabled = true;
        }

        void OnEnable()
        {
            m_TargetComponents.BullsEyeHit += HandleBullsEyeHit;
            m_TargetComponents.BullsEyeMiss += HandleBullsEyeMiss;

            m_TargetComponents.InnerHit += HandleInnerHit;
            m_TargetComponents.InnerMiss += HandleInnerMiss;

            m_TargetComponents.MidHit += HandleMidHit;
            m_TargetComponents.MidMiss += HandleMidMiss;

            m_TargetComponents.OuterHit += HandleOuterHit;
            m_TargetComponents.OuterMiss += HandleOuterMiss;

            m_TargetComponents.OutermostHit += HandleOutermostHit;
            m_TargetComponents.OutermostMiss += HandleOutermostMiss;
        }

        void OnDisable()
        {
            m_TargetComponents.BullsEyeHit -= HandleBullsEyeHit;
            m_TargetComponents.BullsEyeMiss -= HandleBullsEyeMiss;

            m_TargetComponents.InnerHit -= HandleInnerHit;
            m_TargetComponents.InnerMiss -= HandleInnerMiss;

            m_TargetComponents.MidHit -= HandleMidHit;
            m_TargetComponents.MidMiss -= HandleMidMiss;

            m_TargetComponents.OuterHit -= HandleOuterHit;
            m_TargetComponents.OuterMiss -= HandleOuterMiss;

            m_TargetComponents.OutermostHit -= HandleOutermostHit;
            m_TargetComponents.OutermostMiss -= HandleOutermostMiss;
        }

        // Update is called once per frame
        void Update()
        {
            transform.LookAt(Camera.main.transform.position);

            if (Input.GetButtonDown("TargetToggle"))
            {
                m_TargetComponents.Toggle();
            }

            if (m_Measure)
            {
                if (Input.GetButtonDown("TargetGUI"))
                {
                    m_GUIEnabled = !m_GUIEnabled;
                }

                m_FrameCount += 1.0f;

                if (m_Outermost)
                {
                    m_OutermostCount += 1.0f;
                }

                if (m_Outer)
                {
                    m_OutermostCount += 1.0f;
                    m_OuterCount += 1.0f;
                }

                if (m_Mid)
                {
                    m_OutermostCount += 1.0f;
                    m_OuterCount += 1.0f;
                    m_MidCount += 1.0f;
                }

                if (m_Inner)
                {
                    m_OutermostCount += 1.0f;
                    m_OuterCount += 1.0f;
                    m_MidCount += 1.0f;
                    m_InnerCount += 1.0f;
                }

                if (m_BullsEye)
                {
                    m_OutermostCount += 1.0f;
                    m_OuterCount += 1.0f;
                    m_MidCount += 1.0f;
                    m_InnerCount += 1.0f;
                    m_BullsEyeCount += 1.0f;
                }
            }

        }

        void HandleBullsEyeHit()
        {
            m_BullsEye = true;
        }

        void HandleInnerHit()
        {
            m_Inner = true;
        }

        void HandleMidHit()
        {
            m_Mid = true;
        }

        void HandleOuterHit()
        {
            m_Outer = true;
        }

        void HandleOutermostHit()
        {
            m_Outermost = true;
        }

        void HandleBullsEyeMiss()
        {
            m_BullsEye = false;
        }

        void HandleInnerMiss()
        {
            m_Inner = false;
        }

        void HandleMidMiss()
        {
            m_Mid = false;
        }

        void HandleOuterMiss()
        {
            m_Outer = false;
        }

        void HandleOutermostMiss()
        {
            m_Outermost = false;
        }

        void OnGUI()
        {
            if (m_GUIEnabled)
            {
                GUI.color = Color.yellow;
                m_BullsEyeAccuracy = (m_BullsEyeCount / m_FrameCount) * 100.0f;
                m_InnerAccuracy = (m_InnerCount / m_FrameCount) * 100.0f;
                m_MidAccuracy = (m_MidCount / m_FrameCount) * 100.0f;
                m_OuterAccuracy = (m_OuterCount / m_FrameCount) * 100.0f;
                m_OutermostAccuracy = (m_OutermostCount / m_FrameCount) * 100.0f;

                GUI.Label(new Rect(10, 10, 300, 30), "Bulls Eye Accuracy: " + m_BullsEyeAccuracy + "%");
                GUI.Label(new Rect(10, 30, 300, 30), "Inner Accuracy: " + m_InnerAccuracy + "%");
                GUI.Label(new Rect(10, 50, 300, 30), "Mid Accuracy: " + m_MidAccuracy + "%");
                GUI.Label(new Rect(10, 70, 300, 30), "Outer Accuracy: " + m_OuterAccuracy + "%");
                GUI.Label(new Rect(10, 90, 300, 30), "Outermost Accuracy: " + m_OutermostAccuracy + "%");
            }
        }

        public void StartMeasurements()
        {
            m_Measure= true;
        }

        public void StopMeasurements()
        {
            m_Measure = false;
        }

        public void Show()
        {
            m_TargetComponents.Show();
        }

        public void Hide()
        {
            m_TargetComponents.Hide();
        }

        public void Toggle()
        {
            m_TargetComponents.Toggle();
        }

        public void LoadSounds(AudioClip[] clips)
        {
            m_Audio.rs3d_LoadAudioClips(ref clips);

            // Setup the audio
            m_Audio.rs3d_PlayIn3D(true);
            m_Audio.rs3d_PlayOnStart(true);
            m_Audio.rs3d_SetFastSpatialization(true);
            m_Audio.rs3d_SetOptimization(true);
            m_Audio.rs3d_LoopSound(true);
            m_Audio.rs3d_MuteSound(true);

            m_SoundLoaded = true;
        }

        public void SetSoundExtent(float distance)
        {
            m_Audio.fSoundRange_MaxDist = distance;
        }

        public void PlaySound(int nIndex = 0)
        {
            if (m_SoundLoaded)
            {
                m_Audio.rs3d_MuteSound(false, nIndex);
            }
        }

        public void StopSound(int nIndex = 0)
        {
            if (m_SoundLoaded)
            {
                m_Audio.rs3d_MuteSound(true, nIndex);
            }
        }
    }
}


