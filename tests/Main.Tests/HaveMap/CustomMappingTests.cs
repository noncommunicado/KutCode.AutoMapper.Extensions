namespace Main.Tests.HaveMap;

[TestFixture]
public class CustomMappingTests
{
    [Test]
    public void HaveMap_WithCustomTransformation_Success()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(TransformationDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var source = new TransformationSource
        {
            FirstName = "John",
            LastName = "Doe",
            BirthDate = new DateTime(1990, 1, 1),
            Salary = 50000m
        };
        
        // Act
        var result = mapper.Map<TransformationDto>(source);
        
        // Assert
        result.Should().NotBeNull();
        result.FullName.Should().Be("John Doe");
        result.Age.Should().Be(DateTime.Now.Year - 1990);
        result.IsHighEarner.Should().BeFalse(); // 50000 < 75000 threshold
    }

    [Test]
    public void HaveMap_WithConditionalMapping_Success()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(ConditionalDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var activeSource = new ConditionalSource { Status = "Active", Value = "Important" };
        var inactiveSource = new ConditionalSource { Status = "Inactive", Value = "Secret" };
        
        // Act
        var activeResult = mapper.Map<ConditionalDto>(activeSource);
        var inactiveResult = mapper.Map<ConditionalDto>(inactiveSource);
        
        // Assert
        activeResult.Should().NotBeNull();
        activeResult.DisplayValue.Should().Be("Important"); // Active shows value
        
        inactiveResult.Should().NotBeNull();
        inactiveResult.DisplayValue.Should().Be("Hidden"); // Inactive hides value
    }

    [Test]
    public void HaveMap_WithMultipleSourceMappings_Success()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(MultiSourceDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var sourceA = new SourceA { Id = 1, Name = "Source A" };
        var sourceB = new SourceB { Id = 2, Description = "Source B Description" };
        
        // Act
        var resultFromA = mapper.Map<MultiSourceDto>(sourceA);
        var resultFromB = mapper.Map<MultiSourceDto>(sourceB);
        
        // Assert
        resultFromA.Should().NotBeNull();
        resultFromA.Id.Should().Be(1);
        resultFromA.DisplayText.Should().Be("Source A");
        
        resultFromB.Should().NotBeNull();
        resultFromB.Id.Should().Be(2);
        resultFromB.DisplayText.Should().Be("Source B Description");
    }

    [Test]
    public void HaveMap_WithNestedCustomMapping_Success()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(NestedCustomDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var source = new NestedCustomSource
        {
            PersonInfo = new PersonInfo { FirstName = "John", LastName = "Doe" },
            ContactInfo = new ContactInfo { Email = "john@example.com", Phone = "+1234567890" }
        };
        
        // Act
        var result = mapper.Map<NestedCustomDto>(source);
        
        // Assert
        result.Should().NotBeNull();
        result.FullName.Should().Be("John Doe");
        result.Email.Should().Be("john@example.com");
        result.FormattedPhone.Should().Be("(123) 456-7890");
    }

    [Test]
    public void HaveMap_WithValidationAndCleaning_Success()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(ValidatedDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var source = new ValidatedSource
        {
            Email = "  JOHN@EXAMPLE.COM  ",
            PhoneNumber = "1-234-567-8900",
            Website = "http://example.com"
        };
        
        // Act
        var result = mapper.Map<ValidatedDto>(source);
        
        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("john@example.com"); // Trimmed and lowercased
        result.PhoneNumber.Should().Be("12345678900"); // Numbers only
        result.Website.Should().Be("https://example.com"); // Converted to HTTPS
    }

    [Test]
    public void HaveMap_WithComplexBusinessLogic_Success()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(BusinessLogicDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var source = new BusinessLogicSource
        {
            BasePrice = 100m,
            Quantity = 5,
            CustomerType = "Premium",
            OrderDate = DateTime.Now.AddDays(-5)
        };
        
        // Act
        var result = mapper.Map<BusinessLogicDto>(source);
        
        // Assert
        result.Should().NotBeNull();
        result.TotalPrice.Should().Be(450m); // 100 * 5 * 0.9 (10% premium discount)
        result.DiscountApplied.Should().Be(0.10m);
        result.CustomerTier.Should().Be("Premium");
        result.IsRecentOrder.Should().BeTrue();
    }

    // Test classes
    public class TransformationSource
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public decimal Salary { get; set; }
    }

    public class TransformationDto : IHaveMap
    {
        public string FullName { get; init; } = string.Empty;
        public int Age { get; init; }
        public bool IsHighEarner { get; init; }

        public static void Map(Profile profile)
        {
            profile.CreateMap<TransformationSource, TransformationDto>()
                .ForMember(d => d.FullName, opt => 
                    opt.MapFrom(s => $"{s.FirstName} {s.LastName}"))
                .ForMember(d => d.Age, opt => 
                    opt.MapFrom(s => DateTime.Now.Year - s.BirthDate.Year))
                .ForMember(d => d.IsHighEarner, opt => 
                    opt.MapFrom(s => s.Salary > 75000));
        }
    }

    public class ConditionalSource
    {
        public string Status { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class ConditionalDto : IHaveMap
    {
        public string DisplayValue { get; init; } = string.Empty;

        public static void Map(Profile profile)
        {
            profile.CreateMap<ConditionalSource, ConditionalDto>()
                .ForMember(d => d.DisplayValue, opt => 
                    opt.MapFrom(s => s.Status == "Active" ? s.Value : "Hidden"));
        }
    }

    public class SourceA
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class SourceB
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class MultiSourceDto : IHaveMap
    {
        public int Id { get; init; }
        public string DisplayText { get; init; } = string.Empty;

        public static void Map(Profile profile)
        {
            profile.CreateMap<SourceA, MultiSourceDto>()
                .ForMember(d => d.DisplayText, opt => opt.MapFrom(s => s.Name));
                
            profile.CreateMap<SourceB, MultiSourceDto>()
                .ForMember(d => d.DisplayText, opt => opt.MapFrom(s => s.Description));
        }
    }

    public class PersonInfo
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class ContactInfo
    {
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class NestedCustomSource
    {
        public PersonInfo PersonInfo { get; set; } = new();
        public ContactInfo ContactInfo { get; set; } = new();
    }

    public class NestedCustomDto : IHaveMap
    {
        public string FullName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string FormattedPhone { get; init; } = string.Empty;

        public static void Map(Profile profile)
        {
            profile.CreateMap<NestedCustomSource, NestedCustomDto>()
                .ForMember(d => d.FullName, opt => 
                    opt.MapFrom(s => $"{s.PersonInfo.FirstName} {s.PersonInfo.LastName}"))
                .ForMember(d => d.Email, opt => 
                    opt.MapFrom(s => s.ContactInfo.Email))
                .ForMember(d => d.FormattedPhone, opt => 
                    opt.MapFrom(s => FormatPhone(s.ContactInfo.Phone)));
        }

        private static string FormatPhone(string phone)
        {
            // Simple phone formatting: +1234567890 -> (123) 456-7890
            var digits = new string(phone.Where(char.IsDigit).ToArray());
            if (digits.Length >= 10)
            {
                return $"({digits.Substring(0, 3)}) {digits.Substring(3, 3)}-{digits.Substring(6, 4)}";
            }
            return phone;
        }
    }

    public class ValidatedSource
    {
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
    }

    public class ValidatedDto : IHaveMap
    {
        public string Email { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public string Website { get; init; } = string.Empty;

        public static void Map(Profile profile)
        {
            profile.CreateMap<ValidatedSource, ValidatedDto>()
                .ForMember(d => d.Email, opt => 
                    opt.MapFrom(s => s.Email.Trim().ToLowerInvariant()))
                .ForMember(d => d.PhoneNumber, opt => 
                    opt.MapFrom(s => new string(s.PhoneNumber.Where(char.IsDigit).ToArray())))
                .ForMember(d => d.Website, opt => 
                    opt.MapFrom(s => s.Website.Replace("http://", "https://")));
        }
    }

    public class BusinessLogicSource
    {
        public decimal BasePrice { get; set; }
        public int Quantity { get; set; }
        public string CustomerType { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
    }

    public class BusinessLogicDto : IHaveMap
    {
        public decimal TotalPrice { get; init; }
        public decimal DiscountApplied { get; init; }
        public string CustomerTier { get; init; } = string.Empty;
        public bool IsRecentOrder { get; init; }

        public static void Map(Profile profile)
        {
            profile.CreateMap<BusinessLogicSource, BusinessLogicDto>()
                .ForMember(d => d.TotalPrice, opt => 
                    opt.MapFrom(s => CalculateTotalPrice(s)))
                .ForMember(d => d.DiscountApplied, opt => 
                    opt.MapFrom(s => GetDiscount(s.CustomerType)))
                .ForMember(d => d.CustomerTier, opt => 
                    opt.MapFrom(s => s.CustomerType))
                .ForMember(d => d.IsRecentOrder, opt => 
                    opt.MapFrom(s => (DateTime.Now - s.OrderDate).TotalDays <= 7));
        }

        private static decimal CalculateTotalPrice(BusinessLogicSource source)
        {
            var subtotal = source.BasePrice * source.Quantity;
            var discount = GetDiscount(source.CustomerType);
            return subtotal * (1 - discount);
        }

        private static decimal GetDiscount(string customerType)
        {
            return customerType switch
            {
                "Premium" => 0.10m,
                "Gold" => 0.05m,
                "Silver" => 0.02m,
                _ => 0m
            };
        }
    }
} 