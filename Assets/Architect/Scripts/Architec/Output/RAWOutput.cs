using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


namespace Architect.Output
{
    public class RAWOutput : IReferenceOutput
    {
        private string ReferenceToString(Reference reference)
        {
            return $"{reference.from} {reference.to}";
        }

        public void Write(List<Reference> data)
        {
            StreamWriter writer = new StreamWriter("ReferenceData.txt");

            data.ForEach(r => writer.WriteLine(ReferenceToString(r)));

            writer.Close();
        }
    }
}