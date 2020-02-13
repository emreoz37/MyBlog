using System;

namespace Services.Helpers
{
    public class DateTimeHelper : IDateTimeHelper
    {
        #region Fields

        #endregion

        #region Ctor

        #endregion

        #region Methods
        /// <summary>
        /// Converts the date and time to current user date and time
        /// </summary>
        /// <param name="dt">The date and time (represents local system time or UTC time) to convert.</param>
        /// <param name="sourceDateTimeKind">The source datetimekind</param>
        /// <returns>A DateTime value that represents time that corresponds to the dateTime parameter in customer time zone.</returns>
        public virtual DateTime ConvertToUserTime(DateTime dt, DateTimeKind sourceDateTimeKind)
        {
            dt = DateTime.SpecifyKind(dt, sourceDateTimeKind);
            if (sourceDateTimeKind == DateTimeKind.Local && TimeZoneInfo.Local.IsInvalidTime(dt))
                return dt;

            var currentUserTimeZoneInfo = TimeZoneInfo.Local; //TODO : Set Customer time zone info
            return TimeZoneInfo.ConvertTime(dt, currentUserTimeZoneInfo);
        }
        #endregion

    }
}
