using ct.Data.Contexts;
using ct.Domain.Models;
using ct.Tests.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Tests.Data.Repositories
{
    public abstract class RepositoryTestClass
    {
        //this class only serves to drop and recreate the local db when necessary. it also exposes a context object to all sub classes
        protected ctContext context = new ctContext();
        
        public RepositoryTestClass()
        {
            //FORCE DROP DATABASE 
            var csb = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ctContext"].ConnectionString);
            string originalDB = csb.InitialCatalog;
            csb.InitialCatalog = "Master";
            var cn = new SqlConnection(csb.ConnectionString);
            var sc = new SqlCommand(string.Format("IF EXISTS(select * from master.sys.databases where name = '{0}') " +
                    "BEGIN " +
                        "ALTER DATABASE [{0}] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE; " +
                        "DROP DATABASE [{0}]; " +
                    "END", originalDB), cn);
            cn.Open();
            sc.ExecuteNonQuery();
            cn.Close();

            System.Data.Entity.Database.SetInitializer<ctContext>(new LocalDbInitializer());
            ResetDatabase();
        }

        protected void ResetDatabase()
        {
            context.Database.Initialize(true);
        }
    
    }
}
