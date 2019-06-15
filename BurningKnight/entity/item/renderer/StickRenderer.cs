using System;
using Lens.graphics;
using Lens.input;
using Lens.lightJson;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item.renderer {
	public class StickRenderer : ItemRenderer {
		private double lastAngle;
		private Vector2 scale = Vector2.One;
		
		public override void OnUse() {
			scale.X = 1.4f;
			scale.Y = 0.3f;

			Tween.To(1, scale.X, x => scale.X = x, 0.2f);
			Tween.To(1, scale.Y, x => scale.Y = x, 0.2f);
		}

		public override void Render(bool atBack, bool paused, float dt) {
			var region = Item.Region;
			var owner = Item.Owner;
			var origin = new Vector2(region.Width / 2f, region.Height);
			
			if (!atBack && !paused) {
				lastAngle = MathUtils.LerpAngle(lastAngle, owner.AngleTo(Input.Mouse.GamePosition) + Math.PI * 0.5f, dt * 6f);
			}

			var angle = atBack ? (float) Math.PI * (owner.GraphicsComponent.Flipped ? 0.25f : -0.25f) : (float) lastAngle;
			
			Graphics.Render(region, new Vector2(owner.CenterX - region.Width / 2f, owner.Y) + origin, angle, origin, scale);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
		}

		public static void RenderDebug(string id, JsonValue parent, JsonValue root) {
			
			
		}
	}
}