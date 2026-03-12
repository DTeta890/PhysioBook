using FluentValidation.TestHelper;
using PhysioBook.Application.Patients.Queries;
using PhysioBook.Application.Patients.Validators;

namespace PhysioBook.UnitTests.Patients;

public class SearchPatientsValidatorTests
{
    private readonly SearchPatientsValidator _sut = new();

    private static SearchPatientsQuery ValidQuery() => new(
        SearchTerm: "John",
        Limit: 10);

    [Fact]
    public void Valid_Request_ShouldPass()
    {
        var result = _sut.TestValidate(ValidQuery());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_SearchTerm_ShouldFail()
    {
        var query = ValidQuery() with { SearchTerm = "" };
        var result = _sut.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.SearchTerm);
    }

    [Fact]
    public void SearchTerm_Too_Long_ShouldFail()
    {
        var query = ValidQuery() with { SearchTerm = new string('a', 101) };
        var result = _sut.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.SearchTerm);
    }

    [Fact]
    public void Limit_Zero_ShouldFail()
    {
        var query = ValidQuery() with { Limit = 0 };
        var result = _sut.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Limit);
    }

    [Fact]
    public void Limit_Greater_Than_50_ShouldFail()
    {
        var query = ValidQuery() with { Limit = 51 };
        var result = _sut.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Limit);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(25)]
    [InlineData(50)]
    public void Valid_Limit_ShouldPass(int limit)
    {
        var query = ValidQuery() with { Limit = limit };
        var result = _sut.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(x => x.Limit);
    }

    [Fact]
    public void SearchTerm_MaxLength_ShouldPass()
    {
        var query = ValidQuery() with { SearchTerm = new string('a', 100) };
        var result = _sut.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(x => x.SearchTerm);
    }

    [Fact]
    public void SearchTerm_SingleChar_ShouldPass()
    {
        var query = ValidQuery() with { SearchTerm = "a" };
        var result = _sut.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(x => x.SearchTerm);
    }
}
