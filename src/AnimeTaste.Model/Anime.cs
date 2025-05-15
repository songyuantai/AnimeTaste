using AnimeTaste.Model.Const;

namespace AnimeTaste.Model
{
    public class Anime
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Alias { get; set; }

        public string? Description { get; set; }

        public int? SeasonId { get; set; }

        public AnimeStatus Status { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
