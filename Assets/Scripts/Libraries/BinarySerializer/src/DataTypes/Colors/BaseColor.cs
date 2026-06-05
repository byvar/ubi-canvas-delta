using System;

namespace BinarySerializer
{
    public abstract class BaseColor : BinarySerializable, IEquatable<BaseColor>, ISerializerShortLog
    {
        #region Constructors

        protected BaseColor()
        {
            Alpha = 1f;
        }
        protected BaseColor(float r, float g, float b, float a = 1f) 
        {
            Red = r;
            Green = g;
            Blue = b;
            Alpha = a;
        }

        #endregion

        #region Public Properties

        public virtual float Red { get; set; }
        public virtual float Green { get; set; }
        public virtual float Blue { get; set; }
        public virtual float Alpha { get; set; }

        #endregion

        #region Public Static Properties

        public static BaseColor Clear => new CustomColor(0, 0, 0, 0);
        public static BaseColor Black => new CustomColor(0, 0, 0, 1);
        public static BaseColor White => new CustomColor(1, 1, 1, 1);

        #endregion

        #region Equality

        public bool Equals(BaseColor other)
        {
            if (ReferenceEquals(null, other)) 
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Alpha == other.Alpha && Red == other.Red && Green == other.Green && Blue == other.Blue;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;
            if (ReferenceEquals(this, obj)) 
                return true;
            if (obj.GetType() != GetType()) 
                return false;

            return Equals((BaseColor)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Alpha.GetHashCode();
                hashCode = (hashCode * 397) ^ Red.GetHashCode();
                hashCode = (hashCode * 397) ^ Green.GetHashCode();
                hashCode = (hashCode * 397) ^ Blue.GetHashCode();
                return hashCode;
            }
        }

        #endregion

        #region Serializable

        public string ShortLog => ToString();

        #endregion

        #region Public Methods

        public override string ToString() => $"RGBA({(int)(Red * 255)}, {(int)(Green * 255)}, {(int)(Blue * 255)}, {Alpha})";

        #endregion
    }
}