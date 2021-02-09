using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon
{
	public class BfsTask
	{
		public static IEnumerable<SinglyLinkedList<Point>> FindPaths(Map map, Point start, Point[] chests)
		{
			var track = new Dictionary<Point, SinglyLinkedList<Point>>();
			track[start] = new SinglyLinkedList<Point>(start);
			var queue = new Queue<SinglyLinkedList<Point>>();
			queue.Enqueue(track[start]);
			foreach (var chest in chests)
			{
				if (track.ContainsKey(chest))
				{
					yield return track[chest];
					continue;
				}
				var path = FindPath(track, queue, map, start, chest);
				if (path != null)
					yield return path;
			}
		}
		static SinglyLinkedList<Point> FindPath(Dictionary<Point, SinglyLinkedList<Point>> track, Queue<SinglyLinkedList<Point>> queue, Map map, Point start, Point end)
		{
			while (queue.Count != 0)
			{
				var node = queue.Dequeue();
				var incidentNodes = Walker.PossibleDirections
					.Select(direction => node.Value + direction)
					.Where(point => map.InBounds(point) && map.Dungeon[point.X, point.Y] != MapCell.Wall);
				foreach (var nextNode in incidentNodes)
				{
					if (track.ContainsKey(nextNode)) continue;
					track[nextNode] = new SinglyLinkedList<Point>(nextNode, node);
					queue.Enqueue(track[nextNode]);
				}
				if (track.ContainsKey(end)) return track[end];
			}
			if (!track.ContainsKey(end)) return null;
			throw new Exception("There should never be this exception");
		}
	}
}