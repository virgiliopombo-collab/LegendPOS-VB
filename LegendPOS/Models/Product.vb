Public Class Product
    Public Property Id As Integer
    Public Property Name As String
    Public Property Category As String
    Public Property Price As Decimal
    Public Property Barcode As String
    Public Property Stock As Integer
    Public Property IsTaxable As Boolean

    Public Sub New()
        Name = String.Empty
        Category = String.Empty
        Barcode = String.Empty
    End Sub
End Class
