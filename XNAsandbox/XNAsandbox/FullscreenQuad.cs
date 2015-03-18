using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XNAsandbox {
	class FullscreenQuad {
		Game game;
		VertexBuffer vBuffer;

		struct QuadVertex : IVertexType {
			static readonly VertexDeclaration vDecl = new VertexDeclaration(
				new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0)
				);

			public VertexDeclaration VertexDeclaration {
				get { return vDecl; }
			}

			public float x, y;

			public QuadVertex(float X, float Y) {
				x = X;
				y = Y;
			}
		}

		static readonly QuadVertex[] verticies = {
				new QuadVertex(-1, -1),
				new QuadVertex(-1, 1),
				new QuadVertex(1, -1),
				new QuadVertex(1, -1),
				new QuadVertex(-1, 1),
				new QuadVertex(1, 1),
			};

		public FullscreenQuad(Game game) {
			this.game = game;
			vBuffer = new VertexBuffer(game.GraphicsDevice, typeof(QuadVertex), 6, BufferUsage.WriteOnly);
			vBuffer.SetData(verticies);
		}

		public void Draw(){
			game.GraphicsDevice.SetVertexBuffer(vBuffer);
			game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
		}


	}
}
