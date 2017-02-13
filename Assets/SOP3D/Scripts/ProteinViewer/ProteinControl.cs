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
using System.Collections.Generic;
using System.IO;

namespace Sop.ProteinViewer
{
    public class ProteinControl : MonoBehaviour
    {
        public static ProteinControl control;

        public string m_PdbFile;                             // The protein file name to be read in.

        bool m_ViewingChain;                                            // Whether or not we are viewing a single chain or the whole protein.
        public bool ViewingChain
        {                                     
            get { return m_ViewingChain; }
            set { m_ViewingChain = value; }
        }     

        string m_ChainID;                                               // The chain ID - used to determine which chain to use in chain view.
        public string ChainID
        {
            get { return m_ChainID; }
            set { m_ChainID = value; }
        }

        bool m_GUIEnabled;                                              // Whether or not the GUI is enabled for the controller.

        List<Atom> m_Atoms;                                             // A list of atoms in the protein.
        List<Structure> m_Structures;                                   // A list of the secondary structures in the designated PDB file
        List<Chain> m_Chains;                                           // A list of chains in the protien.

        string m_PathToProteins;

        void Awake()
        {

            m_PathToProteins = Application.dataPath + "/SOP3D/PdbFiles";

            // Set up the static controller for the protein if it doesn't exist already.
            if (control == null)
            {
                DontDestroyOnLoad(gameObject);
                control = this;
                ProteinControl.control.LoadPdbFile(m_PdbFile);
            }
            else if (control != null)
            {
                Destroy(gameObject);
            }

        }

        void Start()
        {
            m_GUIEnabled = false;
            m_ViewingChain = false;
        }

        void Update()
        {
            // Toggle the GUI
            if (Input.GetButtonDown("TargetGUI"))
            {
                m_GUIEnabled = !m_GUIEnabled;
            }
        }

        // Reads in the selected pdb file.
        void ReadFile()
        {
            // Create a new list of atoms and structures.
            m_Atoms = new List<Atom>();
            m_Structures = new List<Structure>();

            // Read in the file.
            Stream pdbStream = File.OpenRead(m_PathToProteins + "/" + m_PdbFile + ".pdb");
            StreamReader pdbReader = new StreamReader(pdbStream);

            // Read the file line by line.
            string pdbLine = pdbReader.ReadLine();

            while (pdbLine != null)
            {
                if (pdbLine.StartsWith("ENDMDL"))
                    break;

                if (pdbLine.StartsWith("HELIX") || pdbLine.StartsWith("SHEET"))
                    m_Structures.Add(new Structure(pdbLine));

                if (pdbLine.StartsWith("ATOM"))
                {
                    string type = pdbLine.Substring(12, 4).Trim();
                    if (type == "CA")
                    {
                        Atom atom = new Atom(pdbLine);

                        // Set default secondary structure for atoms
                        atom.structure = Structure.Type.Loop;

                        // Set actual secondary structure for atoms
                        foreach (Structure s in m_Structures)
                        {
                            if (atom.seqNum >= s.startSeqNum && atom.seqNum <= s.endSeqNum)
                            {
                                atom.structure = s.type;
                            }
                        }
                        m_Atoms.Add(atom);
                    }
                }
                pdbLine = pdbReader.ReadLine();
            }
        }

        // Creates the chains of a protein
        void CreateChains()
        {
            // Create a new list of chains.
            m_Chains = new List<Chain>();

            // Create a list to hold the chain atoms.
            List<Atom> chainAtoms = new List<Atom>();

            Atom prevAtom = null;
            Chain chain;

            foreach (Atom curAtom in m_Atoms)
            {
                // Check if we should start a new chain.
                if (prevAtom != null &&
                    curAtom.chainID != prevAtom.chainID)
                {
                    // Create a new chain
                    chain = new Chain(chainAtoms, GetStructs(prevAtom.chainID));
                    m_Chains.Add(chain);
                    chainAtoms = new List<Atom>();
                    chainAtoms.Add(curAtom);
                }
                else
                {
                    // Add atom to current chain
                    chainAtoms.Add(curAtom);
                }
                prevAtom = curAtom;
            }

            // Create a new chain
            chain = new Chain(chainAtoms, GetStructs(prevAtom.chainID));
            m_Chains.Add(chain);
        }

        // Returns a list of secondary structures associated with this chain
        List<Structure> GetStructs(string chainID)
        {
            List<Structure> structures = new List<Structure>();
            foreach (Structure structure in m_Structures)
            {
                if (structure.chainID == chainID)
                    structures.Add(structure);
            }
            return structures;
        }

        // Displays GUI information about the protein.
        void OnGUI()
        {
            if (m_GUIEnabled)
            {
                GUI.Label(new Rect(10, 10, 100, 30), "PDB File: " + m_PdbFile);
                GUI.Label(new Rect(10, 30, 150, 30), "Chain View: " + m_ViewingChain);
                GUI.Label(new Rect(10, 50, 150, 30), "Chain ID: " + m_ChainID);
            }
        }

        // Loads a pdb file into the controller.
        public void LoadPdbFile(string fileName)
        {
            // Set the file name
            m_PdbFile = fileName;

            // Read the file in.
            ReadFile();

            // Center all atoms around the origin.  This
            // is needed before we create the chains to ensure
            // proper positioning.
            Atom.CenterOnOrigin(m_Atoms);

            // Create the protein chains.
            CreateChains();

            // Set the chain view or protien view
            if (m_Chains.Count == 1)
            {
                m_ViewingChain = true;
                m_ChainID = m_Chains[0].id;
            }
            else
                m_ViewingChain = false;

            Debug.Log("Viewing Chain: " + m_ViewingChain);
        }

        // Returns a list of atoms corresponding to either
        // 1) the chain if we are viewing the chain
        // OR
        // 2) all the atom if we are not viewing the chain
        public List<Atom> GetAtoms()
        {
            if (m_ViewingChain)
            {
                foreach (Chain chain in m_Chains)
                {
                    if (chain.id == m_ChainID)
                    {
                        return chain.atoms;
                    }
                }
                Debug.Log("Error: GetAtoms() - viewing chain but couldn't find ID");
            }
            return m_Atoms;
        }

        // Returns a list of secondary structures corresponding to either
        // 1) the chain if we are viewing the chain
        // OR
        // 2) all the atoms if we are not viewing the chain
        public List<Structure> GetStructures()
        {
            if (m_ViewingChain)
            {
                foreach (Chain chain in m_Chains)
                {
                    if (chain.id == m_ChainID)
                    {
                        return chain.structures;
                    }
                }
                Debug.Log("Error: GetStructures() - viewing chain but couldn't find ID");
            }
            return m_Structures;
        }

        // Returns all the chains in the protein.
        public List<Chain> GetChains()
        {
            return m_Chains;
        }

        public void TurnGUIOff()
        {
            m_GUIEnabled = false;
        }

        public void TurnGUIOn()
        {
            m_GUIEnabled = true;
        }

    }

}

