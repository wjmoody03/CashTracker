using ct.Domain.Models;
using ct.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ct.Web.App_Start
{
    public class AutoMapperConfig
    {
        public static void Configure()
        {
            AutoMapper.Mapper.CreateMap<Transaction, TransactionViewModel>()
                .ForMember(vm => vm.Month, vm => vm.MapFrom(t => t.TransactionDate.ToString("MMMM yyyy")));

        }
    }
}