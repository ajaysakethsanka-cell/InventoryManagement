using System.Data;
using Microsoft.Data.SqlClient;
using InventoryManagement.Models;

namespace InventoryManagement.Data
{
    public class ProductRepository
    {
        // READ: all products (or filtered by search text)
        public DataTable GetAll(string search = "")
        {
            const string sql = @"SELECT ProductId, ProductName, Category, Quantity, Price
                                 FROM Products
                                 WHERE ProductName LIKE @s OR Category LIKE @s
                                 ORDER BY ProductId";

            using var con = new SqlConnection(DbConfig.ConnectionString);
            using var da = new SqlDataAdapter(sql, con);
            da.SelectCommand.Parameters.AddWithValue("@s", "%" + search + "%");

            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        // CREATE
        public int Add(Product p)
        {
            const string sql = @"INSERT INTO Products (ProductName, Category, Quantity, Price)
                                 VALUES (@name, @cat, @qty, @price)";

            using var con = new SqlConnection(DbConfig.ConnectionString);
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@name", p.ProductName);
            cmd.Parameters.AddWithValue("@cat", p.Category);
            cmd.Parameters.AddWithValue("@qty", p.Quantity);
            cmd.Parameters.AddWithValue("@price", p.Price);

            con.Open();
            return cmd.ExecuteNonQuery();
        }

        // UPDATE
        public int Update(Product p)
        {
            const string sql = @"UPDATE Products
                                 SET ProductName = @name, Category = @cat,
                                     Quantity = @qty, Price = @price
                                 WHERE ProductId = @id";

            using var con = new SqlConnection(DbConfig.ConnectionString);
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", p.ProductId);
            cmd.Parameters.AddWithValue("@name", p.ProductName);
            cmd.Parameters.AddWithValue("@cat", p.Category);
            cmd.Parameters.AddWithValue("@qty", p.Quantity);
            cmd.Parameters.AddWithValue("@price", p.Price);

            con.Open();
            return cmd.ExecuteNonQuery();
        }

        // DELETE
        public int Delete(int id)
        {
            const string sql = "DELETE FROM Products WHERE ProductId = @id";

            using var con = new SqlConnection(DbConfig.ConnectionString);
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", id);

            con.Open();
            return cmd.ExecuteNonQuery();
        }
    }
}