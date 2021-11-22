using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    /// <summary>
    /// thanh toán chung hoặc có nhiều bussiness liên quan đến thanh toán, không thuộc cụ thể 1 đối tượng nào
    /// </summary>
    public class PaymentsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IPaymentService _paymentService;
        public PaymentsController(IMapper mapper, IUnitOfWorkAsync unitOfWork, IPaymentService paymentService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> SaleOrderCustomerDebtPayment(SaleOrderCustomerDebtPaymentReq val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var res = await _paymentService.SaleOrderCustomerDebtPayment(val);
            _unitOfWork.Commit();
            return Ok(_mapper.Map<SaleOrderPaymentDisplay>(res));
        }

    }
}
