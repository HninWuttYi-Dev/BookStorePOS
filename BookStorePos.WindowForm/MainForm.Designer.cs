namespace BookStorePos.WindowForm;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    private System.Windows.Forms.Panel headerPanel;
    private System.Windows.Forms.Label lblLogo;
    private System.Windows.Forms.Panel contentPanel;
    private System.Windows.Forms.DataGridView dataGridViewBooks;
    
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.TextBox txtTitle;
    private System.Windows.Forms.Label lblAuthor;
    private System.Windows.Forms.TextBox txtAuthor;
    private System.Windows.Forms.Label lblGenre;
    private System.Windows.Forms.TextBox txtGenre;
    private System.Windows.Forms.Label lblDescription;
    private System.Windows.Forms.TextBox txtDescription;
    private System.Windows.Forms.Label lblPrice;
    private System.Windows.Forms.TextBox txtPrice;
    private System.Windows.Forms.Label lblStock;
    private System.Windows.Forms.TextBox txtStock;

    private System.Windows.Forms.Button btnCreate;
    private System.Windows.Forms.Button btnUpdate;
    private System.Windows.Forms.Button btnDelete;
    private System.Windows.Forms.Label lblSectionTitle;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.headerPanel = new System.Windows.Forms.Panel();
        this.lblLogo = new System.Windows.Forms.Label();
        this.contentPanel = new System.Windows.Forms.Panel();
        this.lblSectionTitle = new System.Windows.Forms.Label();
        this.dataGridViewBooks = new System.Windows.Forms.DataGridView();
        
        this.lblTitle = new System.Windows.Forms.Label();
        this.txtTitle = new System.Windows.Forms.TextBox();
        this.lblAuthor = new System.Windows.Forms.Label();
        this.txtAuthor = new System.Windows.Forms.TextBox();
        this.lblGenre = new System.Windows.Forms.Label();
        this.txtGenre = new System.Windows.Forms.TextBox();
        this.lblDescription = new System.Windows.Forms.Label();
        this.txtDescription = new System.Windows.Forms.TextBox();
        this.lblPrice = new System.Windows.Forms.Label();
        this.txtPrice = new System.Windows.Forms.TextBox();
        this.lblStock = new System.Windows.Forms.Label();
        this.txtStock = new System.Windows.Forms.TextBox();
        
        this.btnCreate = new System.Windows.Forms.Button();
        this.btnUpdate = new System.Windows.Forms.Button();
        this.btnDelete = new System.Windows.Forms.Button();
        
        this.headerPanel.SuspendLayout();
        this.contentPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBooks)).BeginInit();
        this.SuspendLayout();
        
        // 
        // headerPanel
        // 
        this.headerPanel.BackColor = System.Drawing.Color.FromArgb(21, 183, 89);
        this.headerPanel.Controls.Add(this.lblLogo);
        this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
        this.headerPanel.Location = new System.Drawing.Point(0, 0);
        this.headerPanel.Name = "headerPanel";
        this.headerPanel.Size = new System.Drawing.Size(1000, 60);
        this.headerPanel.TabIndex = 0;
        // 
        // lblLogo
        // 
        this.lblLogo.AutoSize = true;
        this.lblLogo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.lblLogo.ForeColor = System.Drawing.Color.White;
        this.lblLogo.Location = new System.Drawing.Point(20, 15);
        this.lblLogo.Name = "lblLogo";
        this.lblLogo.Size = new System.Drawing.Size(163, 30);
        this.lblLogo.TabIndex = 0;
        this.lblLogo.Text = "BookStore POS";
        // 
        // contentPanel
        // 
        this.contentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
        this.contentPanel.BackColor = System.Drawing.Color.White;
        this.contentPanel.Controls.Add(this.lblSectionTitle);
        this.contentPanel.Controls.Add(this.dataGridViewBooks);
        this.contentPanel.Controls.Add(this.lblTitle);
        this.contentPanel.Controls.Add(this.txtTitle);
        this.contentPanel.Controls.Add(this.lblAuthor);
        this.contentPanel.Controls.Add(this.txtAuthor);
        this.contentPanel.Controls.Add(this.lblGenre);
        this.contentPanel.Controls.Add(this.txtGenre);
        this.contentPanel.Controls.Add(this.lblDescription);
        this.contentPanel.Controls.Add(this.txtDescription);
        this.contentPanel.Controls.Add(this.lblPrice);
        this.contentPanel.Controls.Add(this.txtPrice);
        this.contentPanel.Controls.Add(this.lblStock);
        this.contentPanel.Controls.Add(this.txtStock);
        this.contentPanel.Controls.Add(this.btnCreate);
        this.contentPanel.Controls.Add(this.btnUpdate);
        this.contentPanel.Controls.Add(this.btnDelete);
        this.contentPanel.Location = new System.Drawing.Point(30, 90);
        this.contentPanel.Name = "contentPanel";
        this.contentPanel.Size = new System.Drawing.Size(940, 550);
        this.contentPanel.TabIndex = 1;
        // 
        // lblSectionTitle
        // 
        this.lblSectionTitle.AutoSize = true;
        this.lblSectionTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.lblSectionTitle.ForeColor = System.Drawing.Color.Gray;
        this.lblSectionTitle.Location = new System.Drawing.Point(30, 20);
        this.lblSectionTitle.Name = "lblSectionTitle";
        this.lblSectionTitle.Size = new System.Drawing.Size(142, 21);
        this.lblSectionTitle.TabIndex = 1;
        this.lblSectionTitle.Text = "Book Management";
        // 
        // txtTitle
        // 
        this.txtTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.txtTitle.Location = new System.Drawing.Point(30, 80);
        this.txtTitle.Name = "txtTitle";
        this.txtTitle.Size = new System.Drawing.Size(200, 25);
        this.txtTitle.TabIndex = 2;
        // 
        // lblTitle
        // 
        this.lblTitle.AutoSize = true;
        this.lblTitle.ForeColor = System.Drawing.Color.Gray;
        this.lblTitle.Location = new System.Drawing.Point(30, 60);
        this.lblTitle.Name = "lblTitle";
        this.lblTitle.Size = new System.Drawing.Size(29, 15);
        this.lblTitle.TabIndex = 3;
        this.lblTitle.Text = "Title";
        // 
        // txtAuthor
        // 
        this.txtAuthor.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.txtAuthor.Location = new System.Drawing.Point(250, 80);
        this.txtAuthor.Name = "txtAuthor";
        this.txtAuthor.Size = new System.Drawing.Size(200, 25);
        this.txtAuthor.TabIndex = 4;
        // 
        // lblAuthor
        // 
        this.lblAuthor.AutoSize = true;
        this.lblAuthor.ForeColor = System.Drawing.Color.Gray;
        this.lblAuthor.Location = new System.Drawing.Point(250, 60);
        this.lblAuthor.Name = "lblAuthor";
        this.lblAuthor.Size = new System.Drawing.Size(44, 15);
        this.lblAuthor.TabIndex = 5;
        this.lblAuthor.Text = "Author";
        // 
        // txtGenre
        // 
        this.txtGenre.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.txtGenre.Location = new System.Drawing.Point(470, 80);
        this.txtGenre.Name = "txtGenre";
        this.txtGenre.Size = new System.Drawing.Size(200, 25);
        this.txtGenre.TabIndex = 6;
        // 
        // lblGenre
        // 
        this.lblGenre.AutoSize = true;
        this.lblGenre.ForeColor = System.Drawing.Color.Gray;
        this.lblGenre.Location = new System.Drawing.Point(470, 60);
        this.lblGenre.Name = "lblGenre";
        this.lblGenre.Size = new System.Drawing.Size(38, 15);
        this.lblGenre.TabIndex = 7;
        this.lblGenre.Text = "Genre";
        // 
        // txtDescription
        // 
        this.txtDescription.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.txtDescription.Location = new System.Drawing.Point(30, 140);
        this.txtDescription.Name = "txtDescription";
        this.txtDescription.Size = new System.Drawing.Size(420, 25);
        this.txtDescription.TabIndex = 8;
        // 
        // lblDescription
        // 
        this.lblDescription.AutoSize = true;
        this.lblDescription.ForeColor = System.Drawing.Color.Gray;
        this.lblDescription.Location = new System.Drawing.Point(30, 120);
        this.lblDescription.Name = "lblDescription";
        this.lblDescription.Size = new System.Drawing.Size(67, 15);
        this.lblDescription.TabIndex = 9;
        this.lblDescription.Text = "Description";
        // 
        // txtPrice
        // 
        this.txtPrice.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.txtPrice.Location = new System.Drawing.Point(470, 140);
        this.txtPrice.Name = "txtPrice";
        this.txtPrice.Size = new System.Drawing.Size(95, 25);
        this.txtPrice.TabIndex = 10;
        // 
        // lblPrice
        // 
        this.lblPrice.AutoSize = true;
        this.lblPrice.ForeColor = System.Drawing.Color.Gray;
        this.lblPrice.Location = new System.Drawing.Point(470, 120);
        this.lblPrice.Name = "lblPrice";
        this.lblPrice.Size = new System.Drawing.Size(58, 15);
        this.lblPrice.TabIndex = 11;
        this.lblPrice.Text = "Price (Ks)";
        // 
        // txtStock
        // 
        this.txtStock.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.txtStock.Location = new System.Drawing.Point(585, 140);
        this.txtStock.Name = "txtStock";
        this.txtStock.Size = new System.Drawing.Size(85, 25);
        this.txtStock.TabIndex = 12;
        // 
        // lblStock
        // 
        this.lblStock.AutoSize = true;
        this.lblStock.ForeColor = System.Drawing.Color.Gray;
        this.lblStock.Location = new System.Drawing.Point(585, 120);
        this.lblStock.Name = "lblStock";
        this.lblStock.Size = new System.Drawing.Size(36, 15);
        this.lblStock.TabIndex = 13;
        this.lblStock.Text = "Stock";
        // 
        // btnCreate
        // 
        this.btnCreate.BackColor = System.Drawing.Color.FromArgb(21, 183, 89);
        this.btnCreate.FlatAppearance.BorderSize = 0;
        this.btnCreate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnCreate.ForeColor = System.Drawing.Color.White;
        this.btnCreate.Location = new System.Drawing.Point(690, 80);
        this.btnCreate.Name = "btnCreate";
        this.btnCreate.Size = new System.Drawing.Size(100, 35);
        this.btnCreate.TabIndex = 14;
        this.btnCreate.Text = "Create Book";
        this.btnCreate.UseVisualStyleBackColor = false;
        this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
        // 
        // btnUpdate
        // 
        this.btnUpdate.BackColor = System.Drawing.Color.FromArgb(21, 183, 89);
        this.btnUpdate.FlatAppearance.BorderSize = 0;
        this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnUpdate.ForeColor = System.Drawing.Color.White;
        this.btnUpdate.Location = new System.Drawing.Point(690, 130);
        this.btnUpdate.Name = "btnUpdate";
        this.btnUpdate.Size = new System.Drawing.Size(100, 35);
        this.btnUpdate.TabIndex = 15;
        this.btnUpdate.Text = "Update";
        this.btnUpdate.UseVisualStyleBackColor = false;
        this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
        // 
        // btnDelete
        // 
        this.btnDelete.BackColor = System.Drawing.Color.White;
        this.btnDelete.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(21, 183, 89);
        this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnDelete.ForeColor = System.Drawing.Color.FromArgb(21, 183, 89);
        this.btnDelete.Location = new System.Drawing.Point(800, 130);
        this.btnDelete.Name = "btnDelete";
        this.btnDelete.Size = new System.Drawing.Size(100, 35);
        this.btnDelete.TabIndex = 16;
        this.btnDelete.Text = "Delete";
        this.btnDelete.UseVisualStyleBackColor = false;
        this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
        // 
        // dataGridViewBooks
        // 
        this.dataGridViewBooks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
        this.dataGridViewBooks.BackgroundColor = System.Drawing.Color.White;
        this.dataGridViewBooks.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.dataGridViewBooks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dataGridViewBooks.Location = new System.Drawing.Point(30, 200);
        this.dataGridViewBooks.Name = "dataGridViewBooks";
        this.dataGridViewBooks.RowTemplate.Height = 25;
        this.dataGridViewBooks.Size = new System.Drawing.Size(880, 320);
        this.dataGridViewBooks.TabIndex = 17;
        this.dataGridViewBooks.SelectionChanged += new System.EventHandler(this.dataGridViewBooks_SelectionChanged);
        this.dataGridViewBooks.ReadOnly = true;
        this.dataGridViewBooks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.dataGridViewBooks.AllowUserToAddRows = false;
        this.dataGridViewBooks.AllowUserToDeleteRows = false;
        // 
        // MainForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.Color.FromArgb(245, 246, 248);
        this.ClientSize = new System.Drawing.Size(1000, 680);
        this.Controls.Add(this.contentPanel);
        this.Controls.Add(this.headerPanel);
        this.Name = "MainForm";
        this.Text = "BookStore POS";
        this.headerPanel.ResumeLayout(false);
        this.headerPanel.PerformLayout();
        this.contentPanel.ResumeLayout(false);
        this.contentPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBooks)).EndInit();
        this.ResumeLayout(false);
    }
}
