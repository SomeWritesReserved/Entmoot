using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Entmoot.TestGame3D
{
	public class Bone
	{
		public Bone(string name) { this.Name = name; }

		public string Name { get; }
		public Quaternion Rotation = Quaternion.Identity;
		public Vector3 OffsetFromParent;
		public Vector3 Size;
		public Bone Parent { get; private set; }

		private Bone[] children;
		public Bone[] Children
		{
			get { return this.children; }
			set
			{
				this.children = value;
				foreach (Bone child in this.children)
				{
					child.Parent = this;
				}
			}
		}
	}

	public class BoneAnimation : Dictionary<string, Quaternion>
	{
	}
}
