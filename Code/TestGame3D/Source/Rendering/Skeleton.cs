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
		public Quaternion Rotation;
		public Vector3 OffsetFromParent;
		public Vector3 Size;

		public Bone[] Children;
	}

	public class BoneAnimation : Dictionary<string, Quaternion>
	{
	}
}
