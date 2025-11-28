using FluentAssertions;
using GameCharacterVsaCqrs.Features.Items;
using GameCharacterVsaCqrs.Tests.Testing;
using Xunit;

namespace GameCharacterVsaCqrs.Tests.Features.Items;

public class CreateItemTests
{
    [Fact]
    public async Task Handle_Persists_new_item()
    {
        using var context = DataContextFactory.CreateInMemoryContext();
        var handler = new CreateItem.Handler(context);

        var result = await handler.Handle(new CreateItem.Command("Sword", 15), CancellationToken.None);

        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be("Sword");
        result.Power.Should().Be(15);
        context.Items.Should().ContainSingle(i => i.Id == result.Id && i.Name == "Sword" && i.Power == 15);
    }
}
