﻿using System.Numerics;

[System.Serializable]
public class Point {
	public int x;
	public int y;

	public Point(int newX, int newY) {
		x = newX;
		y = newY;
	}

	public void Multiply(int value) {
		x *= value;
		y *= value;
	}

	public void Add(Point point) {
		x += point.x;
		y += point.y;
	}

	public bool Equals(Point point)
		=> x == point.x && y == point.y;
	
	public Vector2 ToVector()
		=> new(x, y);
	
	public static Point FromVector(Vector2 vector)
		=> new((int)vector.X, (int)vector.Y);

	public static Point FromVector(Vector3 vector)
		=> new((int)vector.X, (int)vector.Y);

	public static Point Multiply(Point point, int value)
		=> new(point.x * value, point.y * value);
	
	public static Point Add(Point point1, Point point2)
		=> new(point1.x + point2.x, point1.y + point2.y);
	
	public static Point Clone(Point point)
		=> new(point.x, point.y);
	
	public static Point zero => new(0, 0);

	public static Point up => new(0, 1);

	public static Point down => new(0, -1);

	public static Point left => new(-1, 0);

	public static Point right => new(1, 0);
}