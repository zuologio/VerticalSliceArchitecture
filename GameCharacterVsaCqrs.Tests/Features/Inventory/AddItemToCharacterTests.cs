using FluentAssertions;
using GameCharacterVsaCqrs.Entities;
using GameCharacterVsaCqrs.Features.Inventory;
using GameCharacterVsaCqrs.Tests.Testing;
using Xunit;

namespace GameCharacterVsaCqrs.Tests.Features.Inventory;

public class AddItemToCharacterTests
{
    [Fact]
    public async Task Handle_Adds_item_to_character_when_not_owned()
    {
        using var context = DataContextFactory.CreateInMemoryContext();
        var character = new GameCharacter { Name = "Nox", Class = "Mage" };
        var item = new Item { Name = "Staff", Power = 7 };
        context.GameCharacters.Add(character);
        context.Items.Add(item);
        await context.SaveChangesAsync();

        var handler = new AddItemToCharacter.Handler(context);

        var result = await handler.Handle(new AddItemToCharacter.Command(character.Id, item.Id), CancellationToken.None);

        result.Should().BeTrue();
        context.CharacterItems.Should().ContainSingle(ci => ci.GameCharacterId == character.Id && ci.ItemId == item.Id);
    }

    [Fact]
    public async Task Handle_Returns_false_when_character_or_item_missing()
    {
        using var context = DataContextFactory.CreateInMemoryContext();
        var handler = new AddItemToCharacter.Handler(context);
        var missingCharacter = Guid.NewGuid();
        var missingItem = Guid.NewGuid();

        var result = await handler.Handle(new AddItemToCharacter.Command(missingCharacter, missingItem), CancellationToken.None);

        result.Should().BeFalse();
        context.CharacterItems.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Does_not_duplicate_when_item_already_owned()
    {
        using var context = DataContextFactory.CreateInMemoryContext();
        var character = new GameCharacter { Name = "Quill", Class = "Cleric" };
        var item = new Item { Name = "Hammer", Power = 9 };
        var owned = new CharacterItem { GameCharacterId = character.Id, ItemId = item.Id };
        context.GameCharacters.Add(character);
        context.Items.Add(item);
        context.CharacterItems.Add(owned);
        await context.SaveChangesAsync();

        var handler = new AddItemToCharacter.Handler(context);

        var result = await handler.Handle(new AddItemToCharacter.Command(character.Id, item.Id), CancellationToken.None);

        result.Should().BeTrue();
        context.CharacterItems.Should().ContainSingle(ci => ci.GameCharacterId == character.Id && ci.ItemId == item.Id);
    }
}
