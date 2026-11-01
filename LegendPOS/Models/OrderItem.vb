Public Class OrderItem
    Public Property Product As Product
    Public Property Quantity As Integer

    Public ReadOnly Property LineTotal As Decimal
        Get
            Return If(Product Is Nothing, 0D, Product.Price * Quantity)
        End Get
    End Property

    Public Sub New()
        Product = New Product()
        Quantity = 1
    End Sub
End Class
