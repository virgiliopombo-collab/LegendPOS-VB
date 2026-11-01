Imports System.Windows.Forms

Public Class PaymentForm
    Inherits Form

    Private ReadOnly amountDueLabel As Label
    Private ReadOnly paymentMethodCombo As ComboBox
    Private ReadOnly amountReceivedBox As TextBox
    Private ReadOnly customerNameBox As TextBox
    Private ReadOnly payButton As Button
    Private ReadOnly cancelButton As Button

    Public Property AmountDue As Decimal
    Public Property CashPaid As Decimal
    Public Property Change As Decimal
    Public Property PaymentMethod As String
    Public Property CustomerName As String

    Public Sub New(dueAmount As Decimal)
        Me.Text = "Payment"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Width = 420
        Me.Height = 260
        Me.AmountDue = dueAmount

        amountDueLabel = New Label() With {
            .Text = "Amount Due: $0.00",
            .Font = New Font("Segoe UI", 15, FontStyle.Bold),
            .AutoSize = True,
            .Location = New Point(20, 20)
        }

        Dim customerLabel As New Label() With {
            .Text = "Customer:",
            .Location = New Point(20, 70),
            .AutoSize = True
        }

        customerNameBox = New TextBox() With {
            .Location = New Point(120, 66),
            .Width = 240,
            .Text = "Walk-in"
        }

        Dim methodLabel As New Label() With {
            .Text = "Method:",
            .Location = New Point(20, 110),
            .AutoSize = True
        }

        paymentMethodCombo = New ComboBox() With {
            .Location = New Point(120, 106),
            .Width = 240,
            .DropDownStyle = ComboBoxStyle.DropDownList
        }
        paymentMethodCombo.Items.Add("Cash")
        paymentMethodCombo.Items.Add("Card")
        paymentMethodCombo.Items.Add("Mobile")
        paymentMethodCombo.SelectedIndex = 0

        Dim amountLabel As New Label() With {
            .Text = "Received:",
            .Location = New Point(20, 150),
            .AutoSize = True
        }

        amountReceivedBox = New TextBox() With {
            .Location = New Point(120, 146),
            .Width = 240,
            .Text = dueAmount.ToString("0.00")
        }

        payButton = New Button() With {
            .Text = "Complete Sale",
            .Location = New Point(120, 188),
            .Width = 110,
            .DialogResult = DialogResult.OK
        }
        AddHandler payButton.Click, AddressOf PayButton_Click

        cancelButton = New Button() With {
            .Text = "Cancel",
            .Location = New Point(250, 188),
            .Width = 110,
            .DialogResult = DialogResult.Cancel
        }

        Me.Controls.Add(amountDueLabel)
        Me.Controls.Add(customerLabel)
        Me.Controls.Add(customerNameBox)
        Me.Controls.Add(methodLabel)
        Me.Controls.Add(paymentMethodCombo)
        Me.Controls.Add(amountLabel)
        Me.Controls.Add(amountReceivedBox)
        Me.Controls.Add(payButton)
        Me.Controls.Add(cancelButton)

        UpdateAmountText()
    End Sub

    Private Sub PayButton_Click(sender As Object, e As EventArgs)
        Dim customerName = customerNameBox.Text.Trim()
        If String.IsNullOrWhiteSpace(customerName) Then customerName = "Walk-in"
        CustomerName = customerName

        PaymentMethod = paymentMethodCombo.SelectedItem?.ToString()
        If String.IsNullOrWhiteSpace(PaymentMethod) Then PaymentMethod = "Cash"

        Dim parsedAmount As Decimal
        If Not Decimal.TryParse(amountReceivedBox.Text, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, parsedAmount) Then
            MessageBox.Show("Please enter a valid amount.", "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Me.DialogResult = DialogResult.None
            Return
        End If

        CashPaid = parsedAmount
        Change = Math.Max(0D, CashPaid - AmountDue)

        If PaymentMethod <> "Cash" Then
            CashPaid = AmountDue
            Change = 0D
        End If

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub UpdateAmountText()
        amountDueLabel.Text = "Amount Due: " & AmountDue.ToString("C")
    End Sub
End Class
