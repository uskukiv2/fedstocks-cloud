using System.Text;
using System.Text.Json;
using fed.cloud.communication.Recipe;
using gen.common.Extensions;
using gen.fedstocks.web.Client.Application.Extensions;
using gen.fedstocks.web.Client.Application.Interfaces;
using gen.fedstocks.web.Client.Application.Models;
using gen.fedstocks.web.Client.Application.Models.Recipes;
using MapsterMapper;

namespace gen.fedstocks.web.Client.Application.Services.Implementation;

public class RecipeService : IRecipeService
{
    private readonly IMapper _mapper;
    private readonly IApplicationStorage _storage;
    private readonly IHttpClientFactory _clientFactory;

    public RecipeService(IMapper mapper, IHttpClientFactory clientFactory, IApplicationStorage storage)
    {
        _mapper = mapper;
        _storage = storage;
        _clientFactory = clientFactory;
    }

    public async Task<RecipeDto?> GetRecipeAsync(int userId, int recipeId)
    {
        return (await _storage.GetItemsAsync<RecipeDto>(x => x.Id == recipeId)).FirstOrDefault();
    }

    //TODO: do not forget refactor it
    public async Task<IEnumerable<RecipeDto>> GetRecipesAsync(int userId, int recipePageSize, int skipRecipes)
    {
        var httpClient = _clientFactory.CreateClient(AppConstValues.ServerAPI);
        var response = await httpClient.GetAsync($"cloud/api/recipe?size={recipePageSize}&next={skipRecipes}");
        if (!response.IsSuccessStatusCode) return Array.Empty<RecipeDto>();
        
        await using var responseStream = await response.Content.ReadAsStreamAsync();
        var data = (await JsonSerializer.DeserializeAsync<IEnumerable<Recipe>>(responseStream)).ToDto();
        await _storage.SetItemAsync(data);
        return data;
    }

    public async Task<int> SaveChangesAsync(RecipeDto recipe, int userId)
    {
        IList<RecipeDto> recipes = new List<RecipeDto>(await _storage.GetItemsAsync<RecipeDto>());
        if (recipe.IsNew)
        {
            recipe.Id = recipes.Count + 1;
            recipes.Add(recipe);
        }
        else
        {
            var existRecipe = recipes.FirstOrDefault(x => x.Id == recipe.Id)!;
            recipes.Remove(existRecipe);
            recipes.Add(recipe);  
        }

        await _storage.SetItemAsync(recipes);

        await SaveRemoteAsync(recipe).Forget();

        return recipe.Id;
    }

    private async Task SaveRemoteAsync(RecipeDto recipe)
    {
        var httpClient = _clientFactory.CreateClient(AppConstValues.ServerAPI);
        if (recipe.IsNew)
        {
            using var content = new StringContent(JsonSerializer.Serialize(_mapper.Map<Recipe>(recipe)), Encoding.UTF8,
                "application/json");
            var responseNew = await httpClient.PostAsync("cloud/api/recipe", content);
            responseNew.EnsureSuccessStatusCode();
            
            return;
        }
        
        using var updateContent = new StringContent(JsonSerializer.Serialize(_mapper.Map<Recipe>(recipe)), Encoding.UTF8,
            "application/json");
        var responseUpdate = await httpClient.PatchAsync("cloud/api/recipe", updateContent);
        responseUpdate.EnsureSuccessStatusCode();
    }
}