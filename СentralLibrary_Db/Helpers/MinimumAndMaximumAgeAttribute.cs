namespace СentralLibrary_Db.Helpers
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Minimum And Maximum Age Attribute.
    /// </summary>
    public class MinimumAndMaximumAgeAttribute : ValidationAttribute
    {
        readonly int _minimumAge;
        readonly int _maximumAge;

        /// <summary>
        /// Initializes a new instance of the <see cref="MinimumAndMaximumAgeAttribute"/> class.
        /// </summary>
        /// <param name="minimumAge">Records a minimum age.</param>
        /// <param name="maximumAge">Records a maximum age.</param>
        public MinimumAndMaximumAgeAttribute(int minimumAge, int maximumAge)
        {
            this._minimumAge = minimumAge;
            this._maximumAge = maximumAge;
        }

        /// <inheritdoc/>
        public override bool IsValid(object value)
        {
            DateTime date;
            return DateTime.TryParse(value.ToString(), out date)
                ? date.AddYears(this._minimumAge) < DateTime.Now && date.AddYears(this._maximumAge) > DateTime.Now
                : false;
        }
    }
}