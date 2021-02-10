using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon
{
	public class DungeonTask
	{
		public static MoveDirection[] FindShortestPath(Map map)
		{
			HashSet<Point> chests = new HashSet<Point>(map.Chests);
			var pathToExit = BfsTask.FindPaths(map, map.InitialPosition, new[] { map.Exit }).FirstOrDefault();
			if (pathToExit != null)
				foreach (var point in pathToExit)
					if (chests.Contains(point))
						return MakeDirectionFromPath(pathToExit);
			var pathsToChests = BfsTask.FindPaths(map, map.InitialPosition, map.Chests);
			SinglyLinkedList<Point> shortestPath = null;
			foreach (var pathsToChest in pathsToChests)
			{
				var pathFromChestToExit = FindPath(map, pathsToChest, map.Exit);
				if (shortestPath == null || (pathFromChestToExit != null && pathFromChestToExit.Length < shortestPath.Length))
					shortestPath = pathFromChestToExit;
			}
			if (shortestPath == null)
				shortestPath = pathToExit;
			return MakeDirectionFromPath(shortestPath);
		}
		static MoveDirection[] MakeDirectionFromPath(SinglyLinkedList<Point> path)
		{
			if (path == null)
				return new MoveDirection[0];
			MoveDirection[] result = new MoveDirection[path.Length - 1];
			SinglyLinkedList<Point> currentNode = path;
			for (int i = path.Length - 2; i >= 0; i--, currentNode = currentNode.Previous)
			{
				Size offset = new Size(currentNode.Value) - new Size(currentNode.Previous.Value);
				result[i] = Walker.ConvertOffsetToDirection(offset);
			}
			return result;
		}
		static SinglyLinkedList<Point> FindPath(Map map, SinglyLinkedList<Point> start, Point end)
		{
			var track = new Dictionary<Point, SinglyLinkedList<Point>>();
			track[start.Value] = start;
			var queue = new Queue<SinglyLinkedList<Point>>();
			queue.Enqueue(start);
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
