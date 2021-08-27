import { Component, OnInit, Pipe, PipeTransform, ViewChild } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { PrintTemplateConfigChangeType, PrintTemplateConfigDisplay, PrintTemplateConfigSave, PrintTemplateConfigService } from '../print-template-config.service';

@Component({
  selector: 'app-print-template-list',
  templateUrl: './print-template-config-list.component.html',
  styleUrls: ['./print-template-config-list.component.css']
})
export class PrintTemplateConfigListComponent implements OnInit {
  typeFilter: any;
  types: { text: string, value: string }[] = [];
  config = new PrintTemplateConfigDisplay();

  constructor(private configService: PrintTemplateConfigService,
    private router: Router
  ) {
  }

  ngOnInit() {
    this.types = this.configService.types;
    this.typeFilter = this.types[0].value;

    this.loadCurrentConfig();
  }

  loadCurrentConfig() {
    var val = new PrintTemplateConfigChangeType();
    val.type = this.typeFilter;
    this.configService.getDisplay(val).subscribe(res => {
      this.config = res;
      this.config.content = `
      <style type="text/css">.printBox {
              font-family: Arial, sans-serif;
              font-size: 11px;
          }
      
          table {
              page-break-inside: auto;
              border-collapse: collapse;
          }
      
              table td {
                  word-wrap: break-word;
                  word-break: break-all;
              }
      
          tr {
              page-break-inside: avoid;
              page-break-after: auto
          }
      
          img {
              max-width: 100%;
              height: auto;
          }
      </style>
      <div class="printBox">
      <table style="width:100%">
        <tbody>
          <tr>
            <td style="text-align:center">
            <table style="width:100%">
              <tbody>
                <tr>
                  <td style="font-size:11px; text-align:center">{Logo_Cua_Hang}</td>
                </tr>
                <tr>
                  <td style="font-size:11px; text-align:center"><strong style="font-size:11px">{Ten_Cua_Hang} mẫu 3</strong></td>
                </tr>
                <tr>
                  <td style="font-size:11px; text-align:center">Địa chỉ: {Dia_Chi_Chi_Nhanh} - {Phuong_Xa_Chi_Nhanh} - {Khu_Vuc_Chi_Nhanh_QH_TP}</td>
                </tr>
                <tr>
                  <td style="font-size:11px; text-align:center">Điện thoại: {Dien_Thoai_Chi_Nhanh}</td>
                </tr>
              </tbody>
            </table>
            </td>
          </tr>
        </tbody>
      </table>
      
      <div style="padding:10px 0 0; text-align:center"><strong style="font-size:12px">{Tieu_De_In HÓA ĐƠN BÁN HÀNG|HÓA ĐƠN TẠM TÍNH}</strong></div>
      
      <table style="width:100%">
        <tbody>
          <tr>
            <td style="font-size:11px; text-align:center"><span style="color:#1abc9c">Số HĐ: {Ma_Don_Hang}</span></td>
          </tr>
          <tr>
            <td style="font-size:11px; text-align:center"><span style="color:#1abc9c">Ngày {Ngay} tháng {Thang} năm {Nam}</span></td>
          </tr>
        </tbody>
      </table>
      
      <table style="margin:10px 0 15px; width:100%">
        <tbody>
          <tr>
            <td style="font-size:11px"><span style="color:#1abc9c">Khách hàng: {Khach_Hang}</span></td>
          </tr>
          <tr>
            <td style="font-size:11px">SĐT: {So_Dien_Thoai}</td>
          </tr>
          <tr>
            <td style="font-size:11px">Địa chỉ: {Dia_Chi_Khach_Hang} - {Phuong_Xa_Khach_Hang} - {Khu_Vuc_Khach_Hang_QH_TP}</td>
          </tr>
        </tbody>
      </table>
      
      <table cellpadding="3" style="width:98%">
        <tbody>
          <tr>
            <td style="border-bottom:1px solid black; border-top:1px solid black; width:35%"><strong><span style="font-size:11px">Đơn giá</span></strong></td>
            <td style="border-bottom:1px solid black; border-top:1px solid black; text-align:right; width:30%"><strong><span style="font-size:11px">SL</span></strong></td>
            <td style="border-bottom:1px solid black; border-top:1px solid black; text-align:right"><strong><span style="font-size:11px">Thành tiền</span></strong></td>
          </tr>
          <tr>
            <td colspan="3" style="padding-top:3px"><span style="font-size:12px">{Ten_Hang_Hoa}</span></td>
          </tr>
          <tr>
            <td style="border-bottom:1px dashed black"><span style="font-size:11px">{Don_Gia_Chiet_Khau}</span></td>
            <td style="border-bottom:1px dashed black; text-align:right"><span style="font-size:11px">{So_Luong}</span></td>
            <td style="border-bottom:1px dashed black; text-align:right"><span style="font-size:11px">{Thanh_Tien}</span></td>
          </tr>
        </tbody>
      </table>
      
      <table border="0" cellpadding="3" cellspacing="0" style="border-collapse:collapse; margin-top:20px; width:98%">
        <tfoot>
          <tr>
            <td style="font-size:11px; font-weight:bold; text-align:right; white-space:nowrap">Tổng tiền hàng:</td>
            <td style="font-size:11px; font-weight:bold; text-align:right">{Tong_Tien_Hang}</td>
          </tr>
          <tr>
            <td style="font-size:11px; font-weight:bold; text-align:right; white-space:nowrap">Chiết khấu {Chiet_Khau_Hoa_Don_Phan_Tram}:</td>
            <td style="font-size:11px; font-weight:bold; text-align:right">{Chiet_Khau_Hoa_Don}</td>
          </tr>
          <tr>
            <td style="font-size:11px; font-weight:bold; text-align:right; white-space:nowrap">Tổng thanh toán:</td>
            <td style="font-size:11px; font-weight:bold; text-align:right">{Tong_Cong}</td>
          </tr>
          <tr>
            <td colspan="2" style="font-size:11px; font-style:italic; text-align:left">({Tong_Cong_Bang_Chu})</td>
          </tr>
        </tfoot>
      </table>
      
      <table style="margin-top:20px; width:100%">
        <tbody>
          <tr>
            <td style="font-size:11px; font-style:italic; text-align:center">Cảm ơn và hẹn gặp lại!</td>
          </tr>
          <tr>
            <td style="font-size:9px; font-style:italic; text-align:center">Powered by KIOTVIET.</td>
          </tr>
        </tbody>
      </table>
      </div>
      <p><span><span contenteditable="false" style="border-top:1px dashed #ff0000; color:#ffffff; display:block; font-size:0px; height:0px; left:21px; line-height:0px; margin:0px; padding:0px; position:absolute; top:99px; user-select:none; width:494px; z-index:9999"><span style="border-color:transparent; border-right-color:#ff0000; border-style:solid; border-width:8px 8px 8px 0; color:#ffffff; display:block; font-size:0px; height:0px; line-height:0px; margin:0px; padding:0px; position:absolute; right:0px; top:-8px; width:0px; z-index:9999">&nbsp;</span><span style="border-color:transparent; border-left-color:#ff0000; border-style:solid; border-width:8px 0 8px 8px; color:#ffffff; display:block; font-size:0px; height:0px; left:0px; line-height:0px; margin:0px; padding:0px; position:absolute; top:-8px; width:0px; z-index:9999">&nbsp;</span><span contenteditable="false" style="background:url(https://cdn-app.kiotviet.vn/ckeditor/plugins/magicline/images/icon.png?t=I3I8) center no-repeat #ff0000; border-radius:2px; color:#ffffff; cursor:pointer; display:block; font-size:0px; height:17px; line-height:0px; margin:0px; padding:0px; position:absolute; right:17px; top:-8px; width:17px; z-index:9999" title="Insert paragraph here">↵</span></span></span></p>
      
      
      `;
    });

  }

  onChangeType(e) {
    const value = e.target.value;
    this.typeFilter = value;
  }

  onPrint() {

  }

  onEdit() {
    this.router.navigate(["/print-template-config/edit"], { queryParams: { type: this.typeFilter } });
  }
}
