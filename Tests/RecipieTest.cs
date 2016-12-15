using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace RecipieBox
{
  public class RecipieTest : IDisposable
  {
    public RecipieTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=recipie_box_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_EmptyAtFirst()
    {
      //Arrange, Act
      int result = Recipie.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public void Test_EqualOverrideTrueForSameName()
    {
      //Arrange, Act
      Recipie firstRecipie = new Recipie("Name");
      Recipie secondRecipie = new Recipie("Name");

      //Assert
      Assert.Equal(firstRecipie, secondRecipie);
    }

    [Fact]
    public void Test_Save()
    {
      //Arrange
      Recipie testRecipie = new Recipie("Name");
      testRecipie.Save();

      //Act
      List<Recipie> result = Recipie.GetAll();
      List<Recipie> testList = new List<Recipie>{testRecipie};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_SaveAssignsIdToObject()
    {
      //Arrange
      Recipie testRecipie = new Recipie("Name");
      testRecipie.Save();

      //Act
      Recipie savedRecipie = Recipie.GetAll()[0];

      int result = savedRecipie.GetId();
      int testId = testRecipie.GetId();

      //Assert
      Assert.Equal(testId, result);
    }

    [Fact]
    public void Test_FindFindsRecipieInDatabase()
    {
      //Arrange
      Recipie testRecipie = new Recipie("Name");
      testRecipie.Save();

      //Act
      Recipie result = Recipie.Find(testRecipie.GetId());

      //Assert
      Assert.Equal(testRecipie, result);
    }
    [Fact]
    public void Test_AddTag_AddsTagToRecipie()
    {
      //Arrange
      Recipie testRecipie = new Recipie("Name");
      testRecipie.Save();

      Tag testTag = new Tag("Other object name");
      testTag.Save();

      //Act
      testRecipie.AddTag(testTag);

      List<Tag> result = testRecipie.GetTags();
      List<Tag> testList = new List<Tag>{testTag};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_GetTags_ReturnsAllRecipieTags()
    {
      //Arrange
      Recipie testRecipie = new Recipie("Name");
      testRecipie.Save();

      Tag testTag1 = new Tag("Other object name");
      testTag1.Save();

      Tag testTag2 = new Tag("Another object name");
      testTag2.Save();

      //Act
      testRecipie.AddTag(testTag1);
      List<Tag> result = testRecipie.GetTags();
      List<Tag> testList = new List<Tag> {testTag1};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_GetIngredients_ReturnsAllRecipieIngredients()
    {
      //Arrange
      Recipie testRecipie = new Recipie("Pizza");
      testRecipie.Save();

      Ingredient testIngredient1 = new Ingredient("Cheese", testRecipie.GetId());
      testIngredient1.Save();

      Ingredient testIngredient2 = new Ingredient("Tomato", testRecipie.GetId());
      testIngredient2.Save();

      //Act
      List<Ingredient> result = testRecipie.GetIngredients();
      List<Ingredient> testList = new List<Ingredient> {testIngredient1, testIngredient2};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_Delete_DeletesRecipieAssociationsFromDatabase()
    {
      //Arrange
      Tag testTag = new Tag("Other object name");
      testTag.Save();

      string testName = "Name";
      Recipie testRecipie = new Recipie(testName);
      testRecipie.Save();

      //Act
      testRecipie.AddTag(testTag);
      testRecipie.Delete();

      List<Recipie> resultTagRecipies = testTag.GetRecipies();
      List<Recipie> testTagRecipies = new List<Recipie> {};

      //Assert
      Assert.Equal(testTagRecipies, resultTagRecipies);
    }


    [Fact]
    public void Test_Update_UpdatesInDb()
    {
      Recipie testRecipie = new Recipie("Name", 0);
      testRecipie.Save();
      testRecipie.Update("Other name", 0);

      Recipie newRecipie = new Recipie("Other name", 0, testRecipie.GetId());

      Assert.Equal(testRecipie, newRecipie);
    }

    public void Dispose()
    {
      Instruction.DeleteAll();
      Recipie.DeleteAll();
      Tag.DeleteAll();
      Ingredient.DeleteAll();    }
  }
}
