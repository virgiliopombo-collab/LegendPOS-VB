Public Class SaleRecord
    Public Property Id As String
    Public Property CreatedAt As DateTime
    Public Property CustomerName As String
    Public Property PaymentMethod As String
    Public Property CashPaid As Decimal
    Public Property Change As Decimal
    Public Property Subtotal As Decimal
    Public Property Tax As Decimal
    Public Property Total As Decimal
    Public Property Items As List(Of OrderItem)

    Public Sub New()
        Id = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()
        CreatedAt = DateTime.Now
        CustomerName = "Walk-in"
        PaymentMethod = "Cash"
        CashPaid = 0D
        Change = 0D
        Subtotal = 0D
        Tax = 0D
        Total = 0D
        Items = New List(Of OrderItem)()
    End Sub
End Class
