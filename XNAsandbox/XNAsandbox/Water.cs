using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XNAsandbox {
	class Water {
		Game game;

		VertexBuffer surfaceVertexBuffer;
		IndexBuffer surfaceIndexBuffer;

		Effect displayEffect;
		Effect rippleEffect;
		RenderTarget2D frontTex;
		RenderTarget2D backTex;

		FullscreenQuad fullScreenQuad;

		TextureCube enviro;

		Vector2[] waterData;

		int posX;
		int posY;

		float dX;
		float dY;

		int size;

		struct WaterVertex : IVertexType {
			private readonly static VertexDeclaration vDecl = new VertexDeclaration(
				new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
				new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
			);

			public float x, y;
			public float u, v;

			public WaterVertex(float X, float Y, float U, float V) {
				x = X;
				y = Y;
				u = U;
				v = V;
			}

			VertexDeclaration IVertexType.VertexDeclaration {
				get { return vDecl; }
			}
		}

		public Water(Game game, uint size) {
			this.size = (int)size;
			this.game = game;

			WaterVertex[] data = new WaterVertex[(size + 1) * (size + 1)];

			float offsetX = size * 0.5f;
			float offsetY = size * 0.5f;
			float tMul = 1.0f / size;

			for (uint i = 0; i <= size; ++i) {
				for (uint j = 0; j <= size; ++j) {
					data[i * (size + 1) + j] = new WaterVertex(i - offsetX, j - offsetY, i * tMul, j * tMul);
				}
			}

			uint[] indicies = new uint[size * size * 6];

			for (uint i = 0; i < size; ++i) {
				for (uint j = 0; j < size; ++j) {
					uint index = (i * size + j) * 6u;

					indicies[index + 0] = j + (size + 1) * i;
					indicies[index + 1] = indicies[index + 0] + 1;
					indicies[index + 2] = indicies[index + 0] + size + 1;

					indicies[index + 3] = indicies[index + 2];
					indicies[index + 4] = indicies[index + 1];
					indicies[index + 5] = indicies[index + 2] + 1;
				}
			}

			surfaceVertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(WaterVertex), (int)((size + 1) * (size + 1)), BufferUsage.WriteOnly);
			surfaceVertexBuffer.SetData(data);

			surfaceIndexBuffer = new IndexBuffer(game.GraphicsDevice, IndexElementSize.ThirtyTwoBits, (int)(size * size * 6), BufferUsage.WriteOnly);
			surfaceIndexBuffer.SetData(indicies);

			fullScreenQuad = new FullscreenQuad(game);
			enviro = game.Content.Load<TextureCube>("enviro2");
			frontTex = new RenderTarget2D(game.GraphicsDevice, (int)size, (int)size, false, SurfaceFormat.Vector2, DepthFormat.None);
			backTex = new RenderTarget2D(game.GraphicsDevice, (int)size, (int)size, false, SurfaceFormat.Vector2, DepthFormat.None);

			displayEffect = game.Content.Load<Effect>("Water");
			displayEffect.Parameters["enviro"].SetValue(enviro);
			displayEffect.Parameters["fresnelParam"].SetValue(0.5f);
			displayEffect.Parameters["waterTint"].SetValue(new Vector4(0.9f, 0.9f, 1.0f, 0));
			displayEffect.Parameters["pixel"].SetValue(1.0f / size);

			rippleEffect = game.Content.Load<Effect>("Ripple");
			rippleEffect.Parameters["pixel"].SetValue(1.0f / size);

			waterData = new Vector2[size * size];
		}

		public void Draw(Matrix View, Matrix Proj) {
			var temp = frontTex;
			frontTex = backTex;
			backTex = temp;

			game.GraphicsDevice.VertexTextures[0] = null;
			backTex.SetData(waterData);


			game.GraphicsDevice.SetRenderTarget(frontTex);
			rippleEffect.Parameters["tex"].SetValue(backTex);
			rippleEffect.Parameters["dPos"].SetValue(new Vector2(dX, -dY));
			rippleEffect.CurrentTechnique.Passes[0].Apply();

			fullScreenQuad.Draw();


			game.GraphicsDevice.SetRenderTarget(null);
			game.GraphicsDevice.SetVertexBuffer(surfaceVertexBuffer);
			game.GraphicsDevice.Indices = surfaceIndexBuffer;
			displayEffect.Parameters["view"].SetValue(View);
			displayEffect.Parameters["viewProj"].SetValue(View * Proj);
			displayEffect.Parameters["invView"].SetValue(Matrix.Invert(View));
			displayEffect.Parameters["tex"].SetValue(frontTex);
			displayEffect.Parameters["lightPos"].SetValue(Vector3.Transform(new Vector3(10000, 5000, 5000), View));
			displayEffect.Parameters["globalPos"].SetValue(new Vector2(posX, posY));

			displayEffect.CurrentTechnique.Passes[0].Apply();

			game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, surfaceVertexBuffer.VertexCount, 0, surfaceIndexBuffer.IndexCount / 3);

			frontTex.GetData<Vector2>(waterData);

			dX = 0;
			dY = 0;
		}

		public void Touch(Vector3 point) {
			const int touchSize = 4;
			int x = (int)point.X + size / 2 - touchSize / 2;
			int y = (int)-point.Z + size / 2 - touchSize / 2;
			x -= posX;
			y += posY;
			int x2 = x + touchSize;
			int y2 = y + touchSize;
			x = x < 0 ? 0 : x;
			y = y < 0 ? 0 : y;
			x2 = x2 > size ? size : x2;
			y2 = y2 > size ? size : y2;


			for (int i = x; i < x2; ++i) {
				for (int j = y; j < y2; ++j) {
					waterData[j * size + i].Y += 4f;
				}
			}
		}

		public void SetPos(float x, float y) {
			int iX = (int) x;
			int iY = (int) y;
			
			dX += (iX - posX) / (float) size;
			dY += (iY - posY) / (float) size;

			posX = iX;
			posY = iY;
		}
	}
}
