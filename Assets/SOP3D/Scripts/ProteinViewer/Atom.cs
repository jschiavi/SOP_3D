//=============================================================================
// This file is part of the SOP program.
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
using System;

namespace Sop.ProteinViewer
{
    public class Atom
    {
        //private string type;
        //private string codon;
        public string chainID;
        public int seqNum;
        public Atom nextAtom;
        public Atom prevAtom;
        public Vector3 position;
        public Structure.Type structure;


        public Atom (string pdbLine)
        {
            //type = pdbLine.Substring(12, 4).Trim();
            //codon = pdbLine.Substring(17, 3).Trim();
            chainID = pdbLine.Substring(21, 1);
            seqNum = Convert.ToInt32(pdbLine.Substring(22, 4));

            float x = float.Parse(pdbLine.Substring(30, 8));
            float y = float.Parse(pdbLine.Substring(38, 8));
            float z = float.Parse(pdbLine.Substring(46, 8));

            position = new Vector3(x, y, z);
        }

        // Given a bond GameObject, creates a bond between this atom and another atom.
        public GameObject CreateBond(Atom atom, GameObject bond)
        {
            //Position bond
            bond.transform.position = (position - atom.position) / 2.0f + atom.position;

            // Scale cylciner
            var scaleBond = bond.transform.localScale;
            scaleBond.y = (position - atom.position).magnitude / 2.0f;
            bond.transform.localScale = scaleBond;

            //Rotate cylinder
            bond.transform.rotation = Quaternion.FromToRotation(Vector3.up, position - atom.position);

            return bond;
        }

        // Centers a list of atoms about the origin
        public static void CenterOnOrigin(List<Atom> atoms)
        {
            Vector3 avgVector = new Vector3(0, 0, 0);

            foreach (Atom atom in atoms)
                avgVector += atom.position;

            avgVector /= atoms.Count;

            foreach (Atom atom in atoms)
                atom.position -= avgVector;
        }

        // Given a list of atoms, this function returns the bounding box
        public static Bounds GetBounds(List<Atom> atoms)
        {
            if (atoms.Count == 0) return new Bounds();

            float x1 = atoms[0].position.x;
            float x2 = atoms[0].position.x;
            float y1 = atoms[0].position.y;
            float y2 = atoms[0].position.y;
            float z1 = atoms[0].position.z;
            float z2 = atoms[0].position.z;

            foreach (Atom atom in atoms)
            {
                x1 = Math.Min(x1, atom.position.x);
                x2 = Math.Max(x2, atom.position.x);
                y1 = Math.Min(y1, atom.position.y);
                y2 = Math.Max(y2, atom.position.y);
                z1 = Math.Min(z1, atom.position.z);
                z2 = Math.Max(z2, atom.position.z);
            }

            float x = x2 - x1;
            float y = y2 - y1;
            float z = z2 - z1;

            Vector3 size = new Vector3(x, y, z);
            Vector3 center = (size / 2) + new Vector3(x1, y1, z1);

            Bounds bounds = new Bounds(center, size);

            return bounds;
        }

    }
}

