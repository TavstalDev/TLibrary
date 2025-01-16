using System;
using UnityEngine;

namespace Tavstal.TLibrary.Models
{
    [Serializable]
    public class SerializableQuaternion
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public SerializableQuaternion() { }

        public SerializableQuaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public SerializableQuaternion(Quaternion quaternion)
        {
            X = quaternion.x;
            Y = quaternion.y;
            Z = quaternion.z;
            W = quaternion.w;
        }
        
        public SerializableQuaternion(string rawValue) {
            string[] parts = rawValue.Split(';');
            X = float.Parse(parts[0]);
            Y = float.Parse(parts[1]);
            Z = float.Parse(parts[2]);
            W = float.Parse(parts[3]);
        }

        public override bool Equals(object obj)
        {
            if (obj is SerializableQuaternion other && Mathf.Approximately(X, other.X) && Mathf.Approximately(Y, other.Y) && Mathf.Approximately(Z, other.Z) && Mathf.Approximately(W, other.W))
                return true;

            return obj is Quaternion otherQ && Mathf.Approximately(X, otherQ.x) && Mathf.Approximately(Y, otherQ.y) && Mathf.Approximately(Z, otherQ.z) && Mathf.Approximately(W, otherQ.w);
        }

        public Quaternion GetQuaternion() => new Quaternion(X, Y, Z, W);

        public override string ToString()
        {
            return $"{X};{Y};{Z};{W}";
        }

        public override int GetHashCode()
        {
            // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
        }
        
        public static string Serialize(Quaternion q)
        {
            return $"{q.x};{q.y};{q.z};{q.w}";
        }
    }
}