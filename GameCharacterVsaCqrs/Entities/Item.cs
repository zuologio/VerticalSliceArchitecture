using System.ComponentModel.DataAnnotations;

namespace GameCharacterVsaCqrs.Entities
{
    public class Item
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public int Power { get; set; }

        // Navigation for reverse lookup
        public ICollection<CharacterItem> CharacterItems { get; set; } = new List<CharacterItem>();
    }
}
