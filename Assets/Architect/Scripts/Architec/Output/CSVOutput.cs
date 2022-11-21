using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


namespace Architect.Output
{
    public class CSVOutput : IReferenceOutput
    {
        private string ReferenceToString(Reference reference)
        {
            return $"{reference.from};{reference.to}"; 
        }

        public void Write(List<Reference> data)
        {
            data.Sort((a, b) => string.Compare(a.ToString(), b.ToString()));

            StreamWriter writer = new StreamWriter("ReferenceData.csv");

            data.ForEach(r => writer.WriteLine(ReferenceToString(r)));

            writer.Close();
        }
    }
}
