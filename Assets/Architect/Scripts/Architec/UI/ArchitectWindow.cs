using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Architect.Editor
{
    public class ArchitectWindow : EditorWindow
    {
        private const float nodeSize = 100;
        private const float lineWidth = 5;


		public static void OpenWindow(List<Reference> refs)
        {
            ArchitectWindow window = GetWindow<ArchitectWindow>();
            window.titleContent = new GUIContent("Project Architecture");
            window.GenerateNodes(refs);
        }

        private Vector2 viewPos;
        private float scale = 1;
        private Dictionary<string,Module> modules = new Dictionary<string, Module>();
        private List<Reference> references = new List<Reference>();
        

        private void GenerateNodes(List<Reference> refs)
        {
            references = refs;

            refs.ForEach(r => {
                AddNode(r.fromModule, r.fromNode);
                AddNode(r.toModule, r.toNode);
            });

            AlignNodes();
        }

        private void AddNode(string module, string node)
        {
            if (!modules.ContainsKey(module))
                modules.Add(module, new Module(module));

            if (!modules[module].nodes.ContainsKey(node))
                modules[module].nodes.Add(node, new Node(node, modules[module]));
        }

        public List<Node> GetReferencesNodesFrom(Node target)
        {
            return references.FindAll(r => r.fromNode == target.name && r.fromModule == target.parent.name)
                .Select(r => modules[r.toModule].nodes[r.toNode]).ToList();
        }

        public List<Node> GetReferencesNodesTo(Node target)
        {
            return references.FindAll(r => r.toNode == target.name && r.toModule == target.parent.name)
                .Select(r => modules[r.fromModule].nodes[r.fromNode]).ToList();
        }

        public List<Node> GetReferencesNodesBoth(Node target)
        {
            List<Node> refNodes = GetReferencesNodesTo(target);
            refNodes.AddRange(GetReferencesNodesFrom(target));

            return refNodes;
        }

        public float CalculateStrengthOfNode(Node target)
        {
            float fadeIn = GetReferencesNodesTo(target).Count;
            float fade = GetReferencesNodesBoth(target).Count;

            return fadeIn / fade;
        }

        public List<Module> GetReferencesModelsFrom(Module target)
        {
            List<string> clearReference = new List<string>();
            references.FindAll(r => r.fromModule == target.name && r.toModule != r.fromModule)
                .ForEach(r => { if (!clearReference.Contains(r.toModule)) clearReference.Add(r.toModule); });

            return clearReference.Select(r => modules[r]).ToList();
        }

        public List<Module> GetReferencesModelsTo(Module target)
        {
            List<string> clearReference = new List<string>();
            references.FindAll(r => r.toModule == target.name && r.toModule != r.fromModule)
                .ForEach(r => { if (!clearReference.Contains(r.fromModule)) clearReference.Add(r.fromModule); });

            return clearReference.Select(r => modules[r]).ToList();
        }

        public List<Module> GetReferencesModelsBoth(Module target)
        {
            List<Module> refModules = GetReferencesModelsTo(target);
            refModules.AddRange(GetReferencesModelsFrom(target));

            return refModules;
        }


        public float CalculateStrengthOfModule(Module target)
        {
            float fadeIn = GetReferencesModelsTo(target).Count;
            float fade = GetReferencesModelsBoth(target).Count;

            return fadeIn / fade;
        }

        public void OnGUI()
        {
            OnEvent();
            DrawNodes();
            Repaint();
        }

        
        private void AlignNodes()
        {
            List<List<Module>> modulesSheet = new List<List<Module>>();
            for(int i = 0; i < modules.Count / 2f + 1; i++)
            {
                modulesSheet.Add(new List<Module>());
            }

            AlignModules(ref modulesSheet);

            CleanSheet(ref modulesSheet);

			SetModulesPosition(modulesSheet);
		}
        
        private void CleanSheet(ref List<List<Module>> modulesSheet)
        {
			modulesSheet = modulesSheet.Where(ms => ms.Count > 0).ToList();
		}

        private void AlignModules(ref List<List<Module>> modulesSheet)
        {
			foreach (var m in modules)
			{
				AlignInSideModule(m.Value);

				float moduleStrength = CalculateStrengthOfModule(m.Value);
				for (int i = 0; i < modulesSheet.Count; i++)
				{
					if (moduleStrength <= (1f / (modulesSheet.Count - 1)) * i)
					{
						modulesSheet[i].Add(m.Value);
						break;
					}
				}
			}
		}

        private void AlignInSideModule(Module nodule)
        {
			List<List<Node>> nodesSheet = new List<List<Node>>();
			for (int i = 0; i < nodule.nodes.Count / 2f + 1; i++)
			{
				nodesSheet.Add(new List<Node>());
			}

			foreach (var n in nodule.nodes)
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
					nodesSheet[y][x].pos = new Vector2(x, y) * nodeSize * 2 + Vector2.one * 50;
				}
			}

			Module.size = Mathf.Max(max * nodeSize * 2, Module.size);
		}

        private void SetModulesPosition(List<List<Module>> modulesSheet)
        {
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
                        GUI.Box(new Rect((node.Value.globalPos + viewPos) * scale, new Vector2(nodeSize, nodeSize) * scale), node.Value.name);
                        GetReferencesNodesFrom(node.Value).ForEach(rn =>
                        {
                            MakeArrowBetween(node.Value.globalPos + viewPos, rn.globalPos + viewPos, nodeSize);
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

            Vector2 arrowStartTangent = Vector2.zero;
            Vector2 arrowEndTangent = Vector2.zero;

            int tangentForce = (int)blockSize;
            float halfSize = blockSize / 2;
            float quarterSize = blockSize / 4;

            Vector2 fromCenter = from + Vector2.one * halfSize;
            Vector2 toCenter = to + Vector2.one * halfSize;

            if (toCenter.y > (toCenter.x - fromCenter.x) + fromCenter.y && toCenter.y > -(toCenter.x - fromCenter.x) + fromCenter.y)
            {
                arrowStart = from + Vector2.up * halfSize + Vector2.right * quarterSize;
                arrowEnd = to + Vector2.down * halfSize + Vector2.right * quarterSize;
                arrowStartTangent = arrowStart + Vector2.up * tangentForce;
                arrowEndTangent = arrowEnd + Vector2.down * tangentForce;
            }else if (toCenter.y < (toCenter.x - fromCenter.x) + fromCenter.y && toCenter.y > -(toCenter.x - fromCenter.x) + fromCenter.y)
            {
                arrowStart = from + Vector2.right * halfSize + Vector2.down * quarterSize;
                arrowEnd = to + Vector2.left * halfSize + Vector2.down * quarterSize;
                arrowStartTangent = arrowStart + Vector2.right * tangentForce;
                arrowEndTangent = arrowEnd + Vector2.left * tangentForce;
            }
            else if (toCenter.y > (toCenter.x - fromCenter.x) + fromCenter.y && toCenter.y < -(toCenter.x - fromCenter.x) + fromCenter.y)
            {
                arrowStart = from + Vector2.left * halfSize + Vector2.up * quarterSize;
                arrowEnd = to + Vector2.right * halfSize + Vector2.up * quarterSize;
                arrowStartTangent = arrowStart + Vector2.left * tangentForce;
                arrowEndTangent = arrowEnd + Vector2.right * tangentForce;
            }
            else
            {
                arrowStart = from + Vector2.down * halfSize + Vector2.left * quarterSize;
                arrowEnd = to + Vector2.up * halfSize + Vector2.left * quarterSize;
                arrowStartTangent = arrowStart + Vector2.down * tangentForce;
                arrowEndTangent = arrowEnd + Vector2.up * tangentForce;
            }

            arrowEnd += Vector2.one * halfSize;
            arrowStart += Vector2.one * halfSize;
            arrowStartTangent += Vector2.one * halfSize;
            arrowEndTangent += Vector2.one * halfSize;

            Handles.DrawBezier(arrowStart, arrowEnd, arrowStartTangent, arrowEndTangent, Color.white, null, lineWidth);
            DrawArrow(arrowEnd, (arrowEnd - arrowEndTangent).normalized);
            
        }

        private void DrawArrow(Vector2 pos, Vector2 direction)
        {
            direction *= 10;
            Vector2 perpend = new Vector2(direction.y, -direction.x);
            Handles.DrawPolyLine(pos - direction + perpend, pos, pos - direction - perpend);
        }

        public void OnEvent()
        {
            Event @event = Event.current;

            if (@event.type == EventType.MouseDrag)
            {
                viewPos += @event.delta / scale;
                GUI.changed = true;
            }
            if(@event.type == EventType.ScrollWheel)
            {
                scale = Mathf.Clamp(scale - @event.delta.normalized.y * 0.1f, 0.01f, 10);
                GUI.changed = true;
            }
        }
    }
}


