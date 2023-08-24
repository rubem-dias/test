using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Refit;

namespace Persistence.Http.Interfaces
{
    public interface IViaCep
    {
        Task<List<CepResponse>> GetAddress(List<string> cep);
    }
}