namespace GameCharacterVsaCqrs.Entities
{
    public class CharacterItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid GameCharacterId { get; set; }
        public Guid ItemId { get; set; }

        public GameCharacter? GameCharacter { get; set; }
        public Item? Item { get; set; }
    }
}
