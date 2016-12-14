using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace RecipieBox
{
  public class Recipie
  {
    private int _id;
    private string _name;

    public Recipie(string Name, int Id = 0)
    {
      _id = Id;
      _name = Name;
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
          return (idEquality && nameEquality);
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

    public static List<Recipie> GetAll()
    {
      List<Recipie> AllRecipies = new List<Recipie>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM recipies;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int recipieId = rdr.GetInt32(0);
        string recipieName = rdr.GetString(1);
        Recipie newRecipie = new Recipie(recipieName, recipieId);
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

    public void Update(string newName)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE recipies SET name = @NewName OUTPUT INSERTED.name where id = @RecipieId;", conn);

      SqlParameter descParam = new SqlParameter();
      descParam.ParameterName = "@NewName";
      descParam.Value = newName;


      SqlParameter idParam = new SqlParameter();
      idParam.ParameterName = "@RecipieId";
      idParam.Value = this._id;

      cmd.Parameters.Add(descParam);
      cmd.Parameters.Add(idParam);

      SqlDataReader rdr = cmd.ExecuteReader();

      while (rdr.Read())
      {
        this._name = rdr.GetString(0);
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

      SqlCommand cmd = new SqlCommand("INSERT INTO recipies (name) OUTPUT INSERTED.id VALUES (@RecipieName);", conn);

      SqlParameter nameParam = new SqlParameter();
      nameParam.ParameterName = "@RecipieName";
      nameParam.Value = this.GetName();


      cmd.Parameters.Add(nameParam);

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

      while(rdr.Read())
      {
        foundRecipieId = rdr.GetInt32(0);
        foundRecipieName = rdr.GetString(1);
      }
      Recipie foundRecipie = new Recipie(foundRecipieName, foundRecipieId);

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
  }
}
