using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AABBcollisionandresolution {
	public class Player : Rect{

        public Player(Vector2 position, Vector2 size) : base(position, size) {
            Position = position;
            Size = size;
		}

        public void Update() {
            GetMoveInput();
        }

		private void GetMoveInput() {
			bool w = Keyboard.GetState().IsKeyDown(Keys.W);
			bool s = Keyboard.GetState().IsKeyDown(Keys.S);
			bool a = Keyboard.GetState().IsKeyDown(Keys.A);
			bool d = Keyboard.GetState().IsKeyDown(Keys.D);

			if (w) { MoveUp(); } else if (s) { MoveDown(); }
			if (a) { MoveLeft(); } else if (d) { MoveRight(); }

		}

		protected void MoveUp() {
			Velocity += new Vector2(0, -2f);
		}
		protected void MoveDown() {
			Velocity += new Vector2(0, 2f);
		}
		protected void MoveRight() {
			Velocity += new Vector2(2f, 0);
		}
		protected void MoveLeft() {
			Velocity += new Vector2(-2f, 0);
		}
	}
}
