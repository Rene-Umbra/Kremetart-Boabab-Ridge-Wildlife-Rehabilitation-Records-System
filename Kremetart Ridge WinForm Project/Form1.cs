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
        // Input controls for Add / Update.
        private TextBox txtAnimalId = new TextBox();
        private TextBox txtName = new TextBox();
        private TextBox txtSpecies = new TextBox();
        private TextBox txtAge = new TextBox();
        private TextBox txtScore = new TextBox();

        // Buttons for Add / Update.
        private Button btnAdd = new Button();
        private Button btnSearch = new Button();
        private Button btnSave = new Button();
        private Button btnClear = new Button();

        // Summary controls.
        private Button btnGenerateSummary = new Button();
        private TextBox txtSummaryOutput = new TextBox();

        // The ID of the animal currently loaded in edit mode ("" = Add mode).
        private string selectedAnimalId = "";

        /// <summary>
        /// Starts the form, builds the grid, builds the update controls,
        /// builds the summary controls, and loads the animals.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            CreateAnimalGrid();
            CreateUpdateControls();
            CreateSummaryControls();
            RefreshGrid();

            dgvAnimals.SelectionChanged += dgvAnimals_SelectionChanged;
            dgvAnimals.ClearSelection();
        }

        /// <summary>
        /// Reads animals from animals.txt.
        /// Returns false if the file could not be read.
        /// </summary>
        private bool LoadAnimals(out List<Animal> animals, out int skipped)
        {
            animals = new List<Animal>();
            skipped = 0;

            string path = Path.Combine(Application.StartupPath, "animals.txt");

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

        /// <summary>
        /// Creates the DataGridView for viewing animals.
        /// </summary>
        private void CreateAnimalGrid()
        {
            dgvAnimals.Location = new Point(20, 20);
            dgvAnimals.Size = new Size(750, 280);

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

        /// <summary>
        /// Refreshes the grid from animals.txt.
        /// Shows the skipped-line count once after loading.
        /// </summary>
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

        /// <summary>
        /// Creates the input boxes and buttons for Add / Update / Search.
        /// </summary>
        private void CreateUpdateControls()
        {
            Label lblId = new Label();
            lblId.Text = "Animal ID";
            lblId.Location = new Point(20, 320);

            txtAnimalId.Location = new Point(140, 320);

            Label lblName = new Label();
            lblName.Text = "Name";
            lblName.Location = new Point(20, 355);

            txtName.Location = new Point(140, 355);

            Label lblSpecies = new Label();
            lblSpecies.Text = "Species";
            lblSpecies.Location = new Point(20, 390);

            txtSpecies.Location = new Point(140, 390);

            Label lblAge = new Label();
            lblAge.Text = "Age";
            lblAge.Location = new Point(20, 425);

            txtAge.Location = new Point(140, 425);

            Label lblScore = new Label();
            lblScore.Text = "Recovery Score";
            lblScore.Location = new Point(20, 460);

            txtScore.Location = new Point(140, 460);

            btnAdd.Text = "Add Animal";
            btnAdd.Location = new Point(20, 500);
            btnAdd.Size = new Size(110, 30);
            btnAdd.Click += btnAdd_Click;

            btnSearch.Text = "Search";
            btnSearch.Location = new Point(260, 320);
            btnSearch.Click += btnSearch_Click;

            btnSave.Text = "Save Changes";
            btnSave.Location = new Point(140, 500);
            btnSave.Size = new Size(110, 30);
            btnSave.Enabled = false;
            btnSave.Click += btnSave_Click;

            btnClear.Text = "Clear";
            btnClear.Location = new Point(260, 500);
            btnClear.Click += btnClear_Click;

            Controls.AddRange(new Control[]
            {
                lblId, txtAnimalId,
                lblName, txtName,
                lblSpecies, txtSpecies,
                lblAge, txtAge,
                lblScore, txtScore,
                btnAdd, btnSearch, btnSave, btnClear
            });

            ClientSize = new Size(800, 580);
        }

        /// <summary>
        /// Creates the Generate Summary button and the read-only results box.
        /// </summary>
        private void CreateSummaryControls()
        {
            btnGenerateSummary.Text = "Generate Summary";
            btnGenerateSummary.Location = new Point(380, 500);
            btnGenerateSummary.Size = new Size(150, 30);
            btnGenerateSummary.Click += btnGenerateSummary_Click;

            txtSummaryOutput.Location = new Point(380, 320);
            txtSummaryOutput.Size = new Size(390, 170);
            txtSummaryOutput.Multiline = true;
            txtSummaryOutput.ReadOnly = true;
            txtSummaryOutput.ScrollBars = ScrollBars.Vertical;
            txtSummaryOutput.Font = new Font("Consolas", 9F);

            Controls.AddRange(new Control[]
            {
                btnGenerateSummary,
                txtSummaryOutput
            });
        }

        /// <summary>
        /// Loads an animal's details into the text boxes (edit mode).
        /// </summary>
        private void LoadSelectedAnimal(Animal animal)
        {
            selectedAnimalId = animal.AnimalId;

            txtAnimalId.Text = animal.AnimalId;
            txtName.Text = animal.Name;
            txtSpecies.Text = animal.Species;
            txtAge.Text = animal.Age.ToString();
            txtScore.Text = animal.RecoveryScore.ToString();

            txtAnimalId.ReadOnly = true;
            btnAdd.Enabled = false;
            btnSave.Enabled = true;
        }

        /// <summary>
        /// Adds a new animal after validating every field.
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string id = txtAnimalId.Text.Trim().ToUpperInvariant();
            string name = txtName.Text.Trim();
            string species = txtSpecies.Text.Trim();

            if (!Regex.IsMatch(id, @"^WR-[0-9]{4}$"))
            {
                MessageBox.Show("Animal ID must be in the format WR-0000.");
                txtAnimalId.Focus();
                return;
            }

            if (name.Length < 1 || name.Length > 30 || name.Contains("|"))
            {
                MessageBox.Show("Name must be 1-30 characters and cannot contain |.");
                txtName.Focus();
                return;
            }

            if (species.Length < 1 || species.Length > 30 || species.Contains("|"))
            {
                MessageBox.Show("Species must be 1-30 characters and cannot contain |.");
                txtSpecies.Focus();
                return;
            }

            int age;
            if (!int.TryParse(txtAge.Text.Trim(), out age) || age < 0 || age > 100)
            {
                MessageBox.Show("Age must be a whole number between 0 and 100.");
                txtAge.Focus();
                return;
            }

            int score;
            if (!int.TryParse(txtScore.Text.Trim(), out score) || score < 0 || score > 100)
            {
                MessageBox.Show("Recovery Score must be a whole number between 0 and 100.");
                txtScore.Focus();
                return;
            }

            List<Animal> animals;
            int skipped;
            if (!LoadAnimals(out animals, out skipped))
                return;

            foreach (Animal existing in animals)
            {
                if (existing.AnimalId.Equals(id, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("An animal with that ID already exists.");
                    txtAnimalId.Focus();
                    return;
                }
            }

            var result = ClassificationHelper.Classify(score);
            Animal newAnimal = new Animal(id, name, species, age, score,
                                          result.Status, result.HousingUnit);

            animals.Add(newAnimal);

            if (!RewriteAnimals(animals))
                return;

            MessageBox.Show(
                "Animal added.\nStatus: " + result.Status +
                "\nHousing Unit: " + result.HousingUnit);

            ClearFields();
            RefreshGrid();
        }

        /// <summary>
        /// Searches for an animal by its ID (exact match, case-insensitive).
        /// </summary>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string id = txtAnimalId.Text.Trim();

            List<Animal> animals;
            int skipped;

            if (!LoadAnimals(out animals, out skipped))
                return;

            foreach (Animal animal in animals)
            {
                if (animal.AnimalId.Equals(id, StringComparison.OrdinalIgnoreCase))
                {
                    LoadSelectedAnimal(animal);
                    return;
                }
            }

            MessageBox.Show("Animal not found.");
        }

        /// <summary>
        /// Loads an animal when a row is selected.
        /// </summary>
        private void dgvAnimals_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAnimals.SelectedRows.Count == 0)
                return;

            object value = dgvAnimals.SelectedRows[0].Cells[0].Value;
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

        /// <summary>
        /// Validates and saves changes to the animal loaded in edit mode.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (selectedAnimalId == "")
            {
                MessageBox.Show("Please select an animal.");
                return;
            }

            string name = txtName.Text.Trim();
            string species = txtSpecies.Text.Trim();

            if (name.Length < 1 || name.Length > 30 || name.Contains("|"))
            {
                MessageBox.Show("Invalid animal name.");
                txtName.Focus();
                return;
            }

            if (species.Length < 1 || species.Length > 30 || species.Contains("|"))
            {
                MessageBox.Show("Invalid species.");
                txtSpecies.Focus();
                return;
            }

            int age;
            int score;

            if (!int.TryParse(txtAge.Text, out age) || age < 0 || age > 100)
            {
                MessageBox.Show("Age must be between 0 and 100.");
                txtAge.Focus();
                return;
            }

            if (!int.TryParse(txtScore.Text, out score) || score < 0 || score > 100)
            {
                MessageBox.Show("Recovery Score must be between 0 and 100.");
                txtScore.Focus();
                return;
            }

            List<Animal> animals;
            int skipped;
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

                    var result = ClassificationHelper.Classify(score);
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

            if (!RewriteAnimals(animals))
                return;

            MessageBox.Show("Animal updated successfully.");
            ClearFields();
            RefreshGrid();
        }

        /// <summary>
        /// Clears the input boxes and returns the form to Add mode.
        /// </summary>
        private void ClearFields()
        {
            selectedAnimalId = "";

            txtAnimalId.Clear();
            txtName.Clear();
            txtSpecies.Clear();
            txtAge.Clear();
            txtScore.Clear();

            txtAnimalId.ReadOnly = false;
            btnAdd.Enabled = true;
            btnSave.Enabled = false;

            dgvAnimals.ClearSelection();
        }

        /// <summary>
        /// Clears the form (Clear button).
        /// </summary>
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        /// <summary>
        /// Deletes the selected animal after a Yes/No confirmation.
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvAnimals.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an animal to delete.");
                return;
            }

            DataGridViewRow selectedRow = dgvAnimals.SelectedRows[0];
            string animalID = Convert.ToString(selectedRow.Cells[0].Value);
            string animalName = Convert.ToString(selectedRow.Cells[1].Value);

            DialogResult answer = MessageBox.Show(
                $"Delete {animalID} ({animalName})? This cannot be undone.",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (answer == DialogResult.No)
                return;

            List<Animal> animals;
            int skipped;
            if (!LoadAnimals(out animals, out skipped))
                return;

            Animal animalToDelete = null;
            foreach (Animal animal in animals)
            {
                if (animal.AnimalId == animalID)
                {
                    animalToDelete = animal;
                    break;
                }
            }

            if (animalToDelete == null)
            {
                MessageBox.Show("Animal not found.");
                return;
            }

            animals.Remove(animalToDelete);

            if (!RewriteAnimals(animals))
                return;

            if (selectedAnimalId == animalID)
                ClearFields();

            RefreshGrid();
            MessageBox.Show("Animal deleted successfully.");
        }

        /// <summary>
        /// Rewrites animals.txt with the supplied list.
        /// Returns true on success.
        /// </summary>
        private bool RewriteAnimals(List<Animal> animals)
        {
            string path = Path.Combine(Application.StartupPath, "animals.txt");
            List<string> lines = new List<string>();

            foreach (Animal animal in animals)
                lines.Add(animal.ToFileLine());

            try
            {
                File.WriteAllLines(path, lines, new UTF8Encoding(false));
                return true;
            }
            catch (IOException)
            {
                MessageBox.Show("Unable to save animal records.");
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Permission to save the file was denied.");
                return false;
            }
        }

        /// <summary>
        /// Generates the summary report, shows it on the form, and writes summary.txt.
        /// </summary>
        private void btnGenerateSummary_Click(object sender, EventArgs e)
        {
            List<Animal> animals;
            int skipped;

            if (!LoadAnimals(out animals, out skipped))
                return;

            string summaryPath = Path.Combine(
                Application.StartupPath, "summary.txt");

            try
            {
                string summaryText =
                    SummaryGenerator.Generate(animals, summaryPath);

                txtSummaryOutput.Text = summaryText;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Summary Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvAnimals_CellContentClick(
            object sender, DataGridViewCellEventArgs e)
        {
        }
    }
}
