using UnityEngine;
using System.Collections;
using Sop.Utils;
using UnityEngine.SceneManagement;
using Sop.ProteinViewer;

namespace Sop.Menu
{

    public class MenuManager : MonoBehaviour
    {

        public int m_SayNumTimes = 3;

        public VRCameraFade m_CameraFade;
        public Reticle m_Reticle; 
        public SelectionRadial m_Radial; 

        public UIFader m_Buttons;
        public SelectionSlider m_Slider1;
        public SelectionSlider m_Slider2;
        public SelectionSlider m_Slider3;

        //public AudioSource m_AudioInstr; 

        Coroutine m_LoopButtons;

        bool m_BarFilled;

        string m_SceneToLoad = "ProteinViewer";

        void OnEnable()
        {
            m_Slider1.OnBarFilled += HandleOnBarFilled1;
            m_Slider2.OnBarFilled += HandleOnBarFilled2;
            m_Slider3.OnBarFilled += HandleOnBarFilled3;
        }

        void OnDisable()
        {
            m_Slider1.OnBarFilled -= HandleOnBarFilled1;
            m_Slider2.OnBarFilled -= HandleOnBarFilled2;
            m_Slider3.OnBarFilled -= HandleOnBarFilled3;
        }

        void Awake()
        {
            m_BarFilled = false;
        }

        void Start()
        {
            m_Reticle.Show();
            m_Radial.Hide();
            m_Buttons.SetInvisible();
            m_LoopButtons = StartCoroutine(LoopButtons());
        }

        IEnumerator LoopButtons()
        {
            yield return new WaitForSeconds(2f);                        //Small hack to wait for camera fade in.
            //m_AudioInstr.Play();
            //yield return new WaitForSeconds(m_AudioInstr.clip.length);

            yield return m_Buttons.WaitForFadeIn();

            //while (!m_BarFilled)
            //{
            //    yield return m_Slider1.SayName(m_SayNumTimes);
            //    yield return new WaitForSeconds(1);
            //    yield return m_Slider2.SayName(m_SayNumTimes);
            //    yield return new WaitForSeconds(1);
            //    yield return m_Slider3.SayName(m_SayNumTimes);
            //    yield return new WaitForSeconds(1);
            //}
        }

        void HandleOnBarFilled1()
        {
            ProteinControl.control.LoadPdbFile(m_Slider1.m_Text);
            StartCoroutine(Outro());
        }

        void HandleOnBarFilled2()
        {
            ProteinControl.control.LoadPdbFile(m_Slider2.m_Text);
            StartCoroutine(Outro());
        }

        void HandleOnBarFilled3()
        {
            ProteinControl.control.LoadPdbFile(m_Slider3.m_Text);
            StartCoroutine(Outro());
        }

        IEnumerator Outro()
        {
            m_BarFilled = true;
            StopCoroutine(m_LoopButtons);
            yield return StartCoroutine(m_CameraFade.BeginFadeOut(true));
            SceneManager.LoadScene(m_SceneToLoad);
        }

    }
}