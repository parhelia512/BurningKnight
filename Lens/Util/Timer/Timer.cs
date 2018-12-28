﻿using System;
using System.Collections.Generic;

namespace Lens.Util.Timer {
	public static class Timer {
		private static List<TimerTask> tasks = new List<TimerTask>();

		public static void Add(Action fn, float Delay) {
			tasks.Add(new TimerTask(fn, Delay));
		}
		
		public static void Update(float dt) {
			for (int i = tasks.Count - 1; i >= 0; i--) {
				TimerTask task = tasks[i];
				task.Delay -= dt;

				if (task.Delay <= 0) {
					task.Fn?.Invoke();
					tasks.RemoveAt(i);
				}
			}
		}
	}
}