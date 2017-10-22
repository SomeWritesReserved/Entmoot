using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Entmoot.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Entmoot.TestGame3D
{
	public class MainGame : Game
	{
		#region Fields

		private const int maxClients = 1;

		private readonly GraphicsDeviceManager graphicsDeviceManager;
		private BasicEffect basicEffect;

		private readonly StringBuilder stringBuilder = new StringBuilder(2048);
		private SpriteBatch spriteBatch;
		private SpriteFont spriteFont;
		private Graph graph;

		private readonly bool hasServer;
		private readonly RenderSystem renderSystem;
		private readonly NetworkServer networkServer;
		private readonly NetworkClient networkClient;
		private readonly GameServer<CommandData> gameServer;
		private readonly GameClient<CommandData> gameClient;
		private CommandData commandData = new CommandData();

		private bool isNetworkedPaused;
		private int slowFrames;

		private KeyboardState currentKeyboardState;
		private KeyboardState previousKeyboardState;
		private MouseState currentMouseState;
		private MouseState previousMouseState;
		private Point mouseDownPoint;
		private bool isDragging;

		#endregion Fields

		#region Constructors

		public MainGame()
		{
			this.InactiveSleepTime = TimeSpan.Zero;
			this.IsMouseVisible = true;
			this.graphicsDeviceManager = new GraphicsDeviceManager(this);
			this.graphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;
			this.graphicsDeviceManager.PreferMultiSampling = true;
			this.renderSystem = new RenderSystem(this.graphicsDeviceManager);
			this.Content.RootDirectory = "Assets";

			ComponentsDefinition componentsDefinition = new ComponentsDefinition();
			componentsDefinition.RegisterComponentType<SpatialComponent>();
			componentsDefinition.RegisterComponentType<PhysicsComponent>();
			componentsDefinition.RegisterComponentType<ColorComponent>();

			this.hasServer = Environment.GetCommandLineArgs().Any((arg) => arg.Equals("-s", StringComparison.OrdinalIgnoreCase));
			if (this.hasServer)
			{
				this.networkServer = new NetworkServer("1", MainGame.maxClients, 4000, 19876);
				this.gameServer = new GameServer<CommandData>(this.networkServer.ClientNetworkConnections, 20, 30, componentsDefinition, new IServerSystem[] { new PhysicsSystem() });

				// Reserve the first entities for all potential clients
				for (int clientID = 0; clientID < MainGame.maxClients; clientID++)
				{
					this.gameServer.EntityArray.TryCreateEntity(out Entity clientEntity);
					clientEntity.AddComponent<SpatialComponent>().Position.Y = 1.875f;
					clientEntity.AddComponent<PhysicsComponent>();
				}

				for (int x = -2; x <= 2; x++)
				{
					for (int y = -2; y <= 2; y++)
					{
						this.gameServer.EntityArray.TryCreateEntity(out Entity entity);
						entity.AddComponent<SpatialComponent>().Position = new Vector3(x * 40, -20, y * 40);
						entity.AddComponent<SpatialComponent>().Scale = 20;
					}
				}
			}

			this.networkClient = new NetworkClient("1", 4000);
			this.gameClient = new GameClient<CommandData>(this.networkClient, 20, 30, componentsDefinition, new IClientSystem[] { new PhysicsSystem(), this.renderSystem });
		}

		#endregion Constructors

		#region Methods

		protected override void Initialize()
		{
			if (this.hasServer)
			{
				this.networkServer.Start();
			}

			base.Initialize();
		}

		protected override void LoadContent()
		{
			this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
			this.spriteFont = this.Content.Load<SpriteFont>("dev_font");
			this.graph = new Graph(this.GraphicsDevice, new Point(this.GraphicsDevice.Viewport.Width, 100));
			this.graph.Position = new Vector2(0, this.GraphicsDevice.Viewport.Height - 5);

			this.basicEffect = new BasicEffect(this.GraphicsDevice);
			this.basicEffect.LightingEnabled = true;
			this.basicEffect.TextureEnabled = true;
			this.basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90.0f), this.GraphicsDevice.Viewport.AspectRatio, 0.1f, 1000.0f);
			this.basicEffect.EnableDefaultLighting();
			this.basicEffect.SpecularPower = 10000;

			using (FileStream fileStream = new FileStream(@"Assets\dev_cube.png", FileMode.Open, FileAccess.Read))
			{
				this.basicEffect.Texture = Texture2D.FromStream(this.GraphicsDevice, fileStream);
			}
			this.colladaSkeleton = this.loadColladaSkeleton(@"K:\Dexter\Desktop\collada\tpos.dae", 0.1f);
		}

		protected override void OnExiting(object sender, EventArgs args)
		{
			this.networkClient.Disconnect();
			if (this.hasServer)
			{
				this.networkServer.Stop();
			}
			System.Threading.Thread.Sleep(250);
			base.OnExiting(sender, args);
		}

		protected override void OnDeactivated(object sender, EventArgs args)
		{
			this.isDragging = false;
			base.OnDeactivated(sender, args);
		}

		protected override void Update(GameTime gameTime)
		{
			if (gameTime.IsRunningSlowly) { this.slowFrames++; }

			this.currentKeyboardState = Keyboard.GetState();
			this.currentMouseState = Mouse.GetState();

			if (this.isKeyPressed(Keys.U) && !this.networkClient.IsConnected)
			{
				this.networkClient.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 19876));
			}
			else if (this.isKeyPressed(Keys.P))
			{
				this.isNetworkedPaused = !this.isNetworkedPaused;
			}
			else if (this.isKeyPressed(Keys.Escape))
			{
				this.Exit();
			}
			else if (this.isKeyPressed(Keys.K) && (this.currentKeyboardState.IsKeyDown(Keys.LeftControl) || this.currentKeyboardState.IsKeyDown(Keys.RightControl)))
			{
				File.WriteAllLines("animation.txt", this.currentAnimation.SkeletonKeyframes[0].Select((kvp) => string.Format("this.walk1BoneAnimation[\"{0}\"] = new Quaternion({1}f, {2}f, {3}f, {4}f);", kvp.Key, kvp.Value.X, kvp.Value.Y, kvp.Value.Z, kvp.Value.W)));
			}
			else if (this.isKeyPressed(Keys.Up))
			{
			}
			else if (this.isKeyPressed(Keys.Down))
			{
			}

			int tickDifference = this.currentMouseState.ScrollWheelValue - this.previousMouseState.ScrollWheelValue;
			if (tickDifference != 0)
			{
				//if (!this.currentAnimation.SkeletonKeyframes[0].ContainsKey(this.selectedBone.Name)) { this.currentAnimation.SkeletonKeyframes[0][this.selectedBone.Name] = this.selectedBone.Rotation; }
				//float amount = tickDifference * 0.0001f;
				//this.currentAnimation.SkeletonKeyframes[0][this.selectedBone.Name] *= Quaternion.CreateFromYawPitchRoll(0, amount, 0);

				float amount = tickDifference * 0.0005f;
				this.currentSpeed = MathHelper.Clamp(this.currentSpeed + amount, walkSpeed, runSpeed);
			}

			if (this.isRightMousePressed())
			{
				this.isDragging = true;
				this.mouseDownPoint = new Point(this.currentMouseState.X, this.currentMouseState.Y);
			}
			else if (this.currentMouseState.RightButton == ButtonState.Released)
			{
				this.isDragging = false;
			}

			this.updateClientAndServer();
			this.Window.Title = this.networkClient.IsConnected ? "Connected" : "Disconnected";

			this.previousKeyboardState = this.currentKeyboardState;
			this.previousMouseState = this.currentMouseState;
		}

		protected override void Draw(GameTime gameTime)
		{
			if (gameTime.IsRunningSlowly) { this.slowFrames++; }

			this.GraphicsDevice.Clear(Color.Gray);
			if (this.gameClient.HasRenderingStarted)
			{
				this.renderSystem.BasicEffect = this.basicEffect;
				this.gameClient.SystemArray.Render(this.gameClient.RenderedSnapshot.EntityArray, this.gameClient.GetCommandingEntity());
				//this.drawCharacter();
				this.drawBone(this.colladaSkeleton, Matrix.Identity, new SkeletonKeyframe(), new SkeletonKeyframe(), new SkeletonKeyframe(), new SkeletonKeyframe(), 0, InterpolationType.None);
			}

			this.drawDebugUI();
		}

		private Skeleton colladaSkeleton;
		private Skeleton skeleton;
		private Bone selectedBone;
		private SkeletonAnimation currentAnimation = new SkeletonAnimation { SkeletonKeyframes = new[] { new SkeletonKeyframe(), new SkeletonKeyframe(), new SkeletonKeyframe(), new SkeletonKeyframe() } };
		float x = 0;
		const float runSpeed = 0.8f;
		const float runFrameRate = 8;
		const float walkSpeed = 0.175f;
		const float walkFrameRate = 2.2f;
		float currentSpeed = walkSpeed;

		private void drawCharacter()
		{
			if (this.skeleton == null)
			{
				this.skeleton = new Skeleton()
				{
					Bones = new Bone[]
					{
						new Bone("Root")
						{
							OffsetFromParent = new Vector3(0.0f, 7.5f, 0.0f),
							Size = new Vector3(0.1f, 0.1f, 0.1f),
						},
						new Bone("Lower Torso")
						{
							OffsetFromParent = new Vector3(0.0f, 0.0f, 0.0f),
							Size = new Vector3(4.0f, 2.5f, 2.0f),
							ParentIndex = 0,
						},
						new Bone("Upper Torso")
						{
							OffsetFromParent = new Vector3(0.0f, 2.5f, 0.0f),
							Size = new Vector3(4.0f, 3.25f, 2.0f),
							ParentIndex = 1,
						},
						new Bone("Neck")
						{
							OffsetFromParent = new Vector3(0.0f, 3.25f, 0.0f),
							Size = new Vector3(1.0f, 0.25f, 1.0f),
							ParentIndex = 2,
						},
						new Bone("Head")
						{
							OffsetFromParent = new Vector3(0.0f, 0.25f, 0.0f),
							Size = new Vector3(1.5f, 2.0f, 1.75f),
							ParentIndex = 3,
						},
						new Bone("Upper Arm - Right")
						{
							OffsetFromParent = new Vector3(2.0f, 3.0f, 0.0f),
							Size = new Vector3(1.5f, 3.5f, 1.5f),
							ParentIndex = 2,
						},
						new Bone("Lower Arm - Right")
						{
							OffsetFromParent = new Vector3(0.0f, 3.5f, 0.0f),
							Size = new Vector3(1.2f, 2.25f, 1.5f),
							ParentIndex = 5,
						},
						new Bone("Hand - Right")
						{
							OffsetFromParent = new Vector3(0.0f, 2.25f, 0.0f),
							Size = new Vector3(0.5f, 1.5f, 1.5f),
							ParentIndex = 6,
						},
						new Bone("Upper Arm - Left")
						{
							OffsetFromParent = new Vector3(-2.0f, 3.0f, 0.0f),
							Size = new Vector3(1.5f, 3.5f, 1.5f),
							ParentIndex = 2,
						},
						new Bone("Lower Arm - Left")
						{
							OffsetFromParent = new Vector3(0.0f, 3.5f, 0.0f),
							Size = new Vector3(1.2f, 2.25f, 1.5f),
							ParentIndex = 8,
						},
						new Bone("Hand - Left")
						{
							OffsetFromParent = new Vector3(0.0f, 2.25f, 0.0f),
							Size = new Vector3(0.5f, 1.5f, 1.5f),
							ParentIndex = 9,
						},
						new Bone("Upper Leg - Right")
						{
							OffsetFromParent = new Vector3(1.0f, 0.0f, 0.0f),
							Size = new Vector3(1.75f, 3.75f, 1.75f),
							ParentIndex = 0,
						},
						new Bone("Lower Leg - Right")
						{
							OffsetFromParent = new Vector3(0.0f, 3.75f, 0.0f),
							Size = new Vector3(1.6f, 3.25f, 1.65f),
							ParentIndex = 11,
						},
						new Bone("Foot - Right")
						{
							OffsetFromParent = new Vector3(0.0f, 3.25f, 0.0f),
							Size = new Vector3(1.5f, 1.0f, 1.7f),
							ParentIndex = 12,
						},
						new Bone("Toes - Right")
						{
							OffsetFromParent = new Vector3(0.0f, 0.4f, 1.2f),
							Size = new Vector3(1.4f, 0.6f, 0.85f),
							ParentIndex = 13,
						},
						new Bone("Upper Leg - Left")
						{
							OffsetFromParent = new Vector3(-1.0f, 0.0f, 0.0f),
							Size = new Vector3(1.75f, 3.75f, 1.75f),
							ParentIndex = 0,
						},
						new Bone("Lower Leg - Left")
						{
							OffsetFromParent = new Vector3(0.0f, 3.75f, 0.0f),
							Size = new Vector3(1.6f, 3.25f, 1.65f),
							ParentIndex = 15,
						},
						new Bone("Foot - Left")
						{
							OffsetFromParent = new Vector3(0.0f, 3.25f, 0.0f),
							Size = new Vector3(1.5f, 1.0f, 1.7f),
							ParentIndex = 16,
						},
						new Bone("Toes - Left")
						{
							OffsetFromParent = new Vector3(0.0f, 0.4f, 1.2f),
							Size = new Vector3(1.4f, 0.6f, 0.85f),
							ParentIndex = 17,
						},
					},
				};
			}

			float blendAmount = (this.currentSpeed - walkSpeed) / (runSpeed - walkSpeed);
			float frameRate = MathHelper.Lerp(walkFrameRate, runFrameRate, blendAmount);
			this.currentAnimation.Blend(DefinedAnimations.WalkAnimation, DefinedAnimations.RunAnimation, blendAmount);

			//x -= this.currentSpeed;
			if (x < 0) { x = 100; }
			int ticksBetweenKeyframes = (int)Math.Round(frameRate / this.currentSpeed);

			this.currentAnimation.GetAnimation(this.gameClient.FrameTick, ticksBetweenKeyframes, out SkeletonKeyframe keyframePrevious, out SkeletonKeyframe keyframeStart, out SkeletonKeyframe keyframeEnd, out SkeletonKeyframe keyframeNext, out float amount);
			this.drawBone(this.skeleton, Matrix.CreateTranslation(0, 0, x), keyframePrevious, keyframeStart, keyframeEnd, keyframeNext, amount, InterpolationType.CatmullRom);
		}

		private void drawBone(Skeleton skeleton, Matrix transform, SkeletonKeyframe keyframePrevious, SkeletonKeyframe keyframeStart, SkeletonKeyframe keyframeEnd, SkeletonKeyframe keyframeNext, float amount, InterpolationType interpolationType)
		{
			foreach (Bone bone in skeleton.Bones)
			{
				if (!keyframePrevious.TryGetValue(bone.Name, out Quaternion rotationPrevioust)) { rotationPrevioust = Quaternion.Identity; }
				if (!keyframeStart.TryGetValue(bone.Name, out Quaternion rotationStart)) { rotationStart = Quaternion.Identity; }
				if (!keyframeEnd.TryGetValue(bone.Name, out Quaternion rotationEnd)) { rotationEnd = Quaternion.Identity; }
				if (!keyframeNext.TryGetValue(bone.Name, out Quaternion rotationNext)) { rotationNext = Quaternion.Identity; }

				Quaternion finalRotation = rotationStart;
				switch (interpolationType)
				{
					case InterpolationType.Linear:
						finalRotation = Quaternion.Lerp(rotationStart, rotationEnd, amount);
						break;
					case InterpolationType.Slerp:
						finalRotation = Quaternion.Slerp(rotationStart, rotationEnd, amount);
						break;
					case InterpolationType.CatmullRom:
						finalRotation = MainGame.CatmullRom(rotationPrevioust, rotationStart, rotationEnd, rotationNext, amount);
						break;
				}
				bone.Rotation = finalRotation;
			}

			foreach (Bone bone in skeleton.Bones)
			{
				Bone currentBone = bone;
				Matrix boneTransform = Matrix.CreateFromQuaternion(bone.Rotation) * Matrix.CreateTranslation(currentBone.OffsetFromParent);
				while (currentBone.ParentIndex != -1)
				{
					currentBone = skeleton.Bones[currentBone.ParentIndex];
					boneTransform = boneTransform * Matrix.CreateFromQuaternion(currentBone.Rotation) * Matrix.CreateTranslation(currentBone.OffsetFromParent);
				}
				bone.RenderTransform = boneTransform * transform;
				ShapeRenderHelper.RenderOriginBox(this.GraphicsDevice, this.basicEffect, bone.Size, boneTransform * transform);
			}

			this.GraphicsDevice.DepthStencilState = DepthStencilState.None;
			foreach (Bone bone in skeleton.Bones)
			{
				if (bone.ParentIndex >= 0)
				{
					Matrix parentTransform = skeleton.Bones[bone.ParentIndex].RenderTransform;
					ShapeRenderHelper.RenderLine(this.GraphicsDevice, this.basicEffect, parentTransform.Translation, bone.RenderTransform.Translation);
				}
			}
			this.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
		}

		private Skeleton loadColladaSkeleton(string path, float skeletonScale)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(path);

			Skeleton skeleton = new Skeleton()
			{
				Bones = new Bone[]
				{
					new Bone("Hips") { ParentIndex = -1, },
					new Bone("Spine") { ParentIndex = 0, },
					new Bone("Spine1") { ParentIndex = 1, },
					new Bone("Spine2") { ParentIndex = 2, },
					new Bone("Neck") { ParentIndex = 3, },
					new Bone("Head") { ParentIndex = 4, },
					new Bone("LeftShoulder") { ParentIndex = 3, },
					new Bone("LeftArm") { ParentIndex = 6, },
					new Bone("LeftForeArm") { ParentIndex = 7, },
					new Bone("LeftHand") { ParentIndex = 8, },
					new Bone("RightShoulder") { ParentIndex = 3, },
					new Bone("RightArm") { ParentIndex = 10, },
					new Bone("RightForeArm") { ParentIndex = 11, },
					new Bone("RightHand") { ParentIndex = 12, },
					new Bone("LeftUpLeg") { ParentIndex = 0, },
					new Bone("LeftLeg") { ParentIndex = 14, },
					new Bone("LeftFoot") { ParentIndex = 15, },
					new Bone("RightUpLeg") { ParentIndex = 0, },
					new Bone("RightLeg") { ParentIndex = 17, },
					new Bone("RightFoot") { ParentIndex = 18 },
				},
			};

			foreach (Bone bone in skeleton.Bones)
			{
				var animationNode = xmlDocument.SelectNodes(string.Format("COLLADA/library_animations/animation/source[@id='mixamorig_{0}-Matrix-animation-output-transform']/float_array", bone.Name))
					.OfType<XmlNode>()
					.Single();

				string[] animationTransforms = animationNode.InnerText.Replace("\t", "")
					.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);

				Matrix boneTransform = this.parseMatrix(animationTransforms[0]);
				if (!boneTransform.Decompose(out Vector3 scale, out Quaternion rotation, out Vector3 translation)) { throw new Exception("Bad matrix for bone"); }
				bone.OffsetFromParent = translation * skeletonScale;
				bone.Rotation = rotation;
			}
			return skeleton;
		}

		private Matrix parseMatrix(string strvalue)
		{
			float[] values = strvalue.Trim().Split(' ').Select((str) => float.Parse(str)).ToArray();
			if (values.Length != 16) { throw new Exception("Wrong number of matrix elements."); }
			return new Matrix(values[0], values[4], values[8], values[12],
				values[1], values[5], values[9], values[13],
				values[2], values[6], values[10], values[14],
				values[3], values[7], values[11], values[15]);
		}

		private void updateClientAndServer()
		{
			if (this.hasServer && !this.isNetworkedPaused)
			{
				this.networkServer.Update();
				this.gameServer.Update();
			}

			// Client
			{
				if (this.IsActive && this.isDragging)
				{
					float deltaX = (this.mouseDownPoint.X - this.currentMouseState.X) * 0.004f;
					float deltaY = (this.mouseDownPoint.Y - this.currentMouseState.Y) * 0.004f;
					this.commandData.LookAngles.X += deltaX;
					this.commandData.LookAngles.Y = MathHelper.Clamp(this.commandData.LookAngles.Y + deltaY, -MathHelper.Pi * 0.49f, MathHelper.Pi * 0.49f);
					Mouse.SetPosition(this.mouseDownPoint.X, this.mouseDownPoint.Y);
				}

				this.commandData.Commands = Commands.None;
				if (this.currentKeyboardState.IsKeyDown(Keys.W)) { this.commandData.Commands |= Commands.MoveForward; }
				if (this.currentKeyboardState.IsKeyDown(Keys.S)) { this.commandData.Commands |= Commands.MoveBackward; }
				if (this.currentKeyboardState.IsKeyDown(Keys.A)) { this.commandData.Commands |= Commands.MoveLeft; }
				if (this.currentKeyboardState.IsKeyDown(Keys.D)) { this.commandData.Commands |= Commands.MoveRight; }

				if (!this.isNetworkedPaused)
				{
					this.networkClient.Update();
					this.gameClient.Update(this.commandData);
				}
			}
		}

		private void drawDebugUI()
		{
			this.stringBuilder.Clear();

			this.stringBuilder.Append("SlowFrames   ");
			this.stringBuilder.Append(this.slowFrames);
			if (this.hasServer)
			{
				this.stringBuilder.Append("\nSERVER");
				this.stringBuilder.Append("\n FrameTick   ");
				this.stringBuilder.Append(this.gameServer.FrameTick);
				this.stringBuilder.Append("\n RecvBytes/s ");
				this.stringBuilder.Append(Log<LogNetworkServer>.History.Sum((d) => d.ReceivedBytes) / 2);
				this.stringBuilder.Append("\n SentBytes/s ");
				this.stringBuilder.Append(Log<LogNetworkServer>.History.Sum((d) => d.SentBytes) / 2);
				this.stringBuilder.Append("\n Clients     ");
				this.stringBuilder.Append(Log<LogNetworkServer>.Data.ConnectedClients);
				this.stringBuilder.Append("\n Connecting  ");
				this.stringBuilder.Append(Log<LogNetworkServer>.Data.ConnectingClients);
			}

			this.stringBuilder.Append("\nCLIENT");
			this.stringBuilder.Append("\n FrameTick   ");
			this.stringBuilder.Append(this.gameClient.FrameTick);
			this.stringBuilder.Append("\n RecvBytes/s ");
			this.stringBuilder.Append(Log<LogNetworkClient>.History.Sum((d) => d.ReceivedBytes) / 2);
			this.stringBuilder.Append("\n SentBytes/s ");
			this.stringBuilder.Append(Log<LogNetworkClient>.History.Sum((d) => d.SentBytes) / 2);

			BlendState blendState = this.GraphicsDevice.BlendState;
			DepthStencilState depthStencilState = this.GraphicsDevice.DepthStencilState;
			this.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			this.spriteBatch.DrawString(this.spriteFont, this.stringBuilder, Vector2.One, Color.White);
			this.spriteBatch.End();
			this.GraphicsDevice.BlendState = blendState;
			this.GraphicsDevice.DepthStencilState = depthStencilState;

			if (this.hasServer)
			{
				//this.drawGraph(Log<LogNetworkServer>.History, (log) => log.ReceivedBytes, Color.AliceBlue);
				this.drawGraph(Log<LogNetworkServer>.History, (log) => log.SentBytes, Color.AliceBlue);
			}
			else
			{
				this.drawGraph(Log<LogNetworkClient>.History, (log) => log.ReceivedBytes, Color.AliceBlue);
				//this.drawGraph(Log<LogNetworkClient>.History, (log) => log.SentBytes, Color.AliceBlue);
			}
		}

		private float[] graphData = new float[120];
		private void drawGraph<T>(Queue<T> history, Func<T, float> selector, Color color)
		{
			float max = 0;
			int index = this.graphData.Length - history.Count;
			foreach (T t in history)
			{
				float value = selector(t);
				if (value > max) { max = value; }
				this.graphData[index++] = value;
			}
			this.graph.MaxValue = max;
			this.graph.Draw(this.graphData, color);
		}

		private bool isKeyPressed(Keys key)
		{
			return (this.currentKeyboardState.IsKeyDown(key) && this.previousKeyboardState.IsKeyUp(key));
		}

		private bool isLeftMousePressed()
		{
			return (this.currentMouseState.LeftButton == ButtonState.Pressed && this.previousMouseState.LeftButton == ButtonState.Released);
		}

		private bool isRightMousePressed()
		{
			return (this.currentMouseState.RightButton == ButtonState.Pressed && this.previousMouseState.RightButton == ButtonState.Released);
		}

		private static Quaternion CatmullRom(Quaternion previous, Quaternion start, Quaternion end, Quaternion next, float amount)
		{
			Quaternion result = Quaternion.Identity;
			result.X = MathHelper.CatmullRom(previous.X, start.X, end.X, next.X, amount);
			result.Y = MathHelper.CatmullRom(previous.Y, start.Y, end.Y, next.Y, amount);
			result.Z = MathHelper.CatmullRom(previous.Z, start.Z, end.Z, next.Z, amount);
			result.W = MathHelper.CatmullRom(previous.W, start.W, end.W, next.W, amount);
			result.Normalize();
			return result;
		}

		private enum InterpolationType
		{
			None,
			Linear,
			Slerp,
			CatmullRom,
		}

		#endregion Methods
	}
}
