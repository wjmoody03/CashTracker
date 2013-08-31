using ct.Data.Contexts;
using ct.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Tests.Data.Contexts
{
    public class LocalDbInitializer:DropCreateDatabaseAlways<ctContext>
    {
        protected override void Seed(ctContext context)
        {
            context.SaveChanges();
            base.Seed(context);
        }
       
    }
}
