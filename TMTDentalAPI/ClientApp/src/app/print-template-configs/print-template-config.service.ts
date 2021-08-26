import { HttpClient } from '@angular/common/http';
import { Inject, Injectable, Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

export class PrintTemplateConfigSave {
  content: string;
  type: string;
  printPaperSizeId: string;
  companyId: string;
}

export class PrintTemplateConfigDisplay {
  content: string;
  type: string;
  printPaperSizeId: string;
  companyId: string;
}

@Pipe({ name: "safeHtml" })
export class SafeHtmlPipe implements PipeTransform {
  constructor(private sanitizer: DomSanitizer) { }

  transform(value) {
    return this.sanitizer.bypassSecurityTrustHtml(value);
  }
}

@Injectable()
export class PrintTemplateConfigService {
  types = [
    { text: 'Phiếu điều trị', value: 'tmp_sale_order' },
    { text: 'Biên lai khách hàng thanh toán', value: 'tmp_account_payment' },
    { text: 'Tình trạng răng', value: 'tmp_advisory' },
    { text: 'Khách hàng tạm ứng', value: 'tmp_partner_advance' },
    { text: 'Biên lai thanh toán nhà cung cấp', value: 'tmp_supplier_payment' },
    { text: 'Phiếu thu', value: 'tmp_phieu_thu' },
    { text: 'Phiếu chi', value: 'tmp_phieu_chi' },
    { text: 'Công nợ khách hàng', value: 'tmp_partner_debt' },
    { text: 'Phiếu chi hoa hồng', value: 'tmp_agent_commission' },
    { text: 'Phiếu mua hàng', value: 'tmp_purchase_order' },
    { text: 'Phiếu trả hàng', value: 'tmp_purchase_refund' },
    { text: 'Phiếu báo giá', value: 'tmp_quotation' },
    { text: 'Phiếu thanh toán lương nhân viên', value: 'tmp_salary_employee' },
    { text: 'Phiếu tạm ứng', value: 'tmp_salary_advance' },
    { text: 'Phiếu chi lương', value: 'tmp_salary' },
    { text: 'Phiếu nhập kho', value: 'tmp_stock_picking_incoming' },
    { text: 'Phiếu xuất kho', value: 'tmp_stock_picking_outgoing' },
    { text: 'Phiếu kiểm kho', value: 'tmp_stock_inventory' },
    { text: 'Đơn thuốc', value: 'tmp_toathuoc' },
    { text: 'Hóa đơn thuốc', value: 'tmp_medicine_order' },
    { text: 'Phiếu labo', value: 'tmp_labo_order' },
  ];

  apiUrl = 'api/PrintTemplateConfigs';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  get(type) {
    return this.http.get(this.baseApi + this.apiUrl + '/' + type);
  }

  create(val) {
    return this.http.post(this.baseApi + this.apiUrl, val);
  }

  update(val) {
    return this.http.put(this.baseApi + this.apiUrl, val);
  }

  delete(type) {
    return this.http.delete(this.baseApi + this.apiUrl + '/' + type);
  }

}
