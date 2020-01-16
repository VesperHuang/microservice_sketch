using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;

namespace microservice_sketch.Services
{
    public interface IEncryptionService{ 
    
    }

    public class EncryptionService: IEncryptionService,IApiService
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private const string Key = "cxz92k13md8f981hu6y7alkc";

        public EncryptionService(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        public string Encrypt(string input)
        {
            var protector = _dataProtectionProvider.CreateProtector(Key);
            return protector.Protect(input);
        }

        public string Decrypt(string cipherText)
        {
            var protector = _dataProtectionProvider.CreateProtector(Key);
            return protector.Unprotect(cipherText);
        }

        public void info(string function_name)
        {
            Console.WriteLine("Encryption Service Info " + function_name);
        }
    }
}
