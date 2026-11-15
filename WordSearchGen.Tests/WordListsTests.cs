using WordSearchGen.Core;

namespace WordSearchGen.Tests;

public class WordListsTests
{
    public static TheoryData<string> ListNames => ["Insects", "Food", "Heroes"];

    private static IReadOnlyList<string> Get(string name) => name switch
    {
        "Insects" => WordLists.Insects,
        "Food" => WordLists.Food,
        "Heroes" => WordLists.Heroes,
        _ => throw new ArgumentOutOfRangeException(nameof(name))
    };

    [Theory]
    [MemberData(nameof(ListNames))]
    public void EachList_HasTenDistinctLetterOnlyWords(string name)
    {
        var list = Get(name);

        Assert.Equal(10, list.Count);
        Assert.Equal(list.Count, list.Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.All(list, w => Assert.Matches("^[a-z]+$", w));
    }

    [Fact]
    public void Current_IsOneOfTheLists()
    {
        Assert.Contains(WordLists.Current, new[] { WordLists.Insects, WordLists.Food, WordLists.Heroes });
    }
}
