using System;

namespace Kremetart_Ridge_WinForm_Project
{
    /// <summary>
    /// Represents a single animal record in the Baobab Ridge Wildlife Rehabilitation Centre.
    /// Animal ID is immutable once the object is created (cannot be changed after the animal is saved).
    /// </summary>
    public class Animal
    {
        /// <summary>Immutable unique identifier in the format WR-####.</summary>
        public string AnimalId { get; }

        /// <summary>Common name of the animal (1-30 characters, no pipe character).</summary>
        public string Name { get; set; }

        /// <summary>Species name (1-30 characters, no pipe character).</summary>
        public string Species { get; set; }

        /// <summary>Age in whole years (0-100).</summary>
        public int Age { get; set; }

        /// <summary>Recovery Score assigned by the vet (0-100).</summary>
        public int RecoveryScore { get; set; }

        /// <summary>Calculated status (Critical, Serious, Stable, Recovering, Release-Ready).</summary>
        public string Status { get; set; }

        /// <summary>Calculated housing unit based on Recovery Score.</summary>
        public string HousingUnit { get; set; }

        /// <summary>
        /// Creates a new Animal ready for classification.
        /// ID is normalised to upper case and trimmed.
        /// </summary>
        public Animal(string animalId, string name, string species, int age, int recoveryScore)
        {
            AnimalId = animalId?.Trim().ToUpperInvariant() ?? string.Empty;
            Name = name?.Trim() ?? string.Empty;
            Species = species?.Trim() ?? string.Empty;
            Age = age;
            RecoveryScore = recoveryScore;
            Status = string.Empty;
            HousingUnit = string.Empty;
        }

        /// <summary>
        /// Creates an Animal from an existing file record (all seven fields known).
        /// </summary>
        public Animal(string animalId, string name, string species, int age, int recoveryScore,
                      string status, string housingUnit)
        {
            AnimalId = animalId ?? string.Empty;
            Name = name ?? string.Empty;
            Species = species ?? string.Empty;
            Age = age;
            RecoveryScore = recoveryScore;
            Status = status ?? string.Empty;
            HousingUnit = housingUnit ?? string.Empty;
        }

        /// <summary>
        /// Returns the pipe-delimited line exactly as required by animals.txt.
        /// Format: ID|Name|Species|Age|Score|Status|Housing Unit
        /// </summary>
        public string ToFileLine()
        {
            return $"{AnimalId}|{Name}|{Species}|{Age}|{RecoveryScore}|{Status}|{HousingUnit}";
        }
    }
}
