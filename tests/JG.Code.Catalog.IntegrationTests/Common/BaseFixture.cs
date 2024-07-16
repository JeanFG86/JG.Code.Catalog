using Bogus;

namespace JG.Code.Catalog.IntegrationTests.Common;
public class BaseFixture
{
    public BaseFixture() => Faker = new Faker("pt_BR");

    protected Faker Faker {  get; set; }

}
