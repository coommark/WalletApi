using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Core.DomainEntities;
using Wallet.Core.Dto;
using Wallet.Core.Dto.Responses;
using Wallet.Core.Dto.ViewModels;
using Wallet.Core.Membership;

namespace Wallet.Core.Mappings
{
    public class DataProfile : Profile
    {
        public DataProfile()
        {
            CreateMap<ApplicationUser, UserRegistrationDto>();

            CreateMap<CustomerAccount, CustomerAccountCreateResponse>();
            CreateMap<CustomerAccountCreateResponse, CustomerAccount>();


            CreateMap<CustomerAccount, CustomerAccountViewModel>();
            CreateMap<CustomerAccountViewModel, CustomerAccount>();

            CreateMap<CustomerAccount, AccountTypeChildViewModel>();
            CreateMap<AccountTypeChildViewModel, CustomerAccount>();
        }
    }
}
