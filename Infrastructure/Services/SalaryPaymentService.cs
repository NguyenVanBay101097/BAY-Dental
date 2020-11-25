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

            await CheckEmployeePartner(salaryPayment);

            return await CreateAsync(salaryPayment);
        }


        public async Task<IEnumerable<Guid>> CreateAndConfirmMultiSalaryPayment(IEnumerable<MultiSalaryPaymentVm> vals)
        {
            var listSalaryPayment = new List<SalaryPayment>();
            var salaryPaymentDict = new Dictionary<Guid, SalaryPayment>();
            foreach (var item in vals)
            {
                var salaryPayment = new SalaryPayment();
                salaryPayment.JournalId = item.JournalId.Value;
                salaryPayment.Date = item.Date;
                salaryPayment.EmployeeId = item.EmployeeId;
                salaryPayment.Reason = item.Reason;
                salaryPayment.Amount = item.Amount.Value;
                salaryPayment.State = "waitting";
                salaryPayment.Type = "salary";
                salaryPayment.CompanyId = CompanyId;

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



                await CreateAsync(salaryPayment);

                //await ActionConfirm(new List<Guid>() { salaryPayment.Id });

                salaryPaymentDict.Add(item.HrPayslipId.Value, salaryPayment);


            }

            var hrPayslipObj = GetService<IHrPayslipService>();
            var hrpayslipIds = vals.Select(x => x.HrPayslipId).ToList();
            var hrPayslips = await hrPayslipObj.SearchQuery(x => hrpayslipIds.Contains(x.Id)).ToListAsync();
            var hrPayslipDict = hrPayslips.ToDictionary(x => x.Id, x => x);

            ///update salary payment
            foreach (var item in salaryPaymentDict)
            {
                if (!hrPayslipDict.ContainsKey(item.Key))
                    continue;
                var hrPayslip = hrPayslipDict[item.Key];
                hrPayslip.SalaryPaymentId = item.Value.Id;
            }

            await hrPayslipObj.UpdateAsync(hrPayslips);


            var salaryPaymentIds = hrPayslips.Select(x => x.SalaryPaymentId.Value).ToList();

            return salaryPaymentIds;
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
            await CheckEmployeePartner(salaryPayment);
            await UpdateAsync(salaryPayment);
        }


        public async Task CheckEmployeePartner(SalaryPayment val)
        {
            var employeeObj = GetService<IEmployeeService>();
            var partnerObj = GetService<IPartnerService>();

            var employee = await employeeObj.SearchQuery(x => x.Id == val.EmployeeId).FirstOrDefaultAsync();
            if (!employee.PartnerId.HasValue)
            {
                var partner = new Partner()
                {
                    Name = employee.Name,
                    Customer = false,
                    Supplier = false,
                    CompanyId = employee.CompanyId,
                    Phone = employee.Phone,
                    Ref = employee.Ref,
                    Email = employee.Email

                };

                await partnerObj.CreateAsync(partner);
                employee.PartnerId = partner.Id;
                await employeeObj.UpdateAsync(employee);
            }

            val.EmployeeId = employee.Id;

        }

        public async Task ActionConfirm(IEnumerable<Guid> ids)
        {
            var salaryPayments = await SearchQuery(x => ids.Contains(x.Id))
                 .Include(x => x.Company)
                 .Include(x => x.Journal)
                 .Include(x => x.Employee)
                 .Include("Employee.Partner")
                 .ToListAsync();

            var moveObj = GetService<IAccountMoveService>();

            foreach (var salaryPayment in salaryPayments)
            {
                if (salaryPayment.State != "waitting")
                    throw new Exception("Chỉ những phiếu nháp mới được vào sổ.");

                var move = await _PrepareSalaryPaymentMoves(salaryPayment);

                var amlObj = GetService<IAccountMoveLineService>();
                amlObj.PrepareLines(move.Lines);

                await moveObj.CreateMoves(new List<AccountMove>() { move });
                await moveObj.ActionPost(new List<AccountMove>() { move });

                amlObj.ComputeMoveNameState(move.Lines);

                salaryPayment.State = "done";
                salaryPayment.MoveId = move.Id;
            }

            await UpdateAsync(salaryPayments);
        }

        public async Task ActionCancel(IEnumerable<Guid> ids)
        {
            var moveObj = GetService<IAccountMoveService>();
            var moveLineObj = GetService<IAccountMoveLineService>();

            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Move).ToListAsync();

            foreach (var phieu in self)
            {
                await moveObj.ButtonCancel(new List<Guid>() { phieu.MoveId.Value });
                await moveObj.Unlink(new List<Guid>() { phieu.MoveId.Value });
                phieu.State = "waitting";
            }

            await UpdateAsync(self);
        }

        private async Task<AccountMove> _PrepareSalaryPaymentMoves(SalaryPayment val)
        {
            var all_move_vals = new List<AccountMove>();
            var accDebit334 = await getAccount334();
            var accCredit642 = await getAccount642();


            var accountJournalObj = GetService<IAccountJournalService>();

            var accountJournal = await accountJournalObj.GetJournalByTypeAndCompany($"{val.Journal.Type}", val.CompanyId.Value);


            var move = new AccountMove
            {
                JournalId = accountJournal.Id,
                Journal = accountJournal,
                CompanyId = val.CompanyId,
            };

            var rec_pay_line_name = "/";
            if (val.Type == "advance")
                rec_pay_line_name = val.Name + " " + "tạm ứng";
            else if (val.Type == "salary")
                rec_pay_line_name = val.Name + " " + "chi lương";


            var liquidity_line_name = val.Name;
            var balance = val.Amount;
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
                        Ref = val.Name,
                        PartnerId = val.Employee.PartnerId.HasValue ? val.Employee.PartnerId : null
                    },
                    new AccountMoveLine
                    {
                        Name = liquidity_line_name,
                        Debit = balance < 0 ? -balance : 0,
                        Credit = balance > 0 ? balance : 0,
                        AccountId = accCredit642.Id,
                        Account = accCredit642,
                        Move = move,
                        Ref = val.Name,
                        PartnerId = val.Employee.PartnerId.HasValue ? val.Employee.PartnerId : null
                    },
                };

            move.Lines = lines;

            return move;

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

        public async Task<IEnumerable<SalaryPaymentDisplay>> DefaulCreateBy(SalaryPaymentDefaultGetModel val)
        {
            var slipRunObj = GetService<IHrPayslipRunService>();
            var JournalObj = GetService<IAccountJournalService>();
            var slipRun = await slipRunObj.SearchQuery(x => x.Id == val.PayslipRunId).Include("Slips.SalaryPayment")
                .Include(x => x.Slips).ThenInclude(x => x.Employee).Select(x => new
                {
                    Date = x.Date,
                    Slips = x.Slips.Where(y => val.PayslipIds.Contains(y.Id))
                }).FirstOrDefaultAsync();
            var journal = await _mapper.ProjectTo<AccountJournalSimple>(JournalObj.SearchQuery(x => x.Name.ToLower().Contains("tiền mặt"))).FirstOrDefaultAsync();
            if (journal == null)
            {
                journal = await _mapper.ProjectTo<AccountJournalSimple>(JournalObj.SearchQuery()).FirstOrDefaultAsync();
            }

            if (slipRun == null) throw new Exception("không tồn tại đợt lương!");
            var payments = new List<SalaryPaymentDisplay>();
            foreach (var slip in slipRun.Slips)
            {
                if (slip.SalaryPayment != null) throw new Exception(@$"{slip.Employee.Name} đã chi lương");
                payments.Add(new SalaryPaymentDisplay()
                {
                    Amount = slip.NetSalary.GetValueOrDefault(),
                    Date = slipRun.Date.Value,
                    EmployeeId = slip.EmployeeId,
                    Employee = _mapper.Map<EmployeeSimple>(slip.Employee),
                    JournalId = journal.Id,
                    Journal = journal,
                    Reason = "Chi lương tháng" + slipRun.Date.Value.ToString("MM/yyyy"),
                    Type = "advance",
                    HrPayslipId = slip.Id
                });
            }
            return payments;
        }
    }
}
