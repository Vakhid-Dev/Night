using System;

public class Oracle_Procedure
{
    public static UserList list = new UserList();
    public class UserList
    {
        public List<User> users = new List<User>();
    }

    [Serializable]
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public int Age { get; set; }
        public User(int id, string fstname, string lstname, int age)
        {
            this.Id = id;
            this.FirstName = fstname;
            this.LastName = lstname;
            this.Age = age;
        }
        public User()
        {

        }

    }

    static void Run()
    {
      
        OracleConnection conn = DBOracleUtils.GetDBConnection();

        Console.WriteLine("Get Connection: " + conn);
        try
        {
            conn.Open();
            GetUsers(conn);

            Console.WriteLine(conn.ConnectionString, "Successful Connection");
        }
        catch (Exception ex)
        {
            Console.WriteLine("## ERROR: " + ex.Message);
            Console.Read();
            return;
        }
        finally
        {
            conn.Close();
            conn.Dispose();
        }



        Console.Read();

    }

    private static void InsertUser(OracleConnection conn)
    {
        try
        {

            string sql = "Insert into _USER (FIRSTNAME, LASTNAME, AGE) "
                         + " values (:fstName, :lstName, :age) ";
            OracleCommand cmd = new OracleCommand(sql, conn);
            cmd.Parameters.Add(":fstName", OracleDbType.Varchar2, 100).Value = "Alik";
            cmd.Parameters.Add(":lstName", OracleDbType.Varchar2, 100).Value = "Alikov";
            cmd.Parameters.Add(":age", OracleDbType.Int32, 100).Value = 26;
            // Выполнить Command (Используется для delete, insert, update).
            int rowCount = cmd.ExecuteNonQuery();
            Console.WriteLine("Row Count affected = " + rowCount);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e);
            Console.WriteLine(e.StackTrace);
        }
        finally
        {
            conn.Close();
            conn.Dispose();
            conn = null;
        }



    }

    private static void DeleteUser(OracleConnection conn)
    {
        try
        {
            var sql = "Delete from TAXFL_TEST.TEST_LAB_USER " +
                      "WHERE ID = :id";
            var cmd = new OracleCommand(sql, conn);
            cmd.Parameters.Add("id", OracleDbType.Int32, 100).Value = 65;
            var rowCount = cmd.ExecuteNonQuery();
            Console.WriteLine("Row Count affected = " + rowCount);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
        }
        finally
        {
            conn.Close();
            conn.Dispose();
            conn = null;
        }

    }

    private static void UpdateUser(OracleConnection conn)
    {
        try
        {
            string sql = "Update _USER set FIRSTNAME =:fstName ,LASTNAME=:lstName  where ID =:user_id";


            OracleCommand cmd = new OracleCommand(sql, conn);
            cmd.Parameters.Add("fstName", OracleDbType.Varchar2, 100).Value = "Oleg";
            cmd.Parameters.Add("lstName", OracleDbType.Varchar2, 100).Value = "Maximov";
            cmd.Parameters.Add("user_id", OracleDbType.Int32, 100).Value = 3;

            var rowCount = cmd.ExecuteNonQuery();
            Console.WriteLine("Row Count affected = " + rowCount);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            conn.Close();
            conn.Dispose();
            conn = null;
        }

    }
    private static void GetUsers(OracleConnection conn)
    {
        string sql = "SELECT * FROM _USER";
        // OracleCommand cmd = new OracleCommand();

        // cmd.Connection = conn;
        // cmd.CommandText = sql;

        OracleCommand cmd = new OracleCommand(sql, conn);


        using (OracleDataReader reader = cmd.ExecuteReader())
        {
            var ds = new DataSet();
            var adapter = new OracleDataAdapter(cmd);

            adapter.Fill(ds);
            var c = adapter;




            Console.WriteLine("Connection successful!");
            if (reader.HasRows)
            {

                User user;

                while (reader.Read())
                {

                    var id = reader.GetInt32(0);
                    string fname = reader.GetString(1); string lname = reader.GetString(2);
                    int age = reader.GetInt32(3);
                    Console.WriteLine("--------------------");
                    Console.WriteLine("UserId:" + id);
                    Console.WriteLine("FIRSTNAME:" + fname);
                    Console.WriteLine("LASTNAME:" + lname);
                    Console.WriteLine("AGE:" + age);
                    user = new User(id, fname, lname, age);
                    list.users.Add(user);
                }
            }

        }

    }

    private static void CallProcedure(OracleConnection conn)
    {
        try
        {
            var sql = "Get_Employee_Info";
            OracleCommand cmd = new OracleCommand(sql, conn);
            // Видом Command является StoredProcedure
            cmd.CommandType = CommandType.StoredProcedure;
            // Добавить параметр @p_Emp_Id и настроить его значение = 100.
            cmd.Parameters.Add("@p_Emp_Id", OracleDbType.Int32).Value = 100;

            // Добавить параметр @v_Emp_No вида Varchar(20).
            cmd.Parameters.Add(new OracleParameter("@v_Emp_No", OracleDbType.Varchar2, 20));
            cmd.Parameters.Add(new OracleParameter("@v_First_Name", OracleDbType.Varchar2, 50));
            cmd.Parameters.Add(new OracleParameter("@v_Last_Name", OracleDbType.Varchar2, 50));
            cmd.Parameters.Add(new OracleParameter("@v_Hire_Date", OracleDbType.Date));

            // Зарегистрировать параметр @v_Emp_No как OUTPUT.
            cmd.Parameters["@v_Emp_No"].Direction = ParameterDirection.Output;
            cmd.Parameters["@v_First_Name"].Direction = ParameterDirection.Output;
            cmd.Parameters["@v_Last_Name"].Direction = ParameterDirection.Output;
            cmd.Parameters["@v_Hire_Date"].Direction = ParameterDirection.Output;

            // Выполнить процедуру.
            cmd.ExecuteNonQuery();

            // Получить выходные значения.
            string empNo = cmd.Parameters["@v_Emp_No"].Value.ToString();
            string firstName = cmd.Parameters["@v_First_Name"].Value.ToString();
            string lastName = cmd.Parameters["@v_Last_Name"].Value.ToString();
            object hireDateObj = cmd.Parameters["@v_Hire_Date"].Value;

            Console.WriteLine("hireDateObj type: " + hireDateObj.GetType().ToString());
            OracleDate hireDate = (OracleDate)hireDateObj;


            Console.WriteLine("Emp No: " + empNo);
            Console.WriteLine("First Name: " + firstName);
            Console.WriteLine("Last Name: " + lastName);
            Console.WriteLine("Hire Date: " + hireDate);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            conn.Clone();
            conn.Dispose();
            conn = null;
        }

    }

    private static void CallFunction(OracleConnection conn)
    {
        try
        {
            var sql = "Get_Emp_No";
            var cmd = new OracleCommand(sql, conn);
            // Видом Command является StoredProcedure
            cmd.CommandType = CommandType.StoredProcedure;

            // ** Примечание: С Oracle, возвращенный параметр должен быть добавлен первым. 
            OracleParameter resultParam = new OracleParameter("@Result", OracleDbType.Varchar2, 50);

            // ReturnValue
            resultParam.Direction = ParameterDirection.ReturnValue;

            // Добавить в список параметров.
            cmd.Parameters.Add(resultParam);

            // Добавить параметр @p_Emp_Id и настроить его значение = 100.
            cmd.Parameters.Add("@p_Emp_Id", OracleDbType.Int32).Value = 100;

            // Вызвать функцию.
            cmd.ExecuteNonQuery();

            string empNo = null;
            if (resultParam.Value != DBNull.Value)
            {
                Console.WriteLine("resultParam.Value: " + resultParam.Value.GetType().ToString());
                Oracle.DataAccess.Types.OracleString ret = (Oracle.DataAccess.Types.OracleString)resultParam.Value;
                empNo = ret.ToString();
            }
            Console.WriteLine("Emp No: " + empNo);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            conn.Close();
            conn.Dispose();
            conn = null;
        }
    }

    private static void CallExecScalar(OracleConnection conn)
    {
        try
        {
            var sql = "SELECT COUNT(*) FROM _USER";
            var cmd = new OracleCommand(sql, conn);
            cmd.CommandType = CommandType.Text;
            // Метод ExecuteScalar возвращает значение первого столбца на первой строке.
            object countObj = cmd.ExecuteScalar();

            int count = 0;
            if (countObj != null)
            {
                count = Convert.ToInt32(countObj);
            }

            Console.WriteLine("Emp Count: " + count);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            conn.Close();
            conn.Dispose();
            conn = null;
        }
    }
}
