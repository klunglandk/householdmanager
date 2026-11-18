using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HouseholdManager.Database
{
    public class UtcDateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
    {
        public UtcDateTimeOffsetConverter()
            : base(value => value.ToUniversalTime(), value => value)
        {
        }
    }
}
