using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kremetart_Ridge_WinForm_Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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


        }
           
    }
}
