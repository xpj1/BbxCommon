using UnityEngine;

namespace BbxCommon
{
    public static class UnityContainerExtend
    {
        #region Vector
        public static Vector3 SetX(this Vector3 vector, float value)
        {
            return new Vector3(value, vector.y, vector.z);
        }

        public static Vector3 SetY(this Vector3 vector, float value)
        {
            return new Vector3(vector.x, value, vector.z);
        }

        public static Vector3 SetZ(this Vector3 vector, float value)
        {
            return new Vector3(vector.x, vector.y, value);
        }

        public static Vector3 SetXY(this Vector3 vector, float x, float y)
        {
            return new Vector3(x, y, vector.z);
        }

        public static Vector3 SetXZ(this Vector3 vector, float x, float z)
        {
            return new Vector3(x, vector.y, z);
        }

        public static Vector3 SetYZ(this Vector3 vector, float y, float z)
        {
            return new Vector3(vector.x, y, z);
        }
        #endregion

        #region Color
        public static Color SetR(this Color color, float value)
        {
            return new Color(value, color.g, color.b, color.a);
        }

        public static Color SetG(this Color color, float value)
        {
            return new Color(color.r, value, color.b, color.a);
        }

        public static Color SetB(this Color color, float value)
        {
            return new Color(color.r, color.g, value, color.a);
        }

        public static Color SetA(this Color color, float value)
        {
            return new Color(color.r, color.g, color.b, value);
        }
        #endregion

        #region Quaternion
        public static Quaternion SetX(this Quaternion quaternion, float value)
        {
            return new Quaternion(value, quaternion.y, quaternion.z, quaternion.w);
        }

        public static Quaternion SetY(this Quaternion quaternion, float value)
        {
            return new Quaternion(quaternion.x, value, quaternion.z, quaternion.w);
        }

        public static Quaternion SetZ(this Quaternion quaternion, float value)
        {
            return new Quaternion(quaternion.x, quaternion.y, value, quaternion.w);
        }

        public static Quaternion SetW(this Quaternion quaternion, float value)
        {
            return new Quaternion(quaternion.x, quaternion.y, quaternion.z, value);
        }
        #endregion
    }
}
