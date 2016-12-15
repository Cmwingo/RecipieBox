using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace RecipieBox
{
  public class TagTest : IDisposable
  {
    public TagTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=recipie_box_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_TagsEmptyAtFirst()
    {
      //Arrange, Act
      int result = Tag.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public void Test_Equal_ReturnsTrueForSameName()
    {
      //Arrange, Act
      Tag firstTag = new Tag("Name");
      Tag secondTag = new Tag("Name");

      //Assert
      Assert.Equal(firstTag, secondTag);
    }

    [Fact]
    public void Test_Save_SavesTagToDatabase()
    {
      //Arrange
      Tag testTag = new Tag("Name");
      testTag.Save();

      //Act
      List<Tag> result = Tag.GetAll();
      List<Tag> testList = new List<Tag>{testTag};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_Save_AssignsIdToTagObject()
    {
      //Arrange
      Tag testTag = new Tag("Name");
      testTag.Save();

      //Act
      Tag savedTag = Tag.GetAll()[0];

      int result = savedTag.GetId();
      int testId = testTag.GetId();

      //Assert
      Assert.Equal(testId, result);
    }

    [Fact]
    public void Test_Find_FindsTagInDatabase()
    {
      //Arrange
      Tag testTag = new Tag("Name");
      testTag.Save();

      //Act
      Tag foundTag = Tag.Find(testTag.GetId());

      //Assert
      Assert.Equal(testTag, foundTag);
    }

    [Fact]
    public void Test_Delete_DeletesTagFromDatabase()
    {
      //Arrange
      string name1 = "Name";
      Tag testTag1 = new Tag(name1);
      testTag1.Save();

      string name2 = "Other name";
      Tag testTag2 = new Tag(name2);
      testTag2.Save();

      //Act
      testTag1.Delete();
      List<Tag> resultTags = Tag.GetAll();
      List<Tag> testTagList = new List<Tag> {testTag2};

      //Assert
      Assert.Equal(testTagList, resultTags);
    }
    [Fact]
    public void Test_AddRecipie_AddsRecipieToTag()
    {
     //Arrange
      Tag testTag = new Tag("Name");
      testTag.Save();

      Recipie testRecipie = new Recipie("Object name");
      testRecipie.Save();

      Recipie testRecipie2 = new Recipie("Other object name");
      testRecipie2.Save();

     //Act
      testTag.AddRecipie(testRecipie);
      testTag.AddRecipie(testRecipie2);

      List<Recipie> result = testTag.GetRecipies();
      List<Recipie> testList = new List<Recipie>{testRecipie, testRecipie2};

     //Assert
      Assert.Equal(testList, result);
    }
    [Fact]
    public void Test_GetRecipies_ReturnsAllTagRecipies()
    {
      //Arrange
      Tag testTag = new Tag("Name");
      testTag.Save();

      Recipie testRecipie1 = new Recipie("Object name");
      testRecipie1.Save();

      Recipie testRecipie2 = new Recipie("Other object name");
      testRecipie2.Save();

      //Act
      testTag.AddRecipie(testRecipie1);
      List<Recipie> savedRecipies = testTag.GetRecipies();
      List<Recipie> testList = new List<Recipie> {testRecipie1};

      //Assert
      Assert.Equal(testList, savedRecipies);
    }
    [Fact]
    public void Test_Delete_DeletesTagAssociationsFromDatabase()
    {
      //Arrange
      Recipie testRecipie = new Recipie("Object name");
      testRecipie.Save();

      string testName = "Name";
      Tag testTag = new Tag(testName);
      testTag.Save();

      //Act
      testTag.AddRecipie(testRecipie);
      testTag.Delete();

      List<Tag> resultRecipieTags = testRecipie.GetTags();
      List<Tag> testRecipieTags = new List<Tag> {};

      //Assert
      Assert.Equal(testRecipieTags, resultRecipieTags);
    }

    public void Dispose()
    {
      Recipie.DeleteAll();
      Tag.DeleteAll();
      Ingredient.DeleteAll();
    }
  }
}
