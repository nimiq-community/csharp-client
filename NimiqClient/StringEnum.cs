namespace Nimiq
{
    /// <summary>Abstract base class used in string enumerations</summary>
    public abstract class StringEnum
    {
        public string Value { get; set; }

        public StringEnum(string value) { Value = value; }

        public static implicit operator string(StringEnum obj)
        {
            return obj.Value;
        }

        public override string ToString()
        {
            return Value;
        }

        public static bool operator ==(StringEnum a, object b)
        {
            var other = b as StringEnum;
            if (other is null)
            {
                return false;
            }
            return a.Value == other.Value;
        }

        public static bool operator !=(StringEnum a, object b)
        {
            var other = b as StringEnum;
            if (other is null)
            {
                return true;
            }
            return a.Value != other.Value;
        }

        public override bool Equals(object obj)
        {
            var other = obj as StringEnum;
            if (other == null)
            {
                return false;
            }
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
