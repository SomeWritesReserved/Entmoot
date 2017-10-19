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
		public void Blend(SkeletonKeyframe otherA, SkeletonKeyframe otherB, float amount)
		{
			this.Clear();
			foreach (var kvp in otherA)
			{
				if (!otherB.TryGetValue(kvp.Key, out Quaternion otherRotation)) { otherRotation = Quaternion.Identity; }
				this[kvp.Key] = Quaternion.Slerp(kvp.Value, otherRotation, amount);
			}
			foreach (var kvp in otherB)
			{
				if (this.ContainsKey(kvp.Key)) { continue; }

				if (!otherA.TryGetValue(kvp.Key, out Quaternion otherRotation)) { otherRotation = Quaternion.Identity; }
				this[kvp.Key] = Quaternion.Slerp(otherRotation, kvp.Value, amount);
			}
		}
	}

	public class SkeletonAnimation
	{
		public SkeletonKeyframe[] SkeletonKeyframes;

		public void GetAnimation(int frameTick, int ticksBetweenKeyframes, out SkeletonKeyframe keyframePrevious, out SkeletonKeyframe keyframeStart, out SkeletonKeyframe keyframeEnd, out SkeletonKeyframe keyframeNext, out float amount)
		{
			int counts = (frameTick % (ticksBetweenKeyframes * this.SkeletonKeyframes.Length));
			int index = counts / ticksBetweenKeyframes;

			amount = ((frameTick % ticksBetweenKeyframes) / (float)ticksBetweenKeyframes);
			keyframePrevious = this.SkeletonKeyframes[this.getBoundedIndex(index - 1)];
			keyframeStart = this.SkeletonKeyframes[index];
			keyframeEnd = this.SkeletonKeyframes[this.getBoundedIndex(index + 1)];
			keyframeNext = this.SkeletonKeyframes[this.getBoundedIndex(index + 2)];
		}

		private int getBoundedIndex(int index)
		{
			return (index % this.SkeletonKeyframes.Length + this.SkeletonKeyframes.Length) % this.SkeletonKeyframes.Length;
		}

		public void Blend(SkeletonAnimation otherA, SkeletonAnimation otherB, float amount)
		{
			for (int i = 0; i < otherA.SkeletonKeyframes.Length; i++)
			{
				this.SkeletonKeyframes[i].Blend(otherA.SkeletonKeyframes[i], otherB.SkeletonKeyframes[i], amount);
			}
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

		private static SkeletonAnimation walkAnimation;
		public static SkeletonAnimation WalkAnimation
		{
			get
			{
				if (walkAnimation == null)
				{
					SkeletonKeyframe runKeyframe1 = new SkeletonKeyframe();
					SkeletonKeyframe runKeyframe2 = new SkeletonKeyframe();
					SkeletonKeyframe runKeyframe3 = new SkeletonKeyframe();
					SkeletonKeyframe runKeyframe4 = new SkeletonKeyframe();

					runKeyframe1["Upper Arm - Right"] = new Quaternion(0.999712f, 0f, 0f, 0.02399765f);
					runKeyframe1["Upper Arm - Left"] = new Quaternion(0.99955f, 0f, 0f, 0.02999546f);
					runKeyframe1["Upper Leg - Right"] = new Quaternion(0.9941735f, 0f, 0f, -0.1077902f);
					runKeyframe1["Upper Leg - Left"] = new Quaternion(0.999982f, 0f, 0f, 0.005999918f);
					runKeyframe1["Lower Leg - Right"] = new Quaternion(-0.4723895f, 0f, 0f, 0.8813895f);
					runKeyframe1["Lower Leg - Left"] = new Quaternion(-0.0299955f, 0f, 0f, 0.99955f);
					runKeyframe1["Foot - Left"] = new Quaternion(0.04198766f, 0f, 0f, 0.9991181f);
					runKeyframe1["Lower Torso"] = new Quaternion(-0.005999964f, 0f, 0f, 0.999982f);
					runKeyframe1["Head"] = new Quaternion(-0.01199971f, 0f, 0f, 0.999928f);
					runKeyframe1["Lower Arm - Right"] = new Quaternion(0.01799903f, 0f, 0f, 0.999838f);
					runKeyframe1["Hand - Right"] = new Quaternion(0.01799903f, 0f, 0f, 0.999838f);
					runKeyframe1["Lower Arm - Left"] = new Quaternion(0.0299955f, 0f, 0f, 0.99955f);
					runKeyframe1["Hand - Left"] = new Quaternion(0f, 0f, 0f, 1f);
					runKeyframe1["Foot - Right"] = new Quaternion(-0.03599223f, 0f, 0f, 0.999352f);
					runKeyframe1["Toes - Left"] = new Quaternion(-0.005999964f, 0f, 0f, 0.999982f);

					runKeyframe2["Upper Arm - Right"] = new Quaternion(0.9974091f, 0f, 0f, 0.07193777f);
					runKeyframe2["Upper Arm - Left"] = new Quaternion(0.9982005f, 0f, 0f, -0.05996405f);
					runKeyframe2["Upper Leg - Right"] = new Quaternion(0.9928086f, 0f, 0f, -0.1197123f);
					runKeyframe2["Upper Leg - Left"] = new Quaternion(0.990493f, 0f, 0f, 0.1375624f);
					runKeyframe2["Lower Torso"] = new Quaternion(-0.005999964f, 0f, 0f, 0.999982f);
					runKeyframe2["Upper Torso"] = new Quaternion(-0.005999964f, 0f, 0f, 0.999982f);
					runKeyframe2["Lower Leg - Right"] = new Quaternion(-0.01199971f, 0f, 0f, 0.999928f);
					runKeyframe2["Foot - Right"] = new Quaternion(-0.1375624f, 0f, 0f, 0.9904929f);
					runKeyframe2["Lower Leg - Left"] = new Quaternion(-0.04798157f, 0f, 0f, 0.9988482f);
					runKeyframe2["Foot - Left"] = new Quaternion(0.04798158f, 0f, 0f, 0.9988482f);
					runKeyframe2["Toes - Left"] = new Quaternion(0.131617f, 0f, 0f, 0.9913006f);
					runKeyframe2["Head"] = new Quaternion(0.005999964f, 0f, 0f, 0.999982f);
					runKeyframe2["Lower Arm - Right"] = new Quaternion(0.0239977f, 0f, 0f, 0.999712f);
					runKeyframe2["Hand - Right"] = new Quaternion(0.01799903f, 0f, 0f, 0.999838f);
					runKeyframe2["Lower Arm - Left"] = new Quaternion(0.0239977f, 0f, 0f, 0.999712f);
					runKeyframe2["Hand - Left"] = new Quaternion(0.005999964f, 0f, 0f, 0.999982f);

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
					walkAnimation = new SkeletonAnimation()
					{
						SkeletonKeyframes = new SkeletonKeyframe[]
						{
							runKeyframe1,
							runKeyframe2,
							runKeyframe3,
							runKeyframe4,
						},
					};
				}
				return walkAnimation;
			}
		}
	}
}
