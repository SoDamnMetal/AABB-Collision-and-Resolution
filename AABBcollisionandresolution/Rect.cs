using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AABBcollisionandresolution {
	public class Rect {
		private Texture2D texture;
		public Vector2 Position { get; set; }
		public Vector2 Velocity { get; set; }
		public Vector2 Size { get; set; }
		
		public Rect[] Contact;

		public Rect(Vector2 position, Vector2 size) {
			Position = position;
			Size = size;
			Contact = new Rect[4]; 
		}

		public bool PointVsRect(Vector2 p, Rect r) {

			return (p.X >= r.Position.X && p.Y >= r.Position.Y && p.X < r.Position.X + r.Size.X && p.Y < r.Position.Y + r.Size.Y);
		}

		public bool RectVsRect(Rect r1, Rect r2) {

			return (r1.Position.X < r2.Position.X + r2.Size.X && r1.Position.X + r1.Size.X > r2.Position.X && r1.Position.Y < r2.Position.Y + r2.Size.Y && r1.Position.Y + r1.Size.Y > r2.Position.Y);
		}

		private void Swap<T>(ref T lhs, ref T rhs) {
			T temp;
			temp = lhs;
			lhs = rhs;
			rhs = temp;
		}

		private bool RayVsRect(Vector2 ray_origin, Vector2 ray_dir, Rect target, ref Vector2 contact_point, ref Vector2 contact_normal, ref float t_hit_near) {

			Vector2 targetpos = new Vector2(target.Position.X, target.Position.Y);
			Vector2 targetsize = new Vector2(target.Size.X, target.Size.Y);

			Vector2 t_near = (targetpos - ray_origin) / ray_dir;
			Vector2 t_far = (targetpos + targetsize - ray_origin) / ray_dir;

			if (double.IsNaN(t_far.Y) || double.IsNaN(t_far.X)) { return false; }
			if (double.IsNaN(t_near.Y) || double.IsNaN(t_near.X)) { return false; }

			if (t_near.X > t_far.X) { Swap(ref t_near.X, ref t_far.X); }
			if (t_near.Y > t_far.Y) { Swap(ref t_near.Y, ref t_far.Y); }

			if (t_near.X > t_far.Y || t_near.Y > t_far.X) { return false; }

			t_hit_near = Math.Max(t_near.X, t_near.Y);
			float t_hit_far = Math.Min(t_far.X, t_far.Y);

			if (t_hit_far < 0) { return false; }

			contact_point = ray_origin + t_hit_near * ray_dir;

			if (t_near.X > t_near.Y) {
				if (ray_dir.X < 0) {
					contact_normal.X = 1; contact_normal.Y = 0;
				} else {
					contact_normal.X = -1; contact_normal.Y = 0;
				}
			} else if (t_near.X < t_near.Y) {
				if (ray_dir.Y < 0) {
					contact_normal.X = 0; contact_normal.Y = 1;
				} else {
					contact_normal.X = 0; contact_normal.Y = -1;
				}
			}

			return true;
		}

		public bool DynamicRectVsRect(Rect r_in, Rect target, ref Vector2 contact_point, ref Vector2 contact_normal, ref float contact_time, float fElapsedTime) {
			// Check if dynamic rectangle is actually moving - we assume rectangles are NOT in collision to start
			if (r_in.Velocity.X == 0 && r_in.Velocity.Y == 0) {
				return false;
			}

			Rect expanded_target = new Rect(target.Position - r_in.Size / 2, target.Size + r_in.Size);

			Vector2 ray_origin = r_in.Position + r_in.Size / 2;
			Vector2 ray_dir = r_in.Velocity * fElapsedTime;

			if (RayVsRect(ray_origin, ray_dir, expanded_target, ref contact_point, ref contact_normal, ref contact_time)) {

				if (contact_time >= 0.0f && contact_time <= 1.0f) {
					return true;
				} else {
					return false;
				}
			}

			return false;
		}

		public bool ResolveDynamicRectVsRect(Rect r_dynamic, float fTimeStep, Rect r_static) {
			Vector2 contact_point = new Vector2(0, 0); 
			Vector2	contact_normal = new Vector2(0, 0);
			float contact_time = 0.0f;

			if (DynamicRectVsRect(r_dynamic, r_static, ref contact_point, ref contact_normal, ref contact_time, fTimeStep))
			{
				if (contact_normal.Y > 0) r_dynamic.Contact[0] = r_static; else r_dynamic.Contact[0] = null;
				if (contact_normal.X < 0) r_dynamic.Contact[1] = r_static; else r_dynamic.Contact[1] = null;
				if (contact_normal.Y < 0) r_dynamic.Contact[2] = r_static; else r_dynamic.Contact[2] = null;
				if (contact_normal.X > 0) r_dynamic.Contact[3] = r_static; else r_dynamic.Contact[3] = null;

				r_dynamic.Velocity += contact_normal * new Vector2(Math.Abs(r_dynamic.Velocity.X), Math.Abs(r_dynamic.Velocity.Y)) * (1 - contact_time);
				return true;
			}

			return false;
		}

		public void LoadContent(GraphicsDevice g) {

			texture = new Texture2D(g, 1, 1);
			texture.SetData(new Color[] { Color.White });
		}

		public void Draw(SpriteBatch s, Vector2 pos, Vector2 size, Color c) {
			s.Begin();
			s.Draw(texture, new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y), c);
			s.End();
		}

	}
}
