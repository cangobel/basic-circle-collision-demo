using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameA;

public static class ApplyPhysics
{
	public static List<Circle> circles = new();

	public static GameTime gameTime;

	public static void ApplySpeeds()
	{ 
		foreach (var circle in circles)
		{
			if(circle.physics.speed != Vector2.Zero)
			{
				circle.Position += circle.physics.Speed;
			}
		}
	}
}
