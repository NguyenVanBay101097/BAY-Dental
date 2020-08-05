import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { switchMap, map, retry, debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { FormBuilder, FormGroup, FormArray } from '@angular/forms';
import { PartnerService } from '../partner.service';
import { AccountInvoiceDisplay, AccountInvoicePaged, PaymentInfoContent, AccountInvoicePrint } from 'src/app/account-invoices/account-invoice.service';
import { PartnerDisplay } from '../partner-simple';
import { GridDataResult, PageChangeEvent, SelectableSettings } from '@progress/kendo-angular-grid';
import { AccountInvoiceLineDisplay } from 'src/app/account-invoices/account-invoice-line-display';
import { Subject } from 'rxjs';
import { WindowRef, WindowCloseResult, DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog/';
import { WindowService } from '@progress/kendo-angular-dialog';
import { PartnerCustomerInfoComponent } from '../partner-customer-info/partner-customer-info.component';
import { PartnerCreateUpdateComponent } from '../partner-create-update/partner-create-update.component';
import { CustomerInvoiceCreateUpdateComponent } from 'src/app/account-invoices/customer-invoice-create-update/customer-invoice-create-update.component';
import { UserSimple } from 'src/app/users/user-simple';
import { UserPaged, UserService } from 'src/app/users/user.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import * as _ from 'lodash';
import { IntlService } from '@progress/kendo-angular-intl';
import { AccountInvoiceRegisterPaymentDialogComponent } from 'src/app/account-invoices/account-invoice-register-payment-dialog/account-invoice-register-payment-dialog.component';
import { NotificationService } from '@progress/kendo-angular-notification';
import { AccountInvoiceLineDialogComponent } from 'src/app/account-invoices/account-invoice-line-dialog/account-invoice-line-dialog.component';
import { DotKhamDisplay } from 'src/app/dot-khams/dot-khams';
import { FileRestrictions, RemoveEvent, SelectEvent, SuccessEvent, UploadEvent } from '@progress/kendo-angular-upload';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-partner-history',
  templateUrl: './partner-history.component.html',
  styleUrls: ['./partner-history.component.css']
})
export class PartnerHistoryComponent implements OnInit {

  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;

  id: string; // ID của khách hàng
  invSelectedId: string;//Hóa đơn đang được chọn

  customer: PartnerDisplay = new PartnerDisplay;
  loading = false;
  gridLoading = false;
  gridView: GridDataResult;

  limit = 10;
  skip = 0;
  type = 'out_invoice';
  windowOpened = false;

  filteredUsers: UserSimple[];
  formHistory: FormGroup;
  accInvDisplay: AccountInvoiceDisplay = new AccountInvoiceDisplay;
  dotKhams: DotKhamDisplay[] = [];
  invoicePrint: AccountInvoicePrint;

  addressAr: string[] = [];
  address: string = '';
  btnDropdown: any[] = [{ text: 'Sửa', icon: 'edit' }, { text: 'Xóa', icon: 'delete' }, { text: 'In phiếu', icon: 'print' }, { text: 'Mở trong tab mới', icon: 'hyperlink-open' }];

  search: string;
  searchUpdate = new Subject<string>();

  payments: PaymentInfoContent[] = [];

  constructor(private activeRoute: ActivatedRoute, private fb: FormBuilder, private service: PartnerService,
    private windowService: WindowService, private router: Router, private userService: UserService, private intlService: IntlService,
    private notificationService: NotificationService, private dialogService: DialogService, private sanitizer: DomSanitizer) { }

  ngOnInit() {
    this.formHistory = this.fb.group({
      invoiceLines: this.fb.array([]),
      discountType: 'fixed',
      discountFixed: 0,
      discountPercent: 0,
      state: null,
      residual: 0,
      number: null,
      user: null,
      partner: null,
      comment: null,
      dateOrderObj: null,
      amountTotal: null,
      companyId: null,
      dateOrder: null,
      userId: null,
      partnerId: null,
      accountId: null,
      journalId: null,
      type: null
    })

    this.activeRoute.paramMap.pipe(
      switchMap((params: ParamMap) => {
        this.id = params.get("id");
        if (this.id) {
          var val = new AccountInvoicePaged();
          val.partnerId = this.id;
          return this.service.getCustomerInvoices(val);
        }
      })).pipe(
        map(rs1 => (<GridDataResult>{
          data: rs1.items,
          total: rs1.totalItems
        }))
      ).subscribe(rs2 => {
        this.gridView = rs2;
        if (this.gridView.data.length > 0) {
          this.getInvoiceDetail(rs2.data[0].id);
        }
        this.loading = false;
      }
      );


    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadInvoices()
      });

    this.getCustomerInfo();
    this.loadUsers();

  }


  loadInvoices() {
    this.gridLoading = true;
    var val = new AccountInvoicePaged();
    val.limit = this.limit;
    val.offset = this.skip
    val.searchNumber = this.search || '';
    val.partnerId = this.id;
    this.service.getCustomerInvoices(val).pipe(
      map(rs1 => (<GridDataResult>{
        data: rs1.items,
        total: rs1.totalItems
      }))
    ).subscribe(
      rs2 => {
        this.gridView = rs2;
        this.gridLoading = false;
      }
    )
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadInvoices();
  }

  getInvoiceDetail(id) {
    this.invSelectedId = id;
    this.service.getInvoiceDetail(id).subscribe(
      rs => {
        (this.formHistory.get('invoiceLines') as FormArray).clear();
        this.accInvDisplay = rs;
        this.formHistory.patchValue(rs);
        console.log(rs);
        let dateOrder = this.intlService.parseDate(rs.dateOrder);
        this.formHistory.get('dateOrderObj').patchValue(dateOrder);

        const control = this.formHistory.get('invoiceLines') as FormArray;
        rs.invoiceLines.forEach(line => {
          var g = this.fb.group(line);
          g.setControl('teeth', this.fb.array(line.teeth));
          control.push(g);
        });
        console.log(this.invoiceLines);
        this.loadPaymentInfo();
      }
    )

    this.service.getDotKhamList(id).subscribe(
      result => {
        this.dotKhams = result;
      });
  }

  lineProduct(line: FormGroup) {
    var product = line.get('product').value;
    return product ? product.name : '';
  }

  lineTeeth(line: FormGroup) {
    var teeth = line.get('teeth').value as any[];
    return teeth.map(x => x.name).join(',');
  }

  // getTeeth(invLine: AccountInvoiceLineDisplay) {
  //   var teethList = invLine.teeth;
  //   return teethList.map(x => x.name).join(',');
  // }

  getCustomerInfo() {
    this.service.getPartner(this.id).subscribe(
      rs => {
        this.customer = rs;
        console.log(this.customer.avatar);
        this.getAddress();
      }
    );
  }

  getAddress() {
    this.addressAr = [];
    if (this.customer.street && this.customer.street.trim()) {
      this.addressAr.push(this.customer.street);
    }
    if (this.customer.ward && this.customer.ward.name) {
      this.addressAr.push(this.customer.ward.name);
    }
    if (this.customer.district && this.customer.district.name) {
      this.addressAr.push(this.customer.district.name);
    }
    if (this.customer.city && this.customer.city.name) {
      this.addressAr.push(this.customer.city.name);
    }

    this.address = this.addressAr.join(', ');
  }

  openDotKham(dotKhamid, invoiceId) {
  }

  rowSelectionChange(e) {
    console.log(e.selectedRows[0].dataItem);
    this.getInvoiceDetail(e.selectedRows[0].dataItem.id);
  }

  //Thêm hóa đơn
  createNewInvoice() {
    this.invSelectedId = null;
    this.dotKhams = [];
    this.formHistory.reset();
    this.formHistory.get('discountType').setValue('fixed');
    (this.formHistory.get('invoiceLines') as FormArray).clear();
    var defaultVal = new AccountInvoiceDisplay();
    defaultVal.type = "out_invoice";
    this.service.invoiceDefaultGet(defaultVal).subscribe(result => {
      this.formHistory.patchValue(result);
      let dateOrder = this.intlService.parseDate(result.dateOrder);
      this.formHistory.get('dateOrderObj').setValue(dateOrder)

      if (result.user) {
        this.filteredUsers = _.unionBy(this.filteredUsers, [result.user], 'id');
      }
    });
  }

  btnDropdownItemClick(e, id) {
    console.log(e);
    switch (e.text) {
      case "Sửa":
        this.getInvoiceDetail(id);
        break;
      case "Xóa":
        this.deleteItem(id);
        break;
      case "In phiếu":
        this.printInvoice();
        break;
      case "Mở trong tab mới":
        this.router.navigate([]).then(res => { window.open('/customer-invoices/edit/' + id, '_blank'); });
        break;

      default:
        break;
    }
  }

  onChangeDiscountType(event) {
    this.computeAmountTotal();
  }

  loadPaymentInfo() {
    if (this.invSelectedId) {
      this.service.getPaymentInfoJson(this.invSelectedId).subscribe(result => {
        this.payments = result;
      });
    }
  }

  computeAmountTotal() {
    let total = 0;
    this.invoiceLines.controls.forEach(line => {
      total += line.get('priceUnit').value * line.get('quantity').value;
    });
    if (this.discountType === 'percentage') {
      total = total * (1 - this.discountPercent / 100);
    } else {
      total = total - Math.min(total, this.discountFixed);
    }
    this.formHistory.get('amountTotal').patchValue(total);
  }

  loadUsers() {
    this.searchUsers().subscribe(result => {
      this.filteredUsers = _.unionBy(this.filteredUsers, result, 'id');
      console.log(result);
    });
  }

  searchUsers(filter?: string) {
    var val = new UserPaged();
    val.search = filter;
    return this.userService.autocompleteSimple(val);
  }

  showAddLineModal() {
    const windowRef = this.windowService.open({
      title: 'Thêm dịch vụ điều trị',
      content: AccountInvoiceLineDialogComponent,
      resizable: false,
      autoFocusedElement: '[name="name"]',
    });

    const instance = windowRef.content.instance;
    instance.invoiceType = this.type;

    this.windowOpened = true;

    windowRef.result.subscribe((result) => {
      this.windowOpened = false;
      if (!(result instanceof WindowCloseResult)) {
        let line = result as any;
        line.teeth = this.fb.array(line.teeth);
        this.invoiceLines.push(this.fb.group(line));

        this.computeAmountTotal();
      }
    });
  }

  editProfileWindow() {
    const windowRef: WindowRef = this.windowService.open(
      {
        title: 'Thông tin khách hàng',
        content: PartnerCreateUpdateComponent,
        minWidth: 250,
        // width: 920
      });
    this.windowOpened = true;
    const instance = windowRef.content.instance;
    instance.cusId = this.id;
    instance.queryCustomer = true;
    instance.querySupplier = true;

    windowRef.result.subscribe(
      (result) => {
        this.windowOpened = false;
        console.log(result);
        if (!(result instanceof WindowCloseResult)) {
          this.getCustomerInfo();
        }
      }
    )
  }

  actionInvoicePayment() {
    const windowRef = this.windowService.open({
      title: 'Thanh toán',
      content: AccountInvoiceRegisterPaymentDialogComponent,
      resizable: false,
      autoFocusedElement: '[name="name"]',
    });

    const instance = windowRef.content.instance;
    instance.invoiceId = this.invSelectedId;

    this.windowOpened = true;

    windowRef.result.subscribe((result) => {
      this.windowOpened = false;
      if (!(result instanceof WindowCloseResult)) {
        this.getInvoiceDetail(this.invSelectedId);
        this.loadInvoices();
      }
    });
  }

  //Cập nhật - Hủy hóa đơn
  actionCancel() {
    if (this.invSelectedId) {
      this.loading = true;
      this.service.actionCancel([this.invSelectedId]).subscribe(() => {
        this.loading = false;
        this.getInvoiceDetail(this.invSelectedId);
        this.loadInvoices();
      }, () => {
        this.loading = false;
      });
    }
  }

  //Cập nhật - Xác nhận hóa đơn
  actionCancelDraft() {
    if (this.invSelectedId) {
      this.loading = true;
      this.service.actionCancelDraft([this.invSelectedId]).subscribe(() => {
        this.loading = false;
        this.getInvoiceDetail(this.invSelectedId);
        this.loadInvoices();
      }, () => {
        this.loading = false;
      });
    }
  }

  //Lưu
  onSave() {
    if (!this.formHistory.valid) {
      return;
    }

    var val = this.formHistory.value;
    val.partnerId = this.customer.id;
    val.userId = val.user ? val.user.id : null;
    val.dateInvoice = this.intlService.formatDate(val.dateInvoiceD, 'g', 'en-US');

    this.service.createInvoice(val).subscribe(result => {
      this.getInvoiceDetail(result.id);
      this.loadInvoices();
    });
  }

  //Xác nhận + thêm mới
  onSaveConfirm() {
    if (!this.formHistory.valid) {
      return;
    }

    var val = this.formHistory.value;
    val.partnerId = this.customer.id;
    val.userId = val.user ? val.user.id : null;
    val.dateInvoice = this.intlService.formatDate(val.dateInvoiceD, 'g', 'en-US');
    this.loading = true;
    this.service.createInvoice(val).subscribe(result => {
      this.service.invoiceOpen([result.id]).subscribe(() => {
        this.loading = false;
        this.getInvoiceDetail(result.id);
        this.loadInvoices();
      }, () => {
        this.loading = false;
      });
    }, () => {
      this.loading = false;
    });
  }

  //Lưu thay đổi
  onUpdate() {
    if (!this.formHistory.valid) {
      return;
    }

    if (this.invSelectedId) {
      var val = this.formHistory.value;
      val.partnerId = val.partner.id;
      val.userId = val.user ? val.user.id : null;
      val.dateInvoice = this.intlService.formatDate(val.dateInvoiceD, 'g', 'en-US');

      this.service.updateInvoice(this.invSelectedId, val).subscribe(result => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.getInvoiceDetail(this.invSelectedId);
        this.loadInvoices();
      });
    }
  }

  //Chỉ xác nhận, ko update sau khi thêm mới
  onConfirm() {
    if (this.invSelectedId) {
      this.loading = true;
      var val = this.formHistory.value;
      val.partnerId = val.partner.id;
      val.userId = val.user ? val.user.id : null;
      val.dateInvoice = this.intlService.formatDate(val.dateInvoiceD, 'g', 'en-US');

      this.service.updateInvoice(this.invSelectedId, val).subscribe(result => {
        this.service.invoiceOpen([this.invSelectedId]).subscribe(() => {
          this.loading = false;
          this.getInvoiceDetail(this.invSelectedId);
          this.loadInvoices();
        }, () => {
          this.loading = false;
        });
      }, () => {
        this.loading = false;
      })
    }
  }

  //Cập nhật dịch vụ
  editLine(line: FormGroup) {
    const windowRef = this.windowService.open({
      title: 'Sửa dịch vụ điều trị',
      content: AccountInvoiceLineDialogComponent,
      resizable: false,
      autoFocusedElement: '[name="name"]',
    });

    const instance = windowRef.content.instance;
    instance.invoiceType = this.type;
    instance.line = line.value;

    this.windowOpened = true;

    windowRef.result.subscribe((result) => {
      this.windowOpened = false;
      if (result instanceof WindowCloseResult) {
      } else {
        var a = result as any;
        line.patchValue(result);
        line.setControl('teeth', this.fb.array(a.teeth || []));

        this.computeAmountTotal();
      }
    });
  }

  deleteLine(index: number) {
    this.invoiceLines.removeAt(index);

    this.computeAmountTotal();
  }



  //In phiếu
  printInvoice() {
    if (this.invSelectedId) {
      this.service.printInvoice(this.invSelectedId).subscribe(result => {
        this.invoicePrint = result;
        setTimeout(() => {
          var printContents = document.getElementById('printInvoiceDiv').innerHTML;
          var popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
          popupWin.document.open();
          popupWin.document.write(`
              <html>
                <head>
                  <title>Print tab</title>
                  <link rel="stylesheet" type="text/css" href="/assets/css/bootstrap.min.css" />
                  <link rel="stylesheet" type="text/css" href="/assets/css/print.css" />
                </head>
            <body onload="window.print();window.close()">${printContents}</body>
              </html>`
          );
          popupWin.document.close();
          this.invoicePrint = null;
        }, 500);
      });
    }
  }

  //Xóa hóa đơn
  deleteItem(id) {
    const dialog: DialogRef = this.dialogService.open({
      title: 'Xóa hóa đơn',
      content: 'Bạn có chắc chắn muốn xóa?',
      actions: [
        { text: 'Hủy bỏ', value: false },
        { text: 'Đồng ý', primary: true, value: true }
      ],
      width: 450,
      height: 200,
      minWidth: 250
    });

    dialog.result.subscribe((result) => {
      if (result instanceof DialogCloseResult) {
      } else {
        console.log('action', result);
        if (result['value']) {
          this.service.deleteInvoice(id).subscribe(() => {
            this.loadInvoices();
            var firstId = this.invSelectedId == this.gridView.data[0].id ? this.gridView.data[1].id : this.invSelectedId;
            this.getInvoiceDetail(firstId);
          }, err => {
            console.log(err);
          });
        }
      }
    });
  }

  //Phần UPLOAD===============================================================================================
  //************* UPLOAD INPUT
  filesToUpload: File[] = [];
  imagePreviews2: any[] = [];
  imagePreviewsName: any[] = [];

  uploadImage() {
    this.service.uploadImage(this.id, this.filesToUpload[0]).subscribe(
      rs => {
        console.log(rs);
        this.filesToUpload = [];
        this.imagePreviews2 = [];
      }, er => {
        console.log(er);
      }
    );
  }
  // imageUrl: string = "/assets/img/default-image.png";
  handleFileInput(fileList: FileList) {
    this.filesToUpload = [];
    this.imagePreviews2 = [];
    console.log(fileList);
    for (var i = 0; i < fileList.length; i++) {
      this.filesToUpload[i] = fileList.item(i);

      var reader = new FileReader();
      var file = fileList[i];
      reader.onload = (event: any) => {
        var image = {
          id: i,
          src: event.target['result']
        };

        this.imagePreviews2.unshift(image);
      }
      reader.readAsDataURL(file);
    }
  }

  // ************* KENDO UPLOAD 
  // events: string[] = [];
  // imagePreviews: any[] = [];
  // uploadRestrictions: FileRestrictions = {
  //   allowedExtensions: ['.jpg', '.png']
  // };

  // uploadSaveUrl = 'http://tenant3.myproject.com:50396/api/Partners/UploadImage'; // should represent an actual API endpoint
  // uploadRemoveUrl = 'removeUrl'; // should represent an actual API endpoint

  // clearEventHandler(): void {
  //   this.log('Clearing the file upload');
  //   this.imagePreviews = [];
  // }

  // completeEventHandler() {
  //   this.log(`All files processed`);
  // }

  // removeEventHandler(e: RemoveEvent): void {
  //   this.log(`Removing ${e.files[0].name}`);

  //   const index = this.imagePreviews.findIndex(item => item.uid === e.files[0].uid);

  //   if (index >= 0) {
  //     this.imagePreviews.splice(index, 1);
  //   }
  // }

  // selectEventHandler(e: SelectEvent): void {
  //   const that = this;
  //   e.files.forEach((file) => {
  //     that.log(`File selected: ${file.name}`);
  //     if (!file.validationErrors) {
  //       const reader = new FileReader();
  //       reader.onload = function (ev) {
  //         const image = {
  //           src: ev.target['result'],
  //           uid: file.uid
  //         };
  //         that.imagePreviews.unshift(image);
  //       };
  //       reader.readAsDataURL(file.rawFile);
  //     }
  //   });
  // }

  // log(event: string): void {
  //   this.events.unshift(`${event}`);
  // }

  // successEventHandler(e: SuccessEvent) {
  //   console.log("Success");
  //   console.log(e.response);
  // }

  // errorEventHandler(e: ErrorEvent) {
  //   console.log('An error occurred');
  //   console.log(e);
  // }
  //================================KENDO UPLOAD

  get discountFixed() {
    return this.formHistory.get('discountFixed').value || 0;
  }

  get discountPercent() {
    return this.formHistory.get('discountPercent').value || 0;
  }

  get discountType() {
    return this.formHistory.get('discountType').value;
  }

  get invoiceLines() {
    return this.formHistory.get('invoiceLines') as FormArray;
  }

  get getInvNumber() {
    return this.formHistory.get('number').value;
  }

  get getInvState() {
    return this.formHistory.get('state').value;
  }

  get getInvResidual() {
    return this.formHistory.get('residual').value;
  }

  get getInvAmountTotal() {
    return this.formHistory.get('amountTotal').value;
  }

  get getDateOrderObj() {
    return this.formHistory.get('dateOrderObj').value;
  }

  get getUser() {
    return this.formHistory.get('user').value;
  }

  get getComment() {
    return this.formHistory.get('comment').value;
  }

  filterChange() {
    this.userCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.userCbx.loading = true)),
      switchMap(value => this.searchUsers(value))
    ).subscribe(result => {
      this.filteredUsers = result;
      this.userCbx.loading = false;
    });
  }

  stateGet(state) {
    switch (state) {
      case 'open':
        return 'Đã xác nhận';
      case 'paid':
        return 'Đã thanh toán';
      case 'cancel':
        return 'Đã hủy';
      default:
        return 'Nháp';
    }
  }

  //CSS theo trạng thái hóa đơn
  getBackgroundColor(state) {
    switch (state) {
      case 'open':
        return '#17a2b8';
      case 'paid':
        return '#28a745';
      case 'cancel':
        return '#dc3545';
      default:
        return '#6c757d';
    }
  }

  //CSS theo giới tính 
  getBorderColorByGender(gender: string) {
    switch (gender) {
      case 'Male':
        return '#0000ff';
      case 'Female':
        return '#ff0000';
      case 'Other':
        return '#cc33ff';
      default:
        return '#6c757d';
    }
  }
}
