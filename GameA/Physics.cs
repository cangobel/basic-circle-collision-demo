using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameA;

public class Physics
{
	public const float gravity = 10f;

	public const float mass = 10;

	public const float kineticFriction = 0.5f * mass * gravity;

	public static float time => (float)ApplyPhysics.gameTime.ElapsedGameTime.TotalMilliseconds / 1000;

	private Vector2 force;

	public Vector2 Force
	{
		set 
		{ 
			force = value;
			SetKineticFrictionForce(force);

			speed += acceleration * time;
			SetKineticFrictionForce(speed);

            force = Vector2.Zero;
		}

		get { return force; }
	}

	public Vector2 kineticFrictionForce;

	public Vector2 netForce => force + kineticFrictionForce;

	public Vector2 acceleration => netForce / mass;

	public Vector2 speed;

	public Vector2 Speed
	{
		set { speed = value; }

		get { return CalculateSpeed(speed); }
	}

	public Vector2 CalculateSpeed(Vector2 prevSpeed)
	{
		Vector2 nextSpeed = prevSpeed + acceleration * time;

		if (prevSpeed.X < 0 && nextSpeed.X >= 0)
		{
			kineticFrictionForce = Vector2.Zero;
			speed = Vector2.Zero;
			return Vector2.Zero;
		}

		else if (prevSpeed.X > 0 && nextSpeed.X <= 0)
		{
			kineticFrictionForce = Vector2.Zero;
			speed = Vector2.Zero;
			return Vector2.Zero;
		}

		else if (prevSpeed.Y < 0 && nextSpeed.Y >= 0)
		{
			kineticFrictionForce = Vector2.Zero;
			speed = Vector2.Zero;
			return Vector2.Zero;
		}

		else if (prevSpeed.Y > 0 && nextSpeed.Y <= 0)
		{
			kineticFrictionForce = Vector2.Zero;
			speed = Vector2.Zero;
			return Vector2.Zero;
		}

		this.speed = nextSpeed;

		return nextSpeed;
	}

	public void AddSpeed(Vector2 additiveSpeed)
	{
		this.speed += additiveSpeed;
		SetKineticFrictionForce(this.speed);
	}

	public void SetSpeed(Vector2 speed)
	{
		this.speed = speed;
		SetKineticFrictionForce(this.speed);
	}

	public void SetSpeed(char dimension, float magnitude)
	{
		if (dimension == 'X')
			speed.X = magnitude;

		else if (dimension == 'Y')
			speed.Y = magnitude;

		SetKineticFrictionForce(speed);
	}

	public void SetKineticFrictionForce(Vector2 baseVector)
	{
		float hypotenus = MathF.Sqrt(MathF.Pow(baseVector.X, 2) + MathF.Pow(baseVector.Y, 2));

        float X = baseVector.X / hypotenus;
        float Y = baseVector.Y / hypotenus;

		kineticFrictionForce = new Vector2(-X * kineticFriction, -Y * kineticFriction);
	}
}
