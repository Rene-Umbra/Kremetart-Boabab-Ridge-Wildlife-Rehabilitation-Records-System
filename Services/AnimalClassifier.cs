namespace Kremetart_Ridge_WinForm_Project
{
    public static class AnimalClassifier
    {
        public static (string Status, string Housing) Classify(int score)
        {
            if (score <= 19)
            {
                return ("Critical", "ICU");
            }

            if (score <= 39)
            {
                return ("Serious", "High-Dependency Ward");
            }

            if (score <= 59)
            {
                return ("Stable", "Recovery Ward");
            }

            if (score <= 79)
            {
                return ("Recovering", "Outdoor Enclosure");
            }

            return ("Release-Ready", "Pre-Release Camp");
        }
    }
}
