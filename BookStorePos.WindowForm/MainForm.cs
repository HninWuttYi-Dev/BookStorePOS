using System;
using System.Drawing;
using System.Windows.Forms;
using BookStorePOS.Domain.Features.Book;
using BookStorePOS.Domain.Models.Book;

namespace BookStorePos.WindowForm;

public partial class MainForm : Form
{
    private readonly BookService _bookService;

    public MainForm()
    {
        InitializeComponent();
        _bookService = new BookService();
        LoadBooks();
    }

    private void LoadBooks()
    {
        var response = _bookService.GetBooks(new BookListRequestModel());
        if (response.isSuccess)
        {
            dataGridViewBooks.DataSource = response.Data;
        }
        else
        {
            MessageBox.Show(response.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnCreate_Click(object sender, EventArgs e)
    {
        if (decimal.TryParse(txtPrice.Text, out decimal price) && int.TryParse(txtStock.Text, out int stock))
        {
            var req = new BookCreateRequestModel
            {
                Title = txtTitle.Text,
                Author = txtAuthor.Text,
                Genre = txtGenre.Text,
                Description = txtDescription.Text,
                Price = price,
                StockQuantity = stock
            };
            var res = _bookService.CreateBook(req);
            if (res.isSuccess)
            {
                MessageBox.Show("Book created successfully", "Success");
                LoadBooks();
                ClearForm();
            }
            else
            {
                MessageBox.Show(res.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        else
        {
            MessageBox.Show("Invalid price or stock quantity.", "Validation Error");
        }
    }

    private void btnUpdate_Click(object sender, EventArgs e)
    {
        if (dataGridViewBooks.SelectedRows.Count > 0)
        {
            var bookId = (int)dataGridViewBooks.SelectedRows[0].Cells["BookId"].Value;
            decimal.TryParse(txtPrice.Text, out decimal price);
            int.TryParse(txtStock.Text, out int stock);

            var req = new BookPatchRequestModel
            {
                BookId = bookId,
                Title = string.IsNullOrWhiteSpace(txtTitle.Text) ? null : txtTitle.Text,
                Author = string.IsNullOrWhiteSpace(txtAuthor.Text) ? null : txtAuthor.Text,
                Genre = string.IsNullOrWhiteSpace(txtGenre.Text) ? null : txtGenre.Text,
                Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text,
                Price = price > 0 ? price : null,
                StockQuantity = stock > 0 ? stock : null
            };

            var res = _bookService.UpdateBook(req);
            if (res.isSuccess)
            {
                MessageBox.Show("Book updated successfully", "Success");
                LoadBooks();
            }
            else
            {
                MessageBox.Show(res.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
        if (dataGridViewBooks.SelectedRows.Count > 0)
        {
            var bookId = (int)dataGridViewBooks.SelectedRows[0].Cells["BookId"].Value;
            var req = new BookDeleteRequestModel { BookId = bookId };
            var res = _bookService.DeleteBook(req);
            
            if (res.isSuccess)
            {
                MessageBox.Show("Book deleted successfully", "Success");
                LoadBooks();
                ClearForm();
            }
            else
            {
                MessageBox.Show(res.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    
    private void dataGridViewBooks_SelectionChanged(object sender, EventArgs e)
    {
        if (dataGridViewBooks.SelectedRows.Count > 0)
        {
            var row = dataGridViewBooks.SelectedRows[0];
            txtTitle.Text = row.Cells["Title"].Value?.ToString();
            txtAuthor.Text = row.Cells["Author"].Value?.ToString();
            txtGenre.Text = row.Cells["Genre"].Value?.ToString();
            txtDescription.Text = row.Cells["Description"].Value?.ToString();
            txtPrice.Text = row.Cells["Price"].Value?.ToString();
            txtStock.Text = row.Cells["StockQuantity"].Value?.ToString();
        }
    }
    
    private void ClearForm()
    {
        txtTitle.Clear();
        txtAuthor.Clear();
        txtGenre.Clear();
        txtDescription.Clear();
        txtPrice.Clear();
        txtStock.Clear();
    }
}
