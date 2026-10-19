using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Kremetart_Ridge_WinForm_Project
{
    public static class SummaryGenerator
    {
        // All five statuses, in the exact order required by the brief.
        private static readonly string[] Statuses =
        {
            "Critical",
            "Serious",
            "Stable",
            "Recovering",
            "Release-Ready"
        };

        public static string Generate(string animalsFilePath, string summaryFilePath)
        {
            // 1. Load valid animals from animals.txt (skip malformed lines).
            List<Animal> animals = LoadAnimals(animalsFilePath);

            // 2. Calculate totals, averages and per-status counts.
            int total = animals.Count;
            double averageAge = 0;
            double averageScore = 0;

            if (total > 0)
            {
                int sumAge = 0;
                int sumScore = 0;
                foreach (Animal a in animals)
                {
                    sumAge += a.Age;
                    sumScore += a.RecoveryScore;
                }
                averageAge = (double)sumAge / total;
                averageScore = (double)sumScore / total;
            }

            // Count animals per status.
            Dictionary<string, int> statusCounts = new Dictionary<string, int>();
            foreach (string status in Statuses)
                statusCounts[status] = 0;

            foreach (Animal a in animals)
            {
                if (statusCounts.ContainsKey(a.Status))
                    statusCounts[a.Status]++;
            }

            // 3. Build the summary text in the exact required layout.
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("BAOBAB RIDGE WILDLIFE REHABILITATION CENTRE");
            sb.AppendLine("ANIMAL SUMMARY REPORT");
            sb.AppendLine("Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture));
            sb.AppendLine();
            sb.AppendLine("Total animals: " + total);

            // Averages use a full stop decimal separator, two decimal places.
            if (total == 0)
            {
                sb.AppendLine("Average age (years): N/A");
                sb.AppendLine("Average recovery score: N/A");
            }
            else
            {
                sb.AppendLine("Average age (years): " + averageAge.ToString("F2", CultureInfo.InvariantCulture));
                sb.AppendLine("Average recovery score: " + averageScore.ToString("F2", CultureInfo.InvariantCulture));
            }

            sb.AppendLine();
            sb.AppendLine("Animals per status:");
            foreach (string status in Statuses)
                sb.AppendLine(status + ": " + statusCounts[status]);

            string summaryText = sb.ToString();

            // 4. Write summary.txt (overwrite each time, never append).
            try
            {
                File.WriteAllText(summaryFilePath, summaryText, Encoding.UTF8);
            }
            catch (UnauthorizedAccessException)
            {
                throw new Exception("Cannot write summary.txt — permission denied. Check that the file is not read-only.");
            }
            catch (IOException)
            {
                throw new Exception("Cannot write summary.txt — the file is in use by another program. Close it and try again.");
            }
            catch (DirectoryNotFoundException)
            {
                throw new Exception("Cannot write summary.txt — the output folder was not found.");
            }

            return summaryText;
        }

        private static List<Animal> LoadAnimals(string animalsFilePath)
        {
            List<Animal> animals = new List<Animal>();

            if (!File.Exists(animalsFilePath))
                return animals;

            string[] lines;
            try
            {
                lines = File.ReadAllLines(animalsFilePath, Encoding.UTF8);
            }
            catch (UnauthorizedAccessException)
            {
                throw new Exception("Cannot read animals.txt — permission denied.");
            }
            catch (IOException)
            {
                throw new Exception("Cannot read animals.txt — the file is in use by another program.");
            }

            int skipped = 0;
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    skipped++;
                    continue;
                }

                string[] parts = line.Split('|');
                if (parts.Length != 7)
                {
                    skipped++;
                    continue;
                }

                string id = parts[0].Trim();
                string name = parts[1].Trim();
                string species = parts[2].Trim();
                string ageText = parts[3].Trim();
                string scoreText = parts[4].Trim();
                string status = parts[5].Trim();
                string housing = parts[6].Trim();

                // Validate ID format: WR- followed by exactly 4 digits.
                if (!IsValidId(id))
                {
                    skipped++;
                    continue;
                }

                // Validate age and score are whole numbers.
                int age, score;
                if (!int.TryParse(ageText, NumberStyles.Integer, CultureInfo.InvariantCulture, out age) || age < 0 || age > 100)
                {
                    skipped++;
                    continue;
                }
                if (!int.TryParse(scoreText, NumberStyles.Integer, CultureInfo.InvariantCulture, out score) || score < 0 || score > 100)
                {
                    skipped++;
                    continue;
                }

                Animal animal = new Animal
                {
                    AnimalId = id.ToUpperInvariant(),
                    Name = name,
                    Species = species,
                    Age = age,
                    RecoveryScore = score,
                    Status = status,
                    HousingUnit = housing
                };
                animals.Add(animal);
            }

            // Store skipped count so the caller can inform the user once.
            if (skipped > 0)
                System.Windows.Forms.MessageBox.Show(
                    skipped + " malformed line(s) were skipped while loading animals.txt.",
                    "Load Warning",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Warning);

            return animals;
        }

        private static bool IsValidId(string id)
        {
            if (string.IsNullOrEmpty(id) || id.Length != 7)
                return false;
            if (!id.StartsWith("WR-", StringComparison.OrdinalIgnoreCase))
                return false;
            for (int i = 3; i < 7; i++)
            {
                if (!char.IsDigit(id[i]))
                    return false;
            }
            return true;
        }
    }

    public class Animal
    {
        public string AnimalId { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public int Age { get; set; }
        public int RecoveryScore { get; set; }
        public string Status { get; set; }
        public string HousingUnit { get; set; }
    }
}