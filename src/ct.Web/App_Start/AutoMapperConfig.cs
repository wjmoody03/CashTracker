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
                .ForMember(vm => vm.AccountName, vm => vm.MapFrom(t => t.Account.AccountName))
                .ForMember(vm => vm.Amount, vm => vm.MapFrom(t => t.TransactionType.DisplayMultiplier * t.Amount))
                .ForMember(vm => vm.TransactionTypeDescription, vm => vm.MapFrom(t => t.TransactionType.TransactionTypeDescription))
                .ForMember(vm => vm.TransactionType, vm => vm.Ignore()) //this is me being lazy and using inheritance for the view model
                .ForMember(vm => vm.Account, vm => vm.Ignore()) //this is me being lazy and using inheritance for the view model
                .ForMember(vm => vm.Month, vm => vm.MapFrom(t => t.TransactionDate.ToString("MMMM yyyy")));

        }
    }
}