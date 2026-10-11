using System;
using System.Windows.Forms;

namespace Kremetart_Ridge_WinForm_Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnAddAnimal_Click(object sender, EventArgs e)
        {
            string animalId = txtAnimalID.Text.Trim();
            string name = txtName.Text.Trim();
            string species = txtSpecies.Text.Trim();
            int age;
            int recoveryScore;

            if (animalId.Length == 0 || name.Length == 0 || species.Length == 0)
            {
                MessageBox.Show(
                    "Enter an animal ID, name, and species.",
                    "Missing information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtAge.Text.Trim(), out age) || age < 0)
            {
                MessageBox.Show(
                    "Enter a whole-number age of zero or greater.",
                    "Invalid age",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtAge.Focus();
                return;
            }

            if (!int.TryParse(txtRecoveryScore.Text.Trim(), out recoveryScore) ||
                recoveryScore < 0 ||
                recoveryScore > 100)
            {
                MessageBox.Show(
                    "Enter a whole-number recovery score from 0 to 100.",
                    "Invalid recovery score",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtRecoveryScore.Focus();
                return;
            }

            (string status, string housing) = AnimalClassifier.Classify(recoveryScore);
            var animal = new Animal
            {
                AnimalID = animalId,
                Name = name,
                Species = species,
                Age = age,
                RecoveryScore = recoveryScore,
                Status = status,
                HousingUnit = housing
            };

            try
            {
                FileService.SaveAnimal(animal);
            }
            catch (InvalidOperationException exception)
            {
                MessageBox.Show(exception.Message, "Duplicate animal ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAnimalID.Focus();
                return;
            }
            catch (ArgumentException exception)
            {
                MessageBox.Show(exception.Message, "Invalid animal information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (UnauthorizedAccessException exception)
            {
                MessageBox.Show(
                    "The animal record could not be saved because the data file is not accessible.\n" + exception.Message,
                    "Save failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            catch (System.IO.IOException exception)
            {
                MessageBox.Show(
                    "The animal record could not be saved.\n" + exception.Message,
                    "Save failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(
                string.Format("Animal added. Status: {0}; housing: {1}.", status, housing),
                "Animal added",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            txtAnimalID.Clear();
            txtName.Clear();
            txtSpecies.Clear();
            txtAge.Clear();
            txtRecoveryScore.Clear();
            txtAnimalID.Focus();
        }
    }
}
