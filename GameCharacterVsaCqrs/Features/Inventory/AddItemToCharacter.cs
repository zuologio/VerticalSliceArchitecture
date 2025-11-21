using GameCharacterVsaCqrs.Data;
using GameCharacterVsaCqrs.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static GameCharacterVsaCqrs.Features.Inventory.AddItemToCharacter;

namespace GameCharacterVsaCqrs.Features.Inventory
{
    public static class AddItemToCharacter
    {
        public record Command(Guid CharacterId, Guid ItemId) : IRequest<bool>;

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly DataContext _dbContext;

            public Handler(DataContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                var characterExists = await _dbContext.GameCharacters.AnyAsync(c => c.Id == request.CharacterId, cancellationToken);
                var itemExists = await _dbContext.Items.AnyAsync(i => i.Id == request.ItemId, cancellationToken);
                if (!characterExists || !itemExists) return false;

                var alreadyOwned = await _dbContext.CharacterItems
                    .AnyAsync(ci => ci.GameCharacterId == request.CharacterId && ci.ItemId == request.ItemId, cancellationToken);
                if (alreadyOwned) return true;

                _dbContext.CharacterItems.Add(new CharacterItem
                {
                    GameCharacterId = request.CharacterId,
                    ItemId = request.ItemId
                });

                await _dbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
        }
    }

    [ApiController]
    [Route("api/characters")]
    public class AddItemToCharacterEndpoint : ControllerBase
    {
        private readonly IMediator _mediator;

        public AddItemToCharacterEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{characterId}/items/{itemId}")]
        public async Task<IActionResult> Add(Guid characterId, Guid itemId)
        {
            var success = await _mediator.Send(new Command(characterId, itemId));
            return success ? Ok(new { Message = "Item added to character." }) : NotFound();
        }
    }
}
