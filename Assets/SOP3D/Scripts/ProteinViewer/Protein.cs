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


using System.Collections.Generic;
using UnityEngine;
using Sop.Utils;
using System;

namespace Sop.ProteinViewer
{
    public class Protein : MonoBehaviour
    {
        public event Action<string> OnChainSelected;                   // This event is triggered when the selection of the button has finished.

        public float m_TargetSpeed = 1f;
        public Target m_Target;

        public Transform m_Camera;

        Bounds m_Bounds;                                                // The bounding box of the protein or chain.

        List<Atom> m_Atoms;                                             // A list of atoms used in constructing the chain view.
        List<Chain> m_Chains;                                           // A list of chains used in constructing the protein view.

        GameObject m_HelixAtom;                                         // GameObjects prefabs begin ....
        GameObject m_HelixBond;                                         // ...
        GameObject m_SheetAtom;                                         // ...
        GameObject m_SheetBond;                                         // ...
        GameObject m_LoopAtom;                                          // ...
        GameObject m_LoopBond;                                          // ...
        GameObject m_TransitionBond;                                    // ...
        GameObject m_Chain;                                             // ...
        GameObject m_ChainSelector;                                     // ... end

        List<Transform> m_ChainSelectors;                               // A list of chain selectors (the buttons used to select a chain for viewing).

        AudioClip[] m_AudioClips;                                       // A list of audio clips to be played for each secondary structure.

        Vector3 m_SoundStart;                                           // The start position of a sound (used for interpolation).
        int m_AtomNum;                                                  // Atom count (used for interpolation).
        float m_InterpNum = 0.0f;                                       // The interpolation number.

        Vector3 m_Offset;

        void Awake()
        {
            //Load the audio clips for each secondary structure type.
           m_AudioClips = new AudioClip[] {
                (AudioClip)Resources.Load("Audio/ProteinViewer/FeelGood_Sample1"),
                (AudioClip)Resources.Load("Audio/ProteinViewer/FeelGood_Sample2"),
                (AudioClip)Resources.Load("Audio/ProteinViewer/Close_Sample1") };

            // Load the prefabs.
            m_HelixAtom = (GameObject)Resources.Load("Prefabs/ProteinViewer/HelixAtom", typeof(GameObject));
            m_HelixBond = (GameObject)Resources.Load("Prefabs/ProteinViewer/HelixBond", typeof(GameObject));
            m_SheetAtom = (GameObject)Resources.Load("Prefabs/ProteinViewer/SheetAtom", typeof(GameObject));
            m_SheetBond = (GameObject)Resources.Load("Prefabs/ProteinViewer/SheetBond", typeof(GameObject));
            m_LoopAtom = (GameObject)Resources.Load("Prefabs/ProteinViewer/LoopAtom", typeof(GameObject));
            m_LoopBond = (GameObject)Resources.Load("Prefabs/ProteinViewer/LoopBond", typeof(GameObject));
            m_TransitionBond = (GameObject)Resources.Load("Prefabs/ProteinViewer/GenericBond", typeof(GameObject));
            m_Chain = (GameObject)Resources.Load("Prefabs/ProteinViewer/Chain", typeof(GameObject));
            m_ChainSelector = (GameObject)Resources.Load("Prefabs/ProteinViewer/ChainSelector", typeof(GameObject));
        }

        void Start()
        {
            // Load the atoms and chains
            m_Atoms = ProteinControl.control.GetAtoms();
            m_Chains = ProteinControl.control.GetChains();

            // Get the bounds of the atoms
            m_Bounds = Atom.GetBounds(m_Atoms);
            
            // Set the offset
            if (m_Bounds.min.y < 0)
                m_Offset = new Vector3(0, Math.Abs(m_Bounds.min.y) + 1, 0);
            else
                m_Offset = new Vector3(0, 0, 0);

            // NOTE: We must set the bonding box first, as everything else
            // is based on its location.
            m_Bounds.center += m_Offset;

            Debug.Log("Protein Viewing chain?" + ProteinControl.control.ViewingChain);

            // Setup the scene to either view the chain or the protien
            if (ProteinControl.control.ViewingChain)
            {
                CreateChain(m_Atoms, gameObject);
                m_Target.transform.position = m_Atoms[0].position + m_Offset;
                m_Target.LoadSounds(m_AudioClips);
                m_Target.SetSoundExtent(m_Bounds.extents.magnitude);
                m_SoundStart = m_Target.transform.position;
            }
            else
            {
                m_Target.Hide();
                CreateProtein(m_Chains, gameObject);
            }

            Debug.Log(m_Bounds.center);
            Debug.Log(Camera.main.transform.position);
            m_Camera.position += m_Offset;
            transform.position += m_Offset;

            // Uncomment the next line if you would like to visualize the bounding box.
            //DisplayBoundingBox(m_Bounds);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("ShapeToggle"))
                Toggle();

            if (ProteinControl.control.ViewingChain)
            {
                // If not at the end of the chain.
                if (m_AtomNum < m_Atoms.Count)
                {
                    // Reset interpolation
                    if (m_InterpNum > 1.0f)
                    {
                        m_SoundStart = m_Target.transform.position;
                        m_AtomNum += 1;
                        m_InterpNum = 0.0f;
                        if (m_AtomNum >= m_Atoms.Count)
                            return;
                    }

                    Atom curAtom = m_Atoms[m_AtomNum];

                    // Play the appropriate sound.
                    switch (curAtom.structure)
                    {
                        case Structure.Type.Helix:
                            m_Target.StopSound(1);
                            m_Target.StopSound(2);
                            m_Target.PlaySound(0);
                            break;
                        case Structure.Type.Loop:
                            m_Target.StopSound(0);
                            m_Target.StopSound(2);
                            m_Target.PlaySound(1);
                            break;
                        case Structure.Type.Sheet:
                            m_Target.StopSound(0);
                            m_Target.StopSound(1);
                            m_Target.PlaySound(2);
                            break;
                    }

                    // Calculate the amount of interpolation and move the target.
                    Vector3 curPosition = curAtom.position;
                    curPosition += m_Offset;
                    m_InterpNum += Time.deltaTime * m_TargetSpeed;
                    m_Target.transform.position = Vector3.Lerp(m_SoundStart, curPosition, m_InterpNum);
                }
                else  // Reset to start
                {
                    m_AtomNum = 1;
                    m_Target.transform.position = m_Atoms[0].position + m_Offset;
                    m_SoundStart = m_Target.transform.position;
                }
            }
        }

        // Creates the full protein from a list of chains.
        void CreateProtein(List<Chain> chains, GameObject parent)
        {
            m_ChainSelectors = new List<Transform>();

            foreach (Chain chain in chains)
            {
                // Create, position and parent the chain
                GameObject chainGO = Instantiate(m_Chain);
                chainGO.transform.parent = parent.transform;
                chainGO.transform.position = chain.GetBounds().center;
                CreateChain(chain.atoms, chainGO);

                // Create, position and parent the chain selector button.
                GameObject selector = Instantiate(m_ChainSelector) as GameObject;
                selector.transform.parent = parent.transform;
                selector.transform.position = chain.GetBounds().center;

                // Set the id and audio max range for this selector button.
                selector.transform.GetComponent<SelectableItem>().SetID(chain.id);
                selector.transform.GetComponent<SelectableItem>().SetAudioMaxRange(m_Bounds.extents.magnitude);

                // Subscribe to the buttons on selected event.
                selector.transform.GetComponent<SelectableItem>().OnSelected += HandleOnSelected;

                m_ChainSelectors.Add(selector.transform);
            }
        }

        // Creates a chain of atoms from the list of atoms.
        void CreateChain(List<Atom> atoms, GameObject parent)
        {
            Atom prevAtom = null;

            foreach (Atom curAtom in atoms)
            {
                // Generate atom from correct prefab
                GameObject atom = null;

                switch (curAtom.structure)
                {
                    case Structure.Type.Helix:
                        atom = Instantiate(m_HelixAtom);
                        break;
                    case Structure.Type.Sheet:
                        atom = Instantiate(m_SheetAtom);
                        break;
                    case Structure.Type.Loop:
                        atom = Instantiate(m_LoopAtom);
                        break;
                }

                atom.transform.parent = parent.transform;
                atom.transform.position = curAtom.position;

                if (prevAtom != null)
                {
                    // Generate bond from correct prefab
                    GameObject bond = null;
                    if (curAtom.structure == prevAtom.structure)
                        switch (curAtom.structure)
                        {
                            case Structure.Type.Helix:
                                bond = Instantiate(m_HelixBond);
                                break;
                            case Structure.Type.Sheet:
                                bond = Instantiate(m_SheetBond);
                                break;
                            case Structure.Type.Loop:
                                bond = Instantiate(m_LoopBond);
                                break;
                        }
                    else
                        bond = Instantiate(m_TransitionBond);

                    bond = curAtom.CreateBond(prevAtom, bond);
                    bond.transform.parent = parent.transform;
                }
                prevAtom = curAtom;
            }
        }

        // Toggles the chains renderer and colliders.
        private void Toggle()
        {
            if (ProteinControl.control.ViewingChain)
                foreach (Transform child in transform)
                {
                    child.GetComponent<Renderer>().enabled = !child.GetComponent<Renderer>().enabled;
                    child.GetComponent<Collider>().enabled = !child.GetComponent<Collider>().enabled;
                }
            else
                foreach (Transform child in transform)
                {
                    foreach (Transform children in child.GetComponent<Transform>())
                    {
                        if (children.GetComponent<Renderer>() != null)
                            children.GetComponent<Renderer>().enabled = !children.GetComponent<Renderer>().enabled;
                        if (children.GetComponent<Collider>() != null)
                            children.GetComponent<Collider>().enabled = !children.GetComponent<Collider>().enabled;
                    }
                }
        }

        void HandleOnSelected(string param)
        {
            if (OnChainSelected != null)
                OnChainSelected(param);
        }
        
        // Displays a bounding box given the bounds.
        void DisplayBoundingBox(Bounds objectBounds)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = objectBounds.center;
            cube.transform.localScale = objectBounds.size;
        }
    }
}
