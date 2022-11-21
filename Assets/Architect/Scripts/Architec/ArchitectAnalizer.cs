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
        [MenuItem("Architect/CollectData to CSV")]
        static private void AnalyzeToCSV() => Analyze(new Output.CSVOutput());

        [MenuItem("Architect/CollectData to RAW")]
        static private void AnalyzeToRAW() => Analyze(new Output.RAWOutput());

        [MenuItem("Architect/View")]
        static private void AnalyzeToWindow() => Analyze(new Output.WindowOutput());

        static private void Analyze(IReferenceOutput output)
        {
            List<Reference> references = new List<Reference>();

            List<Type> types = Assembly.GetAssembly(typeof(ArchitectAnalizer)).GetTypes().Where(t => !t.FullName.Contains("Architect")).ToList();
            types.ForEach(t =>
            {
                t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(f => types.Contains(f.FieldType)).ToList().ForEach(f =>
                {
                    string fromModule = "---";
                    string toModule = "---";
                    if (Attribute.IsDefined(t, typeof(ParentModuleAttribute)))
                        fromModule = ((ParentModuleAttribute)Attribute.GetCustomAttribute(t, typeof(ParentModuleAttribute))).moduleName;

                    if (Attribute.IsDefined(f.FieldType, typeof(ParentModuleAttribute)))
                        toModule = ((ParentModuleAttribute)Attribute.GetCustomAttribute(f.FieldType, typeof(ParentModuleAttribute))).moduleName;

                    references.Add(new Reference(t.ToString(), f.FieldType.ToString(), fromModule, toModule));
                });
            });

            output.Write(references);

            EditorUtility.DisplayDialog("Save", "Data collected", "OK");
        }
    }
}
