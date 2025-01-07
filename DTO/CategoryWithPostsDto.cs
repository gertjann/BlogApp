namespace BlogApp.DTO
{
    public class CategoryWithPostsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PostDto> Posts { get; set; }  // Postimet që i përkasin kësaj kategorie
    }
}
