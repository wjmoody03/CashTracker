using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ct.Data.Contexts;
using ct.Domain.Models;

namespace ct.Data.Repositories
{
    public interface IReminderRepository:IGenericRepositoryEF<Reminder>
    {
        
    }

    public class ReminderRepository : GenericRepositoryEF<Reminder>, IReminderRepository
    {
        IctContext Context;

        public ReminderRepository(IctContext context)
            : base(context)
        {
            Context = context;
        }
    }
}
