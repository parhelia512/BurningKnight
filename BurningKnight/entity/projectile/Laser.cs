using System;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.level;
using BurningKnight.physics;
using BurningKnight.state;
using Lens.entity;
using Lens.util;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Dynamics.Solver;

namespace BurningKnight.entity.projectile {
	public class Laser : Projectile {
		public float LifeTime = 1.5f;
		public bool Dynamic = true;
		public float AdditionalAngle;
		public float Range = 15f;
		public Vector2 End;
		public bool PlayerRotated;

		private float angle;

		public float Angle {
			get => angle;
			set {
				angle = value;

				if (BodyComponent?.Body != null) {
					BodyComponent.Body.Rotation = angle + AdditionalAngle;
				}
			}
		}

		public static Laser Make(Entity owner, float a, float additional, Item item = null, float damage = 1, float scale = 1f, float range = -1, Laser parent = null) {
			if (owner is Item i) {
				item = i;
				owner = i.Owner;
			}

			var projectile = new Laser {
				Owner = owner,
				FirstOwner = owner,
				Damage = damage,
				Flags = 0,
				Slice = "laser",
				Bounce = 0,
				Scale = scale,
				Color = ProjectileColor.Red,
				Parent = parent,
				Item = item
			};

			owner.Area.Add(projectile);

			var graphics = new LaserGraphicsComponent("projectiles", "laser");
			projectile.AddComponent(graphics);

			if (range > 0) {
				projectile.Range = range;
			}

			if (parent != null) {
				projectile.Scale *= 0.7f;
				projectile.Range *= 0.5f;
			}

			projectile.Position = owner.Center;

			owner.HandleEvent(new ProjectileCreatedEvent {
				Owner = owner,
				Item = item,
				Projectile = projectile
			});

			projectile.Width = 32;
			projectile.Height = 9 * projectile.Scale;

			projectile.CreateBody();

			projectile.AdditionalAngle = additional;
			projectile.Angle = a;

			return projectile;
		}

		public override void AddComponents() {
			base.AddComponents();
			AddTag(Tags.Laser);
		}

		private void CreateBody() {
			AddComponent(new RectBodyComponent(0, -Height * 0.5f, Width, Height));
		}

		private static bool RayShouldCollide(Entity entity) {
			return entity is ProjectileLevelBody || entity is Level || entity is Door;
		}

		public override bool BreaksFrom(Entity entity, BodyComponent body) {
			return false;
		}

		public void Recalculate() {
			var min = 1f;

			Vector2 closest;
			var aim = Owner.GetComponent<AimComponent>();

			Position = aim.Center;

			var from = Position;
			
			if (PlayerRotated) {
				BodyComponent.Body.Rotation = angle = (aim.RealAim - from).ToAngle();
			}

			closest = Position + MathUtils.CreateVector(BodyComponent.Body.Rotation, Range * 5);

			Physics.World.RayCast((fixture, point, normal, fraction) => {
				if (min > fraction && fixture.Body.UserData is BodyComponent b && RayShouldCollide(b.Entity)) {
					min = fraction;
					closest = point;
				}
				
				return min;
			}, from, closest);

			var v = (from - closest);
			var len = v.Length();

			if (Math.Abs(len - Width) > 1) {
				Width = len;
				BodyComponent.Resize(0, -Height * 0.5f, Width, Height);
			}

			BodyComponent.Body.Rotation = angle + AdditionalAngle;
			End = Position + MathUtils.CreateVector(BodyComponent.Body.Rotation, Width);
		}

		private float lastClear;

		public override void Update(float dt) {
			base.Update(dt);

			lastClear += dt;

			if (lastClear >= 0.05f) {
				lastClear = 0;
				EntitiesHurt.Clear();
			}

			if (LifeTime > 0) {
				LifeTime -= dt;

				if (LifeTime <= 0) {
					Break();
					Done = true;
					LifeTime = 0;
					return;
				}
			}

			if (Dynamic) {
				Recalculate();
			}
		}

		public override void Resize(float newScale) {
			Scale = newScale;
			Height = 9 * Scale;

			GetComponent<RectBodyComponent>().Resize(0, 0, Width, Height, true);
		}
	}
}