using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace GameA;

public class Circle
{	
	private class Collision
	{
		public float overflow;

		public Circle item;

		public byte count;

		public Collision(float overflow, Circle item)
		{	
			this.overflow = overflow;
			this.item = item;
			count = 1;
		}
	}

	private List<Collision> collisions;

	public string name;

	public float radius;

	private Vector2 top => new Vector2(0, -radius);

	private Vector2 bottom => new Vector2(0, radius);

	private Vector2 left => new Vector2(-radius, 0);

	private Vector2 right => new Vector2(radius, 0);

	private Vector2 Top => position + top;

	private Vector2 Bottom => position + bottom;

	private Vector2 Left => position + left;

	private Vector2 Right => position + right;

	public Vector2 texturePosition;

	private Vector2 position;

	public Vector2 Position
	{
		get { return position; }

		set
		{
			position = value;
            
            float distance = 0.0f;

			foreach (var item in ApplyPhysics.circles)
			{
				if(item == this) continue;

				distance = Vector2.Distance(position, item.position) - 2 * radius;

				if(distance < 0) 
				{
					Collision c = new Collision(distance, item);

					if (collisions.Count > 0)
					{
						if(!AddSameCollision(c))
							collisions.Add(c);
					}
					else
						collisions.Add(c);
				}
			}

			if (Top.Y < GameBase.topBorderRect.Y + GameBase.topBorderRect.Height)
			{
                float overflow = Top.Y - (GameBase.topBorderRect.Y + GameBase.topBorderRect.Height);
				position.Y += -overflow;
				physics.SetSpeed('Y', -physics.speed.Y);
			}

			if (Bottom.Y > GameBase.bottomBorderRect.Y)
			{
				float overflow = Bottom.Y - GameBase.bottomBorderRect.Y;
				position.Y += -overflow;
				physics.SetSpeed('Y', -physics.speed.Y);
			}

			if (Left.X < GameBase.leftBorderRect.X + GameBase.leftBorderRect.Width)
			{
				float overflow = Left.X - (GameBase.leftBorderRect.X + GameBase.leftBorderRect.Width);
				position.X += -overflow;
				physics.SetSpeed('X', -physics.speed.X);
			}

			if(Right.X > GameBase.rightBorderRect.X)
			{
				float overflow = Right.X - GameBase.rightBorderRect.X;
				position.X += -overflow;
				physics.SetSpeed('X', -physics.speed.X);
			}

			if(collisions.Count > 0)
			{
				Vector2 responses = Vector2.Zero;

                foreach (var item in collisions)
                {
					if (physics.speed == Vector2.Zero)
						continue;

                    Console.WriteLine("item name: {0} , overflow: {1}  , count: {2}",item.item.name, item.overflow, item.count);

					position += ReverseOverlap(item.overflow, physics.speed);

					float cosine = FindCosine(position, item.item.position, physics.speed);
					Vector2 speed = FindCollisionSpeed(physics.speed * cosine, position, item.item.position) / item.count;
						
                    Console.WriteLine("speed: {0} -- collision transfer speed: {1}",physics.speed, speed);

					Vector2 transferForce = speed * Physics.mass / Physics.time;
					responses += -transferForce;

					item.item.physics.Force = transferForce;

					Console.WriteLine("after collision item speed => {0} speed: {1}",item.item.name,item.item.physics.speed);

				}

				if(responses != Vector2.Zero)
					physics.Force = responses;

                Console.WriteLine("after collision {0} speed: {1} \n",name, physics.speed);
            }

			texturePosition = new Vector2(position.X - radius, position.Y - radius);
			
			collisions.Clear();
		}
	}

	public Physics physics;

	public Texture2D texture;

	public Circle(Vector2 position, Texture2D texture, string name)
	{
		this.position = position;
		this.radius = texture.Width / 2;
		this.texture = texture;

        Console.WriteLine(radius);

        texturePosition = new(position.X - radius, position.Y - radius);
		ApplyPhysics.circles.Add(this);

		physics = new();
		collisions = new();

		this.name = name;
	}

	private bool AddSameCollision(Collision c)
	{
		for (int i = 0; i < collisions.Count; i++)
		{
			if (c.overflow == collisions[i].overflow)
			{
				collisions[i].count++;
				c.count = collisions[i].count;

				collisions.Add(c);

				for (int j = i + 2; j < collisions.Count; j++)
				{
					if (collisions[j].overflow == c.overflow)

						collisions[j].count++;
				}

				return true;
			}
		}

		return false;
	}

	public float FindCosine(Vector2 circle1, Vector2 circle2, Vector2 speed)
	{
		Vector2 a = circle2 - circle1;
		
		float dot = a.X * speed.X + a.Y * speed.Y;

		float ahypotenus = MathF.Sqrt(MathF.Pow(a.X, 2) + MathF.Pow(a.Y, 2));
		float speedhypotenus = MathF.Sqrt(MathF.Pow(speed.X, 2) + MathF.Pow(speed.Y, 2));

		float c = ahypotenus * speedhypotenus;

		if (c == 0)

			return 0;

		return dot / c;
	}

	public Vector2 FindCollisionSpeed(Vector2 speed, Vector2 circle1, Vector2 circle2)
	{
		if (speed == Vector2.Zero)

			return Vector2.Zero;

		float speedHypotenusSquare = MathF.Pow(speed.X, 2) + MathF.Pow(speed.Y, 2);

        float X = circle2.X - circle1.X;
        float Y = circle2.Y - circle1.Y;

		float c = MathF.Sqrt(speedHypotenusSquare / (MathF.Pow(X, 2) + MathF.Pow(Y, 2)));

        return new Vector2(X * c, Y * c);
	}

	public Vector2 ReverseOverlap(float overflow, Vector2 speed)
	{
		float hypotenus = MathF.Sqrt(MathF.Pow(speed.X, 2) + MathF.Pow(speed.Y, 2));

		float time = MathF.Abs(overflow) / hypotenus;

		return time * -speed;
	}
}
