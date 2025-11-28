using FluentAssertions;
using GameCharacterVsaCqrs.Features.GameCharacters;
using GameCharacterVsaCqrs.Tests.Testing;
using Xunit;

namespace GameCharacterVsaCqrs.Tests.Features.GameCharacters;

public class CreateCharacterTests
{
    [Fact]
    public async Task Handle_Persists_new_character_with_initial_level()
    {
        using var context = DataContextFactory.CreateInMemoryContext();
        var handler = new CreateCharacter.Handler(context);

        var result = await handler.Handle(new CreateCharacter.Command("Aria", "Mage"), CancellationToken.None);

        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be("Aria");
        result.Class.Should().Be("Mage");
        result.Level.Should().Be(1);
        context.GameCharacters.Should().ContainSingle(c => c.Id == result.Id && c.Name == "Aria" && c.Class == "Mage" && c.Level == 1);
    }
}
