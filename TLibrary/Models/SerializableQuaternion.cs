using System;
using UnityEngine;

namespace Tavstal.TLibrary.Models
{
    /// <summary>
    /// Quaternion that can be serialized.
    /// </summary>
    [Serializable]
    public class SerializableQuaternion
    {
        /// <summary>
        /// The X component of the quaternion.
        /// </summary>
        public float X { get; set; }
        /// <summary>
        /// The Y component of the quaternion.
        /// </summary>
        public float Y { get; set; }
        /// <summary>
        /// The Z component of the quaternion.
        /// </summary>
        public float Z { get; set; }
        /// <summary>
        /// The W component of the quaternion.
        /// </summary>
        public float W { get; set; }

        /// <summary>
        /// Creates a new quaternion with all values set to 0.
        /// </summary>
        public SerializableQuaternion() { }

        /// <summary>
        /// Creates a new quaternion with the specified values.
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        /// <param name="z">The Z component.</param>
        /// <param name="w">The W component.</param>
        public SerializableQuaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Creates a new quaternion from a Unity <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="quaternion">The Unity quaternion to copy values from.</param>
        public SerializableQuaternion(Quaternion quaternion)
        {
            X = quaternion.x;
            Y = quaternion.y;
            Z = quaternion.z;
            W = quaternion.w;
        }
        
        /// <summary>
        /// Creates a new quaternion by parsing a string in the format "X;Y;Z;W".
        /// </summary>
        /// <param name="rawValue">The string to parse (e.g. "0;0;0;1").</param>
        public SerializableQuaternion(string rawValue) {
            string[] parts = rawValue.Split(';');
            X = float.Parse(parts[0]);
            Y = float.Parse(parts[1]);
            Z = float.Parse(parts[2]);
            W = float.Parse(parts[3]);
        }

        /// <summary>
        /// Checks if this quaternion equals another object.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns><see langword="true"/> if the values are approximately equal.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is SerializableQuaternion other && Mathf.Approximately(X, other.X) && Mathf.Approximately(Y, other.Y) && Mathf.Approximately(Z, other.Z) && Mathf.Approximately(W, other.W))
                return true;

            return obj is Quaternion otherQ && Mathf.Approximately(X, otherQ.x) && Mathf.Approximately(Y, otherQ.y) && Mathf.Approximately(Z, otherQ.z) && Mathf.Approximately(W, otherQ.w);
        }

        /// <summary>
        /// Converts this serializable quaternion to a Unity <see cref="Quaternion"/>.
        /// </summary>
        /// <returns>A Unity <see cref="Quaternion"/> with the same values.</returns>
        public Quaternion GetQuaternion() => new Quaternion(X, Y, Z, W);

        /// <summary>
        /// Returns the quaternion as a string in "X;Y;Z;W" format.
        /// </summary>
        /// <returns>A string like "0;0;0;1".</returns>
        public override string ToString()
        {
            return $"{X};{Y};{Z};{W}";
        }

        /// <summary>
        /// Returns the hash code for this quaternion.
        /// </summary>
        /// <returns>An integer hash code.</returns>
        public override int GetHashCode()
        {
            // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
        }
        
        /// <summary>
        /// Converts a Unity <see cref="Quaternion"/> to its string representation.
        /// </summary>
        /// <param name="q">The Unity quaternion to serialize.</param>
        /// <returns>A string in "X;Y;Z;W" format.</returns>
        public static string Serialize(Quaternion q)
        {
            return $"{q.x};{q.y};{q.z};{q.w}";
        }
    }
}