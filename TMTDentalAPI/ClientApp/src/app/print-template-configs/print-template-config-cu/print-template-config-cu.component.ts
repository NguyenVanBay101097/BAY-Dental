import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PrintTemplateConfigChangeType, PrintTemplateConfigDisplay, PrintTemplateConfigSave, PrintTemplateConfigService } from '../print-template-config.service';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { AuthService } from 'src/app/auth/auth.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { PrintTemplateDefault, PrintTemplateService } from '../print-template.service';

@Component({
    selector: 'app-print-template-config-cu',
    templateUrl: './print-template-config-cu.component.html',
    styleUrls: ['./print-template-config-cu.component.css']
})
export class PrintTemplateConfigCuComponent implements OnInit {
    typeFilter: any;
    isEditting = false;
    types: { text: string, value: string }[] = [];
    config = new PrintTemplateConfigDisplay();
    configEdit = new PrintTemplateConfigDisplay();
    configEditor = {
        language: 'vi',
        height: 525,
        contentsCss: '/css/print.css',
        fullPage: true
    };
    constructor(private configService: PrintTemplateConfigService,
        private activeRoute: ActivatedRoute,
        private notifyService: NotifyService,
        private authService: AuthService,
        private printService: PrintService,
        private printTemplateService: PrintTemplateService
    ) { }

    ngOnInit() {
        this.types = this.configService.types;
        this.typeFilter = this.types[0].value;
        this.loadCurrentConfig();
        // this.activeRoute.paramMap.subscribe(x => {
        //   this.typeFilter = x.get("type");
        //   this.loadCurrentConfig();
        // });
    }

    public onReady(editor) {
        editor.ui.getEditableElement().parentElement.insertBefore(
            editor.ui.view.toolbar.element,
            editor.ui.getEditableElement()
        );
    }

    loadCurrentConfig(preType?) {
        var val = new PrintTemplateConfigChangeType();
        val.type = this.typeFilter;
        this.configService.getDisplay(val).subscribe(res => {
            this.config = res;
            // if (!res.content)
            this.config.content = `



      <!DOCTYPE html>
      <html>
      <head>
          <meta charset="utf-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1.0">
          <title>TDental.vn</title>
      
          <!-- Load paper.css for happy printing -->
      
          <link rel="stylesheet" type="text/css" href="/css/print.css" />
          <style>
              @page {
                  size: A4 !important;
                  margin-top: 10mm ;
                  margin-right: 10mm;
                  margin-bottom: 10mm;
                  margin-left:10mm ;
              }
      
          </style>
      </head>
      <body class="">
          <div class="container">
              
      <div class="">
              <div class="d-flex mb-2">
                      <img class="img-fluid me-3" style="width: 150px;" src="http://statics.tdental.vn/images/localhost/210728/1007416043_Capture.PNG" alt="chi nh&#xE1;nh 1">
                  <div>
                      <div class="text-xl font-semibold">chi nh&#xE1;nh 1</div>
                          <div>Địa chỉ: X&#xE3; An Th&#xE1;i, Huy&#x1EC7;n An L&#xE3;o, Th&#xE0;nh ph&#x1ED1; H&#x1EA3;i Ph&#xF2;ng</div>
                          <div>ĐT: 0357142225</div>
                          <div>Email: dfsjaf@gmail.com</div>
                  </div>
              </div>
      
          <div class="text-center">
              <div class="text-3xl font-semibold">PHIẾU ĐIỀU TRỊ</div>
              <div>Ngày 27 tháng 8 năm 2021 </div>
              <div class="font-semibold">Số: SO00193</div>
          </div>
          <div class="o_form_view">
              <div class="o_group mt-0 mb-0">
                  <table class="o_group o_inner_group">
                      <tbody>
                          <tr>
                              <td colspan="2">
                                  <div class="o_horizontal_separator">
                                      THÔNG TIN CHI TIẾT
                                  </div>
                              </td>
                          </tr>
                          <tr>
                              <td colspan="1" class="o_td_label">
                                  <div class="o_form_label o_form_label_help">
                                      Khách hàng
                                  </div>
                              </td>
                              <td colspan="1" style="width: 100%;">
                                  <div class="o_field_widget">kh&#xE1;ch h&#xE0;ng m&#x1EDB;i 3</div>
                              </td>
                          </tr>
      
                      </tbody>
                  </table>
              </div>
          </div>
      
          <div>
              <div class="text-lg font-semibold mb-1">BẢNG KÊ DỊCH VỤ</div>
              <table class="table table-sm table-bordered mb-1">
                  <thead>
                      <tr>
                          <th>Dịch vụ</th>
                          <th class="text-right">Số lượng</th>
                          <th class="text-right">Đơn giá</th>
                          <th class="text-right">Giảm giá</th>
                          <th class="text-right">Thành tiền</th>
                      </tr>
                  </thead>
                  <tbody>
                          <tr *ngFor="let line of saleOrderPrint.orderLines; let i=index">
                              <td>L&#x1EA5;y cao r&#x103;ng v&#xE0; &#x111;&#xE1;nh b&#xF3;ng</td>
                              <td class="text-right">1</td>
                              <td class="text-right">100.000</td>
                              <td class="text-right">0</td>
                              <td class="text-right">100.000</td>
                          </tr>
      
                  </tbody>
              </table>
              <div class="o_form_view float-right">
                  <table class="o_group o_inner_group">
                      <tbody>
                          <tr>
                              <td colspan="1" class="o_td_label pr-3">
                                  <div class="o_form_label">
                                      Tổng tiền
                                  </div>
                              </td>
                              <td colspan="1" class="text-right pe-0">
                                  <div class="o_form_field o_form_field_number">100.000</div>
                              </td>
                          </tr>
                          <tr>
                              <td colspan="1" class="o_td_label pr-3">
                                  <div class="o_form_label">
                                      Đã thanh toán
                                  </div>
                              </td>
                              <td colspan="1" class="text-right pe-0">
                                  <div class="o_form_field o_form_field_number">0</div>
                              </td>
                          </tr>
                          <tr>
                              <td colspan="1" class="o_td_label pr-3">
                                  <div class="o_form_label">
                                      Còn lại
                                  </div>
                              </td>
                              <td colspan="1" class="text-right pe-0">
                                  <div class="o_form_field o_form_field_number">100.000</div>
                              </td>
                          </tr>
                      </tbody>
                  </table>
              </div>
              <div class="clearfix"></div>
          </div>
      
          <div class="d-flex mt-2">
              <div class="col">
                  <div class="text-center">
                      <span class="font-semibold">Người lập phiếu</span> <br />
                      <i>(Ký, họ tên)</i> <br />
                      <span></span> <br />
                      <span></span> <br />
                      <span></span> <br />
                      <span class="font-semibold"></span>
                  </div>
              </div>
              <div class="col">
                  <div class="text-center">
                      <span class="font-semibold"> Khách hàng</span> <br />
                      <i>(Ký, họ tên)</i> <br />
                      <span></span> <br />
                      <span></span> <br />
                      <span></span> <br />
                      <span class="font-semibold">KH&#xC1;CH H&#xC0;NG M&#x1EDA;I 3</span>
                  </div>
              </div>
          </div>
      </div>
          </div>
      </body>
      </html>
      
      `;
        },
            err => {
                if (preType) this.typeFilter = preType;
                console.log(this.typeFilter);

            }
        );

    }


    onDefault() {
        var val = Object.assign({}, this.configEdit) as PrintTemplateDefault;
        val.type = this.typeFilter;
        this.printTemplateService.getDisplay(val).subscribe(res => {
            this.configEdit.content = res;
        });
    }

    onPrint() {
        this.printService.printHtml(this.isEditting ? this.configEdit.content : this.config.content);
    }

    onSave() {
        var val = Object.assign({}, this.configEdit) as PrintTemplateConfigSave;
        val.companyId = this.authService.userInfo ? this.authService.userInfo.companyId : '';
        val.type = this.typeFilter;
        this.configService.createOrUpdate(val).subscribe(res => {
            this.notifyService.notify('success', 'Lưu thành công');
            this.onToggleEdit();
            this.config = this.configEdit;
        });
    }

    onChangeType(e) {
        var prev = this.typeFilter;
        this.typeFilter = e;
        this.loadCurrentConfig(prev);
    }

    onEdit() {
        this.configEdit = Object.assign({}, this.config);
        this.onToggleEdit();
    }

    onToggleEdit() {
        this.isEditting = !this.isEditting;
    }

}
