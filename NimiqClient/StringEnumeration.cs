namespace Nimiq
{
    /// <summary>Abstract base class used in string enumerations.</summary>
    public abstract class StringEnumeration
    {
        /// <summary>Associated value.</summary>
        public string Value { get; set; }

        /// <summary>Constructor.</summary>
        /// <param name="value">The associated value.</param>
        public StringEnumeration(string value) { Value = value; }

        /// <summary>Implicit conversion to string.</summary>
        /// <param name="obj">And StringEnumeration object.</param>
        /// <returns>string object.</returns>
        public static implicit operator string(StringEnumeration obj)
        {
            return obj.Value;
        }

        /// <summary>Get the string associated value.</summary>
        /// <returns>string object.</returns>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>Test whether a StringEnumeration is equal to another object.</summary>
        /// <param name="a">StringEnumeration object.</param>
        /// <param name="b">Another object.</param>
        /// <returns>true if the two objects are equal.</returns>
        public static bool operator ==(StringEnumeration a, object b)
        {
            var other = b as StringEnumeration;
            if (other is null)
            {
                return false;
            }
            return a.Value == other.Value;
        }

        /// <summary>Test whether a StringEnumeration is different to other object.</summary>
        /// <param name="a">StringEnumeration object.</param>
        /// <param name="b">Another object.</param>
        /// <returns>true if the two objects are different.</returns>
        public static bool operator !=(StringEnumeration a, object b)
        {
            var other = b as StringEnumeration;
            if (other is null)
            {
                return true;
            }
            return a.Value != other.Value;
        }

        /// <summary>Test whether a StringEnumeration is equal to another object.</summary>
        /// <param name="obj">Another object.</param>
        /// <returns>true if the objects are equal.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as StringEnumeration;
            if (other == null)
            {
                return false;
            }
            return Value == other.Value;
        }

        /// <summary>Get the hash code of the associated value.</summary>
        /// <returns>An integer value representing the hash of the associated value.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
