using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace RecipieBox
{
  public class Recipie
  {
    private int _id;
    private string _name;
    private int _rating;

    public Recipie(string Name, int Rating = 0, int Id = 0)
    {
      _id = Id;
      _name = Name;
      _rating = Rating;
    }

    public override bool Equals(System.Object otherRecipie)
    {
        if (!(otherRecipie is Recipie))
        {
          return false;
        }
        else {
          Recipie newRecipie = (Recipie) otherRecipie;
          bool idEquality = this.GetId() == newRecipie.GetId();
          bool nameEquality = this.GetName() == newRecipie.GetName();
          bool ratingEquality = this.GetRating() == newRecipie.GetRating();
          return (idEquality && nameEquality && ratingEquality);
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
    public int GetRating()
    {
      return _rating;
    }

    public static List<Recipie> GetAll()
    {
      List<Recipie> AllRecipies = new List<Recipie>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM recipies ORDER BY rating DESC;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int recipieId = rdr.GetInt32(0);
        string recipieName = rdr.GetString(1);
        int rating = rdr.GetInt32(2);
        Recipie newRecipie = new Recipie(recipieName, rating, recipieId);
        AllRecipies.Add(newRecipie);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return AllRecipies;
    }

    public void Update(string newName, int newRating)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE recipies SET name = @NewName, rating = @NewRating OUTPUT INSERTED.name, INSERTED.rating WHERE id = @RecipieId;", conn);

      SqlParameter descParam = new SqlParameter();
      descParam.ParameterName = "@NewName";
      descParam.Value = newName;

      SqlParameter ratingParam = new SqlParameter();
      ratingParam.ParameterName = "@NewRating";
      ratingParam.Value = newRating;



      SqlParameter idParam = new SqlParameter();
      idParam.ParameterName = "@RecipieId";
      idParam.Value = this._id;

      cmd.Parameters.Add(descParam);
      cmd.Parameters.Add(ratingParam);
      cmd.Parameters.Add(idParam);

      SqlDataReader rdr = cmd.ExecuteReader();

      while (rdr.Read())
      {
        this._name = rdr.GetString(0);
        this._rating = rdr.GetInt32(1);
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

      SqlCommand cmd = new SqlCommand("INSERT INTO recipies (name, rating) OUTPUT INSERTED.id VALUES (@RecipieName, @RecipieRating);", conn);

      SqlParameter nameParam = new SqlParameter();
      nameParam.ParameterName = "@RecipieName";
      nameParam.Value = this.GetName();

      SqlParameter ratingParam = new SqlParameter();
      ratingParam.ParameterName = "@RecipieRating";
      ratingParam.Value = this.GetRating();

      cmd.Parameters.Add(nameParam);
      cmd.Parameters.Add(ratingParam);

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
      SqlCommand cmd = new SqlCommand("DELETE FROM recipies;", conn);
      cmd.ExecuteNonQuery();
      conn.Close();
    }

    public static Recipie Find(int id)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM recipies WHERE id = @RecipieId", conn);
      SqlParameter recipieIdParameter = new SqlParameter();
      recipieIdParameter.ParameterName = "@RecipieId";
      recipieIdParameter.Value = id.ToString();
      cmd.Parameters.Add(recipieIdParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      int foundRecipieId = 0;
      string foundRecipieName = null;
      int foundRecipieRating = 0;

      while(rdr.Read())
      {
        foundRecipieId = rdr.GetInt32(0);
        foundRecipieName = rdr.GetString(1);
        foundRecipieRating = rdr.GetInt32(2);
      }
      Recipie foundRecipie = new Recipie(foundRecipieName, foundRecipieRating, foundRecipieId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundRecipie;
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM recipies WHERE id = @RecipieId;", conn);

      SqlParameter recipieIdParameter = new SqlParameter();
      recipieIdParameter.ParameterName = "@RecipieId";
      recipieIdParameter.Value = this.GetId();

      cmd.Parameters.Add(recipieIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public void AddTag(Tag newTag)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO recipies_tags (tag_id, recipie_id) VALUES (@TagId, @RecipieId);", conn);

      SqlParameter tagIdParameter = new SqlParameter();
      tagIdParameter.ParameterName = "@TagId";
      tagIdParameter.Value = newTag.GetId();
      cmd.Parameters.Add(tagIdParameter);

      SqlParameter recipieIdParameter = new SqlParameter();
      recipieIdParameter.ParameterName = "@RecipieId";
      recipieIdParameter.Value = this.GetId();
      cmd.Parameters.Add(recipieIdParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public List<Tag> GetTags()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT tags.* FROM recipies JOIN recipies_tags ON (recipies.id = recipies_tags.recipie_id) JOIN tags ON (recipies_tags.tag_id = tags.id) WHERE recipie_id = @RecipieId;", conn);

      SqlParameter recipieIdParameter = new SqlParameter();
      recipieIdParameter.ParameterName = "@RecipieId";
      recipieIdParameter.Value = this.GetId();
      cmd.Parameters.Add(recipieIdParameter);

      SqlDataReader rdr = cmd.ExecuteReader();

      List<Tag> tags = new List<Tag> {};

      while (rdr.Read())
      {
        int thisTagId = rdr.GetInt32(0);
        string tagName = rdr.GetString(1);
        Tag foundTag = new Tag(tagName, thisTagId);
        tags.Add(foundTag);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return tags;
    }

    public List<Ingredient> GetIngredients()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM ingredients WHERE recipie_id = @RecipieId;", conn);

      SqlParameter recipieIdParameter = new SqlParameter();
      recipieIdParameter.ParameterName = "@RecipieId";
      recipieIdParameter.Value = this.GetId();
      cmd.Parameters.Add(recipieIdParameter);

      SqlDataReader rdr = cmd.ExecuteReader();

      List<Ingredient> ingredients = new List<Ingredient> {};

      while (rdr.Read())
      {
        int thisIngredientId = rdr.GetInt32(0);
        string ingredientName = rdr.GetString(1);
        int ingredientQuantity = rdr.GetInt32(2);
        int ingredientRecipieId = rdr.GetInt32(3);
        string ingredientUnit = rdr.GetString(4);
        Ingredient foundIngredient = new Ingredient(ingredientName, ingredientRecipieId, ingredientQuantity, ingredientUnit, thisIngredientId);
        ingredients.Add(foundIngredient);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return ingredients;
    }

    public List<Instruction> GetInstructions()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM instructions WHERE recipie_id = @RecipieId ORDER BY step_number;", conn);

      SqlParameter recipieIdParameter = new SqlParameter();
      recipieIdParameter.ParameterName = "@RecipieId";
      recipieIdParameter.Value = this.GetId();
      cmd.Parameters.Add(recipieIdParameter);

      SqlDataReader rdr = cmd.ExecuteReader();

      List<Instruction> instructions = new List<Instruction> {};

      while (rdr.Read())
      {
        int thisInstructionId = rdr.GetInt32(0);
        string instructionName = rdr.GetString(1);
        int instructionQuantity = rdr.GetInt32(2);
        int instructionRecipieId = rdr.GetInt32(3);
        Instruction foundInstruction = new Instruction(instructionName, instructionRecipieId, instructionQuantity, thisInstructionId);
        instructions.Add(foundInstruction);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return instructions;
    }
  }
}
