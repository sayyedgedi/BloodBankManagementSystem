using BloodBankManagementSystem.BLL;
using BloodBankManagementSystem.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BloodBankManagementSystem.UI
{
    public partial class frmDonors : Form
    {
        public frmDonors()
        {
            InitializeComponent();

            // Suppress DataGridView cell formatting pop-up exceptions
            this.dgvDonors.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvDonors_DataError);
        }

        // Create objects of Donor BLL, Donor DAL, and User DAL
        donorBLL d = new donorBLL();
        donorDAL dal = new donorDAL();
        userDAL udal = new userDAL();

        // Global Variables for Image Handling
        string imageName = "no-image.jpg";
        string sourcePath = "";
        string destinationPath = "";
        string rowHeaderImage = "no-image.jpg";

        private void frmDonors_Load(object sender, EventArgs e)
        {
            // Display Donors in DataGridView
            DataTable dt = dal.Select();
            dgvDonors.DataSource = dt;

            // Load default image into PictureBox safely without locking the file
            LoadDefaultImage();
        }

        private void dgvDonors_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Suppress formatting exception dialogs when loading non-image data into grid
            e.Cancel = true;
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            // Close form
            this.Hide();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // 1. Get Data from Form
            d.first_name = txtFirstName.Text;
            d.last_name = txtLastName.Text;
            d.email = txtEmail.Text;
            d.gender = cmbGender.Text;
            d.blood_group = cmbBloodGroup.Text;
            d.contact = txtContact.Text;
            d.address = txtAddress.Text;
            d.added_date = DateTime.Now;

            // Get ID of Logged In User
            string loggedInUser = frmLogin.loggedInUser;
            userBLL usr = udal.GetIDFromUsername(loggedInUser);
            d.added_by = usr.user_id;

            d.image_name = imageName;

            // Upload image if custom image was chosen
            if (imageName != "no-image.jpg" && !string.IsNullOrEmpty(sourcePath))
            {
                File.Copy(sourcePath, destinationPath, true);
            }

            // 2. Insert Data into Database
            bool isSuccess = dal.Insert(d);

            if (isSuccess)
            {
                MessageBox.Show("New Donor Added Successfully.");

                // Refresh DataGridView
                DataTable dt = dal.Select();
                dgvDonors.DataSource = dt;

                Clear();
            }
            else
            {
                MessageBox.Show("Failed to Add new Donor.");
            }
        }

        public void Clear()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtEmail.Text = "";
            txtDonorID.Text = "";
            cmbGender.Text = "";
            cmbBloodGroup.Text = "";
            txtContact.Text = "";
            txtAddress.Text = "";
            imageName = "no-image.jpg";
            rowHeaderImage = "no-image.jpg";
            sourcePath = "";
            destinationPath = "";

            LoadDefaultImage();
        }

        private void dgvDonors_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int RowIndex = e.RowIndex;

            txtDonorID.Text = dgvDonors.Rows[RowIndex].Cells[0].Value.ToString();
            txtFirstName.Text = dgvDonors.Rows[RowIndex].Cells[1].Value.ToString();
            txtLastName.Text = dgvDonors.Rows[RowIndex].Cells[2].Value.ToString();
            txtEmail.Text = dgvDonors.Rows[RowIndex].Cells[3].Value.ToString();
            txtContact.Text = dgvDonors.Rows[RowIndex].Cells[4].Value.ToString();
            cmbGender.Text = dgvDonors.Rows[RowIndex].Cells[5].Value.ToString();
            txtAddress.Text = dgvDonors.Rows[RowIndex].Cells[6].Value.ToString();
            cmbBloodGroup.Text = dgvDonors.Rows[RowIndex].Cells[7].Value.ToString();

            imageName = dgvDonors.Rows[RowIndex].Cells[9].Value.ToString();
            rowHeaderImage = imageName;

            // Display Selected Donor Image
            string paths = Application.StartupPath.Substring(0, (Application.StartupPath.Length) - 10);
            string imagePath = paths + "\\images\\" + imageName;

            if (File.Exists(imagePath))
            {
                using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    pictureBoxProfilePicture.Image = Image.FromStream(fs);
                }
            }
            else
            {
                LoadDefaultImage();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDonorID.Text))
            {
                MessageBox.Show("Please select a donor to update.");
                return;
            }

            d.donor_id = int.Parse(txtDonorID.Text);
            d.first_name = txtFirstName.Text;
            d.last_name = txtLastName.Text;
            d.email = txtEmail.Text;
            d.gender = cmbGender.Text;
            d.blood_group = cmbBloodGroup.Text;
            d.contact = txtContact.Text;
            d.address = txtAddress.Text;

            string loggedInUser = frmLogin.loggedInUser;
            userBLL usr = udal.GetIDFromUsername(loggedInUser);
            d.added_by = usr.user_id;

            d.image_name = imageName;

            // Copy new image if updated
            if (imageName != "no-image.jpg" && !string.IsNullOrEmpty(sourcePath))
            {
                File.Copy(sourcePath, destinationPath, true);
            }

            bool isSuccess = dal.Update(d);

            // Remove previous image file if changed and not default
            if (rowHeaderImage != "no-image.jpg" && rowHeaderImage != imageName)
            {
                string path = Application.StartupPath.Substring(0, (Application.StartupPath.Length) - 10);
                string imagePath = path + "\\images\\" + rowHeaderImage;

                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }

            if (isSuccess)
            {
                MessageBox.Show("Donor updated Successfully.");
                Clear();

                DataTable dt = dal.Select();
                dgvDonors.DataSource = dt;
            }
            else
            {
                MessageBox.Show("Failed to update donors.");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDonorID.Text))
            {
                MessageBox.Show("Please select a donor to delete.");
                return;
            }

            d.donor_id = int.Parse(txtDonorID.Text);

            // Delete existing image file if not default
            if (rowHeaderImage != "no-image.jpg")
            {
                string path = Application.StartupPath.Substring(0, (Application.StartupPath.Length) - 10);
                string imagePath = path + "\\images\\" + rowHeaderImage;

                Clear();

                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }

            bool isSuccess = dal.Delete(d);

            if (isSuccess)
            {
                MessageBox.Show("Donor Deleted Successfully.");
                Clear();

                DataTable dt = dal.Select();
                dgvDonors.DataSource = dt;
            }
            else
            {
                MessageBox.Show("Failed to Delete Donor.");
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files Only (*.jpg; *.jpeg; *.png; *.gif)|*.jpg; *.jpeg; *.png; *.gif";

            if (open.ShowDialog() == DialogResult.OK)
            {
                if (open.CheckFileExists)
                {
                    using (FileStream fs = new FileStream(open.FileName, FileMode.Open, FileAccess.Read))
                    {
                        pictureBoxProfilePicture.Image = Image.FromStream(fs);
                    }

                    string ext = Path.GetExtension(open.FileName);
                    string name = Path.GetFileNameWithoutExtension(open.FileName);
                    Guid g = Guid.NewGuid();

                    imageName = "Blood_Bank_MS_" + name + "_" + g + ext;
                    sourcePath = open.FileName;

                    string paths = Application.StartupPath.Substring(0, Application.StartupPath.Length - 10);
                    destinationPath = paths + "\\images\\" + imageName;
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keywords = txtSearch.Text;

            if (!string.IsNullOrEmpty(keywords))
            {
                DataTable dt = dal.Search(keywords);
                dgvDonors.DataSource = dt;
            }
            else
            {
                DataTable dt = dal.Select();
                dgvDonors.DataSource = dt;
            }
        }

        // Helper Method to stream default image safely
        private void LoadDefaultImage()
        {
            string path = Application.StartupPath.Substring(0, (Application.StartupPath.Length) - 10);
            string imagepath = path + "\\images\\no-image.jpg";

            if (File.Exists(imagepath))
            {
                using (FileStream fs = new FileStream(imagepath, FileMode.Open, FileAccess.Read))
                {
                    pictureBoxProfilePicture.Image = Image.FromStream(fs);
                }
            }
        }
    }
}