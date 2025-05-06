using EatUp.Meals.Models;

namespace EatUp.Meals.DTO
{
    public class AddCategoryDTO
    {
        public string Name { get; set; }

        public Category ToCategory()
        {
            return new Category
            {
                Name = Name,
            };
        }
    }

}
