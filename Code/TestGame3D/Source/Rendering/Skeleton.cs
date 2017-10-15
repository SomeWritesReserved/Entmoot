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

	public class SkeletonKeyframe : Dictionary<string, Quaternion>
	{
	}

	public class SkeletonAnimation
	{
		public SkeletonKeyframe[] SkeletonKeyframes;
		public int TicksBetweenKeyframes;

		public void GetAnimation(int frameTick, out SkeletonKeyframe keyframePrevious, out SkeletonKeyframe keyframeStart, out SkeletonKeyframe keyframeEnd, out SkeletonKeyframe keyframeNext, out float amount)
		{
			int counts = (frameTick % (this.TicksBetweenKeyframes * this.SkeletonKeyframes.Length));
			int index = counts / this.TicksBetweenKeyframes;

			amount = ((frameTick % this.TicksBetweenKeyframes) / (float)this.TicksBetweenKeyframes);
			keyframePrevious = this.SkeletonKeyframes[this.getBoundedIndex(index - 1)];
			keyframeStart = this.SkeletonKeyframes[index];
			keyframeEnd = this.SkeletonKeyframes[this.getBoundedIndex(index + 1)];
			keyframeNext = this.SkeletonKeyframes[this.getBoundedIndex(index + 2) ];
		}

		private int getBoundedIndex(int index)
		{
			return (index % this.SkeletonKeyframes.Length + this.SkeletonKeyframes.Length) % this.SkeletonKeyframes.Length;
		}
	}

	public static class DefinedAnimations
	{
		private static SkeletonAnimation bindAnimation;
		public static SkeletonAnimation BindAnimation
		{
			get
			{
				if (bindAnimation == null)
				{
					SkeletonKeyframe bindKeyframe1 = new SkeletonKeyframe();

					bindKeyframe1["Upper Arm - Right"] = Quaternion.CreateFromYawPitchRoll(0, MathHelper.ToRadians(180), 0);
					bindKeyframe1["Upper Arm - Left"] = Quaternion.CreateFromYawPitchRoll(0, MathHelper.ToRadians(180), 0);
					bindKeyframe1["Upper Leg - Right"] = Quaternion.CreateFromYawPitchRoll(0, MathHelper.ToRadians(180), 0);
					bindKeyframe1["Upper Leg - Left"] = Quaternion.CreateFromYawPitchRoll(0, MathHelper.ToRadians(180), 0);
					bindAnimation = new SkeletonAnimation()
					{
						TicksBetweenKeyframes = 1,
						SkeletonKeyframes = new SkeletonKeyframe[]
						{
							bindKeyframe1,
						},
					};
				}
				return bindAnimation;
			}
		}

		private static SkeletonAnimation runAnimation;
		public static SkeletonAnimation RunAnimation
		{
			get
			{
				if (runAnimation == null)
				{
					SkeletonKeyframe runKeyframe1 = new SkeletonKeyframe();
					SkeletonKeyframe runKeyframe2 = new SkeletonKeyframe();
					SkeletonKeyframe runKeyframe3 = new SkeletonKeyframe();
					SkeletonKeyframe runKeyframe4 = new SkeletonKeyframe();

					runKeyframe1["Upper Arm - Right"] = new Quaternion(0.8756577f, 0f, 0f, 0.482932f);
					runKeyframe1["Upper Arm - Left"] = new Quaternion(0.9588137f, 0f, 0f, -0.2840351f);
					runKeyframe1["Upper Leg - Right"] = new Quaternion(0.9849001f, 0f, 0f, 0.1731233f);
					runKeyframe1["Upper Leg - Left"] = new Quaternion(0.8320514f, 0f, 0f, -0.554698f);
					runKeyframe1["Lower Torso"] = new Quaternion(-0.0299955f, 0f, 0f, 0.99955f);
					runKeyframe1["Upper Torso"] = new Quaternion(-0.005999964f, 0f, 0f, 0.999982f);
					runKeyframe1["Neck"] = new Quaternion(-0.01199971f, 0f, 0f, 0.999928f);
					runKeyframe1["Head"] = new Quaternion(-0.0239977f, 0f, 0f, 0.999712f);
					runKeyframe1["Lower Arm - Right"] = new Quaternion(0.4132316f, 0f, 0f, 0.910626f);
					runKeyframe1["Hand - Right"] = new Quaternion(0.03599222f, 0f, 0f, 0.999352f);
					runKeyframe1["Lower Arm - Left"] = new Quaternion(0.7870421f, 0f, 0f, 0.6168985f);
					runKeyframe1["Hand - Left"] = new Quaternion(0.08390125f, 0f, 0f, 0.996474f);
					runKeyframe1["Lower Leg - Right"] = new Quaternion(-0.5794061f, 0f, 0f, 0.8150386f);
					runKeyframe1["Foot - Right"] = new Quaternion(-0.07193781f, 0f, 0f, 0.997409f);
					runKeyframe1["Toes - Right"] = new Quaternion(-0.04198765f, 0f, 0f, 0.9991181f);
					runKeyframe1["Lower Leg - Left"] = new Quaternion(-0.4349655f, 0f, 0f, 0.900447f);
					runKeyframe1["Foot - Left"] = new Quaternion(-0.1137532f, 0f, 0f, 0.993509f);
					runKeyframe1["Toes - Left"] = new Quaternion(-0.0659521f, 0f, 0f, 0.9978227f);

					runKeyframe2["Upper Arm - Right"] = new Quaternion(0.9887707f, 0f, 0f, 0.1494382f);
					runKeyframe2["Upper Arm - Left"] = new Quaternion(0.9998378f, 0f, 0f, -0.01799901f);
					runKeyframe2["Upper Leg - Right"] = new Quaternion(0.9959527f, 0f, 0f, -0.08987861f);
					runKeyframe2["Upper Leg - Left"] = new Quaternion(0.9904928f, 0f, 0f, -0.1375626f);
					runKeyframe2["Lower Torso"] = new Quaternion(-0.02399769f, 0f, 0f, 0.999712f);
					runKeyframe2["Upper Torso"] = new Quaternion(0f, 0f, 0f, 1f);
					runKeyframe2["Neck"] = new Quaternion(-0.01199971f, 0f, 0f, 0.999928f);
					runKeyframe2["Head"] = new Quaternion(-0.0239977f, 0f, 0f, 0.999712f);
					runKeyframe2["Lower Arm - Right"] = new Quaternion(0.318361f, 0f, 0f, 0.9479695f);
					runKeyframe2["Hand - Right"] = new Quaternion(0.03599222f, 0f, 0f, 0.999352f);
					runKeyframe2["Lower Arm - Left"] = new Quaternion(0.6816384f, 0f, 0f, 0.7316884f);
					runKeyframe2["Hand - Left"] = new Quaternion(0.08390125f, 0f, 0f, 0.996474f);
					runKeyframe2["Lower Leg - Right"] = new Quaternion(-0.6457458f, 0f, 0f, 0.763552f);
					runKeyframe2["Foot - Right"] = new Quaternion(-0.1435028f, 0f, 0f, 0.9896498f);
					runKeyframe2["Toes - Right"] = new Quaternion(-0.07193781f, 0f, 0f, 0.9974091f);
					runKeyframe2["Lower Leg - Left"] = new Quaternion(-0.2318703f, 0f, 0f, 0.9727465f);
					runKeyframe2["Foot - Left"] = new Quaternion(0.07193785f, 0f, 0f, 0.997409f);
					runKeyframe2["Toes - Left"] = new Quaternion(0.059964f, 0f, 0f, 0.9982005f);

					foreach (var walk1 in runKeyframe1)
					{
						string name = walk1.Key.Contains("Left") ? walk1.Key.Replace("Left", "Right") :
							walk1.Key.Contains("Right") ? walk1.Key.Replace("Right", "Left") :
							walk1.Key;
						runKeyframe3[name] = walk1.Value;
					}

					foreach (var walk2 in runKeyframe2)
					{
						string name = walk2.Key.Contains("Left") ? walk2.Key.Replace("Left", "Right") :
							walk2.Key.Contains("Right") ? walk2.Key.Replace("Right", "Left") :
							walk2.Key;
						runKeyframe4[name] = walk2.Value;
					}
					runAnimation = new SkeletonAnimation()
					{
						TicksBetweenKeyframes = 15,
						SkeletonKeyframes = new SkeletonKeyframe[]
						{
							runKeyframe1,
							runKeyframe2,
							runKeyframe3,
							runKeyframe4,
						},
					};
				}
				return runAnimation;
			}
		}
	}
}
