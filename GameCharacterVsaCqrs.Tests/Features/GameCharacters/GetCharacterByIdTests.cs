using FluentAssertions;
using GameCharacterVsaCqrs.Entities;
using GameCharacterVsaCqrs.Features.GameCharacters;
using GameCharacterVsaCqrs.Tests.Testing;
using Xunit;

namespace GameCharacterVsaCqrs.Tests.Features.GameCharacters;

public class GetCharacterByIdTests
{
    [Fact]
    public async Task Handle_Returns_character_when_it_exists()
    {
        using var context = DataContextFactory.CreateInMemoryContext();
        var existing = new GameCharacter { Name = "Ryn", Class = "Rogue", Level = 2 };
        context.GameCharacters.Add(existing);
        await context.SaveChangesAsync();

        var handler = new GetCharacterById.Handler(context);

        var result = await handler.Handle(new GetCharacterById.Query(existing.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(existing.Id);
        result.Name.Should().Be("Ryn");
        result.Class.Should().Be("Rogue");
        result.Level.Should().Be(2);
    }

    [Fact]
    public async Task Handle_Returns_null_when_character_is_missing()
    {
        using var context = DataContextFactory.CreateInMemoryContext();
        var handler = new GetCharacterById.Handler(context);

        var result = await handler.Handle(new GetCharacterById.Query(Guid.NewGuid()), CancellationToken.None);

        result.Should().BeNull();
    }
}
