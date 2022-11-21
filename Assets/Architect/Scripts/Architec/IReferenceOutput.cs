using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Architect
{
    public interface IReferenceOutput
    {
        public void Write(List<Reference> data);
    }
}