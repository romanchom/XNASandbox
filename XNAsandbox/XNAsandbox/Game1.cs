using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XNAsandbox {
	public class Game1 : Game {
		GraphicsDeviceManager graphics;
		float cameraYaw = 0;
		float cameraPitch = 0;
		const uint size = 512;
		Vector3 cameraPos = new Vector3(0, 50, 0);
		Vector3 waterPos;

		bool rain = false;
		Water w;

		Matrix view;
		Matrix proj;


		public Game1() {
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferHeight = 1080;
			graphics.PreferredBackBufferWidth = 1920;
			Content.RootDirectory = "Content";
		}

		protected override void Initialize() {
			base.Initialize();
			IsMouseVisible = true; 
		}

		protected override void LoadContent() {
			w = new Water(this, size);
		}

		protected override void UnloadContent() {

		}

		protected override void Update(GameTime gameTime) {
			Input.Update();
			const float mouseSpeed = 0.005f;

			if (Input.GetKey(Keys.Escape)) Exit();

			base.Update(gameTime);

			Vector3 forward = new Vector3();
			forward.X = (float)Mathf.Sin(cameraYaw);
			forward.Z = -(float)Mathf.Cos(cameraYaw);
			Vector3 right = new Vector3();
			right.X = -forward.Z;
			right.Z = forward.X;

			if (Input.GetKey(Keys.W)) {
				cameraPos += forward * size * 0.01f;
			}
			if (Input.GetKey(Keys.S)) {
				cameraPos -= forward * size * 0.01f;
			}

			if (Input.GetKey(Keys.A)) {
				cameraPos -= right * size * 0.01f;
			}
			if (Input.GetKey(Keys.D)) {
				cameraPos += right * size * 0.01f;
			}
			cameraPos.Y += Input.mouseScroll * 0.01f;

			if (Input.GetMouseButton(1)) {
				cameraYaw += Input.mouseDelta.X * mouseSpeed;
				cameraPitch += Input.mouseDelta.Y * mouseSpeed;
			}

			view = Matrix.CreateTranslation(-cameraPos) * Matrix.CreateRotationY(cameraYaw) * Matrix.CreateRotationX(cameraPitch);
			proj = Matrix.CreatePerspectiveFieldOfView(1, GraphicsDevice.Viewport.AspectRatio, 0.01f, 1000.0f);

			waterPos = cameraPos + forward * size * 0.4f;
			w.SetPos(waterPos.X, waterPos.Z);

			
			if (Input.GetMouseButton(0)) {
				Vector3 nearPoint = new Vector3(Input.mousePosition, 0);
				Vector3 farPoint = new Vector3(Input.mousePosition, 1);

				Viewport viewport = GraphicsDevice.Viewport;
				
				nearPoint = viewport.Unproject(nearPoint, proj, view, Matrix.Identity);
				farPoint = viewport.Unproject(farPoint, proj, view, Matrix.Identity);

				Vector3 direction = farPoint - nearPoint;
				direction.Normalize();

				Ray ray = new Ray(nearPoint, direction);

				Plane plane = new Plane(0, 1, 0, 0);

				float? dist = ray.Intersects(plane);

				if(dist != null){
					Vector3 point = nearPoint + direction * dist.GetValueOrDefault();
					w.Touch(point);
				}
			}

			if (Input.GetKeyDown(Keys.R)) {
				rain = !rain;
			}

			if (rain) {
				w.Touch(new Vector3(Random.Range(-size / 2, size / 2), 0, Random.Range(-size / 2, size / 2)) + waterPos);
			}

		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.Black);

			w.Draw(view, proj);
			base.Draw(gameTime);
		}
	}
}
