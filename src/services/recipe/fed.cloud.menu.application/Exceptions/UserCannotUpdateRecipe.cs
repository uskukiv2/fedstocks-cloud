namespace fed.cloud.recipe.application.Exceptions;

public class UserCannotUpdateRecipeException : Exception
{
    public UserCannotUpdateRecipeException(Guid userId, Guid recipeId) : base(
        $"User {userId} don't have rights to update recipe {recipeId}")
    {
        
    }
}