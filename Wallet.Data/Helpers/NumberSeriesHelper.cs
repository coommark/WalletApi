using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Data.Helpers
{
    public static class NumberSeriesHelper
    {
        public static string CreateAccountNumber(int userId, string accountCode)
        {
            string id = userId.ToString();
            var idLenght = id.Length;
            int padLenght = 6;
            switch(idLenght)
            {
                case 2:
                    padLenght = 5;
                    break;
                case 3:
                    padLenght = 4;
                    break;
                case 4:
                    padLenght = 3;
                    break;
                case 5:
                    padLenght = 2;
                    break;
                case 6:
                    padLenght = 1;
                    break;
            }
            char pad = '0';
            string result = userId.ToString().PadLeft(padLenght, pad);
            return accountCode + result;
        }
    }
}
