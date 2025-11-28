using FluentAssertions;
using GameCharacterVsaCqrs.Entities;
using GameCharacterVsaCqrs.Features.GameCharacters;
using GameCharacterVsaCqrs.Tests.Testing;
using Xunit;

namespace GameCharacterVsaCqrs.Tests.Features.GameCharacters;

public class LevelUpCharacterTests
{
    [Fact]
    public async Task Handle_Increments_level_when_character_exists()
    {
        using var context = DataContextFactory.CreateInMemoryContext();
        var character = new GameCharacter { Name = "Thorn", Class = "Warrior", Level = 3 };
        context.GameCharacters.Add(character);
        await context.SaveChangesAsync();

        var handler = new LevelUpCharacter.Handler(context);

        var result = await handler.Handle(new LevelUpCharacter.Command(character.Id), CancellationToken.None);

        result.Should().BeTrue();
        context.GameCharacters.Single().Level.Should().Be(4);
    }

    [Fact]
    public async Task Handle_Returns_false_when_character_missing()
    {
        using var context = DataContextFactory.CreateInMemoryContext();
        var handler = new LevelUpCharacter.Handler(context);

        var result = await handler.Handle(new LevelUpCharacter.Command(Guid.NewGuid()), CancellationToken.None);

        result.Should().BeFalse();
        context.GameCharacters.Should().BeEmpty();
    }
}
