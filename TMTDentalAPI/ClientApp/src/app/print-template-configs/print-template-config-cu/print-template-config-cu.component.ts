import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import '@ckeditor/ckeditor5-build-decoupled-document/build/translations/vi';
import * as DecoupledEditor from '@ckeditor/ckeditor5-build-decoupled-document';
import { PrintTemplateConfigSave, PrintTemplateConfigService } from '../print-template-config.service';

@Component({
  selector: 'app-print-template-config-cu',
  templateUrl: './print-template-config-cu.component.html',
  styleUrls: ['./print-template-config-cu.component.css']
})
export class PrintTemplateConfigCuComponent implements OnInit {
  public editor = DecoupledEditor;
  typeFilter: any;
  types: { text: string, value: string }[] = [];
  type: any;
  config = new PrintTemplateConfigSave();
  configEditor = {
    toolbar: {
      shouldNotGroupWhenFull: true
    },
    language: 'vi',
    defaultLanguage: 'vi'
  };
  constructor(private configService: PrintTemplateConfigService,
    private activeRoute: ActivatedRoute
  ) { }

  ngOnInit() {
    this.types = this.configService.types;
    this.typeFilter = this.types[2].value;

    this.activeRoute.paramMap.subscribe(x => {
      this.type = x.get("type");
      this.loadCurrentConfig();
    });
  }

  public onReady(editor) {
    editor.ui.getEditableElement().parentElement.insertBefore(
      editor.ui.view.toolbar.element,
      editor.ui.getEditableElement()
    );
  }

  loadCurrentConfig() {
    this.config.content = `
    <div style="width: 100%;float: left;font-family:Arial,sans-serif;font-size:13px;padding-bottom: 20px;border-bottom: 1px solid #7a7676;margin-bottom: 20px;display:flex">
    <div style="width: 30%;float: left;">{store_logo}</div>
    
    <div style="width: 35%;float: left;padding-left:10px">
    <div style="padding-bottom: 10px;font-weight: 600;">{store_name}</div>
    
    <div style="padding-bottom: 10px;font-weight: 600;">{store_address}</div>
    
    <div style="padding-bottom: 10px;font-weight: 600;">{store_phone_number}</div>
    
    <div style="padding-bottom: 10px;font-weight: 600;">{store_email}</div>
    </div>
    
    <div style="width: 35%;float: right;">
    <div style="text-align: right;padding-bottom: 10px;">Mã đơn hàng: <span style="font-weight: 600">{order_code}</span></div>
    
    <div style="text-align: right;padding-bottom: 10px;">Ngày tạo: <span style="font-weight: 600">{created_on}</span></div>
    </div>
    </div>
    
    <div style="width: 100%">
    <h1 style="font-family:Arial,sans-serif;font-size:22px;text-align: center;padding-top: 10px;">Đơn hàngHH</h1>
    </div>
    
    <table style="width:100%;margin: 0 0 20px;">
      <tbody style="font-family:Arial,sans-serif;font-size:13px;">
        <tr>
          <td style="width: 35%;">&nbsp;</td>
          <td style="width: 35%;">&nbsp;</td>
          <td>&nbsp;</td>
        </tr>
        <tr>
          <td style="padding-bottom: 10px;"><span style="font-weight: 600;">Hóa đơn đến:</span></td>
          <td style="padding-bottom: 10px;"><span style="font-weight: 600;">Giao hàng đến:</span></td>
          <td style="text-align: right;padding-bottom: 10px;">Điện thoại: <span style="font-weight: 600">{customer_phone_number}</span></td>
        </tr>
        <tr>
          <td style="padding-bottom: 10px;">{customer_name}</td>
          <td style="padding-bottom: 10px;">{customer_name}</td>
          <td style="text-align: right;padding-bottom: 10px;">Email: <span style="font-weight: 600;">{customer_email}</span></td>
        </tr>
        <tr>
          <td style="padding-right: 20px;line-height: 20px;">{billing_address}</td>
          <td style="padding-right: 20px;line-height: 20px;">{shipping_address}</td>
          <td>&nbsp;</td>
        </tr>
      </tbody>
    </table>
    
    <table cellpadding="0" cellspacing="0" style="width: 100%;border-left: 1px solid #7a7676;border-top: 1px solid #7a7676">
      <tbody>
        <tr style="font-family:Arial,sans-serif;font-size: 12px;font-weight: 600">
          <td style="padding: 1%; text-align: center;border-bottom:1px solid #7a7676;border-right:1px solid #7a7676; width: 10%;"><span>STT </span></td>
          <td style="padding: 1%; width: 10%; text-align: left; border-bottom:1px solid #7a7676;border-right:1px solid #7a7676;"><span>Mã sản phẩm </span></td>
          <td style="padding: 1%; width: 20%; text-align: left; border-bottom:1px solid #7a7676;border-right:1px solid #7a7676;"><span>Tên sản phẩm </span></td>
          <td style="padding: 1%; width: 10%; border-bottom: 1px solid rgb(122, 118, 118); border-right: 1px solid rgb(122, 118, 118);"><span>Đơn vị</span></td>
          <td style="padding: 1%; width: 10%; text-align: right; border-bottom:1px solid #7a7676;border-right:1px solid #7a7676;"><span>Số lượng </span></td>
          <td style="padding: 1%; width: 15%; text-align: right; border-bottom:1px solid #7a7676;border-right:1px solid #7a7676;"><span>Đơn giá </span></td>
          <td style="padding: 1%; width: 10%; text-align: right; border-bottom:1px solid #7a7676;border-right:1px solid #7a7676;"><span>Chiết khấu </span></td>
          <td style="padding: 1%; width: 15%; text-align: right; border-bottom:1px solid #7a7676;border-right:1px solid #7a7676;"><span>Thành tiền </span></td>
        </tr>
        <!--<#assign lines = model.orderLineItems>--><!--<#list lines as line>-->
        <tr style="font-family:Arial,sans-serif;font-size: 12px">
          <td style="padding: 1%; text-align: center;border-bottom:1px solid #7a7676;border-right:1px solid #7a7676; width: 10%;"><span>{line_stt}</span></td>
          <td style="padding: 1%; width: 10%; text-align: left; border-bottom:1px solid #7a7676;border-right:1px solid #7a7676;"><span>{line_variant_code}</span></td>
          <td style="padding: 1%; width: 20%; text-align: left; border-bottom:1px solid #7a7676;border-right:1px solid #7a7676;"><span>{line_variant}</span></td>
          <td style="padding: 1%; width: 10%; border-bottom: 1px solid rgb(122, 118, 118); border-right: 1px solid rgb(122, 118, 118);"><span>{line_unit}</span></td>
          <td style="padding: 1%; width: 10%; text-align: right; border-bottom:1px solid #7a7676;border-right:1px solid #7a7676;"><span>{line_quantity}</span></td>
          <td style="padding: 1%; width: 15%; text-align: right; border-bottom:1px solid #7a7676;border-right:1px solid #7a7676;"><span>{line_price}</span></td>
          <td style="padding: 1%; width: 10%; text-align: right; border-bottom:1px solid #7a7676;border-right:1px solid #7a7676;"><span>{line_discount_rate}%</span></td>
          <td style="padding: 1%; width: 15%; text-align: right; border-bottom:1px solid #7a7676;border-right:1px solid #7a7676;"><span>{line_amount}</span></td>
        </tr>
        <!--</#list>-->
      </tbody>
    </table>
    
    <table style="width:100%">
      <tbody>
        <tr>
          <td style="width: 50%;">
          <table border="1" cellpadding="1" cellspacing="1" style="width:500px;">
            <tbody>
              <tr>
                <td>fdsaf</td>
                <td>fdas</td>
              </tr>
              <tr>
                <td><span>{line_stt}</span></td>
                <td><span>{line_variant_code}</span></td>
              </tr>
              <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
              </tr>
            </tbody>
          </table>
    
          <p>&nbsp;</p>
          </td>
          <td style="width: 50%;">&nbsp;</td>
        </tr>
        <tr style="font-family:Arial,sans-serif;font-size: 13px;">
          <td style="width: 50%;padding:1%">&nbsp;</td>
          <td style="width: 50%;border-bottom: 1px solid #7a7676;padding: 10px;">Tổng số lượng<span style="float: right;">{total_quantity}</span></td>
        </tr>
        <tr style="font-family:Arial,sans-serif;font-size: 13px;">
          <td style="width: 50%;padding: 1%">&nbsp;</td>
          <td style="width: 50%;border-bottom: 1px solid #7a7676;padding: 10px;">Tổng Tiền<span style="float: right;">{total}</span></td>
        </tr>
        <tr style="font-family:Arial,sans-serif;font-size: 13px;">
          <td style="width: 50%;padding:1%">&nbsp;</td>
          <td style="width: 50%;border-bottom: 1px solid #7a7676;padding: 10px;">VAT<span style="float: right;">{total_tax}</span></td>
        </tr>
        <tr style="font-family:Arial,sans-serif;font-size: 13px;">
          <td style="width: 50%;padding:1%">&nbsp;</td>
          <td style="width: 50%;border-bottom: 1px solid #7a7676;padding: 10px;">Chiết khấu<span style="float: right;">{order_discount}</span></td>
        </tr>
        <tr style="font-family:Arial,sans-serif;font-size: 13px;border-bottom: 1px solid #7a7676">
          <td style="width: 50%;padding:1%;border-bottom: 0;">&nbsp;</td>
          <td style="width: 50%;border-bottom: 1px solid #222222;padding: 10px;">Phí giao hàng<span style="float: right;">{delivery_fee}</span></td>
        </tr>
        <tr style="font-family:Arial,sans-serif;font-size: 13px;">
          <td style="width: 50%;padding:1%">&nbsp;</td>
          <td style="width: 50%;font-weight: 600;padding: 10px;">Khách phải trả<span style="float: right;">{total_amount}</span></td>
        </tr>
      </tbody>
    </table>
    
    <footer style="page-break-after: always">.</footer>
    
    `;
  }


  onDefault() {

  }

  onPrint() {

  }

  onSave() {

  }
}
