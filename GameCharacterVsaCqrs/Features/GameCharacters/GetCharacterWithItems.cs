using GameCharacterVsaCqrs.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static GameCharacterVsaCqrs.Features.GameCharacters.GetCharacterWithItems;

namespace GameCharacterVsaCqrs.Features.GameCharacters
{
    public static class GetCharacterWithItems
    {
        public record Query(Guid Id) : IRequest<CharacterWithItemsDto?>;

        public class Handler : IRequestHandler<Query, CharacterWithItemsDto?>
        {
            private readonly DataContext _dbContext;

            public Handler(DataContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<CharacterWithItemsDto?> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _dbContext.GameCharacters
                    .Where(c => c.Id == request.Id)
                    .Select(c => new CharacterWithItemsDto(
                        c.Id,
                        c.Name,
                        c.Class,
                        c.Level,
                        c.CharacterItems
                            .Select(ci => new ItemDto(ci.ItemId, ci.Item != null ? ci.Item.Name : string.Empty, ci.Item != null ? ci.Item.Power : 0))
                            .ToList()))
                    .FirstOrDefaultAsync(cancellationToken);
            }
        }

        public record ItemDto(Guid Id, string Name, int Power);
        public record CharacterWithItemsDto(Guid Id, string Name, string Class, int Level, List<ItemDto> Items);
    }

    [ApiController]
    [Route("api/characters")]
    public class GetCharacterWithItemsEndpoint : ControllerBase
    {
        private readonly IMediator _mediator;

        public GetCharacterWithItemsEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}/with-items")]
        public async Task<IActionResult> Get(Guid id)
        {
            var character = await _mediator.Send(new Query(id));
            return character != null ? Ok(character) : NotFound();
        }
    }
}
