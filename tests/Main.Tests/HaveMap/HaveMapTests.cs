namespace Main.Tests.HaveMap;

[TestFixture]
public class HaveMapTests
{
    private record A(string Value);
    private record B(string Value) : IHaveMap
    {
        public static void Map(Profile profile)
        {
            profile.CreateMap<A, B>().ReverseMap();
            profile.CreateMap<C, D>().ReverseMap();
        }
    }
    private record C(string Value) : IHaveMap
    {
        public A Internal { get; set; }
        public static void Map(Profile profile)
        {
            profile.CreateMap<A, D>().ReverseMap();
        }
    }
    private record D(string Value) : IHaveMap
    {
        public B Internal { get; set; }
        public static void Map(Profile profile)
        {
            profile.CreateMap<C, B>().ReverseMap();
        }
    }
    
    [Test]
    public void TwoNotConcurrentMappings_Success()
    {
        AssemblyMappingProfile profile = new(typeof(A), typeof(B));
        IMapper mapper = new Mapper(new MapperConfiguration(c => {
            c.AddProfile(profile);
        }));
		
        var result = mapper.Map<A>(new B ("Model"));
        result.Should().NotBeNull();
        result.Value.Should().Be("Model");
    }

    [Test]
    public void MapDeclaration_InThirdPartyType_Success()
    {
        AssemblyMappingProfile profile = new(typeof(A), typeof(B), typeof(C), typeof(D));
        IMapper mapper = new Mapper(new MapperConfiguration(c => {
            c.AddProfile(profile);
        }));
		
        var a_result = mapper.Map<A>(new D ("D"));
        a_result.Should().NotBeNull();
        a_result.Value.Should().Be("D");
        
        var d_result = mapper.Map<D>(new A ("A"));
        d_result.Should().NotBeNull();
        d_result.Value.Should().Be("A");
    }
    
    [Test]
    public void ComplicatedMapping_Success()
    {
        AssemblyMappingProfile profile = new(typeof(A), typeof(B), typeof(C), typeof(D));
        IMapper mapper = new Mapper(new MapperConfiguration(c => {
            c.AddProfile(profile);
        }));
		
        var result = mapper.Map<C>(new D ("D") {
            Internal = new B("B")
        });
        result.Should().NotBeNull();
        result.Value.Should().Be("D");
        result.Internal.Value.Should().Be("B");
    }
}