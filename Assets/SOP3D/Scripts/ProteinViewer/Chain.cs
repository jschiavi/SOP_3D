using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Sop.ProteinViewer
{
    public class Chain
    {

        public List<Atom> atoms;
        public List<Structure> structures;
        public string id;
        //public Vector3 relativePosition;

        public Chain (List<Atom> atoms, List<Structure> structures)
        {
            //this.relativePosition = pos;
            this.atoms = atoms;
            this.id = atoms[0].chainID;
            this.structures = structures;
            CreateBackbone();
        }

        private void CreateBackbone()
        {
            Atom prevAtom = null;
            foreach (Atom curAtom in atoms)
            {
                if (prevAtom != null)
                {
                    prevAtom.nextAtom = curAtom;
                    curAtom.prevAtom = prevAtom;
                }
                prevAtom = curAtom;
            }
        }

        public Bounds GetBounds()
        {
            return Atom.GetBounds(atoms);
        }

        public void CenterOnOrigin()
        {
            Atom.CenterOnOrigin(atoms);
        }
    }
}
