using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace UniversalWPF
{
    /// <summary>
    /// An interface that returns a string representation of a provided value, using distinct format methods to format several data types.
    /// </summary>
    public interface INumberFormatter2
    {
        /// <summary>
        /// Returns a string representation of a Double value.
        /// </summary>
        /// <param name="value">The Double value to be formatted.</param>
        /// <returns>A string that represents the value.</returns>
        string FormatDouble(Double value);

        /// <summary>
        /// Returns a string representation of an Int64 value.
        /// </summary>
        /// <param name="value">The Int64 value to be formatted.</param>
        /// <returns>A string that represents the value.</returns>
        string FormatInt(Int64 value);

        /// <summary>
        /// Returns a string representation of a UInt64 value.
        /// </summary>
        /// <param name="value">The UInt64 value to be formatted.</param>
        /// <returns>A string that represents the value.</returns>
        string FormatUInt(UInt64 value);
    }

    /*
    public interface INumberRounder
    {
        double RoundDouble(Double value);
        int RoundInt32(Int32 value);
        long RoundInt64(Int64 value);
        float RoundSingle(Single value);
        uint RoundUInt32(UInt32 value);
        ulong RoundUInt64(UInt64 value);
    }*/

    /// <summary>
    /// An interface that parses a string representation of a numeric value.
    /// </summary>
    public interface INumberParser
    {
        /// <summary>
        /// Attempts to parse a string representation of a Double numeric value.
        /// </summary>
        /// <param name="text">The text to be parsed.</param>
        /// <returns>If successful, a Double that corresponds to the string representation, and otherwise null.</returns>
        double? ParseDouble(string text);

        /// <summary>
        /// Attempts to parse a string representation of an integer numeric value.
        /// </summary>
        /// <param name="text">The text to be parsed.</param>
        /// <returns>If successful, an Int64 that corresponds to the string representation, and otherwise null.</returns>
        long? ParseInt(String text);

        /// <summary>
        /// Attempts to parse a string representation of an unsigned integer numeric value.
        /// </summary>
        /// <param name="text">The text to be parsed.</param>
        /// <returns>If successful, a UInt64 that corresponds to the string representation, and otherwise null.</returns>
        ulong? ParseUInt(string text);
    }

    /// <summary>
    /// Formats and parses decimal numbers.
    /// </summary>
    public class DecimalFormatter : INumberFormatter2, INumberParser
    {
        CultureInfo culture;
        /// <summary>
        /// Creates a DecimalFormatter object and initializes it to default values.
        /// </summary>
        public DecimalFormatter() : this(null)
        {
        }

        /// <summary>
        /// Creates a <see cref="DecimalFormatter"/> object initialized by a language list and a geographic region.
        /// </summary>
        /// <param name="culture">The language culture to use for formatting</param>
        public DecimalFormatter(CultureInfo culture)
        {
            this.culture = culture ?? CultureInfo.CurrentCulture;
        }

        /// <inheritdoc />
        public string FormatDouble(double value)
        {
            string format = "0";
            // IntegerDigits;  the minimum number of digits to display for the integer part of the number.
            // IsGrouped;  whether the integer part of the number should be grouped.
            // IsZeroSigned;  whether -0 is formatted as "-0" or "0".
            // SignificantDigits;  the current padding to significant digits when a decimal number is formatted.
            // IsDecimalPointAlwaysDisplayed whether the decimal point of the number should always be displayed.
            // FractionDigits; Gets or sets the minimum number of digits to display for the fraction part of the number.
            if (IntegerDigits > 0)
                format = IsGrouped ? "0,0" : "0";
            if(FractionDigits > 0)
            {
                format += ".";
                for (int i = 0; i < FractionDigits; i++)
                {
                    format += "0";
                }
            }
            else
            {
                format += ".#";
            }
            if (SignificantDigits > 0)
                value = Math.Round(value, SignificantDigits);
            return value.ToString(format, culture);
        }

        /// <inheritdoc />
        public string FormatInt(long value)
        {
            return string.Format(culture, "{0}", value);
        }

        /// <inheritdoc />
        public string FormatUInt(ulong value)
        {
            return string.Format(culture, "{0}", value);
        }

        /// <inheritdoc />
        public double? ParseDouble(string text) => double.TryParse(text, NumberStyles.Any, culture, out double result) ? (double?)result : null;

        /// <inheritdoc />
        public long? ParseInt(string text) => long.TryParse(text, NumberStyles.Any, culture, out long result) ? (long?)result : null;

        /// <inheritdoc />
        public ulong? ParseUInt(string text) => ulong.TryParse(text, NumberStyles.Any, culture, out ulong result) ? (ulong?)result : null;

        /// <summary>
        /// Gets or sets the minimum number of digits to display for the fraction part of the number.
        /// </summary>
        /// <value>The minimum number of digits to display.</value>
        public int FractionDigits { get; set; } = 2;

        /// <summary>
        /// Gets or sets the minimum number of digits to display for the integer part of the number.
        /// </summary>
        /// <value>The minimum number of digits to display.</value>
        public int IntegerDigits { get; set; } = 1;

        /// <summary>
        /// Gets the region that is used when formatting and parsing decimal numbers.
        /// </summary>
        /// <value>The region that is used.</value>
        public string GeographicRegion { get; }

        /// <summary>
        /// Gets or sets whether the decimal point of the number should always be displayed.
        /// </summary>
        /// <value>True if the decimal point of the number should always be displayed, and false otherwise.</value>
        public bool IsDecimalPointAlwaysDisplayed { get; set; }

        /// <summary>
        /// Gets or sets whether the integer part of the number should be grouped.
        /// </summary>
        /// <value>True if the integer part of the number should be grouped, and false otherwise.</value>
        public bool IsGrouped { get; set; }

        /// <summary>
        /// Gets or sets whether -0 is formatted as "-0" or "0".
        /// </summary>
        /// <value>True if -0 is formatted as "-0", and false if -0 is formatted as "0".</value>
        /// <remarks>
        /// <para>You can set this property to specify that DecimalFormatter display negative 0 as "-0". This enables the scenario where you wish to display "-0" when it represents the rounded value of some small negative value (such as -0.00001).</para>
        /// <para>This property defaults to false to be consistent with Windows 8 and Windows Server 2012, in which -0 was always formatted as "0".</para>
        /// </remarks>
        public bool IsZeroSigned { get; set; }

        /// <summary>
        /// Gets the priority list of language identifiers that is used when formatting and parsing decimal numbers.
        /// </summary>
        /// <value>The priority list of language identifiers.</value>
        public IReadOnlyList<string> Languages { get; }

        /*
        /// <summary>
        /// Gets or sets the current rounding strategy to be used when formatting numbers.
        /// </summary>
        public INumberRounder NumberRounder { get; set; }
        */

        /// <summary>
        /// Gets or sets the current padding to significant digits when a decimal number is formatted.
        /// </summary>
        /// <value>The number of significant digits.</value>
        /// <remarks>Trailing zeros are added to the format until the given number of significant digits is exhausted. If there are more digits, this property does not cause them to be truncated.</remarks>
        public int SignificantDigits { get; set; } = 0;

        /// <summary>
        /// Gets the language that was most recently used to format or parse decimal values.
        /// </summary>
        /// <value>The language from the priority list of language identifiers that was most recently used to format or parse decimal values.</value>
        public string ResolvedLanguage => culture.Name;
    }
}
