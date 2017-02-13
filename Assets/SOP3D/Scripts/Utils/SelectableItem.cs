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


using System;
using UnityEngine;
//using RealSpace3D;

namespace Sop.Utils
{
    public class SelectableItem : MonoBehaviour
    {
        public event Action<string> OnSelected;                 // This event is triggered when the item is selected.

        public VRInteractiveItem m_InteractiveItem;             // The interactive item associated with the selectable item.

        //AudioClip[] m_AudioClips;                               // The audio clips used by the selectable item to indicate different states
        //public RealSpace3D_AudioSource m_Audio;                 // Reference to the audio source that will play effects when the user looks at it and when it fills.

        SelectionRadial m_SelectionRadial;                      // Reference to the camera's selection radial.

        bool m_GazeOver;                                        // Whether or not the user's gaze is over this interactive item
        string m_ID;                                            // An optional ID identifying this selectable item.

        private void Awake()
        {
            // Get the selection radial from the main camera.
            m_SelectionRadial = Camera.main.GetComponent<SelectionRadial>();

            //m_AudioClips = new AudioClip[] {
            //    (AudioClip)Resources.Load("Audio/Utils/SelectClip"),
            //    (AudioClip)Resources.Load("Audio/Utils/OnOutClip"),
            //    (AudioClip)Resources.Load("Audio/Utils/OnFilledClip"),
            //    (AudioClip)Resources.Load("Audio/Utils/FillingClip")};
        }

        private void Start()
        {
            // Load the audio clips and set their set the initial states
            //m_Audio.rs3d_LoadAudioClips(ref m_AudioClips);
            //for (int i = 0; i < m_Audio.rs3d_GetClipsCount(); i++)
            //{
            //    m_Audio.rs3d_LoopSound(false, i);
            //    m_Audio.rs3d_PlayIn3D(true, i);
            //    m_Audio.rs3d_MuteSound(true, i);
            //    m_Audio.rs3d_PlayOnStart(false, i);
            //}
            //m_Audio.rs3d_SetFastSpatialization(true);

            // Orient the button to look at the main camera
            transform.LookAt(Camera.main.transform.position);
        }

        private void OnEnable ()
        {
            m_SelectionRadial.OnDown += HandleOnDown;
            m_SelectionRadial.OnUp += HandleOnUp;
            m_SelectionRadial.OnSelectionComplete += HandleSelectionComplete;
            m_InteractiveItem.OnOver += HandleOnOver;
            m_InteractiveItem.OnOut += HandleOnOut;
        }

        private void OnDisable ()
        {
            m_SelectionRadial.OnDown -= HandleOnDown;
            m_SelectionRadial.OnUp -= HandleOnUp;
            m_SelectionRadial.OnSelectionComplete -= HandleSelectionComplete;
            m_InteractiveItem.OnOver -= HandleOnOver;
            m_InteractiveItem.OnOut -= HandleOnOut;
        }
        
        void HandleOnOver()
        {
            // When the user looks at the rendering of the button, show the radial.
            m_SelectionRadial.Show();

            // Play the clip appropriate for when the user starts looking at the bar.
            //m_Audio.rs3d_LoopSound(false, 0);
            //m_Audio.rs3d_MuteSound(false, 0);
            //m_Audio.rs3d_PlaySound(0);

            m_GazeOver = true;
        }

        void HandleOnOut()
        {
            // When the user looks away from the rendering of the scene, hide the radial.
            m_SelectionRadial.Hide();

            m_GazeOver = false;

            //m_Audio.rs3d_LoopSound(false, 3);
            //m_Audio.rs3d_MuteSound(true, 3);
            //if (m_Audio.rs3d_IsPlaying(3))
            //    m_Audio.rs3d_StopSound(3);

            //m_Audio.rs3d_LoopSound(false, 0);
            //m_Audio.rs3d_MuteSound(true, 0);
            //if (m_Audio.rs3d_IsPlaying(0))
            //    m_Audio.rs3d_StopSound(0);

            //// Play the clip appropriate for when the user looks away.
            //m_Audio.rs3d_LoopSound(false, 1);
            //m_Audio.rs3d_MuteSound(false, 1);
            //m_Audio.rs3d_PlaySound(1);
        }

        private void HandleSelectionComplete()
        {
            // If the user is looking at the rendering of the scene when the radial's selection finishes, activate the button.
            if (m_GazeOver)
            {
                //m_Audio.rs3d_LoopSound(false, 3);
                //m_Audio.rs3d_MuteSound(true, 3);
                //if (m_Audio.rs3d_IsPlaying(3))
                //    m_Audio.rs3d_StopSound(3);

                //m_Audio.rs3d_LoopSound(false, 2);
                //m_Audio.rs3d_MuteSound(false, 2);
                //m_Audio.rs3d_PlaySound(2);

                if (OnSelected != null)
                    OnSelected(m_ID);
            }
        }

        void HandleOnDown()
        {
            if (m_GazeOver)
            {
                //m_Audio.rs3d_LoopSound(true, 3);
                //m_Audio.rs3d_MuteSound(false, 3);
                //m_Audio.rs3d_PlaySound(3);
            }
        }

        void HandleOnUp()
        {
            if (m_GazeOver)
            {
                //m_Audio.rs3d_LoopSound(false, 3);
                //m_Audio.rs3d_MuteSound(true, 3);
                //if (m_Audio.rs3d_IsPlaying(3))
                //    m_Audio.rs3d_StopSound(3);
            }
        }

        // Set the max audio range for the selctable item's audio.
        public void SetAudioMaxRange(float maxRange)
        {
            //m_Audio.fSoundRange_MaxDist = maxRange;
        }

        // Optionally used to set the id (e.g. used for the chain to set the chain id)
        public void SetID(string id)
        {
            m_ID = id;
        }
    }
}