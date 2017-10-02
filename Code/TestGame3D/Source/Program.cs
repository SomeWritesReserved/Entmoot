using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using System.Windows.Forms;
using Entmoot.Engine;
using Microsoft.Xna.Framework;

namespace Entmoot.TestGame3D
{
	public static class Program
	{
		#region Methods

		[STAThread]
		public static void Main(string[] args)
		{
			GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

			if (args.Any((arg) => arg.Equals("-ds", StringComparison.OrdinalIgnoreCase)))
			{
				const int maxClient = 4;
				ComponentsDefinition componentsDefinition = new ComponentsDefinition();
				componentsDefinition.RegisterComponentType<SpatialComponent>();
				componentsDefinition.RegisterComponentType<ColorComponent>();
				NetworkServer networkServer = new NetworkServer("1", maxClient, 4000, 19876);
				GameServer<CommandData> gameServer = new GameServer<CommandData>(networkServer.ClientNetworkConnections, 20, 30, componentsDefinition, new ISystem[] { new SpinnerSystem() });
				// Reserve the first entities for all potential clients
				for (int clientID = 0; clientID < maxClient; clientID++)
				{
					gameServer.EntityArray.TryCreateEntity(out Entity clientEntity);
					clientEntity.AddComponent<SpatialComponent>();
					clientEntity.AddComponent<ColorComponent>().Color = new Color(0.5f, 0.5f, 1.0f);
				}

				// Make some dummy entities that we'll remove to have a gap in entity IDs
				gameServer.EntityArray.TryCreateEntity(out Entity dummy1);
				gameServer.EntityArray.TryCreateEntity(out Entity dummy2);

				// Add some stuff to the world
				for (int x = -1; x <= 1; x++)
				{
					for (int z = -1; z <= 1; z++)
					{
						gameServer.EntityArray.TryCreateEntity(out Entity entity);
						entity.AddComponent<SpatialComponent>().Position = new Vector3(x * 5, 0, z * 5);
						if (entity.ID == 11)
						{
							entity.AddComponent<ColorComponent>().Color = new Color(1.0f, 0.5f, 0.5f);
						}
						else if (entity.ID == 12)
						{
							entity.AddComponent<ColorComponent>().Color = new Color(0.0f, 1.0f, 0.5f);
						}
					}
				}

				// Remove those dummy entities to have a gap in entity IDs
				gameServer.EntityArray.RemoveEntity(dummy1);
				gameServer.EntityArray.RemoveEntity(dummy2);
				gameServer.EntityArray.EndUpdate();
				networkServer.Start();
				GC.Collect(10, GCCollectionMode.Forced, true, true);

				Program.runDedicatedServer(networkServer, gameServer);
				networkServer.Stop();

				System.Threading.Thread.Sleep(250);
			}
			else
			{
				using (MainGame game = new MainGame())
				{
					game.Run();
				}
			}
		}

		private static void runDedicatedServer(NetworkServer networkServer, GameServer<CommandData> gameServer)
		{
			int tick = 0;
			while (true)
			{
				networkServer.Update();
				gameServer.Update();
				System.Threading.Thread.Sleep(15);
				tick++;
			}
		}

		#endregion Methods
	}
}
