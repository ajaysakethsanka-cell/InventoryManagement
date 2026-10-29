using InventoryManagement.Data;
using InventoryManagement.Models;

namespace InventoryManagement.Forms
{
    public class ProductForm : Form
    {
        private readonly ProductRepository _repo = new();

        // Controls
        private readonly TextBox txtId = new() { ReadOnly = true };
        private readonly TextBox txtName = new();
        private readonly TextBox txtCategory = new();
        private readonly NumericUpDown numQuantity = new() { Maximum = 100000 };
        private readonly TextBox txtPrice = new();
        private readonly TextBox txtSearch = new();
        private readonly Button btnAdd = new() { Text = "Add" };
        private readonly Button btnUpdate = new() { Text = "Update" };
        private readonly Button btnDelete = new() { Text = "Delete" };
        private readonly Button btnClear = new() { Text = "Clear" };
        private readonly DataGridView dgvProducts = new();

        public ProductForm()
        {
            BuildUi();
            Load += ProductForm_Load;
        }

        // ---------- UI built in code ----------
        private void BuildUi()
        {
            Text = "Inventory Management";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(900, 560);
            MinimumSize = new Size(800, 500);

            // Top panel holds inputs and buttons
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 230 };

            AddRow(pnlTop, "Product ID", txtId, 20);
            AddRow(pnlTop, "Product Name", txtName, 55);
            AddRow(pnlTop, "Category", txtCategory, 90);
            AddRow(pnlTop, "Quantity", numQuantity, 125);
            AddRow(pnlTop, "Price", txtPrice, 160);

            // Buttons
            int x = 130;
            foreach (var b in new[] { btnAdd, btnUpdate, btnDelete, btnClear })
            {
                b.SetBounds(x, 192, 80, 30);
                pnlTop.Controls.Add(b);
                x += 90;
            }

            // Search box (right side)
            var lblSearch = new Label { Text = "Search", AutoSize = true, Location = new Point(480, 24) };
            txtSearch.SetBounds(540, 20, 250, 25);
            pnlTop.Controls.AddRange(new Control[] { lblSearch, txtSearch });

            // Grid
            dgvProducts.Dock = DockStyle.Fill;
            dgvProducts.ReadOnly = true;
            dgvProducts.AllowUserToAddRows = false;
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.MultiSelect = false;
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Add the Fill control first, then the Top panel
            Controls.Add(dgvProducts);
            Controls.Add(pnlTop);

            // Connect events
            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnClear.Click += btnClear_Click;
            txtSearch.TextChanged += txtSearch_TextChanged;
            dgvProducts.CellClick += dgvProducts_CellClick;
        }

        // Helper: label on the left, input control on the right
        private static void AddRow(Panel parent, string label, Control input, int y)
        {
            var lbl = new Label { Text = label, AutoSize = true, Location = new Point(20, y + 3) };
            input.SetBounds(130, y, 250, 25);
            parent.Controls.Add(lbl);
            parent.Controls.Add(input);
        }

        // ---------- Events ----------
        private void ProductForm_Load(object? sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                dgvProducts.DataSource = _repo.GetAll(txtSearch.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message);
            }
        }

        private void ClearFields()
        {
            txtId.Clear();
            txtName.Clear();
            txtCategory.Clear();
            txtPrice.Clear();
            numQuantity.Value = 0;
            txtName.Focus();
        }

        // Validate input and build a Product object
        private bool TryGetProduct(out Product product)
        {
            product = new Product();

            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtCategory.Text))
            {
                MessageBox.Show("Product name and category are required.");
                return false;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Enter a valid price.");
                return false;
            }

            product.ProductName = txtName.Text.Trim();
            product.Category = txtCategory.Text.Trim();
            product.Quantity = (int)numQuantity.Value;
            product.Price = price;

            if (int.TryParse(txtId.Text, out int id))
                product.ProductId = id;

            return true;
        }

        // CREATE
        private void btnAdd_Click(object? sender, EventArgs e)
        {
            if (!TryGetProduct(out var p)) return;

            try
            {
                _repo.Add(p);
                MessageBox.Show("Product added successfully.");
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // UPDATE
        private void btnUpdate_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtId.Text))
            {
                MessageBox.Show("Select a product from the grid first.");
                return;
            }
            if (!TryGetProduct(out var p)) return;

            try
            {
                _repo.Update(p);
                MessageBox.Show("Product updated successfully.");
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // DELETE
        private void btnDelete_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Select a product from the grid first.");
                return;
            }

            var confirm = MessageBox.Show("Delete this product?", "Confirm",
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            try
            {
                _repo.Delete(id);
                MessageBox.Show("Product deleted.");
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnClear_Click(object? sender, EventArgs e) => ClearFields();

        // SEARCH
        private void txtSearch_TextChanged(object? sender, EventArgs e) => LoadData();

        // Click a row to fill the fields
        private void dgvProducts_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvProducts.Rows[e.RowIndex];
            txtId.Text = row.Cells["ProductId"].Value?.ToString();
            txtName.Text = row.Cells["ProductName"].Value?.ToString();
            txtCategory.Text = row.Cells["Category"].Value?.ToString();
            numQuantity.Value = Convert.ToDecimal(row.Cells["Quantity"].Value);
            txtPrice.Text = row.Cells["Price"].Value?.ToString();
        }
    }
}