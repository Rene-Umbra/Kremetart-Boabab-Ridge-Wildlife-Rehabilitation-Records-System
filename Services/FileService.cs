using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Kremetart_Ridge_WinForm_Project
{
    public class FileService
    {
        private const string Header = "AnimalID|Name|Species|Age|RecoveryScore|Status|HousingUnit";
        private readonly string filePath;

        public FileService()
            : this(GetDefaultFilePath())
        {
        }

        public FileService(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("A file path is required.", nameof(filePath));
            }

            this.filePath = filePath;
        }

        public List<Animal> LoadAnimals()
        {
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            if (lines.Length == 0 || !string.Equals(lines[0], Header, StringComparison.Ordinal))
            {
                throw new InvalidDataException("The animals file is missing the expected header.");
            }

            var animals = new List<Animal>();
            for (int i = 1; i < lines.Length; i++)
            {
                string[] fields = lines[i].Split('|');
                if (fields.Length != 7)
                {
                    throw new InvalidDataException(
                        string.Format(CultureInfo.InvariantCulture, "Invalid animal record on line {0}.", i + 1));
                }

                int age;
                int recoveryScore;
                if (!int.TryParse(fields[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out age) ||
                    !int.TryParse(fields[4], NumberStyles.Integer, CultureInfo.InvariantCulture, out recoveryScore))
                {
                    throw new InvalidDataException(
                        string.Format(CultureInfo.InvariantCulture, "Invalid numeric value on line {0}.", i + 1));
                }

                animals.Add(new Animal
                {
                    AnimalID = fields[0],
                    Name = fields[1],
                    Species = fields[2],
                    Age = age,
                    RecoveryScore = recoveryScore,
                    Status = fields[5],
                    HousingUnit = fields[6]
                });
            }

            return animals;
        }

        public void SaveAnimals(IEnumerable<Animal> animals)
        {
            if (animals == null)
            {
                throw new ArgumentNullException(nameof(animals));
            }

            var lines = new List<string> { Header };
            foreach (Animal animal in animals)
            {
                if (animal == null)
                {
                    throw new ArgumentException("The animal collection cannot contain null entries.", nameof(animals));
                }

                lines.Add(FormatAnimalLine(animal));
            }

            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllLines(filePath, lines, new UTF8Encoding(false));
        }

        public static void SaveAnimal(Animal animal)
        {
            if (animal == null)
            {
                throw new ArgumentNullException(nameof(animal));
            }

            if (AnimalExists(animal.AnimalID))
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "An animal with ID '{0}' already exists.", animal.AnimalID));
            }

            string path = GetDefaultFilePath();
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            bool writeHeader = !File.Exists(path) || new FileInfo(path).Length == 0;
            string content = (writeHeader ? Header + Environment.NewLine : string.Empty) +
                FormatAnimalLine(animal) + Environment.NewLine;
            File.AppendAllText(path, content, new UTF8Encoding(false));
        }

        public static bool AnimalExists(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("An animal ID is required.", nameof(id));
            }

            string path = GetDefaultFilePath();
            if (!File.Exists(path))
            {
                return false;
            }

            foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
            {
                int separatorIndex = line.IndexOf('|');
                if (separatorIndex >= 0 &&
                    string.Equals(line.Substring(0, separatorIndex), id, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static string GetDefaultFilePath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "animals.txt");
        }

        private static string FormatAnimalLine(Animal animal)
        {
            return string.Join("|",
                FormatField(animal.AnimalID),
                FormatField(animal.Name),
                FormatField(animal.Species),
                animal.Age.ToString(CultureInfo.InvariantCulture),
                animal.RecoveryScore.ToString(CultureInfo.InvariantCulture),
                FormatField(animal.Status),
                FormatField(animal.HousingUnit));
        }

        private static string FormatField(string value)
        {
            value = value ?? string.Empty;
            if (value.IndexOfAny(new[] { '|', '\r', '\n' }) >= 0)
            {
                throw new ArgumentException("Animal text fields cannot contain pipes or line breaks.");
            }

            return value;
        }
    }
}
