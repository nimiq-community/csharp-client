using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nimiq
{
    /// <summary>JsonConverter used in string enumeration serialization.</summary>
    public class StringEnumerationConverter : JsonConverter<StringEnumeration>
    {
        /// <summary>Whether a type is a subclass of <c>StringEnumeration</c>.</summary>
        /// <param name="typeToConvert">Type to check.</param>
        /// <returns>True if is a subclass.</returns>
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(StringEnumeration).IsAssignableFrom(typeToConvert);
        }

        /// <summary>Read the string value.</summary>
        /// <param name="reader">Reader to access the encoded JSON text.</param>
        /// <param name="typeToConvert">Type of the object to deserialize.</param>
        /// <param name="options">Options for the deserialization.</param>
        /// <returns>Underlying string enumeration type.</returns>
        public override StringEnumeration Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return (StringEnumeration)Activator.CreateInstance(typeToConvert, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { reader.GetString() }, null, null);
        }

        /// <summary>Write the string value.</summary>
        /// <param name="writer">Writer to encode the JSON text.</param>
        /// <param name="value">Object to serialize.</param>
        /// <param name="options">Options for the serialization.</param>
        public override void Write(Utf8JsonWriter writer, StringEnumeration value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }

    /// <summary>Abstract base class used in string enumerations.</summary>
    public abstract class StringEnumeration
    {
        /// <summary>Associated value.</summary>
        public string Value { get; set; }

        /// <summary>Initializes the enumeration from a string.</summary>
        /// <param name="value">The associated value.</param>
        public StringEnumeration(string value) { Value = value; }

        /// <summary>Implicit conversion to string.</summary>
        /// <param name="obj">And StringEnumeration object.</param>
        /// <returns>string object.</returns>
        public static implicit operator string(StringEnumeration obj)
        {
            if (obj is null)
            {
                return null;
            }
            return obj.ToString();
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
            if (a is null)
            {
                return b is null;
            }
            return !(b is null) && a.Value == b.ToString();
        }

        /// <summary>Test whether a StringEnumeration is different to other object.</summary>
        /// <param name="a">StringEnumeration object.</param>
        /// <param name="b">Another object.</param>
        /// <returns>true if the two objects are different.</returns>
        public static bool operator !=(StringEnumeration a, object b)
        {
            return !(a == b);
        }

        /// <summary>Test whether a StringEnumeration is equal to another object.</summary>
        /// <param name="obj">Another object.</param>
        /// <returns>true if the objects are equal.</returns>
        public override bool Equals(object obj)
        {
            return this == obj;
        }

        /// <summary>Get the hash code of the associated value.</summary>
        /// <returns>An integer value representing the hash of the associated value.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
