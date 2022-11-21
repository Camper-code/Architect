using System.Collections.Generic;

namespace Architect.Output
{
    public class WindowOutput : IReferenceOutput
    {
        public void Write(List<Reference> data)
        {
            Architect.Editor.ArchitectWindow.OpenWindow(data);
        }
    }
}
