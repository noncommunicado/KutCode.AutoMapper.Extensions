namespace Integration.Tests;

[TestFixture]
public class RealWorldScenarioTests
{
    private ServiceProvider _serviceProvider = null!;
    private IMapper _mapper = null!;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddMappings(typeof(RealWorldScenarioTests).Assembly);
        _serviceProvider = services.BuildServiceProvider();
        _mapper = _serviceProvider.GetRequiredService<IMapper>();
    }

    [TearDown]
    public void TearDown()
    {
        _serviceProvider?.Dispose();
    }

    [Test]
    public void ECommerce_OrderProcessing_CompleteWorkflow()
    {
        // Arrange - Create a complex e-commerce order
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = "ORD-2024-001",
            Customer = new Customer
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Address = new Address
                {
                    Street = "123 Main St",
                    City = "New York",
                    State = "NY",
                    ZipCode = "10001",
                    Country = "USA"
                }
            },
            Items = new List<OrderItem>
            {
                new() { ProductId = 1, ProductName = "Laptop", Quantity = 1, UnitPrice = 999.99m },
                new() { ProductId = 2, ProductName = "Mouse", Quantity = 2, UnitPrice = 29.99m }
            },
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            PaymentInfo = new PaymentInfo
            {
                Method = PaymentMethod.CreditCard,
                Amount = 1059.97m,
                Currency = "USD",
                TransactionId = "TXN-123456"
            }
        };

        // Act - Map to DTO for API response
        var orderDto = _mapper.Map<OrderDto>(order);

        // Assert - Verify complete mapping
        orderDto.Should().NotBeNull();
        orderDto.Id.Should().Be(order.Id);
        orderDto.OrderNumber.Should().Be("ORD-2024-001");
        orderDto.CustomerName.Should().Be("John Doe");
        orderDto.CustomerEmail.Should().Be("john.doe@example.com");
        orderDto.ShippingAddress.Should().Be("123 Main St, New York, NY 10001, USA");
        orderDto.Items.Should().HaveCount(2);
        orderDto.TotalAmount.Should().Be(1059.97m);
        orderDto.Status.Should().Be("Pending");
        orderDto.PaymentMethod.Should().Be("CreditCard");
    }

    [Test]
    public void UserManagement_ProfileUpdate_WithValidation()
    {
        // Arrange
        var userProfile = new UserProfile
        {
            Id = 123,
            Username = "johndoe",
            Email = "  JOHN.DOE@EXAMPLE.COM  ",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "+1-555-123-4567",
            DateOfBirth = new DateTime(1990, 5, 15),
            Preferences = new UserPreferences
            {
                Theme = "Dark",
                Language = "en-US",
                NotificationsEnabled = true,
                NewsletterSubscribed = false
            },
            Roles = new List<string> { "User", "Premium" }
        };

        // Act
        var userDto = _mapper.Map<UserProfileDto>(userProfile);

        // Assert
        userDto.Should().NotBeNull();
        userDto.Id.Should().Be(123);
        userDto.DisplayName.Should().Be("John Doe");
        userDto.Email.Should().Be("john.doe@example.com"); // Cleaned and normalized
        userDto.PhoneNumber.Should().Be("15551234567"); // Numbers only
        userDto.Age.Should().Be(DateTime.Now.Year - 1990);
        userDto.IsPremium.Should().BeTrue();
        userDto.PreferencesJson.Should().Contain("Dark");
    }

    [Test]
    public void FinancialReporting_TransactionAggregation_Success()
    {
        // Arrange
        var transactions = new List<Transaction>
        {
            new() { Id = 1, Amount = 100.50m, Type = TransactionType.Credit, Date = DateTime.Today.AddDays(-1), Category = "Sales" },
            new() { Id = 2, Amount = -25.00m, Type = TransactionType.Debit, Date = DateTime.Today.AddDays(-2), Category = "Expenses" },
            new() { Id = 3, Amount = 200.00m, Type = TransactionType.Credit, Date = DateTime.Today.AddDays(-3), Category = "Sales" },
            new() { Id = 4, Amount = -50.75m, Type = TransactionType.Debit, Date = DateTime.Today.AddDays(-4), Category = "Expenses" }
        };

        var account = new Account
        {
            Id = 1,
            AccountNumber = "ACC-001",
            Balance = 224.75m,
            Transactions = transactions,
            Owner = new Customer { FirstName = "Jane", LastName = "Smith", Email = "jane@example.com" }
        };

        // Act
        var reportDto = _mapper.Map<FinancialReportDto>(account);

        // Assert
        reportDto.Should().NotBeNull();
        reportDto.AccountNumber.Should().Be("ACC-001");
        reportDto.CurrentBalance.Should().Be(224.75m);
        reportDto.TotalCredits.Should().Be(300.50m);
        reportDto.TotalDebits.Should().Be(75.75m);
        reportDto.TransactionCount.Should().Be(4);
        reportDto.OwnerName.Should().Be("Jane Smith");
        reportDto.ReportGeneratedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Test]
    public void InventoryManagement_ProductCatalog_WithCategories()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Electronics",
            Description = "Electronic devices and accessories"
        };

        var products = new List<Product>
        {
            new() { Id = 1, Name = "Laptop", Price = 999.99m, Stock = 10, Category = category, IsActive = true },
            new() { Id = 2, Name = "Mouse", Price = 29.99m, Stock = 50, Category = category, IsActive = true },
            new() { Id = 3, Name = "Keyboard", Price = 79.99m, Stock = 0, Category = category, IsActive = false }
        };

        category.Products = products;

        // Act
        var catalogDto = _mapper.Map<ProductCatalogDto>(category);

        // Assert
        catalogDto.Should().NotBeNull();
        catalogDto.CategoryName.Should().Be("Electronics");
        catalogDto.TotalProducts.Should().Be(3);
        catalogDto.ActiveProducts.Should().Be(2);
        catalogDto.TotalValue.Should().Be(1109.97m); // Sum of all product values
        catalogDto.OutOfStockCount.Should().Be(1);
        catalogDto.Products.Should().HaveCount(3);
        catalogDto.Products.First().FormattedPrice.Should().Be("$999,99");
    }

    // Domain Models
    public class Order
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public Customer Customer { get; set; } = new();
        public List<OrderItem> Items { get; set; } = new();
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentInfo PaymentInfo { get; set; } = new();
    }

    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Address Address { get; set; } = new();
    }

    public class Address
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    public class OrderItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class PaymentInfo
    {
        public PaymentMethod Method { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
    }

    public enum OrderStatus { Pending, Processing, Shipped, Delivered, Cancelled }
    public enum PaymentMethod { CreditCard, DebitCard, PayPal, BankTransfer }

    // DTOs with custom mappings
    public class OrderDto : IHaveMap
    {
        public Guid Id { get; init; }
        public string OrderNumber { get; init; } = string.Empty;
        public string CustomerName { get; init; } = string.Empty;
        public string CustomerEmail { get; init; } = string.Empty;
        public string ShippingAddress { get; init; } = string.Empty;
        public List<OrderItemDto> Items { get; init; } = new();
        public decimal TotalAmount { get; init; }
        public string Status { get; init; } = string.Empty;
        public string PaymentMethod { get; init; } = string.Empty;
        public DateTime OrderDate { get; init; }

        public static void Map(Profile profile)
        {
            profile.CreateMap<Order, OrderDto>()
                .ForMember(d => d.CustomerName, opt => 
                    opt.MapFrom(s => $"{s.Customer.FirstName} {s.Customer.LastName}"))
                .ForMember(d => d.CustomerEmail, opt => 
                    opt.MapFrom(s => s.Customer.Email))
                .ForMember(d => d.ShippingAddress, opt => 
                    opt.MapFrom(s => $"{s.Customer.Address.Street}, {s.Customer.Address.City}, {s.Customer.Address.State} {s.Customer.Address.ZipCode}, {s.Customer.Address.Country}"))
                .ForMember(d => d.TotalAmount, opt => 
                    opt.MapFrom(s => s.PaymentInfo.Amount))
                .ForMember(d => d.Status, opt => 
                    opt.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.PaymentMethod, opt => 
                    opt.MapFrom(s => s.PaymentInfo.Method.ToString()));
        }
    }

    public class OrderItemDto : IMapWith<OrderItem>
    {
        public int ProductId { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal TotalPrice => Quantity * UnitPrice;
    }

    // User Management Models
    public class UserProfile
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public UserPreferences Preferences { get; set; } = new();
        public List<string> Roles { get; set; } = new();
    }

    public class UserPreferences
    {
        public string Theme { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public bool NotificationsEnabled { get; set; }
        public bool NewsletterSubscribed { get; set; }
    }

    public class UserProfileDto : IHaveMap
    {
        public int Id { get; init; }
        public string DisplayName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public int Age { get; init; }
        public bool IsPremium { get; init; }
        public string PreferencesJson { get; init; } = string.Empty;

        public static void Map(Profile profile)
        {
            profile.CreateMap<UserProfile, UserProfileDto>()
                .ForMember(d => d.DisplayName, opt => 
                    opt.MapFrom(s => $"{s.FirstName} {s.LastName}"))
                .ForMember(d => d.Email, opt => 
                    opt.MapFrom(s => s.Email.Trim().ToLowerInvariant()))
                .ForMember(d => d.PhoneNumber, opt => 
                    opt.MapFrom(s => new string(s.PhoneNumber.Where(char.IsDigit).ToArray())))
                .ForMember(d => d.Age, opt => 
                    opt.MapFrom(s => DateTime.Now.Year - s.DateOfBirth.Year))
                .ForMember(d => d.IsPremium, opt => 
                    opt.MapFrom(s => s.Roles.Contains("Premium")))
                .ForMember(d => d.PreferencesJson, opt => 
                    opt.MapFrom(s => System.Text.Json.JsonSerializer.Serialize(s.Preferences, (System.Text.Json.JsonSerializerOptions?)null)));
        }
    }

    // Financial Models
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; } = string.Empty;
    }

    public class Account
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public List<Transaction> Transactions { get; set; } = new();
        public Customer Owner { get; set; } = new();
    }

    public enum TransactionType { Credit, Debit }

    public class FinancialReportDto : IHaveMap
    {
        public string AccountNumber { get; init; } = string.Empty;
        public decimal CurrentBalance { get; init; }
        public decimal TotalCredits { get; init; }
        public decimal TotalDebits { get; init; }
        public int TransactionCount { get; init; }
        public string OwnerName { get; init; } = string.Empty;
        public DateTime ReportGeneratedAt { get; init; }

        public static void Map(Profile profile)
        {
            profile.CreateMap<Account, FinancialReportDto>()
                .ForMember(d => d.CurrentBalance, opt => 
                    opt.MapFrom(s => s.Balance))
                .ForMember(d => d.TotalCredits, opt => 
                    opt.MapFrom(s => s.Transactions.Where(t => t.Type == TransactionType.Credit).Sum(t => t.Amount)))
                .ForMember(d => d.TotalDebits, opt => 
                    opt.MapFrom(s => Math.Abs(s.Transactions.Where(t => t.Type == TransactionType.Debit).Sum(t => t.Amount))))
                .ForMember(d => d.TransactionCount, opt => 
                    opt.MapFrom(s => s.Transactions.Count))
                .ForMember(d => d.OwnerName, opt => 
                    opt.MapFrom(s => $"{s.Owner.FirstName} {s.Owner.LastName}"))
                .ForMember(d => d.ReportGeneratedAt, opt => 
                    opt.MapFrom(s => DateTime.UtcNow));
        }
    }

    // Inventory Models
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Product> Products { get; set; } = new();
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public Category Category { get; set; } = new();
        public bool IsActive { get; set; }
    }

    public class ProductCatalogDto : IHaveMap
    {
        public string CategoryName { get; init; } = string.Empty;
        public int TotalProducts { get; init; }
        public int ActiveProducts { get; init; }
        public decimal TotalValue { get; init; }
        public int OutOfStockCount { get; init; }
        public List<ProductDto> Products { get; init; } = new();

        public static void Map(Profile profile)
        {
            profile.CreateMap<Category, ProductCatalogDto>()
                .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.TotalProducts, opt => opt.MapFrom(s => s.Products.Count))
                .ForMember(d => d.ActiveProducts, opt => opt.MapFrom(s => s.Products.Count(p => p.IsActive)))
                .ForMember(d => d.TotalValue, opt => opt.MapFrom(s => s.Products.Sum(p => p.Price)))
                .ForMember(d => d.OutOfStockCount, opt => opt.MapFrom(s => s.Products.Count(p => p.Stock == 0)));
        }
    }

    public class ProductDto : IHaveMap
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string FormattedPrice { get; init; } = string.Empty;
        public int Stock { get; init; }
        public bool IsActive { get; init; }
        public string StockStatus { get; init; } = string.Empty;

        public static void Map(Profile profile)
        {
            profile.CreateMap<Product, ProductDto>()
                .ForMember(d => d.FormattedPrice, opt => opt.MapFrom(s => $"${s.Price:F2}"))
                .ForMember(d => d.StockStatus, opt => opt.MapFrom(s => 
                    s.Stock == 0 ? "Out of Stock" : 
                    s.Stock < 10 ? "Low Stock" : "In Stock"));
        }
    }
} 