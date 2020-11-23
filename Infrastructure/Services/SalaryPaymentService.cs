using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class SalaryPaymentService : BaseService<SalaryPayment>, ISalaryPaymentService
    {
        private readonly IMapper _mapper;

        public SalaryPaymentService(IAsyncRepository<SalaryPayment> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }



        public async Task<SalaryPayment> CreateSalaryPayment(SalaryPaymentSave val)
        {
            var salaryPayment = _mapper.Map<SalaryPayment>(val);

            if (string.IsNullOrEmpty(salaryPayment.Name))
            {
                var seqObj = GetService<IIRSequenceService>();
                if (salaryPayment.Type == "advance")
                {
                    salaryPayment.Name = await seqObj.NextByCode("salarypayment.advance");
                    if (string.IsNullOrEmpty(salaryPayment.Name))
                    {
                        await _InsertSalaryPaymentAdvanceSequence();
                        salaryPayment.Name = await seqObj.NextByCode("salarypayment.advance");
                    }
                }
                else if (salaryPayment.Type == "salary")
                {
                    salaryPayment.Name = await seqObj.NextByCode("salarypayment.salary");
                    if (string.IsNullOrEmpty(salaryPayment.Name))
                    {
                        await _InsertSalaryPaymentSalarySequence();
                        salaryPayment.Name = await seqObj.NextByCode("salarypayment.salary");
                    }
                }
                else
                    throw new Exception("Not support");
            }

            salaryPayment.CompanyId = CompanyId;

            return await CreateAsync(salaryPayment);
        }


        public async Task CreateAndConfirmMultiSalaryPayment(IEnumerable<SalaryPaymentSave> vals)
        {
            var listSalaryPayment = new List<SalaryPayment>();
            foreach(var item in vals)
            {
                var salaryPayment = _mapper.Map<SalaryPayment>(item);

                if (string.IsNullOrEmpty(salaryPayment.Name))
                {
                    var seqObj = GetService<IIRSequenceService>();
                    if (salaryPayment.Type == "advance")
                    {
                        salaryPayment.Name = await seqObj.NextByCode("salarypayment.advance");
                        if (string.IsNullOrEmpty(salaryPayment.Name))
                        {
                            await _InsertSalaryPaymentAdvanceSequence();
                            salaryPayment.Name = await seqObj.NextByCode("salarypayment.advance");
                        }
                    }
                    else if (salaryPayment.Type == "salary")
                    {
                        salaryPayment.Name = await seqObj.NextByCode("salarypayment.salary");
                        if (string.IsNullOrEmpty(salaryPayment.Name))
                        {
                            await _InsertSalaryPaymentSalarySequence();
                            salaryPayment.Name = await seqObj.NextByCode("salarypayment.salary");
                        }
                    }
                    else
                        throw new Exception("Not support");
                }

                salaryPayment.CompanyId = CompanyId;

                listSalaryPayment.Add(salaryPayment);
            }

            await CreateAsync(listSalaryPayment);

            /// xac nhan 
            var ids = listSalaryPayment.Select(x => x.Id).ToList();
            await ActionConfirm(ids);
        }




        private async Task _InsertSalaryPaymentAdvanceSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Name = "Phiếu tạm ứng",
                Code = "salarypayment.advance",
                Prefix = "AVC/{yyyy}/",
                Padding = 4
            });
        }

        private async Task _InsertSalaryPaymentSalarySequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Name = "Phiếu chi lương",
                Code = "salarypayment.salary",
                Prefix = "PW/{yyyy}/",
                Padding = 4
            });
        }

        public async Task UpdateSalaryPayment(Guid id, SalaryPaymentSave val)
        {
            var salaryPayment = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (salaryPayment == null)
                throw new Exception("Phiếu không tồn tại");

            salaryPayment = _mapper.Map(val, salaryPayment);

            await UpdateAsync(salaryPayment);
        }

        public override async Task<SalaryPayment> CreateAsync(SalaryPayment entity)
        {
            var employeeObj = GetService<IEmployeeService>();
            var partnerObj = GetService<IPartnerService>();
            var model = await base.CreateAsync(entity);

            var employee = await employeeObj.SearchQuery(x => x.Id == entity.Id).FirstOrDefaultAsync();
            if (!employee.PartnerId.HasValue)
            {
                var partner = new Partner()
                {
                    Name = employee.Name,
                    Customer = false,
                    Supplier = false,
                };

                await partnerObj.CreateAsync(partner);
                employee.PartnerId = partner.Id;
                await employeeObj.UpdateAsync(employee);
            }
            return model;
        }

        public override async Task UpdateAsync(SalaryPayment entity)
        {
            var employeeObj = GetService<IEmployeeService>();
            var partnerObj = GetService<IPartnerService>();
            await base.UpdateAsync(entity);

            var employee = await employeeObj.SearchQuery(x => x.Id == entity.Id).FirstOrDefaultAsync();
            if (!employee.PartnerId.HasValue)
            {
                var partner = new Partner()
                {
                    Name = employee.Name,
                    Customer = false,
                    Supplier = false,
                };

                await partnerObj.CreateAsync(partner);
                employee.PartnerId = partner.Id;
                await employeeObj.UpdateAsync(employee);
            }
        }

        public async Task ActionConfirm(IEnumerable<Guid> ids)
        {
            var salaryPayments = await SearchQuery(x => ids.Contains(x.Id))
                 .Include(x => x.Company)
                 .Include(x => x.Journal)
                 .Include(x => x.Employee)
                 .Include("Employee.Partner")
                 .Include(x => x.MoveLines)
                 .ToListAsync();

            var moveObj = GetService<IAccountMoveService>();

            foreach (var salaryPayment in salaryPayments)
            {
                if (salaryPayment.State != "draft")
                    throw new Exception("Chỉ những phiếu nháp mới được vào sổ.");

                var moves = await _PrepareSalaryPaymentMoves(salaryPayments);

                var amlObj = GetService<IAccountMoveLineService>();
                foreach (var move in moves)
                    amlObj.PrepareLines(move.Lines);

                await moveObj.CreateMoves(moves);
                await moveObj.ActionPost(moves);

                foreach (var move in moves)
                    amlObj.ComputeMoveNameState(move.Lines);

                salaryPayment.State = "posted";
            }

            await UpdateAsync(salaryPayments);
        }

        public async Task ActionCancel(IEnumerable<Guid> ids)
        {
            var moveObj = GetService<IAccountMoveService>();
            var moveLineObj = GetService<IAccountMoveLineService>();

            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.MoveLines).ToListAsync();

            foreach (var phieu in self)
            {
                var move_ids = phieu.MoveLines.Select(x => x.MoveId).Distinct().ToList();
                await moveObj.ButtonCancel(move_ids);
                await moveObj.Unlink(move_ids);

                phieu.State = "draft";
            }

            await UpdateAsync(self);
        }

        private async Task<IList<AccountMove>> _PrepareSalaryPaymentMoves(IList<SalaryPayment> val)
        {
            var all_move_vals = new List<AccountMove>();
            var accDebit334 = await getAccount334();
            var accCredit642 = await getAccount642();
            foreach (var phieu in val)
            {

                var accountJournalObj = GetService<IAccountJournalService>();

                var accountJournal = await accountJournalObj.GetJournalByTypeAndCompany($"{phieu.Journal.Type}", phieu.CompanyId.Value);


                var move = new AccountMove
                {
                    JournalId = accountJournal.Id,
                    Journal = accountJournal,
                    CompanyId = phieu.CompanyId,
                };

                var rec_pay_line_name = "/";
                if (phieu.Type == "advance")
                    rec_pay_line_name = "tạm ứng";
                else if (phieu.Type == "salary")
                    rec_pay_line_name = "chi lương";


                var liquidity_line_name = phieu.Name;
                var balance = phieu.Amount;
                var lines = new List<AccountMoveLine>()
                {

                     new AccountMoveLine
                    {
                        Name =  rec_pay_line_name,
                        Debit = balance > 0 ? balance : 0,
                        Credit = balance < 0 ? -balance : 0,
                        AccountId = accDebit334.Id,
                        Account = accDebit334,
                        Move = move,
                        PartnerId = phieu.Employee.PartnerId.HasValue ? phieu.Employee.PartnerId : null
                    },
                    new AccountMoveLine
                    {
                        Name = liquidity_line_name,
                        Debit = balance < 0 ? -balance : 0,
                        Credit = balance > 0 ? balance : 0,
                        AccountId = accCredit642.Id,
                        Account = accCredit642,
                        Move = move,
                        PartnerId = phieu.Employee.PartnerId.HasValue ? phieu.Employee.PartnerId : null
                    },
                };

                move.Lines = lines;

                all_move_vals.Add(move);
            }

            return all_move_vals;

        }

        public async Task<AccountAccount> getAccount334()
        {
            var irModelDataObj = GetService<IIRModelDataService>();
            var accountObj = GetService<IAccountAccountService>();
            var accountJournalObj = GetService<IAccountJournalService>();

            var currentLiabilities = await irModelDataObj.GetRef<AccountAccountType>("account.data_account_type_current_liabilities");
            var acc334 = new AccountAccount();
            acc334 = await accountObj.SearchQuery(x => x.Code == "334" && x.CompanyId == CompanyId).FirstOrDefaultAsync();
            if (acc334 != null)
            {
                acc334 = new AccountAccount
                {
                    Name = "Phải trả người lao động",
                    Code = "334",
                    InternalType = currentLiabilities.Type,
                    UserTypeId = currentLiabilities.Id,
                    CompanyId = CompanyId,
                };

                await accountObj.CreateAsync(acc334);
            }

            return acc334;

        }

        public async Task<AccountAccount> getAccount642()
        {
            var irModelDataObj = GetService<IIRModelDataService>();
            var accountObj = GetService<IAccountAccountService>();
            var accountJournalObj = GetService<IAccountJournalService>();

            var expensesType = await irModelDataObj.GetRef<AccountAccountType>("account.data_account_type_expenses");
            var acc642 = new AccountAccount();
            acc642 = await accountObj.SearchQuery(x => x.Code == "642" && x.CompanyId == CompanyId).FirstOrDefaultAsync();
            if (acc642 != null)
            {
                acc642 = new AccountAccount
                {
                    Name = "Phải trả người lao động",
                    Code = "642",
                    InternalType = expensesType.Type,
                    UserTypeId = expensesType.Id,
                    CompanyId = CompanyId,
                };

                await accountObj.CreateAsync(acc642);
            }

            return acc642;

        }

        public async Task InsertModelsIfNotExists()
        {
            var modelObj = GetService<IIRModelService>();
            var modelDataObj = GetService<IIRModelDataService>();
            var model = await modelDataObj.GetRef<IRModel>("account.model_salary_payment");
            if (model == null)
            {
                model = new IRModel
                {
                    Name = "Phiếu tạm ứng chi lương",
                    Model = "SalaryPayment",
                };

                modelObj.Sudo = true;
                await modelObj.CreateAsync(model);

                await modelDataObj.CreateAsync(new IRModelData
                {
                    Name = "model_salary_payment",
                    Module = "sale",
                    Model = "ir.model",
                    ResId = model.Id.ToString()
                });
            }
        }
    }
}
