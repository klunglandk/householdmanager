using HouseholdManager.Database.DataModels;

namespace HouseholdManager.Services
{
    public static class MeasurementConverter
    {
        private const decimal LiterThresholdInMilliliters = 500m;
        private const decimal KilogramThresholdInGrams = 1000m;
        private const int Decimals = 3;

        private enum Family { None, Weight, Volume }

        private static Family GetFamily(MeasurementUnit unit) => unit switch
        {
            MeasurementUnit.Gram or MeasurementUnit.Kilogram => Family.Weight,
            MeasurementUnit.Milliliter or MeasurementUnit.Deciliter or MeasurementUnit.Liter => Family.Volume,
            _ => Family.None
        };

        private static decimal GetBaseUnitFactor(MeasurementUnit unit) => unit switch
        {
            MeasurementUnit.Kilogram => 1000m,
            MeasurementUnit.Deciliter => 100m,
            MeasurementUnit.Liter => 1000m,
            _ => 1m
        };

        public static bool CanCombine(MeasurementUnit first, MeasurementUnit second)
        {
            if (first == second) return true;
            var family = GetFamily(first);
            return family != Family.None && family == GetFamily(second);
        }

        public static (decimal Amount, MeasurementUnit Unit) Add(decimal firstAmount, MeasurementUnit firstUnit, decimal secondAmount, MeasurementUnit secondUnit)
        {
            if (!CanCombine(firstUnit, secondUnit))
            {
                throw new ArgumentException($"Cannot combine {firstUnit} and {secondUnit}.");
            }

            if (GetFamily(firstUnit) == Family.None)
            {
                return (RoundAndTrim(firstAmount + secondAmount), firstUnit);
            }

            var baseUnit = GetFamily(firstUnit) == Family.Weight ? MeasurementUnit.Gram : MeasurementUnit.Milliliter;
            var total = firstAmount * GetBaseUnitFactor(firstUnit) + secondAmount * GetBaseUnitFactor(secondUnit);
            return Normalize(total, baseUnit);
        }

        public static (decimal Amount, MeasurementUnit Unit) Normalize(decimal amount, MeasurementUnit unit)
        {
            switch (GetFamily(unit))
            {
                case Family.Weight:
                    var grams = Round(amount * GetBaseUnitFactor(unit));
                    return grams >= KilogramThresholdInGrams
                        ? (RoundAndTrim(grams / 1000m), MeasurementUnit.Kilogram)
                        : (RoundAndTrim(grams), MeasurementUnit.Gram);

                case Family.Volume:
                    var milliliters = Round(amount * GetBaseUnitFactor(unit));
                    return milliliters >= LiterThresholdInMilliliters
                        ? (RoundAndTrim(milliliters / 1000m), MeasurementUnit.Liter)
                        : (RoundAndTrim(milliliters), MeasurementUnit.Milliliter);

                default:
                    return (RoundAndTrim(amount), unit);
            }
        }

        private static decimal Round(decimal value) => decimal.Round(value, Decimals, MidpointRounding.AwayFromZero);

        private static decimal RoundAndTrim(decimal value) => Round(value) / 1.000000000000000000000000000000000m;
    }
}
