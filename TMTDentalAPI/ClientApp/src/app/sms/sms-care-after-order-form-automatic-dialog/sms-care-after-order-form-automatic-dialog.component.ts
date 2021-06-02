import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent, MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { EventEmitter } from 'events';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import { ProductPaged, ProductService } from 'src/app/products/product.service';
import { SmsAccountService, SmsAccountPaged } from '../sms-account.service';
import { SmsCampaignService } from '../sms-campaign.service';
import { SmsComfirmDialogComponent } from '../sms-comfirm-dialog/sms-comfirm-dialog.component';
import { SmsConfigService } from '../sms-config.service';
import { SmsMessageService } from '../sms-message.service';
import { SmsTemplateCrUpComponent } from '../sms-template-cr-up/sms-template-cr-up.component';
import { SmsTemplateService, SmsTemplateFilter } from '../sms-template.service';

@Component({
  selector: 'app-sms-care-after-order-form-automatic-dialog',
  templateUrl: './sms-care-after-order-form-automatic-dialog.component.html',
  styleUrls: ['./sms-care-after-order-form-automatic-dialog.component.css']
})
export class SmsCareAfterOrderFormAutomaticDialogComponent implements OnInit {

  @ViewChild("smsTemplateCbx", { static: true }) smsTemplateCbx: ComboBoxComponent
  @ViewChild("smsAccountCbx", { static: true }) smsAccountCbx: ComboBoxComponent
  formGroup: FormGroup;
  filteredConfigSMS: any[];
  filteredSmsAccount: any[];
  filteredProductCategories: any[] = [];
  filteredProducts: any[] = [];
  skip: number = 0;
  id: string;
  limit: number = 20;
  type: string;
  filteredTemplate: any[];
  filter: string = 'productCategory';
  textareaLimit: number = 200;
  isTemplateCopy = false;
  template: any = {
    text: '',
    templateType: 'text'
  };
  title: string;
  public today: Date = new Date;
  public timeReminder: Date = new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDay(), 0, 30, 0);
  public timeRunJob: Date = new Date();
  constructor(
    private fb: FormBuilder,
    private modalService: NgbModal,
    public activeModal: NgbActiveModal,
    private smsTemplateService: SmsTemplateService,
    private smsConfigService: SmsConfigService,
    private intlService: IntlService,
    private smsAccountService: SmsAccountService,
    private productCategoryService: ProductCategoryService,
    private productService: ProductService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      template: null,
      smsAccount: [null, Validators.required],
      isCareAfterOrderAutomation: false,
      dateTimeSend: new Date(),
      filter: this.filter,
      products: [],
      productCategories: [],
      typeTimeBeforSend: ['day', Validators.required],
      timeBeforSend: 1,
      name: ['', Validators.required],
      templateName: '',
      type: 'sale-order-line',
    })
    if (this.id) {
      this.loadDataFormApi();
    }
    this.loadSmsTemplate();
    this.loadAccount();
    this.smsTemplateCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.smsTemplateCbx.loading = true)),
      switchMap(value => this.searchSmsTemplate(value))
    ).subscribe((result: any) => {
      this.filteredTemplate = result;
      this.smsTemplateCbx.loading = false;
    });

    this.smsAccountCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.smsAccountCbx.loading = true)),
      switchMap(value => this.searchAccount(value))
    ).subscribe((result: any) => {
      this.filteredSmsAccount = result;
      this.smsAccountCbx.loading = false;
    });

    this.loadProductCategory();
    this.loadProduct();

  }

  loadDataFormApi() {
    this.smsConfigService.getDisplay(this.id).subscribe(
      (res: any) => {
        if (res) {
          if (res.products && res.products.length > 0) {
            this.filter = "product"
            res.filter = "product"
          }
          if (res.productCategories && res.productCategories.length > 0) {
            this.filter = "productCategory"
            res.filter = "productCategory"
          }
          this.formGroup.patchValue(res);
          if (res.body) {
            this.template = JSON.parse(res.body);
          }
          if (res.dateSend) {
            this.formGroup.get('dateTimeSend').patchValue(new Date(res.dateSend))
          }
        } else {
          this.id = null;
        }
      }
    )
  }

  checkedTemplateCopy(event) {
    var check = event.target.checked
    if (check) {
      this.isTemplateCopy = true;
    } else {
      this.isTemplateCopy = false;
    }

  }

  loadAccount() {
    this.searchAccount().subscribe(
      (result: any) => {
        if (result && result.items) {
          this.filteredSmsAccount = result.items
        }
      }
    )
  }

  searchAccount(q?: string) {
    var val = new SmsAccountPaged();
    val.limit = 20;
    val.offset = 0;
    val.search = q || '';
    return this.smsAccountService.getPaged(val);
  }

  loadSmsTemplate() {
    this.searchSmsTemplate().subscribe(
      (res: any) => {
        this.filteredTemplate = res;
      }
    )
  }

  onChangeTemplate(event) {
    if (event && event.body) {
      this.template = JSON.parse(event.body);
    } else {
      this.template = {
        text: '',
        templateType: 'text'
      }
    }
  }

  searchSmsTemplate(q?: string) {
    var filter = new SmsTemplateFilter();
    filter.search = q || "";
    filter.type = "saleOrderLine";
    return this.smsTemplateService.getAutoComplete(filter);
  }

  onSave() {
    if (this.formGroup.invalid) return;
    if (!this.template.text) return;
    var val = this.formGroup.value;
    val.smsAccountId = val.smsAccount ? val.smsAccount.id : null;
    val.dateSend = this.intlService.formatDate(val.dateTimeSend, "yyyy-MM-ddTHH:mm");
    val.timeBeforSend = Number.parseInt(val.timeBeforSend);
    val.templateId = val.template ? val.template.id : null;
    val.body = this.template ? JSON.stringify(this.template) : '';
    val.productIds = val.products ? val.products.map(x => x.id) : [];
    val.productCategoryIds = val.productCategories ? val.productCategories.map(x => x.id) : [];
    if (this.id) {
      this.smsConfigService.update(this.id, val).subscribe(
        res => {
          this.notify("cập nhật thiết lập thành công", true);
          this.activeModal.close();
        }
      )
    } else {
      this.smsConfigService.create(val).subscribe(
        res => {
          this.notify("thiết lập thành công", true);
          this.activeModal.close();
        }
      )
    }
    if (this.isTemplateCopy && val.templateName != '') {
      var valueTemplate = {
        name: val.templateName,
        body: val.body,
        type: "saleOrderLine"
      }
      this.smsTemplateService.create(valueTemplate).subscribe(
        () => {
          this.loadSmsTemplate();
        }
      )
    }
  }

  // addTemplate() {
  //   const modalRef = this.modalService.open(SmsTemplateCrUpComponent, { size: 'lg', windowClass: 'o_technical_modal' });
  //   modalRef.componentInstance.title = 'Tạo mẫu tin';
  //   modalRef.componentInstance.templateTypeTab = "saleOrderLine";
  //   modalRef.result.then((val) => {
  //     this.loadSmsTemplate();
  //   })
  // }
  notify(title, isSuccess = true) {
    this.notificationService.show({
      content: title,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: isSuccess ? 'success' : 'error', icon: true },
    });
  }

  onChangeRadioButton(event: any) {
    var filter = event.currentTarget.value;
    if (filter) {
      this.filter = filter;
      this.formGroup.get('products').patchValue(null);
      this.formGroup.get('productCategories').patchValue(null);
    }
  }


  loadProductCategory(q?: string) {
    var val = new ProductCategoryPaged();
    val.limit = 20;
    val.offset = 0;
    val.type = 'service';
    val.search = q || ''
    this.productCategoryService.autocomplete(val).subscribe(
      res => {
        if (res) {
          this.filteredProductCategories = res;
        }
      }
    );
  }

  loadProduct(q?: string) {
    var val = new ProductPaged();
    val.limit = 20;
    val.offset = 0;
    val.type = "service";
    val.type2 = "service"
    val.search = q || ''
    this.productService.autocomplete2(val).subscribe(
      res => {
        if (res) {
          this.filteredProducts = res;
        }
      }
    );
  }

}
