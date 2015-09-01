using ct.Data.Repositories;
using ct.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Business
{
    public static class CategoryGuesser
    {
        public static void ApplyCategories(IEnumerable<CategoryGuess> guesses, ref AccountDownloadResult adr)
        {
            var gLookup = guesses.ToLookup(g => g.Description, g => g.Category);
            foreach(var t in adr.NewTransactions)
            {
                t.Category = gLookup[t.Description].FirstOrDefault();
                if(t.Category== null)
                {
                    adr.TransactionsWithUnknownCategories++;
                }
            }
        }
    }
}
