Imports System.Windows.Forms

Public Class ReportsForm
    Inherits Form

    Private ReadOnly grid As DataGridView
    Private ReadOnly totalLabel As Label

    Public Sub New(sales As List(Of SaleRecord))
        Me.Text = "Sales Reports"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Width = 780
        Me.Height = 520
        Me.FormBorderStyle = FormBorderStyle.FixedDialog

        totalLabel = New Label() With {
            .Text = "Total Sales: $0.00",
            .Font = New Font("Segoe UI", 14, FontStyle.Bold),
            .Location = New Point(20, 20),
            .AutoSize = True
        }

        grid = New DataGridView() With {
            .Location = New Point(20, 60),
            .Width = 720,
            .Height = 400,
            .ReadOnly = True,
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .AutoGenerateColumns = False,
            .RowHeadersVisible = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
        }

        grid.Columns.Add("DateColumn", "Date")
        grid.Columns.Add("CustomerColumn", "Customer")
        grid.Columns.Add("PaymentColumn", "Payment")
        grid.Columns.Add("TotalColumn", "Total")

        For Each col As DataGridViewColumn In grid.Columns
            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        Next

        Me.Controls.Add(totalLabel)
        Me.Controls.Add(grid)

        LoadSales(sales)
    End Sub

    Private Sub LoadSales(sales As List(Of SaleRecord))
        Dim rows = sales.Select(Function(s) New With {
            .Date = s.CreatedAt.ToString("MM/dd/yyyy HH:mm"),
            .Customer = s.CustomerName,
            .Payment = s.PaymentMethod,
            .Total = s.Total.ToString("C")
        }).ToList()

        grid.Rows.Clear()
        For Each row In rows
            grid.Rows.Add(row.Date, row.Customer, row.Payment, row.Total)
        Next

        Dim total = If(sales Is Nothing, 0D, sales.Sum(Function(s) s.Total))
        totalLabel.Text = "Total Sales: " & total.ToString("C")
    End Sub
End Class
