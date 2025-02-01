namespace JG.Code.Catalog.UnitTests.Application.CastMember.UpdateCastMember;

public class UpdateCastMemberDataGenerator
{
    public static IEnumerable<object[]> GetCastMemberToUpdate(int times = 10)
    {
        var fixture = new UpdateCastMemberTestFixture();
        for (int i = 0; i < times; i++)
        {
            var exampleCategory = fixture.GetExampleCastMember();
            var exampleInput = fixture.GetValidInput(exampleCategory.Id);

            yield return new object[] { exampleCategory, exampleInput };
        }
    }
}