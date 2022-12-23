using System;

namespace Architect
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ParentModuleAttribute : Attribute
    {
        public string moduleName;

        public ParentModuleAttribute(string moduleName)
        {
            this.moduleName = moduleName;
        }
    }
}
