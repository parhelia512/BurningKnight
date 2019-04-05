using BurningKnight.entity.level.walls;

namespace BurningKnight.entity.level.rooms.entrance {
	public class EntranceWallPool : WallRegistry {
		public static EntranceWallPool Instance = new EntranceWallPool();
		
		protected override void SetupRooms() {
			Add(new WallPainter(), 1f);
		}
	}
}