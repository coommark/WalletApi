using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Core.DomainEntities;
using Wallet.Core.Dto;
using Wallet.Core.Dto.Requests;
using Wallet.Core.Dto.ViewModels;
using Wallet.Core.Membership;

namespace Wallet.Core.Mappings
{
    public class DataProfile : Profile
    {
        public DataProfile()
        {
            CreateMap<ApplicationUser, UserRegistrationRequest>();

            CreateMap<CustomerAccount, CustomerAccountViewModel>();
            CreateMap<CustomerAccountViewModel, CustomerAccount>();

            CreateMap<CustomerAccount, AccountTypeChildViewModel>();
            CreateMap<AccountTypeChildViewModel, CustomerAccount>();

            //add
            CreateMap<CustomerAccountStatus, CustomerAccountStatusViewModel>();
            CreateMap<CustomerAccountStatusViewModel, CustomerAccountStatus>();

            CreateMap<CustomerTransaction, CustomerTransactionViewModel>();
            CreateMap<CustomerTransactionViewModel, CustomerTransaction>();

            CreateMap<Message, MessageViewModel>();
            CreateMap<MessageViewModel, Message>();
        }
    }
}
