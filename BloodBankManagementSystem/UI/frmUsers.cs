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
    public partial class frmUsers : Form
    {
        public frmUsers()
        {
            InitializeComponent();
        }

        // Create Objects of userBLL and userDAL
        userBLL u = new userBLL();
        userDAL dal = new userDAL();

        string imageName = "no-image.jpg";
        string sourcePath = "";
        string destinationPath = "";

        // Global Variable for the image to delete
        string rowHeaderImage;

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            // Add functionality to close this form
            this.Hide();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Step 1: Get the Values from UI and split Full Name into First and Last Name
            string[] nameParts = txtFullName.Text.Trim().Split(' ');
            u.first_name = nameParts.Length > 0 ? nameParts[0] : "";
            u.last_name = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";

            u.email = txtEmail.Text;
            u.username = txtUsername.Text;
            u.password = txtPassword.Text;
            u.contact = txtContact.Text;
            u.address = txtAddress.Text;
            u.added_date = DateTime.Now;
            u.image_name = imageName;

            // Upload the image if it is selected
            if (!string.IsNullOrEmpty(sourcePath) && File.Exists(sourcePath) && imageName != "no-image.jpg")
            {
                File.Copy(sourcePath, destinationPath, true);
            }

            // Step 2: Adding the Values from UI to the Database
            bool success = dal.Insert(u);

            // Step 3: Check whether the Data is Inserted Successfully or Not
            if (success == true)
            {
                MessageBox.Show("New User added Successfully.");

                // Display the user in DataGrid View
                DataTable dt = dal.Select();
                dgvUsers.DataSource = dt;

                // Clear TextBoxes
                Clear();
            }
            else
            {
                MessageBox.Show("Failed to Add New User.");
            }
        }

        // Method or Function to Clear TextBoxes
        public void Clear()
        {
            txtFullName.Text = "";
            txtEmail.Text = "";
            txtUsername.Text = "";
            txtContact.Text = "";
            txtAddress.Text = "";
            txtPassword.Text = "";
            txtUserID.Text = "";

            // Reset image tracking variables
            imageName = "no-image.jpg";
            sourcePath = "";
            destinationPath = "";

            // Get the Image path without locking file streams
            string paths = Application.StartupPath.Substring(0, (Application.StartupPath.Length - 10));
            string imagePath = paths + "\\images\\no-image.jpg";

            if (pictureBoxProfilePicture.Image != null)
            {
                pictureBoxProfilePicture.Image.Dispose();
                pictureBoxProfilePicture.Image = null;
            }

            if (File.Exists(imagePath))
            {
                using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(imagePath)))
                {
                    pictureBoxProfilePicture.Image = Image.FromStream(ms);
                }
            }
        }

        private void dgvUsers_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int RowIndex = e.RowIndex;

            // Read values by Column Name safely
            txtUserID.Text = dgvUsers.Rows[RowIndex].Cells["user_id"].Value.ToString();

            string fname = dgvUsers.Rows[RowIndex].Cells["first_name"].Value.ToString();
            string lname = dgvUsers.Rows[RowIndex].Cells["last_name"].Value.ToString();
            txtFullName.Text = (fname + " " + lname).Trim();

            txtUsername.Text = dgvUsers.Rows[RowIndex].Cells["username"].Value.ToString();
            txtEmail.Text = dgvUsers.Rows[RowIndex].Cells["email"].Value.ToString();
            txtPassword.Text = dgvUsers.Rows[RowIndex].Cells["password"].Value.ToString();
            txtContact.Text = dgvUsers.Rows[RowIndex].Cells["contact"].Value.ToString();
            txtAddress.Text = dgvUsers.Rows[RowIndex].Cells["address"].Value.ToString();
            imageName = dgvUsers.Rows[RowIndex].Cells["image_name"].Value.ToString();

            rowHeaderImage = imageName;

            string paths = Application.StartupPath.Substring(0, (Application.StartupPath.Length - 10));
            string imagePath = paths + "\\images\\" + imageName;

            if (pictureBoxProfilePicture.Image != null)
            {
                pictureBoxProfilePicture.Image.Dispose();
                pictureBoxProfilePicture.Image = null;
            }

            if (File.Exists(imagePath))
            {
                using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(imagePath)))
                {
                    pictureBoxProfilePicture.Image = Image.FromStream(ms);
                }
            }
            else
            {
                string defaultPath = paths + "\\images\\no-image.jpg";
                if (File.Exists(defaultPath))
                {
                    using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(defaultPath)))
                    {
                        pictureBoxProfilePicture.Image = Image.FromStream(ms);
                    }
                }
            }
        }

        private void frmUsers_Load(object sender, EventArgs e)
        {
            // Display the Users in DataGrid View When the Form is Loaded
            DataTable dt = dal.Select();
            dgvUsers.DataSource = dt;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Step 1: Get the Values from UI
            u.user_id = int.Parse(txtUserID.Text);

            string[] nameParts = txtFullName.Text.Trim().Split(' ');
            u.first_name = nameParts.Length > 0 ? nameParts[0] : "";
            u.last_name = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";

            u.email = txtEmail.Text;
            u.username = txtUsername.Text;
            u.password = txtPassword.Text;
            u.contact = txtContact.Text;
            u.address = txtAddress.Text;
            u.added_date = DateTime.Now;
            u.image_name = imageName;

            // Upload New Image if selected
            if (!string.IsNullOrEmpty(sourcePath) && File.Exists(sourcePath))
            {
                File.Copy(sourcePath, destinationPath, true);
            }

            // Step 2: Update database
            bool success = dal.Update(u);

            // Remove the previous Image if replaced
            if (rowHeaderImage != "no-image.jpg" && rowHeaderImage != imageName)
            {
                string paths = Application.StartupPath.Substring(0, (Application.StartupPath.Length - 10));
                string imagePath = paths + "\\images\\" + rowHeaderImage;

                Clear();

                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }

            if (success == true)
            {
                MessageBox.Show("User Updated Successfully.");

                DataTable dt = dal.Select();
                dgvUsers.DataSource = dt;

                Clear();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            u.user_id = int.Parse(txtUserID.Text);

            if (rowHeaderImage != "no-image.jpg")
            {
                string paths = Application.StartupPath.Substring(0, (Application.StartupPath.Length - 10));
                string imagePath = paths + "\\images\\" + rowHeaderImage;

                Clear();

                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }

            bool success = dal.Delete(u);

            if (success == true)
            {
                MessageBox.Show("User Deleted Successfully.");

                DataTable dt = dal.Select();
                dgvUsers.DataSource = dt;

                Clear();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.PNG; *.gif;)|*.jpg; *.jpeg; *.png; *.PNG; *.gif;";

            if (open.ShowDialog() == DialogResult.OK)
            {
                if (open.CheckFileExists)
                {
                    string ext = Path.GetExtension(open.FileName);
                    Random random = new Random();
                    int RandInt = random.Next(0, 1000);

                    imageName = "Blood_Bank_MS_" + RandInt + ext;
                    sourcePath = open.FileName;

                    string paths = Application.StartupPath.Substring(0, Application.StartupPath.Length - 10);
                    destinationPath = paths + "\\images\\" + imageName;

                    if (pictureBoxProfilePicture.Image != null)
                    {
                        pictureBoxProfilePicture.Image.Dispose();
                        pictureBoxProfilePicture.Image = null;
                    }

                    using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(sourcePath)))
                    {
                        pictureBoxProfilePicture.Image = Image.FromStream(ms);
                    }
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keywords = txtSearch.Text;

            if (!string.IsNullOrEmpty(keywords))
            {
                DataTable dt = dal.Search(keywords);
                dgvUsers.DataSource = dt;
            }
            else
            {
                DataTable dt = dal.Select();
                dgvUsers.DataSource = dt;
            }
        }
    }
}