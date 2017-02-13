using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using RealSpace3D;

namespace Sop.Utils
{
    // This class works similarly to the SelectionRadial class except
    // it has a physical manifestation in the scene.  This can be
    // either a UI slider or a mesh with the SlidingUV shader.  The
    // functions as a bar that fills up whilst the user looks at it
    // and holds down the Fire1 button.
    public class SelectionSlider : MonoBehaviour
    {
        public event Action OnBarFilled;                                    // This event is triggered when the bar finishes filling.

        public VRInput m_VRInput;                                           // Reference to the VRInput to detect button presses.

        public UIFader m_UIFader;                                           // Optional reference to a UIFader, used if the SelectionSlider needs to fade out.
        public Collider m_Collider;                                         // Optional reference to the Collider used to detect the user's gaze, turned off when the UIFader is not visible.

        public float m_Duration = 2f;                                       // The length of time it takes for the bar to fill.

        public string m_Text;                                               // The text displayed in the selection slider.
        public Text m_TextComponent;                                        // Refernce to the component that is going to display the text.

        //public AudioClip m_TalkClip;                                        // Optional reference to clip to play to say the text of the button.
        //public RealSpace3D_AudioSource m_AudioTalk;                         // Reference to the audio source that will say the text of the button.

        //public RealSpace3D_AudioSource m_Audio;                             // Reference to the audio source that will play effects when the user looks at it and when it fills.

        public  VRInteractiveItem m_InteractiveItem;                        // Reference to the VRInteractiveItem to determine when to fill the bar.

        public Slider m_Slider;                                             // Optional reference to the UI slider (unnecessary if using a standard Renderer).
        public Renderer m_Renderer;                                         // Optional reference to a renderer (unnecessary if using a UI slider).

        public SelectionRadial m_SelectionRadial;                           // Optional reference to the SelectionRadial, if non-null the duration of the SelectionRadial will be used instead.

        public bool m_DisableOnBarFill;                                     // Whether the bar should stop reacting once it's been filled (for single use bars).
        public GameObject m_BarCanvas;                                      // Optional reference to the GameObject that holds the slider (only necessary if DisappearOnBarFill is true).

        public bool m_DisappearOnBarFill;                                   // Whether the bar should disappear instantly once it's been filled.

        AudioClip m_OnOverClip;                                             // The clip to play when the user looks at the bar.
        AudioClip m_OnOutClip;                                              // The clip to play when the user looks at the bar.
        AudioClip m_OnFilledClip;                                           // The clip to play when the bar finishes filling.
        AudioClip m_FillingClip;                                            // The clip to play when the bar is filling.

        bool m_BarFilled;                                                   // Whether the bar is currently filled.
        bool m_GazeOver;                                                    // Whether the user is currently looking at the bar.
        float m_Timer;                                                      // Used to determine how much of the bar should be filled.

        Coroutine m_FillBarRoutine;                                         // Reference to the coroutine that controls the bar filling up, used to stop it if required.


        private const string k_SliderMaterialPropertyName = "_SliderValue"; // The name of the property on the SlidingUV shader that needs to be changed in order for it to fill.


        void OnEnable ()
        {
            m_VRInput.OnDown += HandleDown;
            m_VRInput.OnUp += HandleUp;

            m_InteractiveItem.OnOver += HandleOver;
            m_InteractiveItem.OnOut += HandleOut;
        }

        void OnDisable ()
        {
            m_VRInput.OnDown -= HandleDown;
            m_VRInput.OnUp -= HandleUp;

            m_InteractiveItem.OnOver -= HandleOver;
            m_InteractiveItem.OnOut -= HandleOut;
        }

        void Awake()
        {
            if (m_TextComponent && m_Text != string.Empty)
            {
                m_TextComponent.text = m_Text;
            }

            //if (m_TalkClip != null)
            //{
            //    m_AudioTalk.rs3d_LoadAudioClip(m_TalkClip);
            //}

            //m_OnOverClip = (AudioClip)Resources.Load("Audio/Utils/SelectClip");
            //m_OnOutClip = (AudioClip)Resources.Load("Audio/Utils/OnOutClip");
            //m_OnFilledClip = (AudioClip)Resources.Load("Audio/Utils/OnFilledClip");
            //m_FillingClip = (AudioClip)Resources.Load("Audio/Utils/FillingClip");

        }

        void Start()
        {

        }

        void Update ()
        {
            if(!m_UIFader)
                return;

            // If this bar is using a UIFader turn off the collider when it's invisible.
            m_Collider.enabled = m_UIFader.Visible;
        }

        IEnumerator FillBar ()
        {
            // When the bar starts to fill, reset the timer.
            m_Timer = 0f;

            // The amount of time it takes to fill is either the duration set in the inspector, or the duration of the radial.
            float fillTime = m_SelectionRadial != null ? m_SelectionRadial.SelectionDuration : m_Duration;

            // Until the timer is greater than the fill time...
            while (m_Timer < fillTime)
            {
                // ... add to the timer the difference between frames.
                m_Timer += Time.deltaTime;

                // Set the value of the slider or the UV based on the normalised time.
                SetSliderValue(m_Timer / fillTime);
                
                // Wait until next frame.
                yield return null;

                // If the user is still looking at the bar, go on to the next iteration of the loop.
                if (m_GazeOver)
                    continue;

                // If the user is no longer looking at the bar, reset the timer and bar and leave the function.
                m_Timer = 0f;
                SetSliderValue (0f);

                // Stop the filling sound
                //m_Audio.rs3d_LoopSound(false);
                //m_Audio.rs3d_StopSound();

                yield break;
            }

            // If the loop has finished the bar is now full.
            m_BarFilled = true;

            // If anything has subscribed to OnBarFilled call it now.
            if (OnBarFilled != null)
                OnBarFilled ();

            // Play the clip for when the bar is filled.
            //m_Audio.rs3d_LoadAudioClip(m_OnFilledClip);
            //m_Audio.rs3d_PlaySound();

            // If the bar should be disabled once it is filled, do so now.
            if (m_DisableOnBarFill)
                enabled = false;
        }


        void SetSliderValue (float sliderValue)
        {
            // If there is a slider component set it's value to the given slider value.
            if (m_Slider)
                m_Slider.value = sliderValue;

            // If there is a renderer set the shader's property to the given slider value.
            if(m_Renderer)
                m_Renderer.sharedMaterial.SetFloat (k_SliderMaterialPropertyName, sliderValue);
        }


        void HandleDown ()
        {
            // If the user is looking at the bar start the FillBar coroutine and store a reference to it.
            if (m_GazeOver)
            {
                // Start the filling sound
                //m_Audio.rs3d_LoadAudioClip(m_FillingClip);
                //m_Audio.rs3d_LoopSound(true);
                //m_Audio.rs3d_PlaySound();

                m_FillBarRoutine = StartCoroutine(FillBar());
            }

        }


        void HandleUp ()
        {
            // If the coroutine has been started (and thus we have a reference to it) stop it.
            if(m_FillBarRoutine != null)
            {
                // Stop the filling sound
                //m_Audio.rs3d_StopSound();
                //m_Audio.rs3d_LoopSound(false);

                StopCoroutine(m_FillBarRoutine);
            }

            // Reset the timer and bar values.
            m_Timer = 0f;
            SetSliderValue(0f);
        }


        void HandleOver ()
        {
            // The user is now looking at the bar.
            m_GazeOver = true;

            // Play the clip appropriate for when the user starts looking at the bar.
            //m_Audio.rs3d_LoadAudioClip(m_OnOverClip);
            //m_Audio.rs3d_PlaySound();
            //StartCoroutine(WaitForSound(m_OnOverClip));

        }

        //IEnumerator WaitForSound(AudioClip clip)
        //{
        //    yield return new WaitForSeconds(clip.length);
        //}

        void HandleOut ()
        {
            // The user is no longer looking at the bar.
            m_GazeOver = false;

            // If the coroutine has been started (and thus we have a reference to it) stop it.
            if (m_FillBarRoutine != null)
            {
                // Stop the filling sound
                //m_Audio.rs3d_StopSound();
                //m_Audio.rs3d_LoopSound(false);

                StopCoroutine(m_FillBarRoutine);
            }

            // Play the out sound;
            //m_Audio.rs3d_LoadAudioClip(m_OnOutClip);
            //m_Audio.rs3d_PlaySound();

            // Reset the timer and bar values.
            m_Timer = 0f;
            SetSliderValue(0f);
        }

        public IEnumerator WaitForBarToFill()
        {
            // If the bar should disappear when it's filled, it needs to be visible now.
            if (m_BarCanvas && m_DisappearOnBarFill)
                m_BarCanvas.SetActive(true);

            // Currently the bar is unfilled.
            m_BarFilled = false;

            // Reset the timer and set the slider value as such.
            m_Timer = 0f;
            SetSliderValue(0f);

            // Keep coming back each frame until the bar is filled.
            while (!m_BarFilled)
            {
                yield return null;
            }

            // If the bar should disappear once it's filled, turn it off.
            if (m_BarCanvas && m_DisappearOnBarFill)
                m_BarCanvas.SetActive(false);
        }

        //public IEnumerator SayName(int numTimes)
        //{
        //    m_AudioTalk.rs3d_PlaySound();
        //    yield return new WaitForSeconds(m_AudioTalk.rs3d_GetClipLength() * numTimes);
        //    m_AudioTalk.rs3d_StopSound();
        //}
    }
}