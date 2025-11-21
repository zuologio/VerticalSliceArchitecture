using GameCharacterVsaCqrs.Data;
using GameCharacterVsaCqrs.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static GameCharacterVsaCqrs.Features.Items.CreateItem;

namespace GameCharacterVsaCqrs.Features.Items
{
    public static class CreateItem
    {
        public record Command(string Name, int Power) : IRequest<Item>;

        public class Handler : IRequestHandler<Command, Item>
        {
            private readonly DataContext _dbContext;

            public Handler(DataContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<Item> Handle(Command request, CancellationToken cancellationToken)
            {
                var item = new Item
                {
                    Name = request.Name,
                    Power = request.Power
                };

                _dbContext.Items.Add(item);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return item;
            }
        }
    }

    [ApiController]
    [Route("api/items")]
    public class CreateItemEndpoint : ControllerBase
    {
        private readonly IMediator _mediator;

        public CreateItemEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Command command)
        {
            var item = await _mediator.Send(command);
            return CreatedAtAction(nameof(Create), new { id = item.Id }, item);
        }
    }
}
