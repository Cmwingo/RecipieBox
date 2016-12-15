using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace RecipieBox
{
  public class IngredientTest : IDisposable
  {
    public IngredientTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=recipie_box_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_EmptyAtFirst()
    {
      //Arrange, Act
      int result = Ingredient.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public void Test_EqualOverrideTrueForSameName()
    {
      //Arrange, Act
      Ingredient firstIngredient = new Ingredient("Cheese", 0, 0);
      Ingredient secondIngredient = new Ingredient("Cheese", 0, 0);

      //Assert
      Assert.Equal(firstIngredient, secondIngredient);
    }

    [Fact]
    public void Test_Save()
    {
      //Arrange
      Ingredient testIngredient = new Ingredient("Cheese", 0, 0);
      testIngredient.Save();

      //Act
      List<Ingredient> result = Ingredient.GetAll();
      List<Ingredient> testList = new List<Ingredient>{testIngredient};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_SaveAssignsIdToObject()
    {
      //Arrange
      Ingredient testIngredient = new Ingredient("Cheese", 0, 0);
      testIngredient.Save();

      //Act
      Ingredient savedIngredient = Ingredient.GetAll()[0];

      int result = savedIngredient.GetId();
      int testId = testIngredient.GetId();

      //Assert
      Assert.Equal(testId, result);
    }

    [Fact]
    public void Test_FindFindsIngredientInDatabase()
    {
      //Arrange
      Ingredient testIngredient = new Ingredient("Name", 0, 0);
      testIngredient.Save();

      //Act
      Ingredient result = Ingredient.Find(testIngredient.GetId());

      //Assert
      Assert.Equal(testIngredient, result);
    }

    [Fact]
    public void Test_Update_UpdatesInDb()
    {
      Ingredient testIngredient = new Ingredient("Name", 0, 0);
      testIngredient.Save();
      testIngredient.Update("Other name", 0);

      Ingredient newIngredient = new Ingredient("Other name", 0, 0, testIngredient.GetId());

      Assert.Equal(testIngredient, newIngredient);
    }

    public void Dispose()
    {
      Ingredient.DeleteAll();
      Recipie.DeleteAll();
      Tag.DeleteAll();
    }
  }
}
