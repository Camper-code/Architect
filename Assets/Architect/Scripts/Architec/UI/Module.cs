
using System.Collections.Generic;
using UnityEngine;

namespace Architect.Editor
{
	public class Module
	{
		static public float size = 100;

		public string name;
		public Vector2 pos;
		public Dictionary<string, Node> nodes = new Dictionary<string, Node>();

		public Module(string name)
		{
			this.pos = Vector2.zero;
			this.name = name;
		}
	}
}