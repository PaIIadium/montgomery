namespace ModularExponentiation
{
    public static class OverflowCalculator
    {
        public static bool CalculateAdditionOverflow(bool bit1, bool bit2, bool prevOverflow)
        {
            if (bit1)
            {
                return bit2 || prevOverflow;
            }

            return bit2 && prevOverflow;
        }

        public static bool CalculateSubtractionOverflow(bool bit1, bool bit2, bool prevOverflow)
        {
            if (bit1)
            {
                return bit2 && prevOverflow;
            }

            return bit2 || prevOverflow;
        }
    }
}