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

        public override async Task<IEnumerable<SalaryPayment>> CreateAsync(IEnumerable<SalaryPayment> self)
        {
            foreach (var payment in self)
            {
                if (string.IsNullOrEmpty(payment.Name))
                {
                    var seqObj = GetService<IIRSequenceService>();
                    if (payment.Type == "advance")
                    {
                        payment.Name = await seqObj.NextByCode("salarypayment.advance");
                        if (string.IsNullOrEmpty(payment.Name))
                        {
                            await _InsertSalaryPaymentAdvanceSequence();
                            payment.Name = await seqObj.NextByCode("salarypayment.advance");
                        }
                    }
                    else if (payment.Type == "salary")
                    {
                        payment.Name = await seqObj.NextByCode("salarypayment.salary");
                        if (string.IsNullOrEmpty(payment.Name))
                        {
                            await _InsertSalaryPaymentSalarySequence();
                            payment.Name = await seqObj.NextByCode("salarypayment.salary");
                        }
                    }
                }
            }

            return await base.CreateAsync(self);
        }


        public async Task<SalaryPayment> CreateSalaryPayment(SalaryPaymentSave val)
        {
            var salaryPayment = _mapper.Map<SalaryPayment>(val);
            salaryPayment.CompanyId = CompanyId;

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
                salaryPayment.State = "waiting";
                salaryPayment.Type = "salary";
                salaryPayment.CompanyId = CompanyId;

                listSalaryPayment.Add(salaryPayment);
                salaryPaymentDict.Add(item.HrPayslipId.Value, salaryPayment);
            }

            await CreateAsync(listSalaryPayment);
            await ActionConfirm(listSalaryPayment.Select(x => x.Id).ToList());

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
                 .Include("Journal.DefaultDebitAccount")
                 .Include(x => x.Employee)
                 .ToListAsync();

            var moveObj = GetService<IAccountMoveService>();

            foreach (var salaryPayment in salaryPayments)
            {
                if (salaryPayment.State != "waiting")
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
                phieu.State = "waiting";
            }

            await UpdateAsync(self);
        }

        private async Task<AccountMove> _PrepareSalaryPaymentMoves(SalaryPayment payment)
        {
            var accDebit334 = await getAccount334();
            var liquidity_line_account = payment.Journal.DefaultDebitAccount;
            var partnerId = await GetPartnerForMoveLine(payment);

            var move = new AccountMove
            {
                Date = payment.Date,
                Ref = payment.Reason,
                JournalId = payment.JournalId,
                Journal = payment.Journal,
                PartnerId = partnerId,
                CompanyId = payment.CompanyId,
            };

            var rec_pay_line_name = "/";
            if (payment.Type == "advance")
                rec_pay_line_name = payment.Name + " " + "Tạm ứng";
            else if (payment.Type == "salary")
                rec_pay_line_name = payment.Name + " " + "Chi lương";

            var liquidity_line_name = payment.Name;
            var balance = payment.Amount;
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
                    Ref = payment.Name,
                    PartnerId = partnerId
                },
                new AccountMoveLine
                {
                    Name = liquidity_line_name,
                    Debit = balance < 0 ? -balance : 0,
                    Credit = balance > 0 ? balance : 0,
                    AccountId = liquidity_line_account.Id,
                    Account = liquidity_line_account,
                    Move = move,
                    Ref = payment.Name,
                    PartnerId = partnerId
                },
            };

            move.Lines = lines;

            return move;

        }

        private async Task<Guid> GetPartnerForMoveLine(SalaryPayment payment)
        {
            if (payment.Employee.PartnerId.HasValue)
                return payment.Employee.PartnerId.Value;

            var partnerObj = GetService<IPartnerService>();
            var employeeObj = GetService<IEmployeeService>();

            var employee = payment.Employee;
            var partner = new Partner()
            {
                Name = employee.Name,
                Customer = false,
                Supplier = false,
                CompanyId = employee.CompanyId,
                Phone = employee.Phone,
                Ref = employee.Ref,
                Email = employee.Email,
                Employee = true
            };

            await partnerObj.CreateAsync(partner);
            employee.PartnerId = partner.Id;
            await employeeObj.UpdateAsync(employee);

            return partner.Id;
        }

        public async Task<AccountAccount> getAccount334()
        {
            var irModelDataObj = GetService<IIRModelDataService>();
            var accountObj = GetService<IAccountAccountService>();
            var accountJournalObj = GetService<IAccountJournalService>();

            var currentLiabilities = await irModelDataObj.GetRef<AccountAccountType>("account.data_account_type_current_liabilities");
            var acc334 = new AccountAccount();
            acc334 = await accountObj.SearchQuery(x => x.Code == "334" && x.CompanyId == CompanyId).FirstOrDefaultAsync();
            if (acc334 == null)
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
            if (acc642 == null)
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
                if (slip.SalaryPayment != null) continue;
                payments.Add(new SalaryPaymentDisplay()
                {
                    Amount = slip.NetSalary.GetValueOrDefault(),
                    Date = DateTime.Now,
                    EmployeeId = slip.EmployeeId,
                    Employee = _mapper.Map<EmployeeSimple>(slip.Employee),
                    JournalId = journal.Id,
                    Journal = journal,
                    Reason = "Chi lương tháng " + slipRun.Date.Value.ToString("MM/yyyy"),
                    Type = "advance",
                    HrPayslipId = slip.Id
                });
            }
            return payments;
        }
    }
}
