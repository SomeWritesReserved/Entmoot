using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
				this.gameServer = new GameServer<CommandData>(this.networkServer.ClientNetworkConnections, 20, 30, componentsDefinition, new IServerSystem[] { new SpinnerSystem(), new PhysicsSystem() });

				// Reserve the first entities for all potential clients
				for (int clientID = 0; clientID < MainGame.maxClients; clientID++)
				{
					this.gameServer.EntityArray.TryCreateEntity(out Entity clientEntity);
					clientEntity.AddComponent<SpatialComponent>().Position.Y = 1.875f;
					clientEntity.AddComponent<PhysicsComponent>();
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

			using (FileStream fileStream = new FileStream(@"Assets\dev_cube.png", FileMode.Open, FileAccess.Read))
			{
				this.basicEffect.Texture = Texture2D.FromStream(this.GraphicsDevice, fileStream);
			}
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
			else if (this.isKeyPressed(Keys.K) && this.currentKeyboardState.IsKeyDown(Keys.LeftControl))
			{
				File.WriteAllLines("animation.txt", this.walk1BoneAnimation.Select((kvp) => string.Format("this.walk1BoneAnimation[\"{0}\"] = new Quaternion({1}f, {2}f, {3}f, {4}f);", kvp.Key, kvp.Value.X, kvp.Value.Y, kvp.Value.Z, kvp.Value.W)));
			}
			else if (this.isKeyPressed(Keys.Up))
			{
				if (this.selectedBone.Parent != null)
				{
					this.selectedBone = this.selectedBone.Parent;
				}
			}
			else if (this.isKeyPressed(Keys.Down))
			{
				if (this.selectedBone.Children != null && this.selectedBone.Children.Any())
				{
					if (this.selectedBone.Children.Length == 1 || this.currentKeyboardState.IsKeyDown(Keys.NumPad1))
					{
						this.selectedBone = this.selectedBone.Children[0];
					}
					else if (this.currentKeyboardState.IsKeyDown(Keys.NumPad2))
					{
						this.selectedBone = this.selectedBone.Children[1];
					}
					else if (this.currentKeyboardState.IsKeyDown(Keys.NumPad3))
					{
						this.selectedBone = this.selectedBone.Children[2];
					}
				}
			}

			int tickDifference = this.currentMouseState.ScrollWheelValue - this.previousMouseState.ScrollWheelValue;
			if (tickDifference != 0)
			{
				if (!this.walk1BoneAnimation.ContainsKey(this.selectedBone.Name)) { this.walk1BoneAnimation[this.selectedBone.Name] = this.selectedBone.Rotation; }
				float amount = tickDifference * 0.0001f;
				this.walk1BoneAnimation[this.selectedBone.Name] *= Quaternion.CreateFromYawPitchRoll(0, amount, 0);
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
				this.drawCharacter();
			}
			ShapeRenderHelper.RenderOriginBox(this.GraphicsDevice, this.basicEffect, Vector3.One * 10, Matrix.CreateTranslation(0, -10, 0));

			this.drawDebugUI();
		}

		private Bone selectedBone;
		private Bone rootBone;
		private BoneAnimation walk1BoneAnimation = new BoneAnimation();
		private BoneAnimation walk2BoneAnimation = new BoneAnimation();
		private BoneAnimation walk3BoneAnimation = new BoneAnimation();
		private BoneAnimation walk4BoneAnimation = new BoneAnimation();
		private void drawCharacter()
		{
			if (this.rootBone == null)
			{
				this.rootBone = new Bone("Root")
				{
					OffsetFromParent = new Vector3(0.0f, 7.5f, 0.0f),
					Size = new Vector3(0.1f, 0.1f, 0.1f),
					Children = new Bone[] {
						new Bone("Lower Torso")
						{
							OffsetFromParent = new Vector3(0.0f, 0.0f, 0.0f),
							Size = new Vector3(4.0f, 2.5f, 2.0f),
							Children = new Bone[] {
								new Bone("Upper Torso")
								{
									OffsetFromParent = new Vector3(0.0f, 2.5f, 0.0f),
									Size = new Vector3(4.0f, 3.25f, 2.0f),
									Children = new Bone[] {
										new Bone("Neck")
										{
											OffsetFromParent = new Vector3(0.0f, 3.25f, 0.0f),
											Size = new Vector3(1.0f, 0.25f, 1.0f),
											Children = new Bone[] {
												new Bone("Head")
												{
													OffsetFromParent = new Vector3(0.0f, 0.25f, 0.0f),
													Size = new Vector3(1.5f, 2.0f, 1.75f),
												},
											},
										},
										new Bone("Upper Arm - Right")
										{
											OffsetFromParent = new Vector3(2.0f, 3.0f, 0.0f),
											Size = new Vector3(1.5f, 3.5f, 1.5f),
											Children = new Bone[] {
												new Bone("Lower Arm - Right")
												{
													OffsetFromParent = new Vector3(0.0f, 3.5f, 0.0f),
													Size = new Vector3(1.2f, 2.25f, 1.5f),
													Children = new Bone[] {
														new Bone("Hand - Right")
														{
															OffsetFromParent = new Vector3(0.0f, 2.25f, 0.0f),
															Size = new Vector3(0.5f, 1.5f, 1.5f),
														},
													},
												},
											},
										},
										new Bone("Upper Arm - Left")
										{
											OffsetFromParent = new Vector3(-2.0f, 3.0f, 0.0f),
											Size = new Vector3(1.5f, 3.5f, 1.5f),
											Children = new Bone[] {
												new Bone("Lower Arm - Left")
												{
													OffsetFromParent = new Vector3(0.0f, 3.5f, 0.0f),
													Size = new Vector3(1.2f, 2.25f, 1.5f),
													Children = new Bone[] {
														new Bone("Hand - Left")
														{
															OffsetFromParent = new Vector3(0.0f, 2.25f, 0.0f),
															Size = new Vector3(0.5f, 1.5f, 1.5f),
														},
													},
												},
											},
										},
									},
								},
							},
						},
						new Bone("Upper Leg - Right")
						{
							OffsetFromParent = new Vector3(1.0f, 0.0f, 0.0f),
							Size = new Vector3(1.75f, 3.75f, 1.75f),
							Children = new Bone[] {
								new Bone("Lower Leg - Right")
								{
									OffsetFromParent = new Vector3(0.0f, 3.75f, 0.0f),
									Size = new Vector3(1.6f, 3.25f, 1.65f),
									Children = new Bone[] {
										new Bone("Foot - Right")
										{
											OffsetFromParent = new Vector3(0.0f, 3.25f, 0.0f),
											Size = new Vector3(1.5f, 1.0f, 1.7f),
											Children = new Bone[] {
												new Bone("Toes - Right")
												{
													OffsetFromParent = new Vector3(0.0f, 0.4f, 1.2f),
													Size = new Vector3(1.4f, 0.6f, 0.85f),
												}
											},
										},
									},
								},
							},
						},
						new Bone("Upper Leg - Left")
						{
							OffsetFromParent = new Vector3(-1.0f, 0.0f, 0.0f),
							Size = new Vector3(1.75f, 3.75f, 1.75f),
							Children = new Bone[] {
								new Bone("Lower Leg - Left")
								{
									OffsetFromParent = new Vector3(0.0f, 3.75f, 0.0f),
									Size = new Vector3(1.6f, 3.25f, 1.65f),
									Children = new Bone[] {
										new Bone("Foot - Left")
										{
											OffsetFromParent = new Vector3(0.0f, 3.25f, 0.0f),
											Size = new Vector3(1.5f, 1.0f, 1.7f),
											Children = new Bone[] {
												new Bone("Toes - Left")
												{
													OffsetFromParent = new Vector3(0.0f, 0.4f, 1.2f),
													Size = new Vector3(1.4f, 0.6f, 0.85f),
												}
											},
										},
									},
								},
							},
						},
					},
				};
				this.selectedBone = this.rootBone;

				this.walk1BoneAnimation["Upper Arm - Right"] = new Quaternion(0.9887707f, 0f, 0f, 0.1494382f);
				this.walk1BoneAnimation["Upper Arm - Left"] = new Quaternion(0.9998378f, 0f, 0f, -0.01799901f);
				this.walk1BoneAnimation["Upper Leg - Right"] = new Quaternion(0.9959527f, 0f, 0f, -0.08987861f);
				this.walk1BoneAnimation["Upper Leg - Left"] = new Quaternion(0.9904928f, 0f, 0f, -0.1375626f);
				this.walk1BoneAnimation["Lower Torso"] = new Quaternion(-0.02399769f, 0f, 0f, 0.999712f);
				this.walk1BoneAnimation["Upper Torso"] = new Quaternion(0f, 0f, 0f, 1f);
				this.walk1BoneAnimation["Neck"] = new Quaternion(-0.01199971f, 0f, 0f, 0.999928f);
				this.walk1BoneAnimation["Head"] = new Quaternion(-0.0239977f, 0f, 0f, 0.999712f);
				this.walk1BoneAnimation["Lower Arm - Right"] = new Quaternion(0.318361f, 0f, 0f, 0.9479695f);
				this.walk1BoneAnimation["Hand - Right"] = new Quaternion(0.03599222f, 0f, 0f, 0.999352f);
				this.walk1BoneAnimation["Lower Arm - Left"] = new Quaternion(0.6816384f, 0f, 0f, 0.7316884f);
				this.walk1BoneAnimation["Hand - Left"] = new Quaternion(0.08390125f, 0f, 0f, 0.996474f);
				this.walk1BoneAnimation["Lower Leg - Right"] = new Quaternion(-0.6457458f, 0f, 0f, 0.763552f);
				this.walk1BoneAnimation["Foot - Right"] = new Quaternion(-0.1435028f, 0f, 0f, 0.9896498f);
				this.walk1BoneAnimation["Toes - Right"] = new Quaternion(-0.07193781f, 0f, 0f, 0.9974091f);
				this.walk1BoneAnimation["Lower Leg - Left"] = new Quaternion(-0.2318703f, 0f, 0f, 0.9727465f);
				this.walk1BoneAnimation["Foot - Left"] = new Quaternion(0.07193785f, 0f, 0f, 0.997409f);
				this.walk1BoneAnimation["Toes - Left"] = new Quaternion(0.059964f, 0f, 0f, 0.9982005f);

				this.walk2BoneAnimation["Upper Arm - Right"] = new Quaternion(0.8756577f, 0f, 0f, 0.482932f);
				this.walk2BoneAnimation["Upper Arm - Left"] = new Quaternion(0.9588137f, 0f, 0f, -0.2840351f);
				this.walk2BoneAnimation["Upper Leg - Right"] = new Quaternion(0.9849001f, 0f, 0f, 0.1731233f);
				this.walk2BoneAnimation["Upper Leg - Left"] = new Quaternion(0.8320514f, 0f, 0f, -0.554698f);
				this.walk2BoneAnimation["Lower Torso"] = new Quaternion(-0.0299955f, 0f, 0f, 0.99955f);
				this.walk2BoneAnimation["Upper Torso"] = new Quaternion(-0.005999964f, 0f, 0f, 0.999982f);
				this.walk2BoneAnimation["Neck"] = new Quaternion(-0.01199971f, 0f, 0f, 0.999928f);
				this.walk2BoneAnimation["Head"] = new Quaternion(-0.0239977f, 0f, 0f, 0.999712f);
				this.walk2BoneAnimation["Lower Arm - Right"] = new Quaternion(0.4132316f, 0f, 0f, 0.910626f);
				this.walk2BoneAnimation["Hand - Right"] = new Quaternion(0.03599222f, 0f, 0f, 0.999352f);
				this.walk2BoneAnimation["Lower Arm - Left"] = new Quaternion(0.7870421f, 0f, 0f, 0.6168985f);
				this.walk2BoneAnimation["Hand - Left"] = new Quaternion(0.08390125f, 0f, 0f, 0.996474f);
				this.walk2BoneAnimation["Lower Leg - Right"] = new Quaternion(-0.5794061f, 0f, 0f, 0.8150386f);
				this.walk2BoneAnimation["Foot - Right"] = new Quaternion(-0.07193781f, 0f, 0f, 0.997409f);
				this.walk2BoneAnimation["Toes - Right"] = new Quaternion(-0.04198765f, 0f, 0f, 0.9991181f);
				this.walk2BoneAnimation["Lower Leg - Left"] = new Quaternion(-0.4349655f, 0f, 0f, 0.900447f);
				this.walk2BoneAnimation["Foot - Left"] = new Quaternion(-0.1137532f, 0f, 0f, 0.993509f);
				this.walk2BoneAnimation["Toes - Left"] = new Quaternion(-0.0659521f, 0f, 0f, 0.9978227f);

				foreach (var walk1 in this.walk1BoneAnimation)
				{
					string name = walk1.Key.Contains("Left") ? walk1.Key.Replace("Left", "Right") :
						walk1.Key.Contains("Right") ? walk1.Key.Replace("Right", "Left") :
						walk1.Key;
					this.walk3BoneAnimation[name] = walk1.Value;
				}

				foreach (var walk2 in this.walk2BoneAnimation)
				{
					string name = walk2.Key.Contains("Left") ? walk2.Key.Replace("Left", "Right") :
						walk2.Key.Contains("Right") ? walk2.Key.Replace("Right", "Left") :
						walk2.Key;
					this.walk4BoneAnimation[name] = walk2.Value;
				}
			}

			BoneAnimation pre;
			BoneAnimation startBoneAnimation;
			BoneAnimation endBoneAnimation;
			BoneAnimation post;
			int counts = (this.gameClient.FrameTick % 60);
			if (counts < 15)
			{
				pre = this.walk2BoneAnimation;
				startBoneAnimation = this.walk1BoneAnimation;
				endBoneAnimation = this.walk4BoneAnimation;
				post = this.walk3BoneAnimation;
			}
			else if (counts < 30)
			{
				pre = this.walk1BoneAnimation;
				startBoneAnimation = this.walk4BoneAnimation;
				endBoneAnimation = this.walk3BoneAnimation;
				post = this.walk2BoneAnimation;
			}
			else if (counts < 45)
			{
				pre = this.walk4BoneAnimation;
				startBoneAnimation = this.walk3BoneAnimation;
				endBoneAnimation = this.walk2BoneAnimation;
				post = this.walk1BoneAnimation;
			}
			else
			{
				pre = this.walk3BoneAnimation;
				startBoneAnimation = this.walk2BoneAnimation;
				endBoneAnimation = this.walk1BoneAnimation;
				post = this.walk4BoneAnimation;
			}

			float amount = ((this.gameClient.FrameTick % 15) / 15.0f);
			this.drawBone(this.rootBone, Matrix.Identity, pre, startBoneAnimation, endBoneAnimation, post, amount, InterpolationType.CatmullRom);
		}

		private void drawBone(Bone bone, Matrix transform, BoneAnimation pre, BoneAnimation boneAnimationA, BoneAnimation boneAnimationB, BoneAnimation post, float amount, InterpolationType interpolationType)
		{
			if (!pre.TryGetValue(bone.Name, out Quaternion rotationPre)) { rotationPre = Quaternion.Identity; }
			if (!boneAnimationA.TryGetValue(bone.Name, out Quaternion rotationA)) { rotationA = Quaternion.Identity; }
			if (!boneAnimationB.TryGetValue(bone.Name, out Quaternion rotationB)) { rotationB = Quaternion.Identity; }
			if (!post.TryGetValue(bone.Name, out Quaternion rotationPost)) { rotationPost = Quaternion.Identity; }

			Quaternion finalRotation = rotationA;
			switch (interpolationType)
			{
				case InterpolationType.Linear:
					finalRotation = Quaternion.Lerp(rotationA, rotationB, amount);
					break;
				case InterpolationType.Slerp:
					finalRotation = Quaternion.Slerp(rotationA, rotationB, amount);
					break;
				case InterpolationType.CatmullRom:
					finalRotation = MainGame.CatmullRom(rotationPre, rotationA, rotationB, rotationPost, amount);
					break;
			}

			this.basicEffect.DiffuseColor = (bone == this.selectedBone) ? Vector3.Right : Vector3.One;

			transform = Matrix.CreateFromQuaternion(finalRotation) * Matrix.CreateTranslation(bone.OffsetFromParent) * transform;
			ShapeRenderHelper.RenderOriginBox(this.GraphicsDevice, this.basicEffect, bone.Size, transform);

			if (bone.Children != null)
			{
				foreach (Bone childBond in bone.Children)
				{
					this.drawBone(childBond, transform, pre, boneAnimationA, boneAnimationB, post, amount, interpolationType);
				}
			}
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

		private static Quaternion CatmullRom(Quaternion before, Quaternion a, Quaternion b, Quaternion after, float amount)
		{
			Quaternion result = Quaternion.Identity;
			result.X = MathHelper.CatmullRom(before.X, a.X, b.X, after.X, amount);
			result.Y = MathHelper.CatmullRom(before.Y, a.Y, b.Y, after.Y, amount);
			result.Z = MathHelper.CatmullRom(before.Z, a.Z, b.Z, after.Z, amount);
			result.W = MathHelper.CatmullRom(before.W, a.W, b.W, after.W, amount);
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
