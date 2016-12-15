using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace RecipieBox
{
  public class Instruction
  {
    private int _id;
    private string _name;
    private int _stepNumber;
    private int _recipieId;

    public Instruction(string Name, int RecipieId, int StepNumber = 0, int Id = 0)
    {
      _id = Id;
      _name = Name;
      _stepNumber = StepNumber;
      _recipieId = RecipieId;
    }

    public override bool Equals(System.Object otherInstruction)
    {
        if (!(otherInstruction is Instruction))
        {
          return false;
        }
        else {
          Instruction newInstruction = (Instruction) otherInstruction;
          bool idEquality = this.GetId() == newInstruction.GetId();
          bool nameEquality = this.GetName() == newInstruction.GetName();
          bool stepNumberEquality = this.GetStepNumber() == newInstruction.GetStepNumber();
          bool recipieIdEquality = this.GetRecipieId() == newInstruction.GetRecipieId();

          return (idEquality && nameEquality && stepNumberEquality && recipieIdEquality);
        }
    }

    public int GetId()
    {
      return _id;
    }
    public string GetName()
    {
      return _name;
    }
    public void SetName(string newName)
    {
      _name = newName;
    }
    public int GetStepNumber()
    {
      return _stepNumber;
    }
    public void SetStepNumber(int newStepNumber)
    {
      _stepNumber = newStepNumber;
    }
    public int GetRecipieId()
    {
      return _recipieId;
    }
    public void SetRecipieId(int newRecipieId)
    {
      _recipieId = newRecipieId;
    }

    public static List<Instruction> GetAll()
    {
      List<Instruction> AllInstructions = new List<Instruction>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM instructions ORDER BY step_number;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int instructionId = rdr.GetInt32(0);
        string instructionName = rdr.GetString(1);
        int stepNumber = rdr.GetInt32(2);
        int recipieId = rdr.GetInt32(3);
        Instruction newInstruction = new Instruction(instructionName, recipieId, stepNumber,  instructionId);
        AllInstructions.Add(newInstruction);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return AllInstructions;
    }

    public void Update(string newName, int newStepNumber)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE instructions SET name = @NewName, step_number = @NewStepNumber OUTPUT INSERTED.name, INSERTED.step_number WHERE id = @InstructionId;", conn);

      SqlParameter descParam = new SqlParameter();
      descParam.ParameterName = "@NewName";
      descParam.Value = newName;

      SqlParameter stepNumberParam = new SqlParameter();
      stepNumberParam.ParameterName = "@NewStepNumber";
      stepNumberParam.Value = newStepNumber;

      SqlParameter idParam = new SqlParameter();
      idParam.ParameterName = "@InstructionId";
      idParam.Value = this._id;

      cmd.Parameters.Add(descParam);
      cmd.Parameters.Add(stepNumberParam);
      cmd.Parameters.Add(idParam);

      SqlDataReader rdr = cmd.ExecuteReader();

      while (rdr.Read())
      {
        this._name = rdr.GetString(0);
        this._stepNumber = rdr.GetInt32(1);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO instructions (name, recipie_id, step_number) OUTPUT INSERTED.id VALUES (@InstructionName, @InstructionRecipieId, @InstructionStepNumber);", conn);

      SqlParameter nameParam = new SqlParameter();
      nameParam.ParameterName = "@InstructionName";
      nameParam.Value = this.GetName();

      SqlParameter stepNumberParam = new SqlParameter();
      stepNumberParam.ParameterName = "@InstructionStepNumber";
      stepNumberParam.Value = this.GetStepNumber();

      SqlParameter recipieIdParam = new SqlParameter();
      recipieIdParam.ParameterName = "@InstructionRecipieId";
      recipieIdParam.Value = this.GetRecipieId();


      cmd.Parameters.Add(nameParam);
      cmd.Parameters.Add(stepNumberParam);
      cmd.Parameters.Add(recipieIdParam);


      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM instructions;", conn);
      cmd.ExecuteNonQuery();
      conn.Close();
    }

    public static Instruction Find(int id)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM instructions WHERE id = @InstructionId", conn);
      SqlParameter instructionIdParameter = new SqlParameter();
      instructionIdParameter.ParameterName = "@InstructionId";
      instructionIdParameter.Value = id.ToString();
      cmd.Parameters.Add(instructionIdParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      int foundInstructionId = 0;
      string foundInstructionName = null;
      int foundInstructionStepNumber = 0;
      int foundInstructionRecipieId = 0;

      while(rdr.Read())
      {
        foundInstructionId = rdr.GetInt32(0);
        foundInstructionName = rdr.GetString(1);
        foundInstructionStepNumber = rdr.GetInt32(2);
        foundInstructionRecipieId = rdr.GetInt32(3);
      }
      Instruction foundInstruction = new Instruction(foundInstructionName, foundInstructionRecipieId, foundInstructionStepNumber, foundInstructionId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundInstruction;
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM instructions WHERE id = @InstructionId;", conn);

      SqlParameter instructionIdParameter = new SqlParameter();
      instructionIdParameter.ParameterName = "@InstructionId";
      instructionIdParameter.Value = this.GetId();

      cmd.Parameters.Add(instructionIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }
  }
}
