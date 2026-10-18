
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Kremetart_Ridge_WinForm_Project
{
    public partial class Form1 : Form
    {

        private TextBox txtAnimalId = new TextBox();
        private TextBox txtName = new TextBox();
        private TextBox txtSpecies = new TextBox();
        private TextBox txtAge = new TextBox();
        private TextBox txtScore = new TextBox();

        private Button btnSearch = new Button();
        private Button btnSave = new Button();
        private Button btnClear = new Button();

        private string selectedAnimalId = "";

        // Starts the form and loads the animals.
        public Form1()
        {
            InitializeComponent();
            CreateAnimalGrid();
            CreateUpdateControls();
            RefreshGrid();

            dgvAnimals.SelectionChanged += dgvAnimals_SelectionChanged;
            dgvAnimals.ClearSelection();
        }

        // Reads animals from the text file.
        // Returns false if the file could not be read.
        private bool LoadAnimals(out List<Animal> animals, out int skipped)
        {
            animals = new List<Animal>();
            skipped = 0;

            string path = Path.Combine(
                Application.StartupPath, "animals.txt");

            try
            {
                if (!File.Exists(path))
                {
                    File.WriteAllText(path, "");
                    return true;
                }

                foreach (string line in File.ReadAllLines(path))
                {
                    string[] parts = line.Split('|');

                    if (parts.Length != 7)
                    {
                        skipped++;
                        continue;
                    }

                    int age;
                    int score;

                    if (!int.TryParse(parts[3], out age) ||
                        !int.TryParse(parts[4], out score) ||
                        !Regex.IsMatch(parts[0], @"^WR-[0-9]{4}$") ||
                        age < 0 || age > 100 ||
                        score < 0 || score > 100)
                    {
                        skipped++;
                        continue;
                    }

                    Animal animal = new Animal(
                        parts[0], parts[1], parts[2],
                        age, score, parts[5], parts[6]);

                    animals.Add(animal);
                }

                return true;
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Animal file was not found.");
            }
            catch (IOException)
            {
                MessageBox.Show("Unable to access animals.txt.");
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Permission denied for animals.txt.");
            }

            return false;
        }

        // Creates the table for viewing animals.
        private void CreateAnimalGrid()
        {
            dgvAnimals.Location = new Point(20, 20);
            dgvAnimals.Size = new Size(750, 300);

            dgvAnimals.ReadOnly = true;
            dgvAnimals.AllowUserToAddRows = false;
            dgvAnimals.AllowUserToDeleteRows = false;
            dgvAnimals.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvAnimals.MultiSelect = false;
            dgvAnimals.AutoGenerateColumns = false;

            dgvAnimals.Columns.Add("AnimalId", "Animal ID");
            dgvAnimals.Columns.Add("Name", "Name");
            dgvAnimals.Columns.Add("Species", "Species");
            dgvAnimals.Columns.Add("Age", "Age");
            dgvAnimals.Columns.Add("RecoveryScore", "Recovery Score");
            dgvAnimals.Columns.Add("Status", "Status");
            dgvAnimals.Columns.Add("HousingUnit", "Housing Unit");

            Controls.Add(dgvAnimals);
        }

        // Refreshes the table using the latest file records.
        private void RefreshGrid()
        {
            List<Animal> animals;
            int skipped;

            if (!LoadAnimals(out animals, out skipped))
                return;

            dgvAnimals.Rows.Clear();

            foreach (Animal animal in animals)
            {
                dgvAnimals.Rows.Add(
                    animal.AnimalId,
                    animal.Name,
                    animal.Species,
                    animal.Age,
                    animal.RecoveryScore,
                    animal.Status,
                    animal.HousingUnit);
            }

            dgvAnimals.ClearSelection();

            if (skipped > 0)
            {
                MessageBox.Show(
                    skipped + " invalid record(s) were skipped.");
            }
        }

        // Creates the text boxes and buttons for updating.
        private void CreateUpdateControls()
        {
            Label lblId = new Label();
            lblId.Text = "Animal ID";
            lblId.Location = new Point(20, 340);

            txtAnimalId.Location = new Point(140, 340);

            Label lblName = new Label();
            lblName.Text = "Name";
            lblName.Location = new Point(20, 375);

            txtName.Location = new Point(140, 375);

            Label lblSpecies = new Label();
            lblSpecies.Text = "Species";
            lblSpecies.Location = new Point(20, 410);

            txtSpecies.Location = new Point(140, 410);

            Label lblAge = new Label();
            lblAge.Text = "Age";
            lblAge.Location = new Point(20, 445);

            txtAge.Location = new Point(140, 445);

            Label lblScore = new Label();
            lblScore.Text = "Recovery Score";
            lblScore.Location = new Point(20, 480);

            txtScore.Location = new Point(140, 480);

            btnSearch.Text = "Search";
            btnSearch.Location = new Point(260, 340);
            btnSearch.Click += btnSearch_Click;

            btnSave.Text = "Save Changes";
            btnSave.Location = new Point(140, 520);
            btnSave.Size = new Size(110, 30);
            btnSave.Enabled = false;
            btnSave.Click += btnSave_Click;

            btnClear.Text = "Clear";
            btnClear.Location = new Point(260, 520);
            btnClear.Click += btnClear_Click;

            Controls.AddRange(new Control[]
            {
                lblId, txtAnimalId,
                lblName, txtName,
                lblSpecies, txtSpecies,
                lblAge, txtAge,
                lblScore, txtScore,
                btnSearch, btnSave, btnClear
            });

            ClientSize = new Size(800, 580);
        }

        // Loads an animal's details into the text boxes.
        private void LoadSelectedAnimal(Animal animal)
        {
            selectedAnimalId = animal.AnimalId;

            txtAnimalId.Text = animal.AnimalId;
            txtName.Text = animal.Name;
            txtSpecies.Text = animal.Species;
            txtAge.Text = animal.Age.ToString();
            txtScore.Text = animal.RecoveryScore.ToString();

            txtAnimalId.ReadOnly = true;
            btnSave.Enabled = true;
        }

        // Searches for an animal by its ID.
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string id = txtAnimalId.Text.Trim();

            List<Animal> animals;
            int skipped;

            if (!LoadAnimals(out animals, out skipped))
                return;

            foreach (Animal animal in animals)
            {
                if (animal.AnimalId.Equals(
                    id, StringComparison.OrdinalIgnoreCase))
                {
                    LoadSelectedAnimal(animal);
                    return;
                }
            }

            MessageBox.Show("Animal not found.");
        }

        // Loads an animal when a row is selected.
        private void dgvAnimals_SelectionChanged(
            object sender, EventArgs e)
        {
            if (dgvAnimals.SelectedRows.Count == 0)
                return;

            object value =
                dgvAnimals.SelectedRows[0].Cells[0].Value;

            if (value == null)
                return;

            string id = value.ToString();

            List<Animal> animals;
            int skipped;

            if (!LoadAnimals(out animals, out skipped))
                return;

            foreach (Animal animal in animals)
            {
                if (animal.AnimalId == id)
                {
                    LoadSelectedAnimal(animal);
                    return;
                }
            }
        }

        // Validates and saves changes to an animal.
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (selectedAnimalId == "")
            {
                MessageBox.Show("Please select an animal.");
                return;
            }

            string name = txtName.Text.Trim();
            string species = txtSpecies.Text.Trim();

            if (name.Length < 1 ||
                name.Length > 30 || name.Contains("|"))
            {
                MessageBox.Show("Invalid animal name.");
                txtName.Focus();
                return;
            }

            if (species.Length < 1 ||
                species.Length > 30 || species.Contains("|"))
            {
                MessageBox.Show("Invalid species.");
                txtSpecies.Focus();
                return;
            }

            int age;
            int score;

            if (!int.TryParse(txtAge.Text, out age) ||
                age < 0 || age > 100)
            {
                MessageBox.Show(
                    "Age must be between 0 and 100.");
                txtAge.Focus();
                return;
            }

            if (!int.TryParse(txtScore.Text, out score) ||
                score < 0 || score > 100)
            {
                MessageBox.Show(
                    "Recovery Score must be between 0 and 100.");
                txtScore.Focus();
                return;
            }

            List<Animal> animals;
            int skipped;

            // Do not save if reading the file failed.
            if (!LoadAnimals(out animals, out skipped))
                return;

            bool found = false;

            foreach (Animal animal in animals)
            {
                if (animal.AnimalId == selectedAnimalId)
                {
                    animal.Name = name;
                    animal.Species = species;
                    animal.Age = age;
                    animal.RecoveryScore = score;

                    var result =
                        ClassificationHelper.Classify(score);

                    animal.Status = result.Status;
                    animal.HousingUnit = result.HousingUnit;

                    found = true;
                    break;
                }
            }

            if (!found)
            {
                MessageBox.Show("Animal no longer exists.");
                return;
            }

            string path = Path.Combine(
                Application.StartupPath, "animals.txt");

            List<string> lines = new List<string>();

            foreach (Animal animal in animals)
            {
                lines.Add(animal.ToFileLine());
            }

            try
            {
                File.WriteAllLines(
                    path, lines, new UTF8Encoding(false));

                MessageBox.Show("Animal updated successfully.");

                ClearFields();
                RefreshGrid();
            }
            catch (IOException)
            {
                MessageBox.Show("Unable to save animal changes.");
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Permission denied when saving.");
            }
        }

        private void ClearFields()
        {
            selectedAnimalId = "";

            txtAnimalId.Clear();
            txtName.Clear();
            txtSpecies.Clear();
            txtAge.Clear();
            txtScore.Clear();

            txtAnimalId.ReadOnly = false;
            btnSave.Enabled = false;

            dgvAnimals.ClearSelection();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvAnimals.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an animal to delete");
                return;
                
            }

            DataGridViewRow selectedRow = dgvAnimals.SelectedRows[0];
            string animalID = Convert.ToString(selectedRow.Cells[0].Value);
            string animalName = Convert.ToString(selectedRow.Cells[1].Value);

            DialogResult Answer = MessageBox.Show($"Delete {animalID} ({animalName})? This cannot be undone.", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if ( Answer == DialogResult.No )
            {
                return;
            }

            List<Animal> animals;
            int skipped;

            if (!LoadAnimals(out animals, out skipped))
            {

                return;
            }

            Animal animalToDelete = null;
            foreach (Animal animal in animals)
            {
                if(animal.AnimalId == animalID)
                {
                    animalToDelete = animal;
                    break;

                }

                if (animalToDelete == null)
                {
                    MessageBox.Show("Animal not found.");
                    return;
                }

                animals.Remove(animalToDelete);
            }
            
            if (!RewriteAnimals(animals))
            {
                return;
            }

            if (selectedAnimalId == animalID)
            {
                ClearFields();
            }

            RefreshGrid();
            MessageBox.Show("Animal deleted successfully.");
        }
        
        private bool RewriteAnimals(List<Animal> animals)
        {
            string path = Path.Combine(Application.StartupPath, "animals.txt");
            List<string> lines = new List<string>();

            foreach (Animal animal in animals)
            {
                lines.Add(animal.ToFileLine());
            }

            try
            {
                File.WriteAllLines(path, lines, new UTF8Encoding(false));
                return true;
            }
            catch (IOException)
            {

                MessageBox.Show("Unable to save animal records");
                return false;
            }

            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Permission to save the file was denied");
                return false;
            }

        }

        private void dgvAnimals_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
