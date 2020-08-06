using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entmoot.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entmoot.Game.Zombtown
{
	public class Render2dSystem : IClientSystem
	{
		#region Constructors

		public Render2dSystem(GraphicsDeviceManager graphicsDeviceManager)
		{
			this.GraphicsDeviceManager = graphicsDeviceManager;
		}

		#endregion Constructors

		#region Properties

		public GraphicsDeviceManager GraphicsDeviceManager { get; }

		public BasicEffect BasicEffect { get; set; }

		#endregion Properties

		#region Methods

		public void ClientUpdate(EntityArray entityArray, Entity commandingEntity)
		{
		}

		public void ClientPrediction(EntityArray entityArray, Entity commandingEntity)
		{
		}

		public void ClientRender(EntityArray entityArray, Entity commandingEntity)
		{
		}

		#endregion Methods
	}
}
