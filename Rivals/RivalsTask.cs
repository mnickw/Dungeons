using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Rivals
{
	public class RivalsTask
	{
		public static IEnumerable<OwnedLocation> AssignOwners(Map map)
		{
			var visited = new HashSet<Point>();
			var queue = new Queue<OwnedLocation>();

			for (int i = 0; i < map.Players.Length; i++)
			{
				visited.Add(map.Players[i]);
				queue.Enqueue(new OwnedLocation(i, map.Players[i], 0));
			}

			while (queue.Count != 0)
			{
				var node = queue.Dequeue();
				yield return node;
				var incidentNodes = new Point[]
				{
					new Point(node.Location.X, node.Location.Y+1),
					new Point(node.Location.X, node.Location.Y-1),
					new Point(node.Location.X+1, node.Location.Y),
					new Point(node.Location.X-1, node.Location.Y)
				}.Where(point => map.InBounds(point) && map.Maze[point.X, point.Y] != MapCell.Wall);
				foreach (var nextNode in incidentNodes.Where(incidentNode => !visited.Contains(incidentNode)))
				{
					visited.Add(nextNode);
					queue.Enqueue(new OwnedLocation(node.Owner, nextNode, node.Distance + 1));
				}
			}
		}
	}
}
