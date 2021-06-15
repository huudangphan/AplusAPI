using System.Data;
using Apzon.Libraries.HDBConnection.Interfaces;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Entities;
using Apzon.PosmanErp.Services.Interfaces.Sales;
using Apzon.PosmanErp.Services.Repository.Documents;

namespace Apzon.PosmanErp.Services.Repository.Sales
{
    internal class QuotationRepository : DocumentsRepository, IQuotation
    {
        public QuotationRepository(IDatabaseClient databaseService) : base(databaseService)
        {
        }
    }
}