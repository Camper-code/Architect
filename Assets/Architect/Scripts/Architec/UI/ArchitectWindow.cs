using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Architect.Editor
{
    public class Module
    {
        static public float size = 100;

        public string name;
        public Vector2 pos;
        public Dictionary<string, Node> nodes = new Dictionary<string, Node>();

        public Module(Vector2 pos, string name)
        {
            this.pos = pos;
            this.name = name;
        }
    }

    public class Node
    {
        public string name;
        public Module parent;
        public Vector2 pos;
        public Vector2 globalPos
        {
            get => parent.pos + pos;
            set => pos = value - parent.pos;
        }



        public Node(Vector2 pos, string name, Module parent)
        {
            this.pos = pos;
            this.name = name;
            this.parent = parent;
        }
    }

    public class ArchitectWindow : EditorWindow
    {
        private GUIStyle style;
        private Vector2 viewPos;
        float scale = 1;

        public static void OpenWindow(List<Reference> refs)
        {
            ArchitectWindow window = GetWindow<ArchitectWindow>();
            window.titleContent = new GUIContent("Project Architecture");
            window.GenerateNodes(refs);
        }

        private Dictionary<string,Module> modules = new Dictionary<string, Module>();
        private List<Reference> references = new List<Reference>();
        
        private void AddNode(string module, string node)
        {
            if (!modules.ContainsKey(module))
                modules.Add(module, new Module(new Vector2(Random.Range(-1000,1000), Random.Range(-1000, 1000)),module));

            if (!modules[module].nodes.ContainsKey(node))
                modules[module].nodes.Add(node, new Node(new Vector2(Random.Range(-250,150), Random.Range(-250, 150)), node, modules[module]));
        }

        private void GenerateNodes(List<Reference> refs)
        {
            references = refs;

            refs.ForEach(r => {
                AddNode(r.fromModule, r.from);
                AddNode(r.toModule, r.to);
            });

            AlignNodes();
        }

        public List<Node> GetReferencesNodesFrom(Node focus)
        {
            return references.FindAll(r => r.from == focus.name && r.fromModule == focus.parent.name)
                .Select(r => modules[r.toModule].nodes[r.to]).ToList();
        }

        public List<Node> GetReferencesNodesTo(Node focus)
        {
            return references.FindAll(r => r.to == focus.name && r.toModule == focus.parent.name)
                .Select(r => modules[r.fromModule].nodes[r.from]).ToList();
        }

        public List<Node> GetReferencesNodesBoth(Node focus)
        {
            List<Node> refNodes = GetReferencesNodesTo(focus);
            refNodes.AddRange(GetReferencesNodesFrom(focus));

            return refNodes;
        }

        public float CalculateStrengthOfNode(Node focus)
        {
            float fadeIn = GetReferencesNodesTo(focus).Count;
            float fade = GetReferencesNodesBoth(focus).Count;

            return fadeIn / fade;
        }

        public List<Module> GetReferencesModelsFrom(Module focus)
        {
            List<string> clearReference = new List<string>();
            references.FindAll(r => r.fromModule == focus.name && r.toModule != r.fromModule)
                .ForEach(r => { if (!clearReference.Contains(r.toModule)) clearReference.Add(r.toModule); });

            return clearReference.Select(r => modules[r]).ToList();
        }

        public List<Module> GetReferencesModelsTo(Module focus)
        {
            List<string> clearReference = new List<string>();
            references.FindAll(r => r.toModule == focus.name && r.toModule != r.fromModule)
                .ForEach(r => { if (!clearReference.Contains(r.fromModule)) clearReference.Add(r.fromModule); });

            return clearReference.Select(r => modules[r]).ToList();
        }

        public List<Module> GetReferencesModelsBoth(Module focus)
        {
            List<Module> refModules = GetReferencesModelsTo(focus);
            refModules.AddRange(GetReferencesModelsFrom(focus));

            return refModules;
        }


        public float CalculateStrengthOfModule(Module focus)
        {
            float fadeIn = GetReferencesModelsTo(focus).Count;
            float fade = GetReferencesModelsBoth(focus).Count;

            return fadeIn / fade;
        }

        public void OnEnable()
        {
            style = new GUIStyle();
            style.normal.background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png");
            style.border = new RectOffset(12, 12, 12, 12);
        }

        public void OnGUI()
        {
            OnEvent();

            DrawNodes();

            Handles.DrawBezier(Vector3.one* - 100, Vector3.one* 100, Vector3.zero, Vector3.zero, Color.white, null, 25);

            Repaint();
        }

        
        private void AlignNodes()
        {
            List<List<Module>> modulesSheet = new List<List<Module>>();
            for(int i = 0; i < modules.Count / 2f + 1; i++)
            {
                modulesSheet.Add(new List<Module>());
            }

            foreach (var m in modules)
            {

                List<List<Node>> nodesSheet = new List<List<Node>>();
                for (int i = 0; i < m.Value.nodes.Count / 2f + 1; i++)
                {
                    nodesSheet.Add(new List<Node>());
                }
                
                foreach (var n in m.Value.nodes)
                {
                    float nodeStrength = CalculateStrengthOfNode(n.Value);
                    for (int i = 0; i < nodesSheet.Count; i++)
                    {
                        if (nodeStrength <= 1f / (nodesSheet.Count - 1) * i)
                        {
                            nodesSheet[i].Add(n.Value);
                            break;
                        }
                    }
                }

                nodesSheet = nodesSheet.Where(ns => ns.Count > 0).ToList();

                int max = nodesSheet.Count;
                for (int y = 0; y < nodesSheet.Count; y++)
                {
                    max = Mathf.Max(max, nodesSheet[y].Count);
                    for (int x = 0; x < nodesSheet[y].Count; x++)
                    {
                        nodesSheet[y][x].pos = new Vector2(x, y) * 200 + Vector2.one * 50;
                    }
                }
                Module.size = Mathf.Max(max * 200, Module.size);

                float moduleStrength = CalculateStrengthOfModule(m.Value);
                for (int i = 0; i < modulesSheet.Count; i++)
                {
                    if (moduleStrength <= (1f / (modulesSheet.Count-1)) * i)
                    {
                        modulesSheet[i].Add(m.Value);
                        break;
                    }
                }
            }

           
            modulesSheet = modulesSheet.Where(ms => ms.Count > 0).ToList();


            for (int y = 0; y < modulesSheet.Count; y++)
            {
                for (int x = 0; x < modulesSheet[y].Count; x++)
                {
                    modulesSheet[y][x].pos = new Vector2(x, y) * Module.size * 2;
                }
            }
        }
        

        private void DrawNodes()
        {
            foreach (var module in modules)
            {
                GUI.backgroundColor = Color.red;
                GUI.Box(new Rect((module.Value.pos + viewPos) * scale, Vector2.one * Module.size * scale), module.Value.name);
                if (scale < 0.7f)
                {
                    GetReferencesModelsFrom(module.Value).ForEach(rm =>
                        {
                            MakeArrowBetween(module.Value.pos + viewPos, rm.pos + viewPos, Module.size);
                        });

                    
                }else
                {
                    foreach (var node in module.Value.nodes)
                    {
                        GUI.backgroundColor = Color.blue;
                        GUI.Box(new Rect((node.Value.globalPos + viewPos) * scale, new Vector2(100, 100) * scale), node.Value.name);
                        GetReferencesNodesFrom(node.Value).ForEach(rn =>
                        {
                            MakeArrowBetween(node.Value.globalPos + viewPos, rn.globalPos + viewPos, 100);
                        });
                    }
                }
            }
        }

        private void MakeArrowBetween(Vector2 from, Vector2 to, float blockSize)
        {
            from *= scale;
            to *= scale;
            blockSize *= scale;

            Vector2 arrowStart = Vector2.zero;
            Vector2 arrowEnd = Vector2.zero;

            Vector2 arrowStartTanget = Vector2.zero;
            Vector2 arrowEndTanget = Vector2.zero;

            int tangetForce = (int)blockSize;
            float halfSize = blockSize / 2;
            float quarterSize = blockSize / 4;

            Vector2 fromC = from + Vector2.one * halfSize;
            Vector2 toC = to + Vector2.one * halfSize;

            if (toC.y > (toC.x - fromC.x) + fromC.y && toC.y > -(toC.x - fromC.x) + fromC.y)
            {
                arrowStart = from + Vector2.up * halfSize + Vector2.right * quarterSize;
                arrowEnd = to + Vector2.down * halfSize + Vector2.right * quarterSize;
                arrowStartTanget = arrowStart + Vector2.up * tangetForce;
                arrowEndTanget = arrowEnd + Vector2.down * tangetForce;
            }else if (toC.y < (toC.x - fromC.x) + fromC.y && toC.y > -(toC.x - fromC.x) + fromC.y)
            {
                arrowStart = from + Vector2.right * halfSize + Vector2.down * quarterSize;
                arrowEnd = to + Vector2.left * halfSize + Vector2.down * quarterSize;
                arrowStartTanget = arrowStart + Vector2.right * tangetForce;
                arrowEndTanget = arrowEnd + Vector2.left * tangetForce;
            }
            else if (toC.y > (toC.x - fromC.x) + fromC.y && toC.y < -(toC.x - fromC.x) + fromC.y)
            {
                arrowStart = from + Vector2.left * halfSize + Vector2.up * quarterSize;
                arrowEnd = to + Vector2.right * halfSize + Vector2.up * quarterSize;
                arrowStartTanget = arrowStart + Vector2.left * tangetForce;
                arrowEndTanget = arrowEnd + Vector2.right * tangetForce;
            }
            else
            {
                arrowStart = from + Vector2.down * halfSize + Vector2.left * quarterSize;
                arrowEnd = to + Vector2.up * halfSize + Vector2.left * quarterSize;
                arrowStartTanget = arrowStart + Vector2.down * tangetForce;
                arrowEndTanget = arrowEnd + Vector2.up * tangetForce;
            }

            arrowEnd += Vector2.one * halfSize;
            arrowStart += Vector2.one * halfSize;
            arrowStartTanget += Vector2.one * halfSize;
            arrowEndTanget += Vector2.one * halfSize;

            Handles.DrawBezier(arrowStart, arrowEnd, arrowStartTanget, arrowEndTanget, Color.white, null, 5);
            DrawArrow(arrowEnd, (arrowEnd - arrowEndTanget).normalized);
            
        }

        private void DrawArrow(Vector2 pos, Vector2 direction)
        {
            direction *= 10;
            Vector2 perpend = new Vector2(direction.y, -direction.x);
            Handles.DrawPolyLine(pos - direction + perpend, pos, pos - direction - perpend);
        }

        public void OnEvent()
        {
            Event e = Event.current;

            if (e.type == EventType.MouseDrag)
            {
                viewPos += e.delta / scale;
                GUI.changed = true;
            }
            if(e.type == EventType.ScrollWheel)
            {
                scale = Mathf.Clamp(scale - e.delta.normalized.y * 0.1f, 0.01f, 10);
                GUI.changed = true;
            }
            //GUIUtility.ScaleAroundPivot(Vector2.one / scale, Vector2.zero * 100);
        }
    }
}


