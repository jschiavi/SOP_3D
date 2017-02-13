using UnityEngine;
using System.Collections;
using Sop.Utils;
using UnityEngine.SceneManagement;

namespace Sop.ProteinViewer
{
    public class PVManager : MonoBehaviour
    {
        public Reticle m_Reticle;                        
        public SelectionRadial m_Radial;
        public Protein m_Protein;
        public VRCameraFade m_CameraFade;

        public string m_SceneToLoad = "MainMenu";    // The name of the scene to load.
        string m_NextScene;

        void Awake()
        {

        }

        void OnEnable()
        {
            m_Protein.OnChainSelected += HandleOnChainSelected;
        }

        void OnDisable()
        {
            m_Protein.OnChainSelected -= HandleOnChainSelected;
        }

        void Start()
        {
            m_Reticle.Show();
            m_Radial.Hide();
        }

        void HandleOnChainSelected(string id)
        {
            ProteinControl.control.ViewingChain = true;
            ProteinControl.control.ChainID = id;

            m_NextScene = "ProteinViewer";
            StartCoroutine(LoadNextScene());
        }

        IEnumerator LoadNextScene()
        {
            //If the camera is already fading, ignore.
            if (m_CameraFade.IsFading)
                yield break;

            // Wait for the camera to fade out.
            yield return StartCoroutine(m_CameraFade.BeginFadeOut(true));

            SceneManager.LoadScene(m_NextScene, LoadSceneMode.Single);
        }
    }
}