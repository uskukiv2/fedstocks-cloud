using fed.cloud.menu.infrastructure.Configurations.RepoDb;
using RepoDb;

namespace fed.cloud.menu.infrastructure.Utilities;

public static class RepoDbUtilities
{
    public static void MapAllEntities()
    {
        var repoDbClassMapper = new RepoDbClassMapper();
        
        repoDbClassMapper.MapRecipeModels();
        
        repoDbClassMapper.MapUnitEntity();
        repoDbClassMapper.MapIngredientEntity();
        repoDbClassMapper.MapRecipeEntity();
        repoDbClassMapper.MapRecipeIngredientEntity();
        repoDbClassMapper.MapUserEntity();
    }
}