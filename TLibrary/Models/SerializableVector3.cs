using System;
using UnityEngine;

namespace Tavstal.TLibrary.Models
{
    /// <summary>
    /// Vector3 that can be serialized.
    /// </summary>
    [Serializable]
    public class SerializableVector3
    {
        /// <summary>
        /// The X coordinate of the vector.
        /// </summary>
        public float X { get; set; }
        /// <summary>
        /// The Y coordinate of the vector.
        /// </summary>
        public float Y { get; set; }
        /// <summary>
        /// The Z coordinate of the vector.
        /// </summary>
        public float Z { get; set; }

        /// <summary>
        /// Creates a new vector with all values set to 0.
        /// </summary>
        public SerializableVector3() { }

        /// <summary>
        /// Creates a new vector from a Unity <see cref="Vector3"/>.
        /// </summary>
        /// <param name="p">The Unity vector to copy values from.</param>
        public SerializableVector3(Vector3 p)
        {
            X = p.x;
            Y = p.y;
            Z = p.z;
        }

        /// <summary>
        /// Creates a new vector with the specified coordinates.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        public SerializableVector3(float x, float y, float z) {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Creates a new vector by parsing a string in the format "X;Y;Z".
        /// </summary>
        /// <param name="rawValue">The string to parse (e.g. "1.5;2.0;3.2").</param>
        public SerializableVector3(string rawValue) {
            string[] parts = rawValue.Split(';');
            X = float.Parse(parts[0]);
            Y = float.Parse(parts[1]);
            Z = float.Parse(parts[2]);
        }

        /// <summary>
        /// Checks if this vector equals another object.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns><see langword="true"/> if the values are approximately equal.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is SerializableVector3 other && Mathf.Approximately(X, other.X) && Mathf.Approximately(Y, other.Y) && Mathf.Approximately(Z, other.Z))
                return true;

            return obj is Vector3 other3 && Mathf.Approximately(X, other3.x) && Mathf.Approximately(Y, other3.y) && Mathf.Approximately(Z, other3.z);
        }

        /// <summary>
        /// Converts this serializable vector to a Unity <see cref="Vector3"/>.
        /// </summary>
        /// <returns>A Unity <see cref="Vector3"/> with the same coordinates.</returns>
        public Vector3 GetVector3() => new Vector3(X, Y, Z);

        /// <summary>
        /// Returns the vector as a string in "X;Y;Z" format.
        /// </summary>
        /// <returns>A string like "1.5;2.0;3.2".</returns>
        public override string ToString()
        {
            return $"{X};{Y};{Z}";
        }

        /// <summary>
        /// Returns the hash code for this vector.
        /// </summary>
        /// <returns>An integer hash code.</returns>
        public override int GetHashCode()
        {
            // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
        }
        
        /// <summary>
        /// Converts a Unity <see cref="Vector3"/> to its string representation.
        /// </summary>
        /// <param name="v">The Unity vector to serialize.</param>
        /// <returns>A string in "X;Y;Z" format.</returns>
        public static string Serialize(Vector3 v)
        {
            return $"{v.x};{v.y};{v.z}";
        }
    }
}
