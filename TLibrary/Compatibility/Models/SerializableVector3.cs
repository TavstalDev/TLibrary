using System;
using UnityEngine;

namespace Tavstal.TLibrary.Compatibility
{
    /// <summary>
    /// Vector3 that can be serialized.
    /// </summary>
    [Serializable]
    public class SerializableVector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public SerializableVector3() { }

        public SerializableVector3(Vector3 p)
        {
            X = p.x;
            Y = p.y;
            Z = p.z;
        }

        public SerializableVector3(float x, float y, float z) {
            X = x;
            Y = y;
            Z = z;
        }

        public SerializableVector3(string rawValue) {
            string[] parts = rawValue.Split(';');
            X = float.Parse(parts[0]);
            Y = float.Parse(parts[1]);
            Z = float.Parse(parts[2]);
        }

        public override bool Equals(object obj)
        {
            if (obj is SerializableVector3 other && X == other.X && Y == other.Y && Z == other.Z)
                return true;

            if (obj is Vector3 other3 && X == other3.x && Y == other3.y && Z == other3.z)
                return true;

            return false;
        }

        public Vector3 GetVector3() => new Vector3(X, Y, Z);

        public override string ToString()
        {
            return $"{X};{Y};{Z}";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
