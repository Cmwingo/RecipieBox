using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace RecipieBox
{
  public class Tag
  {
    private int _id;
    private string _name;

    public Tag(string Name, int Id = 0)
    {
      _id = Id;
      _name = Name;
    }

    public override bool Equals(System.Object otherTag)
    {
        if (!(otherTag is Tag))
        {
          return false;
        }
        else
        {
          Tag newTag = (Tag) otherTag;
          bool idEquality = this.GetId() == newTag.GetId();
          bool nameEquality = this.GetName() == newTag.GetName();
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
    public static List<Tag> GetAll()
    {
      List<Tag> allTags = new List<Tag>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM tags;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int tagId = rdr.GetInt32(0);
        string tagName = rdr.GetString(1);
        Tag newTag = new Tag(tagName, tagId);
        allTags.Add(newTag);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allTags;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO tags (name) OUTPUT INSERTED.id VALUES (@TagName);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@TagName";
      nameParameter.Value = this.GetName();
      cmd.Parameters.Add(nameParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM tags;", conn);
      cmd.ExecuteNonQuery();
      conn.Close();
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM tags WHERE id = @TagId; DELETE FROM recipies_tags WHERE tag_id = @TagId;", conn);

      SqlParameter catId = new SqlParameter();
      catId.ParameterName = "@TagId";
      catId.Value = this.GetId();

      cmd.Parameters.Add(catId);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public static Tag Find(int id)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM tags WHERE id = @TagId;", conn);
      SqlParameter tagIdParameter = new SqlParameter();
      tagIdParameter.ParameterName = "@TagId";
      tagIdParameter.Value = id.ToString();
      cmd.Parameters.Add(tagIdParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      int foundTagId = 0;
      string foundTagDescription = null;

      while(rdr.Read())
      {
        foundTagId = rdr.GetInt32(0);
        foundTagDescription = rdr.GetString(1);
      }
      Tag foundTag = new Tag(foundTagDescription, foundTagId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundTag;
    }

    public void Update(string newName)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE tags SET name = @NewName OUTPUT INSERTED.name where id = @TagId;", conn);

      SqlParameter descParam = new SqlParameter();
      descParam.ParameterName = "@NewName";
      descParam.Value = newName;


      SqlParameter idParam = new SqlParameter();
      idParam.ParameterName = "@TagId";
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

    public void AddRecipie(Recipie newRecipie)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO recipies_tags (tag_id, recipie_id) VALUES (@TagId, @RecipieId);", conn);

      SqlParameter tagIdParameter = new SqlParameter();
      tagIdParameter.ParameterName = "@TagId";
      tagIdParameter.Value = this.GetId();
      cmd.Parameters.Add(tagIdParameter);

      SqlParameter recipieIdParameter = new SqlParameter();
      recipieIdParameter.ParameterName = "@RecipieId";
      recipieIdParameter.Value = newRecipie.GetId();
      cmd.Parameters.Add(recipieIdParameter);

      cmd.ExecuteNonQuery();

      if(conn!=null)
      {
        conn.Close();
      }
    }

    public List<Recipie> GetRecipies()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT recipies.* FROM tags JOIN recipies_tags ON (tags.id = recipies_tags.tag_id) JOIN recipies ON (recipies_tags.recipie_id = recipies.id) WHERE tags.id = @TagId;", conn);

      SqlParameter tagIdParameter = new SqlParameter();
      tagIdParameter.ParameterName = "@TagId";
      tagIdParameter.Value = this.GetId();
      cmd.Parameters.Add(tagIdParameter);

      SqlDataReader rdr = cmd.ExecuteReader();

      List<int> recipieIds = new List<int> {};
      while(rdr.Read())
      {
        int recipieId = rdr.GetInt32(0);
        recipieIds.Add(recipieId);
      }
      if (rdr!=null)
      {
        rdr.Close();
      }

      List<Recipie> recipies = new List<Recipie> {};
      foreach (int recipieId in recipieIds)
      {
        SqlCommand recipieQuery = new SqlCommand("SELECT * FROM recipies WHERE id = @RecipieId;", conn);

        SqlParameter recipieIdParameter = new SqlParameter();
        recipieIdParameter.ParameterName = "@RecipieId";
        recipieIdParameter.Value = recipieId;
        recipieQuery.Parameters.Add(recipieIdParameter);

        SqlDataReader queryReader = recipieQuery.ExecuteReader();
        while(queryReader.Read())
        {
          int thisRecipieId = queryReader.GetInt32(0);
          string recipieDescription = queryReader.GetString(1);
          Recipie foundRecipie = new Recipie(recipieDescription, thisRecipieId);
          recipies.Add(foundRecipie);
        }
        if(queryReader!=null)
        {
          queryReader.Close();
        }
      }
      if(conn!=null)
      {
        conn.Close();
      }
      return recipies;
    }
  }
}
