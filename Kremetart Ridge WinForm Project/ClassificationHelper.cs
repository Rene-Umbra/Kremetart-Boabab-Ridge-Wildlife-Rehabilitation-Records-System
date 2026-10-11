using System;

namespace Kremetart_Ridge_WinForm_Project
{
    /// <summary>
    /// Contains the single shared classification method used by both Add and Update.
    /// Status and Housing Unit strings must match the brief exactly.
    /// </summary>
    public static class ClassificationHelper
    {
        /// <summary>
        /// Calculates Status and Housing Unit from a Recovery Score (0-100).
        /// This is the SINGLE source of truth – called by both Add and Update features.
        /// Boundaries are inclusive on both ends of each band (no gaps).
        /// </summary>
        /// <param name="recoveryScore">Whole number between 0 and 100 inclusive.</param>
        /// <returns>Tuple of (Status, HousingUnit) with the exact strings required by the brief.</returns>
        public static (string Status, string HousingUnit) Classify(int recoveryScore)
        {
            // Tested boundaries: 0, 19, 20, 39, 40, 59, 60, 79, 80, 100
            if (recoveryScore >= 0 && recoveryScore <= 19)
                return ("Critical", "Intensive Care Unit");

            if (recoveryScore >= 20 && recoveryScore <= 39)
                return ("Serious", "High-Dependency Ward");

            if (recoveryScore >= 40 && recoveryScore <= 59)
                return ("Stable", "Recovery Ward");

            if (recoveryScore >= 60 && recoveryScore <= 79)
                return ("Recovering", "Outdoor Enclosure");

            if (recoveryScore >= 80 && recoveryScore <= 100)
                return ("Release-Ready", "Pre-Release Camp");

            // Defensive – validation should prevent this path
            throw new ArgumentOutOfRangeException(nameof(recoveryScore),
                "Recovery Score must be a whole number between 0 and 100 inclusive.");
        }
    }
}
