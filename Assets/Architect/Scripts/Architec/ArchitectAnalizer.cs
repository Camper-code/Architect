using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;

namespace Architect
{

    public class ArchitectAnalizer
    {
        [MenuItem("Architect/View")]
        static private void AnalyzeToWindow() => Analyze(new Output.WindowOutput());

        static private void Analyze(IReferenceOutput output)
        {
            Type[] types = GetAllUserTypes();
            FieldInfo[] fields = CollectFields(types);
            List<Reference> references = DetectReferences(fields);
            output.Write(references);
        }

        static private Type[] GetAllUserTypes()
        {
            Assembly currentAssembly = Assembly.GetAssembly(typeof(ArchitectAnalizer));

			Type[] allTypes = currentAssembly.GetTypes();
            Type[] userTypes = allTypes.Where(t => !t.FullName.Contains("Architect")).ToArray();

			return userTypes;
		}

        static private FieldInfo[] CollectFields(Type[] types)
        {
            List<FieldInfo> fields = new List<FieldInfo>();
            foreach(Type type in types)
            {
                FieldInfo[] typeFields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo[] correctFields = typeFields.Where(f => types.Contains(f.FieldType)).ToArray();
				fields.AddRange(correctFields);
			}
            return fields.ToArray();
        }

        static private List<Reference> DetectReferences(FieldInfo[] fields)
        {
            List<Reference> references = new List<Reference>();

            foreach (FieldInfo field in fields)
            {
                Type declaringType = field.DeclaringType;


				string fromModule = null;
                string toModule = null;
                if (Attribute.IsDefined(declaringType, typeof(ParentModuleAttribute)))
                    fromModule = ((ParentModuleAttribute)Attribute.GetCustomAttribute(declaringType, typeof(ParentModuleAttribute))).moduleName;

                if (Attribute.IsDefined(field.FieldType, typeof(ParentModuleAttribute)))
                    toModule = ((ParentModuleAttribute)Attribute.GetCustomAttribute(field.FieldType, typeof(ParentModuleAttribute))).moduleName;

                if (fromModule != null && toModule != null)
                    references.Add(new Reference(declaringType.ToString(), field.FieldType.ToString(), fromModule, toModule));
            }

            return references;
		}
	}
}
