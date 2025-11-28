using FluentAssertions;
using GameCharacterVsaCqrs.Entities;
using GameCharacterVsaCqrs.Features.GameCharacters;
using GameCharacterVsaCqrs.Tests.Testing;
using Xunit;

namespace GameCharacterVsaCqrs.Tests.Features.GameCharacters;

public class GetCharacterWithItemsTests
{
    [Fact]
    public async Task Handle_Returns_character_with_items()
    {
        using var context = DataContextFactory.CreateInMemoryContext();
        var character = new GameCharacter { Name = "Lyra", Class = "Ranger", Level = 5 };
        var item = new Item { Name = "Longbow", Power = 12 };
        var link = new CharacterItem
        {
            GameCharacterId = character.Id,
            ItemId = item.Id,
            GameCharacter = character,
            Item = item
        };
        context.GameCharacters.Add(character);
        context.Items.Add(item);
        context.CharacterItems.Add(link);
        await context.SaveChangesAsync();

        var handler = new GetCharacterWithItems.Handler(context);

        var result = await handler.Handle(new GetCharacterWithItems.Query(character.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(character.Id);
        result.Name.Should().Be("Lyra");
        result.Class.Should().Be("Ranger");
        result.Level.Should().Be(5);
        result.Items.Should().ContainSingle(i => i.Id == item.Id && i.Name == "Longbow" && i.Power == 12);
    }

    [Fact]
    public async Task Handle_Returns_null_when_character_not_found()
    {
        using var context = DataContextFactory.CreateInMemoryContext();
        var handler = new GetCharacterWithItems.Handler(context);

        var result = await handler.Handle(new GetCharacterWithItems.Query(Guid.NewGuid()), CancellationToken.None);

        result.Should().BeNull();
    }
}
