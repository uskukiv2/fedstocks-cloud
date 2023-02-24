using fed.cloud.menu.data.Abstract;
using fed.cloud.menu.data.Entities;

namespace fed.cloud.menu.data.Repository;

public interface IRecipeRepository : IRepository<Recipe>
{
    IAsyncEnumerable<Recipe> GetAllByUserIdAsync(Guid userId);
    IQueryable<Recipe> GetByPageAsync(int size, int positions);

    Task DeleteRecipe(Guid id, CancellationToken ct);
}