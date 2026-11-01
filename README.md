Imports System.Drawing
Imports System.Windows.Forms

Public Class MainForm
    Inherits Form

    Private currentProducts As List(Of Product)
    Private cart As List(Of OrderItem)
    Private selectedCategory As String = "All"
    Private taxRate As Decimal = 0.08D

    Private headerPanel As Panel
    Private titleLabel As Label
    Private searchBox As TextBox
    Private reportButton As Button
    Private productButton As Button
    Private customerLabel As Label
    Private categoryPanel As FlowLayoutPanel
    Private productPanel As FlowLayoutPanel
    Private cartGrid As DataGridView
    Private subtotalLabel As Label
    Private taxLabel As Label
    Private totalLabel As Label
    Private cashButton As Button
    Private clearButton As Button
    Private removeButton As Button
    Private quantityButton As Button

    Public Sub New()
        Me.Text = "Legend Style POS"
        Me.Size = New Size(1280, 760)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.MinimumSize = New Size(1100, 700)
        Me.BackColor = Color.FromArgb(245, 247, 250)

        cart = New List(Of OrderItem)()
        currentProducts = PosDataService.GetProducts()

        InitializeLayout()
        LoadCategories()
        RefreshProductButtons()
        RefreshCart()
    End Sub

    Private Sub InitializeLayout()
        headerPanel = New Panel() With {
            .Dock = DockStyle.Top,
            .Height = 82,
            .BackColor = Color.FromArgb(32, 43, 73)
        }

        titleLabel = New Label() With {
            .Text = "LEGEND POS",
            .ForeColor = Color.White,
            .Font = New Font("Segoe UI", 22, FontStyle.Bold),
            .Location = New Point(24, 22),
            .AutoSize = True
        }

        searchBox = New TextBox() With {
            .Width = 260,
            .Height = 32,
            .Location = New Point(820, 24),
            .Font = New Font("Segoe UI", 12),
            .PlaceholderText = "Search products"
        }
        AddHandler searchBox.TextChanged, AddressOf SearchBox_TextChanged

        productButton = New Button() With {
            .Text = "Products",
            .Location = New Point(1095, 22),
            .Width = 90,
            .Height = 36,
            .BackColor = Color.FromArgb(71, 126, 255),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        AddHandler productButton.Click, AddressOf ProductButton_Click

        reportButton = New Button() With {
            .Text = "Reports",
            .Location = New Point(1195, 22),
            .Width = 90,
            .Height = 36,
            .BackColor = Color.FromArgb(255, 160, 67),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        AddHandler reportButton.Click, AddressOf ReportsButton_Click

        headerPanel.Controls.Add(titleLabel)
        headerPanel.Controls.Add(searchBox)
        headerPanel.Controls.Add(productButton)
        headerPanel.Controls.Add(reportButton)
        Me.Controls.Add(headerPanel)

        customerLabel = New Label() With {
            .Text = "Walk-in Customer",
            .Font = New Font("Segoe UI", 14, FontStyle.Bold),
            .ForeColor = Color.FromArgb(40, 50, 70),
            .Location = New Point(20, 105),
            .AutoSize = True
        }
        Me.Controls.Add(customerLabel)

        categoryPanel = New FlowLayoutPanel() With {
            .Location = New Point(20, 145),
            .Width = 650,
            .Height = 80,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = False
        }
        Me.Controls.Add(categoryPanel)

        productPanel = New FlowLayoutPanel() With {
            .Location = New Point(20, 235),
            .Width = 650,
            .Height = 430,
            .AutoScroll = True,
            .BackColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle
        }
        Me.Controls.Add(productPanel)

        cartGrid = New DataGridView() With {
            .Location = New Point(700, 145),
            .Width = 520,
            .Height = 360,
            .ReadOnly = True,
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .RowHeadersVisible = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .BackgroundColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle
        }
        cartGrid.Columns.Add("Product", "Item")
        cartGrid.Columns.Add("Qty", "Qty")
        cartGrid.Columns.Add("Price", "Price")
        cartGrid.Columns.Add("Total", "Total")
        Me.Controls.Add(cartGrid)

        subtotalLabel = New Label() With {
            .Text = "Subtotal: $0.00",
            .Location = New Point(700, 525),
            .Font = New Font("Segoe UI", 12, FontStyle.Bold),
            .ForeColor = Color.FromArgb(45, 62, 82),
            .AutoSize = True
        }
        Me.Controls.Add(subtotalLabel)

        taxLabel = New Label() With {
            .Text = "Tax: $0.00",
            .Location = New Point(700, 555),
            .Font = New Font("Segoe UI", 12, FontStyle.Bold),
            .ForeColor = Color.FromArgb(45, 62, 82),
            .AutoSize = True
        }
        Me.Controls.Add(taxLabel)

        totalLabel = New Label() With {
            .Text = "Total: $0.00",
            .Location = New Point(700, 590),
            .Font = New Font("Segoe UI", 16, FontStyle.Bold),
            .ForeColor = Color.FromArgb(28, 165, 109),
            .AutoSize = True
        }
        Me.Controls.Add(totalLabel)

        cashButton = New Button() With {
            .Text = "Checkout",
            .Location = New Point(700, 630),
            .Width = 150,
            .Height = 42,
            .BackColor = Color.FromArgb(42, 184, 110),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        AddHandler cashButton.Click, AddressOf CheckoutButton_Click
        Me.Controls.Add(cashButton)

        removeButton = New Button() With {
            .Text = "Remove",
            .Location = New Point(870, 630),
            .Width = 120,
            .Height = 42,
            .BackColor = Color.FromArgb(255, 94, 83),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        AddHandler removeButton.Click, AddressOf RemoveButton_Click
        Me.Controls.Add(removeButton)

        clearButton = New Button() With {
            .Text = "Clear",
            .Location = New Point(1010, 630),
            .Width = 100,
            .Height = 42,
            .BackColor = Color.FromArgb(118, 131, 154),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        AddHandler clearButton.Click, AddressOf ClearButton_Click
        Me.Controls.Add(clearButton)

        quantityButton = New Button() With {
            .Text = "+ Qty",
            .Location = New Point(1125, 630),
            .Width = 95,
            .Height = 42,
            .BackColor = Color.FromArgb(80, 110, 255),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        AddHandler quantityButton.Click, AddressOf IncreaseQtyButton_Click
        Me.Controls.Add(quantityButton)
    End Sub

    Private Sub LoadCategories()
        categoryPanel.Controls.Clear()
        Dim categories = New List(Of String) From {"All", "Coffee", "Bakery", "Food", "Drinks"}
        For Each category In categories
            Dim button As New Button() With {
                .Text = category,
                .Width = 90,
                .Height = 38,
                .FlatStyle = FlatStyle.Flat,
                .BackColor = If(category = selectedCategory, Color.FromArgb(54, 95, 181), Color.FromArgb(233, 236, 242)),
                .ForeColor = If(category = selectedCategory, Color.White, Color.FromArgb(45, 62, 82))
            }
            AddHandler button.Click, Sub(sender, e) 
                selectedCategory = category
                LoadCategories()
                RefreshProductButtons()
            End Sub
            categoryPanel.Controls.Add(button)
        Next
    End Sub

    Private Sub RefreshProductButtons()
        productPanel.Controls.Clear()
        Dim filteredProducts = currentProducts
        If selectedCategory <> "All" Then
            filteredProducts = currentProducts.Where(Function(p) p.Category = selectedCategory).ToList()
        End If

        If Not String.IsNullOrWhiteSpace(searchBox.Text) Then
            Dim term = searchBox.Text.Trim().ToLowerInvariant()
            filteredProducts = filteredProducts.Where(Function(p) p.Name.ToLowerInvariant().Contains(term) OrElse p.Barcode.Contains(term)).ToList()
        End If

        For Each product In filteredProducts
            Dim card As New Button() With {
                .Text = product.Name & vbCrLf & product.Price.ToString("C") & vbCrLf & "Stock: " & product.Stock,
                .TextAlign = ContentAlignment.MiddleLeft,
                .Width = 180,
                .Height = 90,
                .FlatStyle = FlatStyle.Flat,
                .BackColor = Color.FromArgb(240, 243, 248),
                .ForeColor = Color.FromArgb(45, 62, 82),
                .Font = New Font("Segoe UI", 10, FontStyle.Bold),
                .Tag = product,
                .Padding = New Padding(8)
            }
            AddHandler card.Click, Sub(sender, e) AddToCart(DirectCast(DirectCast(sender, Button).Tag, Product))
            productPanel.Controls.Add(card)
        Next
    End Sub

    Private Sub SearchBox_TextChanged(sender As Object, e As EventArgs)
        RefreshProductButtons()
    End Sub

    Private Sub ProductButton_Click(sender As Object, e As EventArgs)
        Dim form As New ProductManagerForm()
        form.ShowDialog(Me)
        currentProducts = PosDataService.GetProducts()
        RefreshProductButtons()
    End Sub

    Private Sub ReportsButton_Click(sender As Object, e As EventArgs)
        Dim form As New ReportsForm(PosDataService.GetSales())
        form.ShowDialog(Me)
    End Sub

    Private Sub AddToCart(product As Product)
        If product Is Nothing Then Return

        Dim existing = cart.FirstOrDefault(Function(item) item.Product.Id = product.Id)
        If existing Is Nothing Then
            cart.Add(New OrderItem With {.Product = product, .Quantity = 1})
        Else
            existing.Quantity += 1
        End If

        RefreshCart()
    End Sub

    Private Sub RefreshCart()
        cartGrid.Rows.Clear()
        For Each item In cart
            cartGrid.Rows.Add(item.Product.Name, item.Quantity, item.Product.Price.ToString("C"), item.LineTotal.ToString("C"))
        Next

        Dim subtotal = cart.Sum(Function(item) item.LineTotal)
        Dim tax = subtotal * taxRate
        Dim total = subtotal + tax
        subtotalLabel.Text = "Subtotal: " & subtotal.ToString("C")
        taxLabel.Text = "Tax: " & tax.ToString("C")
        totalLabel.Text = "Total: " & total.ToString("C")
    End Sub

    Private Sub CheckoutButton_Click(sender As Object, e As EventArgs)
        If cart.Count = 0 Then
            MessageBox.Show("Please add at least one product to the cart.", "Empty Cart", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim subtotal = cart.Sum(Function(item) item.LineTotal)
        Dim tax = subtotal * taxRate
        Dim total = subtotal + tax

        Using paymentForm As New PaymentForm(total)
            If paymentForm.ShowDialog(Me) = DialogResult.OK Then
                Dim sale As New SaleRecord() With {
                    .CustomerName = paymentForm.CustomerName,
                    .PaymentMethod = paymentForm.PaymentMethod,
                    .CashPaid = paymentForm.CashPaid,
                    .Change = paymentForm.Change,
                    .Subtotal = subtotal,
                    .Tax = tax,
                    .Total = total,
                    .Items = cart.ToList()
                }
                PosDataService.SaveSale(sale)
                cart.Clear()
                RefreshCart()
                MessageBox.Show("Sale completed successfully.", "Complete Sale", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End Using
    End Sub

    Private Sub RemoveButton_Click(sender As Object, e As EventArgs)
        If cartGrid.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select an item in the cart.", "Remove Item", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim rowIndex = cartGrid.SelectedRows(0).Index
        If rowIndex >= 0 AndAlso rowIndex < cart.Count Then
            cart.RemoveAt(rowIndex)
            RefreshCart()
        End If
    End Sub

    Private Sub ClearButton_Click(sender As Object, e As EventArgs)
        cart.Clear()
        RefreshCart()
    End Sub

    Private Sub IncreaseQtyButton_Click(sender As Object, e As EventArgs)
        If cartGrid.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select an item to change quantity.", "Change Quantity", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim rowIndex = cartGrid.SelectedRows(0).Index
        If rowIndex >= 0 AndAlso rowIndex < cart.Count Then
            cart(rowIndex).Quantity += 1
            RefreshCart()
        End If
    End Sub
End Class
