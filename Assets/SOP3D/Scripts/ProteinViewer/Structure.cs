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


using System;

namespace Sop.ProteinViewer
{
    public class Structure
    {
        public int startSeqNum;
        public int endSeqNum;
        public string chainID;
        public Type type;

        public enum Type {Helix, Sheet, Loop};

        public Structure(string pdbLine)
        {
            string type = pdbLine.Substring(0, 6).Trim();
            switch (type)
            {
                case "HELIX":
                    chainID = pdbLine.Substring(19, 1);
                    startSeqNum = Convert.ToInt32(pdbLine.Substring(21,4));
                    endSeqNum = Convert.ToInt32(pdbLine.Substring(33,4));
                    this.type = Type.Helix;
                    break;
                case "SHEET":
                    chainID = pdbLine.Substring(21, 1);
                    startSeqNum = Convert.ToInt32(pdbLine.Substring(22,4));
                    endSeqNum = Convert.ToInt32(pdbLine.Substring(33,4));
                    this.type = Type.Sheet;
                    break;
            }
        }
    }
}

