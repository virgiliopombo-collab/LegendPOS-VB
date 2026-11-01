Imports System.Windows.Forms

Public Class ProductManagerForm
    Inherits Form

    Private ReadOnly productsGrid As DataGridView
    Private ReadOnly addButton As Button
    Private ReadOnly deleteButton As Button
    Private ReadOnly closeButton As Button

    Private products As List(Of Product)

    Public Sub New()
        Me.Text = "Products"
        Me.Width = 760
        Me.Height = 500
        Me.StartPosition = FormStartPosition.CenterParent
        Me.FormBorderStyle = FormBorderStyle.FixedDialog

        productsGrid = New DataGridView() With {
            .Location = New Point(20, 20),
            .Width = 700,
            .Height = 360,
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .ReadOnly = False,
            .AutoGenerateColumns = False,
            .RowHeadersVisible = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
        }

        productsGrid.Columns.Add("Name", "Name")
        productsGrid.Columns.Add("Category", "Category")
        productsGrid.Columns.Add("Price", "Price")
        productsGrid.Columns.Add("Stock", "Stock")

        addButton = New Button() With {
            .Text = "Add Sample Item",
            .Location = New Point(20, 400),
            .Width = 160
        }
        AddHandler addButton.Click, AddressOf AddButton_Click

        deleteButton = New Button() With {
            .Text = "Delete Selected",
            .Location = New Point(200, 400),
            .Width = 160
        }
        AddHandler deleteButton.Click, AddressOf DeleteButton_Click

        closeButton = New Button() With {
            .Text = "Close",
            .Location = New Point(560, 400),
            .Width = 160,
            .DialogResult = DialogResult.OK
        }

        Me.Controls.Add(productsGrid)
        Me.Controls.Add(addButton)
        Me.Controls.Add(deleteButton)
        Me.Controls.Add(closeButton)

        RefreshProducts()
    End Sub

    Private Sub RefreshProducts()
        products = PosDataService.GetProducts()
        productsGrid.Rows.Clear()

        For Each product In products
            productsGrid.Rows.Add(product.Name, product.Category, product.Price.ToString("0.00"), product.Stock)
        Next
    End Sub

    Private Sub AddButton_Click(sender As Object, e As EventArgs)
        Dim nextId = If(products.Count = 0, 1, products.Max(Function(p) p.Id) + 1)
        Dim newProduct As New Product With {
            .Id = nextId,
            .Name = "New Product",
            .Category = "General",
            .Price = 5.0D,
            .Barcode = nextId.ToString(),
            .Stock = 25,
            .IsTaxable = True
        }
        PosDataService.SaveProduct(newProduct)
        RefreshProducts()
    End Sub

    Private Sub DeleteButton_Click(sender As Object, e As EventArgs)
        If productsGrid.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a product to delete.", "Delete Product", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim selectedIndex = productsGrid.SelectedRows(0).Index
        If selectedIndex >= 0 AndAlso selectedIndex < products.Count Then
            Dim productToDelete = products(selectedIndex)
            PosDataService.DeleteProduct(productToDelete.Id)
            RefreshProducts()
        End If
    End Sub
End Class
