using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace RecipieBox
{
  public class InstructionTest : IDisposable
  {
    public InstructionTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=recipie_box_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_EmptyAtFirst()
    {
      //Arrange, Act
      int result = Instruction.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public void Test_EqualOverrideTrueForSameBake()
    {
      //Arrange, Act
      Instruction firstInstruction = new Instruction("Bake", 0, 0);
      Instruction secondInstruction = new Instruction("Bake", 0, 0);

      //Assert
      Assert.Equal(firstInstruction, secondInstruction);
    }

    [Fact]
    public void Test_Save()
    {
      //Arrange
      Instruction testInstruction = new Instruction("Bake", 0, 0);
      testInstruction.Save();

      //Act
      List<Instruction> result = Instruction.GetAll();
      List<Instruction> testList = new List<Instruction>{testInstruction};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_SaveAssignsIdToObject()
    {
      //Arrange
      Instruction testInstruction = new Instruction("Bake", 0, 0);
      testInstruction.Save();

      //Act
      Instruction savedInstruction = Instruction.GetAll()[0];

      int result = savedInstruction.GetId();
      int testId = testInstruction.GetId();

      //Assert
      Assert.Equal(testId, result);
    }

    [Fact]
    public void Test_FindFindsInstructionInDatabase()
    {
      //Arrange
      Instruction testInstruction = new Instruction("Bake", 0, 0);
      testInstruction.Save();

      //Act
      Instruction result = Instruction.Find(testInstruction.GetId());

      //Assert
      Assert.Equal(testInstruction, result);
    }

    [Fact]
    public void Test_Update_UpdatesInDb()
    {
      Instruction testInstruction = new Instruction("Bake", 0, 0);
      testInstruction.Save();
      testInstruction.Update("Broil", 0);

      Instruction newInstruction = new Instruction("Broil", 0, 0, testInstruction.GetId());

      Assert.Equal(testInstruction, newInstruction);
    }

    public void Dispose()
    {
      Instruction.DeleteAll();
      Recipie.DeleteAll();
      Tag.DeleteAll();
      Ingredient.DeleteAll();
    }
  }
}
