using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Architect.Editor
{
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

		public Node(string name, Module parent)
		{
			this.pos = Vector2.zero;
			this.name = name;
			this.parent = parent;
		}
	}
}