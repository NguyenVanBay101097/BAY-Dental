using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AccountPartialReconcileService : BaseService<AccountPartialReconcile>, IAccountPartialReconcileService
    {

        public AccountPartialReconcileService(IAsyncRepository<AccountPartialReconcile> repository, IHttpContextAccessor httpContextAccessor)
        : base(repository, httpContextAccessor)
        {
        }

        public override async Task<IEnumerable<AccountPartialReconcile>> CreateAsync(IEnumerable<AccountPartialReconcile> recs)
        {
            await base.CreateAsync(recs);
            //check if the reconcilation is full
            //first, gather all journal items involved in the reconciliation just created
            var partial_rec_set = recs.Distinct();
            var aml_set = new List<AccountMoveLine>().AsEnumerable();
            decimal total_debit = 0;
            decimal total_credit = 0;
            AccountMoveLine amlToBalance = null;
            foreach (var partialRec in partial_rec_set)
            {
                foreach (var aml in new AccountMoveLine[] { partialRec.DebitMove, partialRec.CreditMove })
                {
                    if (!aml_set.Contains(aml))
                    {
                        if (aml.AmountResidual != 0)
                            amlToBalance = aml;
                        total_debit += aml.Debit;
                        total_credit += aml.Credit;
                        aml_set = aml_set.Union(new List<AccountMoveLine>() { aml });
                    }
                }
            }

            if (total_debit == total_credit)
            {
                //mark the reference of the full reconciliation on the partial ones and on the entries
                var fullRecObj = GetService<IAccountFullReconcileService>();
                await fullRecObj.CreateAsync(new AccountFullReconcile
                {
                    PartialReconciles = partial_rec_set.ToList(),
                    ReconciledLines = aml_set.ToList(),
                });
            }


            return recs;
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.FullReconcile).ToListAsync();
            await Unlink(self);
        }

        public async Task Unlink(IEnumerable<AccountPartialReconcile> self)
        {
            var full_to_unlink = new List<AccountFullReconcile>().AsEnumerable();
            foreach (var rec in self)
            {
                if (rec.FullReconcile != null)
                    full_to_unlink = full_to_unlink.Union(new List<AccountFullReconcile>() { rec.FullReconcile });
            }

            await DeleteAsync(self);
            if (full_to_unlink.Any())
            {
                var accFullRecObj = GetService<IAccountFullReconcileService>();
                await accFullRecObj.DeleteAsync(full_to_unlink);
            }
        }
    }
}
