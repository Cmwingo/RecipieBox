using Nancy;
using System.Collections.Generic;

namespace RecipieBox
{
  public class HomeModule : NancyModule
  {
    public HomeModule()
    {
      Get["/"] = _ => {
      return View["index.cshtml"];
      };

      Get["/recipies"] = _ => {
        List<Recipie> AllRecipies = Recipie.GetAll();
        return View["recipies.cshtml", AllRecipies];
      };

      Get["/tags"] = _ => {
        List<Tag> AllTags = Tag.GetAll();
        return View["tags.cshtml", AllTags];
      };

      Get["/recipies/new"] = _ => {
      return View["recipies_form.cshtml"];
      };

      Post["/recipies/new"] = _ => {
        Recipie newRecipie = new Recipie(Request.Form["recipie-description"]);
        newRecipie.Save();
        return View["success.cshtml"];
      };

      Get["/tags/new"] = _ => {
        return View["tags_form.cshtml"];
      };

      Post["/tags/new"] = _ => {
        Tag newTag = new Tag(Request.Form["tag-name"]);
        newTag.Save();
        return View["success.cshtml"];
      };

      Get["recipies/{id}"] = parameters => {
        Dictionary<string, object> model = new Dictionary<string, object>();
        Recipie SelectedRecipie = Recipie.Find(parameters.id);
        List<Tag> RecipieTags = SelectedRecipie.GetTags();
        List<Tag> AllTags = Tag.GetAll();
        List<Ingredient> RecipieIngredients = SelectedRecipie.GetIngredients();
        List<Instruction> RecipieInstructions = SelectedRecipie.GetInstructions();
        model.Add("recipie", SelectedRecipie);
        model.Add("recipieTags", RecipieTags);
        model.Add("allTags", AllTags);
        model.Add("recipieIngredients", RecipieIngredients);
        model.Add("recipieInstructions", RecipieInstructions);
        return View["recipie.cshtml", model];
      };

      Get["tags/{id}"] = parameters => {
        Dictionary<string, object> model = new Dictionary<string, object>();
        Tag SelectedTag = Tag.Find(parameters.id);
        List<Recipie> TagRecipies = SelectedTag.GetRecipies();
        List<Recipie> AllRecipies = Recipie.GetAll();
        model.Add("tag", SelectedTag);
        model.Add("tagRecipies", TagRecipies);
        model.Add("allRecipies", AllRecipies);
        return View["tag.cshtml", model];
      };

      Post["recipie/add_tag"] = _ => {
        Tag tag = Tag.Find(Request.Form["tag-id"]);
        Recipie recipie = Recipie.Find(Request.Form["recipie-id"]);
        recipie.AddTag(tag);
        return View["success.cshtml"];
      };

      Post["tag/add_recipie"] = _ => {
        Tag tag = Tag.Find(Request.Form["tag-id"]);
        Recipie recipie = Recipie.Find(Request.Form["recipie-id"]);
        tag.AddRecipie(recipie);
        return View["success.cshtml"];
      };

      Get["recipies/update/{id}"] = parameters =>
      {
        Recipie foundRecipie = Recipie.Find(parameters.id);
        return View["recipie_update.cshtml", foundRecipie];
      };

      Patch["recipies/update/{id}"] = parameters =>
      {
        Recipie foundRecipie = Recipie.Find(parameters.id);
        foundRecipie.Update(Request.Form["new-description"], Request.Form["new-rating"]);
        return View["success.cshtml"];
      };

      Get["tags/update/{id}"] = parameters =>
      {
        Tag foundTag = Tag.Find(parameters.id);
        return View["tag_update.cshtml", foundTag];
      };

      Patch["tags/update/{id}"] = parameters =>
      {
        Tag foundTag = Tag.Find(parameters.id);
        foundTag.Update(Request.Form["new-description"]);
        return View["success.cshtml"];
      };

      Delete["tag/delete/{id}"] = parameters =>
      {
        Tag foundTag = Tag.Find(parameters.id);
        foundTag.Delete();
        return View["success.cshtml"];
      };

      Delete["recipie/delete/{id}"] = parameters =>
      {
        Recipie foundRecipie = Recipie.Find(parameters.id);
        foundRecipie.Delete();
        return View["success.cshtml"];
      };

      Get["recipie/add_ingredient"] = _ => {
        Recipie selectedRecipie = Recipie.Find(Request.Query["recipie-id"]);
        return View["ingredients_form.cshtml", selectedRecipie];
      };

      Post["ingredients/new"] = _ => {
        Recipie selectedRecipie = Recipie.Find(Request.Form["recipie-id"]);
        Ingredient newIngredient = new Ingredient(Request.Form["ingredient-description"], Request.Form["recipie-id"], Request.Form["ingredient-quantity"], Request.Form["ingredient-unit"]);
        newIngredient.Save();
        return View["success.cshtml"];
      };

      Get["recipie/add_instruction"] = _ => {
        Recipie selectedRecipie = Recipie.Find(Request.Query["recipie-id"]);
        return View["instructions_form.cshtml", selectedRecipie];
      };

      Post["instructions/new"] = _ => {
        Recipie selectedRecipie = Recipie.Find(Request.Form["recipie-id"]);
        Instruction newInstruction = new Instruction(Request.Form["instruction-description"], Request.Form["recipie-id"], Request.Form["instruction-step-number"]);
        newInstruction.Save();
        return View["success.cshtml"];
      };

      Get["ingredients/update/{id}"] = parameters =>
      {
        Ingredient foundIngredient = Ingredient.Find(parameters.id);
        return View["ingredient_update.cshtml", foundIngredient];
      };

      Patch["ingredients/update/{id}"] = parameters =>
      {
        Ingredient foundIngredient = Ingredient.Find(parameters.id);
        foundIngredient.Update(Request.Form["new-description"], Request.Form["new-quantity"], Request.Form["new-unit"]);
        return View["success.cshtml"];
      };

      Get["instructions/update/{id}"] = parameters =>
      {
        Instruction foundInstruction = Instruction.Find(parameters.id);
        return View["instruction_update.cshtml", foundInstruction];
      };

      Patch["instructions/update/{id}"] = parameters =>
      {
        Instruction foundInstruction = Instruction.Find(parameters.id);
        foundInstruction.Update(Request.Form["new-description"], Request.Form["new-step-number"]);
        return View["success.cshtml"];
      };

      Delete["ingredients/delete/{id}"] = parameters =>
      {
        Ingredient foundIngredient = Ingredient.Find(parameters.id);
        foundIngredient.Delete();
        return View["success.cshtml"];
      };

      Delete["instructions/delete/{id}"] = parameters =>
      {
        Instruction foundInstruction = Instruction.Find(parameters.id);
        foundInstruction.Delete();
        return View["success.cshtml"];
      };
    }
  }
}
