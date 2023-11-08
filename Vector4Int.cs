// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable NotAccessedField.Global
// ReSharper disable UnusedType.Global

using System;
using UnityEngine;

public struct Vector4Int
{
    public int x;
    public int y;
    public int z;
    public int w;

    public Vector4Int(int x, int y, int z, int w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public Vector4Int(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = 0;
    }

    public Vector4Int(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.z = 0;
        this.w = 0;
    }
    public override bool Equals(object other) => other is Vector4Int other1 && Equals(other1);
    public bool Equals(Vector4Int other) => x == other.x && y == other.y && z == other.z && w == other.w;

    public static bool operator ==(Vector4Int lhs, Vector4Int rhs) => lhs.Equals(rhs);
    public static bool operator !=(Vector4Int lhs, Vector4Int rhs) => !lhs.Equals(rhs);

    public static Vector4Int operator +(Vector4Int a, Vector4Int b) => new(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
    public static Vector4Int operator -(Vector4Int a, Vector4Int b) => new(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
    public static Vector4Int operator *(Vector4Int a, Vector4Int b) => new(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
    public static Vector4Int operator /(Vector4Int a, Vector4Int b) => new(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
    public static Vector4Int operator -(Vector4Int a) => new(-a.x, -a.y, -a.z, -a.w);
    public static Vector4Int operator *(Vector4Int a, int b) => new(a.x * b, a.y * b, a.z * b, a.w * b);
    public static Vector4Int operator *(int a, Vector4Int b) => new(a * b.x, a * b.y, a * b.z, a * b.w);
    public static Vector4Int operator /(Vector4Int a, int b) => new(a.x / b, a.y / b, a.z / b, a.w / b);
    public static Vector4Int operator /(int a, Vector4Int b) => new(a / b.x, a / b.y, a / b.z, a / b.w);

    public override int GetHashCode() => HashCode.Combine(x, y, z, w);

    public static implicit operator Vector4Int(Vector3Int v) => new (v.x, v.y, v.z, 0);
    public static implicit operator Vector3Int(Vector4Int v) => new (v.x, v.y, v.z);
    public static implicit operator Vector4Int(Vector2Int v) => new (v.x, v.y, 0, 0);
    public static implicit operator Vector2Int(Vector4Int v) => new (v.x, v.y);
    public static implicit operator Vector4Int(Vector3 v) => new ((int)v.x, (int)v.y, (int)v.z, 0);
    public static implicit operator Vector3(Vector4Int v) => new (v.x, v.y, v.z);
    public static implicit operator Vector4Int(Vector2 v) => new ((int)v.x, (int)v.y, 0, 0);
    public static implicit operator Vector2(Vector4Int v) => new (v.x, v.y);

    public static explicit operator Vector4(Vector4Int v) => new (v.x, v.y, v.z, v.w);
    public static explicit operator Vector4Int(Vector4 v) => new((int)v.x, (int)v.y, (int)v.z, (int)v.w);

    public static Vector4Int zero => new (0, 0, 0, 0);
    public static Vector4Int one => new (1, 1, 1, 1);

}