﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Lahjakorttiappi.DatabaseController
{
    class DatabaseController
    {
       

        //Connects to sql database
        SqlConnection connect = new SqlConnection();
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Lahjakortti;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public bool connectDatabase()
        {
            connect.ConnectionString = connectionString;
            try
            {
                connect.Open();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Virheilmoitukset:" + e);
                connect.Close();
                return false;
            }
        }

        //Closes the connection between the database
        public void disconnectDatabse()
        {
            connect.Close();
        }

        //Gets all data from Asiakastiedot table
        public DataSet bringAllData(DataSet ds)
        {
            connectDatabase();
            var select = @"SELECT aTied.ID, aTied.Etunimi, aTied.Sukunimi, aTied.Osoite, aTied.PuhNro, aTied.Sahkoposti, aTied.Postinumero, aTied.Paikka, lahj.Voimassaolo, palv.Palvelu, til.PVM, til.Kerrat, til.Kesto, til.Maksettu, til.Saaja
                        FROM [Asiakastiedot] as aTied
                        INNER JOIN [Palvelut] as palv
                        ON aTied.PalveluID = palv.ID
                        INNER JOIN [Tilaukset] til
                        ON aTied.TilausID = til.ID
                        INNER JOIN [Lahjakortti] as lahj
                        ON aTied.LahjakorttiID = lahj.ID
                        INNER JOIN [Myyja] as sell
                        ON aTied.MyyjaID = sell.ID";
            var c = connect;
            var dataAdapter = new SqlDataAdapter(select, c);
            var commandBuilder = new SqlCommandBuilder(dataAdapter);
            ds.Tables.Add("CustomerInfo");
            dataAdapter.Fill(ds, "CustomerInfo");
            disconnectDatabse();
            return ds;
        }
        
        //fills product screen with product info
        public DataSet bringProductInfo(DataSet ds)
        {
            connectDatabase();
            var select = "SELECT * FROM Palvelut";
            var c = connect;
            var dataAdapter = new SqlDataAdapter(select, c);
            var commandBuilder = new SqlCommandBuilder(dataAdapter);
            ds.Tables.Add("ProductInfo");
            dataAdapter.Fill(ds, "ProductInfo");
            disconnectDatabse();
            return ds;
        }

        public DataSet bringStaffInfo(DataSet ds)
        {
            connectDatabase();
            var select = "SELECT * FROM Myyja";
            var c = connect;
            var dataAdapter = new SqlDataAdapter(select, c);
            var commandBuilder = new SqlCommandBuilder(dataAdapter);
            ds.Tables.Add("Seller");
            dataAdapter.Fill(ds, "Seller");
            disconnectDatabse();
            return ds;
        }

        public DataTable bringStaffInfo2(DataTable dt)
        {
            connectDatabase();
            var select = "SELECT * FROM Myyja";
            var c = connect;
            var dataAdapter = new SqlDataAdapter(select, c);
            var commandBuilder = new SqlCommandBuilder(dataAdapter);
            dataAdapter.Fill(dt);
            disconnectDatabse();
            return dt;
        }

        public List<Class.Products> bringProducts()
        {
            List<Class.Products> prod = new List<Class.Products>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Palvelut", connect);
            connectDatabase();
            SqlDataReader read = cmd.ExecuteReader();
            if(read.HasRows)
            {
                while(read.Read())
                {
                    Class.Products product = new Class.Products();
                    product.PalveluNro = Convert.ToInt32(read.GetValue(0));
                    product.Palvelu = read.GetValue(1).ToString();
                    prod.Add(product);
                }
            }
            disconnectDatabse();
            return prod;
        }

        public Tuple<Class.Asiakastiedot, Class.giftCard, Class.Orders> fetchData(int indexID, Class.Asiakastiedot customerInfo, Class.giftCard giftCard, Class.Orders order)
        {
            Tuple<Class.Asiakastiedot, Class.giftCard, Class.Orders> tuple = new Tuple<Class.Asiakastiedot, Class.giftCard, Class.Orders>(customerInfo, giftCard, order);
            SqlCommand cmd = new SqlCommand("SELECT * FROM Asiakastiedot WHERE ID = @id", connect);
            cmd.Parameters.AddWithValue("@id", indexID);
            connectDatabase();
            SqlDataReader read = cmd.ExecuteReader();
            if(read.HasRows)
            {
                while(read.Read())
                {
                    tuple.Item1.AsiakasNro = Convert.ToInt32(read.GetValue(0));
                    tuple.Item1.Etunimi = read.GetValue(1).ToString();
                    tuple.Item1.Sukunimi = read.GetValue(2).ToString();
                    tuple.Item1.Osoite = read.GetValue(3).ToString();
                    tuple.Item1.PuhNro = read.GetValue(4).ToString();
                    tuple.Item1.Sahkoposti = read.GetValue(5).ToString();
                    tuple.Item1.Postinumero = read.GetValue(6).ToString();
                    tuple.Item1.Paikka = read.GetValue(7).ToString();
                    tuple.Item1.PalveluID = Convert.ToInt32(read.GetValue(8));
                    tuple.Item1.TilausID = Convert.ToInt32(read.GetValue(9));
                    tuple.Item1.LahjakorttiID = Convert.ToInt32(read.GetValue(10));
                }
            }
            read.Close();
            SqlCommand cmd1 = new SqlCommand("SELECT * FROM Lahjakortti WHERE ID = @id", connect);
            cmd1.Parameters.AddWithValue("@id", customerInfo.LahjakorttiID);
            read = cmd1.ExecuteReader();
            if(read.HasRows)
            {
                while(read.Read())
                {
                    tuple.Item2.ID = Convert.ToInt32(read.GetValue(0));
                    tuple.Item2.Voimassaolo = Convert.ToDateTime(read.GetValue(1));
                    //tuple.Item2.Voimassaolo = Convert.ToDateTime(read.GetValue(1).ToString());
                    //tuple.Item2.Voimassaolo = DateTime.ParseExact(read.GetValue(2).ToString(), "yyyy-MM-dd", null);
                }
            }
            read.Close();
            SqlCommand cmd2 = new SqlCommand("SELECT * FROM Tilaukset WHERE ID = @id", connect);
            cmd2.Parameters.AddWithValue("@id", customerInfo.TilausID);
            read = cmd2.ExecuteReader();
            if(read.HasRows)
            {
                while(read.Read())
                {
                    tuple.Item3.ID = Convert.ToInt32(read.GetValue(0));
                    tuple.Item3.Pvm = Convert.ToDateTime(read.GetValue(1));
                    tuple.Item3.Duration = read.GetValue(2).ToString();
                    tuple.Item3.Usages = read.GetValue(3).ToString();
                    tuple.Item3.Recipient = read.GetValue(4).ToString();
                    tuple.Item3.Paid = Convert.ToInt32(read.GetValue(5));
                }
            }
            disconnectDatabse();
            return tuple;
        }

        /*public DataSet bringProductsOnly(DataSet ds)
        {
            connectDatabase();
            var select = "SELECT * FROM Palvelut";
            var c = connect;
            var dataAdapter = new SqlDataAdapter(select, c);
            var commandBuilder = new SqlCommandBuilder(dataAdapter);
            dataAdapter.Fill(ds, "ProductInfo");
            disconnectDatabse();
            return ds;
        }*/

        //removes products from database
        public void removeProductById(int id)
        {
            connectDatabase();
            SqlCommand cmd = new SqlCommand("DELETE FROM Palvelut WHERE ID = @id", connect);
            cmd.Parameters.AddWithValue("@id", id);
            using(cmd)
            {
                cmd.ExecuteNonQuery();
                
            }
            disconnectDatabse();
    }

        //removes Customer Info from database
        public void removeCustomerInfoById(int id)
        {
            List<int> CustomerInfoId = new List<int>();
            connectDatabase();

            //bring the ids from customerinfo table so we can utilize them to delete data from all the tables
            SqlCommand command = new SqlCommand(@"SELECT PalveluID, TilausID, LahjakorttiID
                                            FROM [Asiakastiedot]
                                            WHERE ID = @id" , connect);
            command.Parameters.AddWithValue("@id", id);
            SqlDataReader read = command.ExecuteReader();
            if(read.HasRows)
            {
                while(read.Read())
                {
                    CustomerInfoId.Add(Convert.ToInt32(read.GetValue(0)));
                    CustomerInfoId.Add(Convert.ToInt32(read.GetValue(1)));
                    CustomerInfoId.Add(Convert.ToInt32(read.GetValue(2)));
                }
            }
            read.Close();
            SqlCommand cmd = new SqlCommand("DELETE FROM Asiakastiedot WHERE ID = @id", connect);
            cmd.Parameters.AddWithValue("@id", id);
            using (cmd)
            {
                cmd.ExecuteNonQuery();

            }
            SqlCommand deletePalvelu = new SqlCommand("DELETE FROM Palvelut WHERE ID = @id", connect);
            deletePalvelu.Parameters.AddWithValue("@id", CustomerInfoId[0]);
            using(deletePalvelu)
            {
                deletePalvelu.ExecuteNonQuery();
            }
            SqlCommand deleteTilaukset = new SqlCommand("DELETE FROM Tilaukset WHERE ID = @id", connect);
            deleteTilaukset.Parameters.AddWithValue("@id", CustomerInfoId[1]);
            using(deleteTilaukset)
            {
                deleteTilaukset.ExecuteNonQuery();
            }
            SqlCommand deleteLahjakortti = new SqlCommand("DELETE FROM Lahjakortti WHERE ID = @id", connect);
            deleteLahjakortti.Parameters.AddWithValue("@id", CustomerInfoId[2]);
            using(deleteLahjakortti)
            {
                deleteLahjakortti.ExecuteNonQuery();
            }
            disconnectDatabse();
        }

        //saves the product to database
        public bool addProduct(Class.Products prod)
        {
            bool error = false;
            SqlCommand cmd = new SqlCommand("INSERT INTO [Palvelut]([Palvelu]) VALUES (@Palvelu)", connect);
            cmd.Parameters.AddWithValue("@Palvelu", prod.Palvelu);
            connectDatabase();
            using (cmd)
            {
                try
                {
                    cmd.ExecuteNonQuery();
                    disconnectDatabse();
                }
                catch (Exception ex)
                {
                    disconnectDatabse();
                    error = true;
                    //merkintä logiin tai jtn vastaavaa
                }
            }
            if(error == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void changeCustomerInfo(Class.Asiakastiedot info)
        {

        }

        public void addCustomerData(Class.Asiakastiedot info, Class.giftCard giftCard, Class.Orders order)
        {
            connectDatabase();

            SqlCommand cmd1 = new SqlCommand("INSERT INTO [Lahjakortti](Voimassaolo)" +
                        "VALUES (@expiration); SELECT SCOPE_IDENTITY(); ", connect);
            cmd1.Parameters.AddWithValue("@expiration", giftCard.Voimassaolo);
            var tulos1 = cmd1.ExecuteScalar();
            info.LahjakorttiID = Convert.ToInt32(tulos1);


            SqlCommand cmd2 = new SqlCommand("INSERT INTO [Tilaukset](PVM, Kesto, Kerrat, Saaja, Maksettu)" +
                        "VALUES (@pvm, @Kesto, @usages, @recipient, @paid); SELECT SCOPE_IDENTITY();", connect);
            cmd2.Parameters.AddWithValue("@pvm", order.Pvm);
            cmd2.Parameters.AddWithValue("@Kesto", order.Duration);
            cmd2.Parameters.AddWithValue("@usages", order.Usages);
            cmd2.Parameters.AddWithValue("@recipient", order.Recipient);
            cmd2.Parameters.AddWithValue("@paid", order.Paid);
            var tulos = cmd2.ExecuteScalar();
            info.TilausID = Convert.ToInt32(tulos);


            SqlCommand cmd = new SqlCommand("INSERT INTO [Asiakastiedot](Etunimi, Sukunimi, Osoite, PuhNro, Sahkoposti, Postinumero, Paikka, PalveluID, TilausID, LahjakorttiID, MyyjaID)" +
                        "VALUES (@eNim, @sNim, @os, @pNro, @sPosti, @zip, @ptPaikka, @palvID, @tilID, @lahjID, @myyjaID)", connect);
            cmd.Parameters.AddWithValue("@eNim", info.Etunimi);
            cmd.Parameters.AddWithValue("@sNim", info.Sukunimi);
            cmd.Parameters.AddWithValue("@os", info.Osoite);
            cmd.Parameters.AddWithValue("@pNro", info.PuhNro);
            cmd.Parameters.AddWithValue("@sPosti", info.Sahkoposti);
            cmd.Parameters.AddWithValue("@zip", info.Postinumero);
            cmd.Parameters.AddWithValue("@ptPaikka", info.Paikka);
            cmd.Parameters.AddWithValue("@lahjID", info.LahjakorttiID);
            cmd.Parameters.AddWithValue("@tilID", info.TilausID);
            cmd.Parameters.AddWithValue("@palvID", info.PalveluID);
            cmd.Parameters.AddWithValue("@myyjaID", info.MyyjaID);

            cmd.ExecuteNonQuery();
            
            disconnectDatabse();
        }

        public List<Class.Orders> getOrders()
        {
            List<Class.Orders> allOrders = new List<Class.Orders>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Tilaukset", connect);
            connectDatabase();
            SqlDataReader read = cmd.ExecuteReader();
            if (read.HasRows)
            {
                while (read.Read())
                {
                    Class.Orders order = new Class.Orders();
                    order.ID = Convert.ToInt32(read.GetValue(0));
                    order.Pvm = Convert.ToDateTime(read.GetValue(1));
                    order.Duration = read.GetValue(2).ToString();
                    order.Usages = read.GetValue(3).ToString();
                    order.Recipient = read.GetValue(4).ToString();
                    order.Paid = Convert.ToInt32(read.GetValue(5));
                    allOrders.Add(order);
                }
            }
            disconnectDatabse();
            return allOrders;
        }

        public List<Class.Seller> getSellers()
        {
            List<Class.Seller> allSellers = new List<Class.Seller>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Myyja", connect);
            connectDatabase();
            SqlDataReader read = cmd.ExecuteReader();
            if (read.HasRows)
            {
                while (read.Read())
                {
                    Class.Seller seller = new Class.Seller();
                    seller.ID = Convert.ToInt32(read.GetValue(0));
                    seller.Myyja = read.GetValue(1).ToString();
                    allSellers.Add(seller);
                }
            }
            disconnectDatabse();
            return allSellers;
        }

        public bool addStaff(Class.Seller seller)
        {
            bool added = true;
            SqlCommand cmd = new SqlCommand("INSERT INTO [Myyja](Nimi)" +
                    "VALUES" +
                    "(@staffName)", connect);
            cmd.Parameters.AddWithValue("@staffName", seller.Myyja);
            connectDatabase();
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                added = false;
            }
            disconnectDatabse();
            return added;
        }
        
        public void removeStaffById(int id)
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM [Myyja] WHERE ID = @id", connect);
            cmd.Parameters.AddWithValue("@id", id);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {

            }
        }

        /* public void addTime(string time)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO [Tilaukset](Kesto)" +
                    "VALUES (@Kesto)", connect);
            cmd.Parameters.AddWithValue("@Kesto", time);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {

            }
        }

        public void removeTime(int id)
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM ", connect);
        }*/
    }
}
