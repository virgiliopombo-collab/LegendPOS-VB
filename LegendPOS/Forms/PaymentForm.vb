Imports System.IO
Imports System.Text.Json

Public Class PosDataService
    Private Shared ReadOnly DataFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LegendPOS")
    Private Shared ReadOnly DataFile As String = Path.Combine(DataFolder, "pos_data.json")

    Public Shared Function GetProducts() As List(Of Product)
        EnsureFileExists()
        Dim products = LoadProducts()
        If products.Count = 0 Then
            products = CreateSeedProducts()
            SaveProducts(products)
        End If
        Return products
    End Function

    Public Shared Function GetSales() As List(Of SaleRecord)
        EnsureFileExists()
        Return LoadSales()
    End Function

    Public Shared Sub SaveSale(sale As SaleRecord)
        Dim sales = GetSales()
        sales.Insert(0, sale)
        SaveSales(sales)
    End Sub

    Public Shared Sub SaveProduct(product As Product)
        Dim products = GetProducts()
        Dim existing = products.FirstOrDefault(Function(p) p.Id = product.Id)
        If existing Is Nothing Then
            products.Add(product)
        Else
            Dim index = products.IndexOf(existing)
            products(index) = product
        End If
        SaveProducts(products)
    End Sub

    Public Shared Sub DeleteProduct(productId As Integer)
        Dim products = GetProducts()
        products = products.Where(Function(p) p.Id <> productId).ToList()
        SaveProducts(products)
    End Sub

    Public Shared Function GetSalesTotalForToday() As Decimal
        Return GetSales().Where(Function(s) s.CreatedAt.Date = Date.Today).Sum(Function(s) s.Total)
    End Function

    Private Shared Sub EnsureFileExists()
        If Not Directory.Exists(DataFolder) Then
            Directory.CreateDirectory(DataFolder)
        End If

        If Not File.Exists(DataFile) Then
            File.WriteAllText(DataFile, "{\"Products\":[],\"Sales\":[]}")
        End If
    End Sub

    Private Shared Function LoadProducts() As List(Of Product)
        Dim json = File.ReadAllText(DataFile)
        If String.IsNullOrWhiteSpace(json) Then Return New List(Of Product)()

        Dim data = JsonSerializer.Deserialize(Of PosStore)(json)
        Return If(data?.Products, New List(Of Product)())
    End Function

    Private Shared Function LoadSales() As List(Of SaleRecord)
        Dim json = File.ReadAllText(DataFile)
        If String.IsNullOrWhiteSpace(json) Then Return New List(Of SaleRecord)()

        Dim data = JsonSerializer.Deserialize(Of PosStore)(json)
        Return If(data?.Sales, New List(Of SaleRecord)())
    End Function

    Private Shared Sub SaveProducts(products As List(Of Product))
        Dim data = New PosStore() With {
            .Products = products,
            .Sales = LoadSales()
        }
        File.WriteAllText(DataFile, JsonSerializer.Serialize(data, New JsonSerializerOptions With {.WriteIndented = True}))
    End Sub

    Private Shared Sub SaveSales(sales As List(Of SaleRecord))
        Dim data = New PosStore() With {
            .Products = LoadProducts(),
            .Sales = sales
        }
        File.WriteAllText(DataFile, JsonSerializer.Serialize(data, New JsonSerializerOptions With {.WriteIndented = True}))
    End Sub

    Private Shared Function CreateSeedProducts() As List(Of Product)
        Return New List(Of Product) From {
            New Product With {.Id = 1, .Name = "Cappuccino", .Category = "Coffee", .Price = 4.5D, .Barcode = "1001", .Stock = 200, .IsTaxable = True},
            New Product With {.Id = 2, .Name = "Latte", .Category = "Coffee", .Price = 5.5D, .Barcode = "1002", .Stock = 150, .IsTaxable = True},
            New Product With {.Id = 3, .Name = "Croissant", .Category = "Bakery", .Price = 3.25D, .Barcode = "2001", .Stock = 80, .IsTaxable = True},
            New Product With {.Id = 4, .Name = "Sandwich", .Category = "Food", .Price = 8.75D, .Barcode = "3001", .Stock = 60, .IsTaxable = True},
            New Product With {.Id = 5, .Name = "Mineral Water", .Category = "Drinks", .Price = 2.5D, .Barcode = "4001", .Stock = 120, .IsTaxable = True},
            New Product With {.Id = 6, .Name = "Burger", .Category = "Food", .Price = 11.5D, .Barcode = "3002", .Stock = 40, .IsTaxable = True},
            New Product With {.Id = 7, .Name = "Tea", .Category = "Drinks", .Price = 3.0D, .Barcode = "4002", .Stock = 100, .IsTaxable = True},
            New Product With {.Id = 8, .Name = "Chocolate Muffin", .Category = "Bakery", .Price = 4.0D, .Barcode = "2002", .Stock = 50, .IsTaxable = True}
        }
    End Function

    Private Class PosStore
        Public Property Products As List(Of Product)
        Public Property Sales As List(Of SaleRecord)

        Public Sub New()
            Products = New List(Of Product)()
            Sales = New List(Of SaleRecord)()
        End Sub
    End Class
End Class
