using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace AABBcollisionandresolution {
	public class Game1 : Game {

		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private Vector2 screen;
		private List<Rect> vRects;
		private Player player;

		public Game1() {
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			screen = new Vector2(1280, 720);
		}

		protected override void Initialize() {
			graphics.PreferredBackBufferHeight = (int)screen.Y;
			graphics.PreferredBackBufferWidth = (int)screen.X;
			graphics.ApplyChanges();

			vRects = new List<Rect>();
			player = new Player(new Vector2(10f, 10f), new Vector2(10f, 30f));
			vRects.Add(player);
			vRects.Add(new Rect(new Vector2(10f, 200f), new Vector2(20f, 20f)));
			vRects.Add(new Rect(new Vector2(30f, 200f), new Vector2(20f, 20f)));
			vRects.Add(new Rect(new Vector2(50f, 200f), new Vector2(20f, 20f)));
			vRects.Add(new Rect(new Vector2(70f, 200f), new Vector2(20f, 20f)));
			vRects.Add(new Rect(new Vector2(90f, 200f), new Vector2(20f, 20f)));
			vRects.Add(new Rect(new Vector2(110f, 200f), new Vector2(20f, 20f)));
			vRects.Add(new Rect(new Vector2(130f, 200f), new Vector2(20f, 20f)));
			vRects.Add(new Rect(new Vector2(150f, 200f), new Vector2(20f, 20f)));
			vRects.Add(new Rect(new Vector2(170f, 200f), new Vector2(20f, 20f)));
			vRects.Add(new Rect(new Vector2(190f, 200f), new Vector2(20f, 20f)));

			vRects.Add(new Rect(new Vector2(5f, 150f), new Vector2(5f, 70f)));
			vRects.Add(new Rect(new Vector2(210f, 150f), new Vector2(5f, 70f)));

			base.Initialize();
		}

		protected override void LoadContent() {
			spriteBatch = new SpriteBatch(GraphicsDevice);

			foreach (Rect r in vRects) {
				r.LoadContent(GraphicsDevice);
			}
		}

		protected override void Update(GameTime gameTime) {

			float fElapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

			player.Update();

			// Sort collisions in order of distance
			Vector2 cp = new Vector2(0, 0);
			Vector2 cn = new Vector2(0, 0);
			float t = 0;
			List<KeyValuePair<int, float>> z = new List<KeyValuePair<int, float>>();

			// Work out collision point, add it to vector along with rect ID
			for (int i = 1; i < vRects.Count; i++) {
				if (player.DynamicRectVsRect(player, vRects[i], ref cp, ref cn, ref t, fElapsedTime)) {
					z.Add(new KeyValuePair<int, float>(i, t));
				}
			}

			// Do the sort
			z.Sort((a, b) => a.Value.CompareTo(b.Value));

			// Now resolve the collision in correct order 
			foreach (var j in z) {
				player.ResolveDynamicRectVsRect(player, fElapsedTime, vRects[j.Key]);
			}

			// Update players position
			player.Position += player.Velocity * fElapsedTime;

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.DarkBlue);

			foreach (Rect r in vRects) {
				r.Draw(spriteBatch, r.Position, r.Size, Color.White);
			}

			//Embellish the "in contact" rectangles in yellow
			for (int i = 0; i < 4; i++) {
				if (player.Contact[i] != null) {
					player.Contact[i].Draw(spriteBatch, player.Contact[i].Position, player.Contact[i].Size, Color.Yellow);
					player.Contact[i] = null;
				}
			}

			// Draw players velocity vector
			if (player.Velocity.LengthSquared() > 0) {
				player.Velocity.Normalize();
				spriteBatch.Begin();
				spriteBatch.DrawLine(player.Position + player.Size / 2, (player.Position + player.Size / 2) + player.Velocity, Color.Red);
				spriteBatch.End();
			}
			

			base.Draw(gameTime);
		}


	}
}
