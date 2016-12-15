using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace RecipieBox
{
  public class Ingredient
  {
    private int _id;
    private string _name;
    private int _quantity;
    private int _recipieId;
    private string _unit;

    public Ingredient(string Name, int RecipieId, int Quantity = 0, string Unit = " ", int Id = 0)
    {
      _id = Id;
      _name = Name;
      _quantity = Quantity;
      _unit = Unit;
      _recipieId = RecipieId;
    }

    public override bool Equals(System.Object otherIngredient)
    {
        if (!(otherIngredient is Ingredient))
        {
          return false;
        }
        else {
          Ingredient newIngredient = (Ingredient) otherIngredient;
          bool idEquality = this.GetId() == newIngredient.GetId();
          bool nameEquality = this.GetName() == newIngredient.GetName();
          bool quantityEquality = this.GetQuantity() == newIngredient.GetQuantity();
          bool recipieIdEquality = this.GetRecipieId() == newIngredient.GetRecipieId();

          return (idEquality && nameEquality && quantityEquality && recipieIdEquality);
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
    public int GetQuantity()
    {
      return _quantity;
    }
    public void SetQuantity(int newQuantity)
    {
      _quantity = newQuantity;
    }
    public int GetRecipieId()
    {
      return _recipieId;
    }
    public void SetRecipieId(int newRecipieId)
    {
      _recipieId = newRecipieId;
    }
    public string GetUnit()
    {
      return _unit;
    }
    public void SetUnit(string newUnit)
    {
      _unit = newUnit;
    }


    public static List<Ingredient> GetAll()
    {
      List<Ingredient> AllIngredients = new List<Ingredient>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM ingredients;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int ingredientId = rdr.GetInt32(0);
        string ingredientName = rdr.GetString(1);
        int quantity = rdr.GetInt32(2);
        int recipieId = rdr.GetInt32(3);
        string unit = rdr.GetString(4);
        Ingredient newIngredient = new Ingredient(ingredientName, recipieId, quantity, unit, ingredientId);
        AllIngredients.Add(newIngredient);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return AllIngredients;
    }

    public void Update(string newName, int newQuantity, string newUnit)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE ingredients SET name = @NewName, quantity = @NewQuantity, unit = @NewUnit OUTPUT INSERTED.name, INSERTED.quantity, INSERTED.unit WHERE id = @IngredientId;", conn);

      SqlParameter descParam = new SqlParameter();
      descParam.ParameterName = "@NewName";
      descParam.Value = newName;

      SqlParameter quantityParam = new SqlParameter();
      quantityParam.ParameterName = "@NewQuantity";
      quantityParam.Value = newQuantity;

      SqlParameter unitParam = new SqlParameter();
      unitParam.ParameterName = "@NewUnit";
      unitParam.Value = newUnit;

      SqlParameter idParam = new SqlParameter();
      idParam.ParameterName = "@IngredientId";
      idParam.Value = this._id;

      cmd.Parameters.Add(descParam);
      cmd.Parameters.Add(quantityParam);
      cmd.Parameters.Add(unitParam);
      cmd.Parameters.Add(idParam);

      SqlDataReader rdr = cmd.ExecuteReader();

      while (rdr.Read())
      {
        this._name = rdr.GetString(0);
        this._quantity = rdr.GetInt32(1);
        this._unit = rdr.GetString(2);
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

      SqlCommand cmd = new SqlCommand("INSERT INTO ingredients (name, recipie_id, quantity, unit) OUTPUT INSERTED.id VALUES (@IngredientName, @IngredientRecipieId, @IngredientQuantity, @IngredientUnit);", conn);

      SqlParameter nameParam = new SqlParameter();
      nameParam.ParameterName = "@IngredientName";
      nameParam.Value = this.GetName();

      SqlParameter quantityParam = new SqlParameter();
      quantityParam.ParameterName = "@IngredientQuantity";
      quantityParam.Value = this.GetQuantity();

      SqlParameter recipieIdParam = new SqlParameter();
      recipieIdParam.ParameterName = "@IngredientRecipieId";
      recipieIdParam.Value = this.GetRecipieId();

      SqlParameter unitParam = new SqlParameter();
      unitParam.ParameterName = "@IngredientUnit";
      unitParam.Value = this.GetUnit();

      cmd.Parameters.Add(nameParam);
      cmd.Parameters.Add(quantityParam);
      cmd.Parameters.Add(recipieIdParam);
      cmd.Parameters.Add(unitParam);

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
      SqlCommand cmd = new SqlCommand("DELETE FROM ingredients;", conn);
      cmd.ExecuteNonQuery();
      conn.Close();
    }

    public static Ingredient Find(int id)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM ingredients WHERE id = @IngredientId", conn);
      SqlParameter ingredientIdParameter = new SqlParameter();
      ingredientIdParameter.ParameterName = "@IngredientId";
      ingredientIdParameter.Value = id.ToString();
      cmd.Parameters.Add(ingredientIdParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      int foundIngredientId = 0;
      string foundIngredientName = null;
      int foundIngredientQuantity = 0;
      int foundIngredientRecipieId = 0;
      string foundIngredientUnit = null;

      while(rdr.Read())
      {
        foundIngredientId = rdr.GetInt32(0);
        foundIngredientName = rdr.GetString(1);
        foundIngredientQuantity = rdr.GetInt32(2);
        foundIngredientRecipieId = rdr.GetInt32(3);
        foundIngredientUnit = rdr.GetString(4);
      }
      Ingredient foundIngredient = new Ingredient(foundIngredientName, foundIngredientRecipieId, foundIngredientQuantity, foundIngredientUnit, foundIngredientId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundIngredient;
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM ingredients WHERE id = @IngredientId;", conn);

      SqlParameter ingredientIdParameter = new SqlParameter();
      ingredientIdParameter.ParameterName = "@IngredientId";
      ingredientIdParameter.Value = this.GetId();

      cmd.Parameters.Add(ingredientIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }
  }
}
