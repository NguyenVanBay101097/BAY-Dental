import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, Validators } from '@angular/forms';
import { PartnerSimple, PartnerPaged } from 'src/app/partners/partner-simple';
import { UserSimple } from 'src/app/users/user-simple';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { PartnerService, PartnerFilter } from 'src/app/partners/partner.service';
import { UserService, UserPaged } from 'src/app/users/user.service';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { IntlService } from '@progress/kendo-angular-intl';
import { WindowService, WindowCloseResult } from '@progress/kendo-angular-dialog';
import { Observable } from 'rxjs';
import { map, debounceTime, switchMap, tap } from 'rxjs/operators';
import { AccountInvoiceLineDialogComponent } from '../account-invoice-line-dialog/account-invoice-line-dialog.component';
import { AccountInvoiceLineDisplay } from '../account-invoice-line-display';
import { AccountInvoiceService, AccountInvoiceDisplay, PaymentInfoContent, AccountInvoicePrint } from '../account-invoice.service';
import { AccountInvoiceRegisterPaymentDialogComponent } from '../account-invoice-register-payment-dialog/account-invoice-register-payment-dialog.component';
import { InvoiceCreateDotkhamDialogComponent } from '../invoice-create-dotkham-dialog/invoice-create-dotkham-dialog.component';
import { DotKhamService } from 'src/app/dot-khams/dot-kham.service';
import * as _ from 'lodash';
import { NotificationService } from '@progress/kendo-angular-notification';
import { LaboOrderLineCuDialogComponent } from 'src/app/labo-order-lines/labo-order-line-cu-dialog/labo-order-line-cu-dialog.component';
import { PrintService } from 'src/app/print.service';
import { DotKhamLineService } from 'src/app/dot-khams/dot-kham-line.service';
import { DotKhamBasic } from 'src/app/dot-khams/dot-khams';

@Component({
  selector: 'app-customer-invoice-create-update',
  templateUrl: './customer-invoice-create-update.component.html',
  styleUrls: ['./customer-invoice-create-update.component.css']
})
export class CustomerInvoiceCreateUpdateComponent implements OnInit {
  orderForm: FormGroup;
  id: string;
  type = 'out_invoice';
  filteredPartners: PartnerSimple[];
  filteredUsers: UserSimple[];
  payments: PaymentInfoContent[] = [];
  record: AccountInvoiceDisplay;
  dotKhams: DotKhamBasic[] = [];
  loading = false;
  invoicePrint: AccountInvoicePrint;

  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;
  opened = false;

  constructor(private fb: FormBuilder, private partnerService: PartnerService,
    private userService: UserService, private route: ActivatedRoute,
    public intlService: IntlService, private windowService: WindowService, private accountInvoiceService: AccountInvoiceService,
    private notificationService: NotificationService,
    private router: Router, private printService: PrintService, private dotkhamLinesService: DotKhamLineService) { }

  ngOnInit() {
    this.orderForm = this.fb.group({
      partner: [null, Validators.required],
      user: null,
      dateOrderObj: null,
      dateOrder: null,
      invoiceLines: this.fb.array([]),
      companyId: null,
      userId: null,
      partnerId: null,
      accountId: null,
      journalId: null,
      type: null,
      state: null,
      number: null,
      amountTotal: 0,
      discountType: 'fixed',
      discountPercent: 0,
      discountFixed: 0,
      residual: 0,
      comment: null
    });

    setTimeout(() => {
      this.partnerCbx.focus();
    }, 200);

    this.route.paramMap.pipe(
      switchMap((params: ParamMap) => {
        this.id = params.get("id");
        if (this.id) {
          return this.accountInvoiceService.get(this.id);
        } else {
          var defaultVal = new AccountInvoiceDisplay();
          defaultVal.type = "out_invoice";
          return this.accountInvoiceService.defaultGet(defaultVal);
        }
      })).subscribe(result => {
        this.orderForm.patchValue(result);
        let dateOrder = this.intlService.parseDate(result.dateOrder);
        this.orderForm.get('dateOrderObj').setValue(dateOrder);

        const control = this.orderForm.get('invoiceLines') as FormArray;
        result.invoiceLines.forEach(line => {
          var g = this.fb.group(line);
          g.setControl('teeth', this.fb.array(line.teeth));
          control.push(g);
        });

        if (result.partner) {
          this.filteredPartners = _.unionBy(this.filteredPartners, [result.partner], 'id');
        }
        if (result.user) {
          this.filteredUsers = _.unionBy(this.filteredUsers, [result.user], 'id');
        }
      });


    this.partnerCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.partnerCbx.loading = true)),
      switchMap(value => this.searchPartners(value))
    ).subscribe(result => {
      this.filteredPartners = result;
      this.partnerCbx.loading = false;
    });

    this.userCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.userCbx.loading = true)),
      switchMap(value => this.searchUsers(value))
    ).subscribe(result => {
      this.filteredUsers = result;
      this.userCbx.loading = false;
    });

    this.loadPaymentInfo();
    this.loadDotKhamList();
    this.loadPartners();
    this.loadUsers();
  }

  createNew() {
    this.router.navigate(['/customer-invoices/create']);
  }

  loadDotKhamList() {
    if (this.id) {
      return this.accountInvoiceService.getDotKhamList(this.id).subscribe(result => {
        this.dotKhams = result;
      });
    }
  }

  onChangeDiscountType(event) {
    this.computeAmountTotal();
  }

  printInvoice() {
    if (this.id) {
      this.accountInvoiceService.getPrint(this.id).subscribe(result => {
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

  loadPartners() {
    this.searchPartners().subscribe(result => {
      this.filteredPartners = _.unionBy(this.filteredPartners, result, 'id');
      console.log(result);
    });
  }

  loadUsers() {
    this.searchUsers().subscribe(result => {
      this.filteredUsers = _.unionBy(this.filteredUsers, result, 'id');
      console.log(result);
    });
  }

  searchPartners(filter?: string) {
    var val = new PartnerPaged();
    val.customer = true;
    val.searchNamePhoneRef = filter;
    return this.partnerService.getAutocompleteSimple(val);
  }

  searchUsers(filter?: string) {
    var val = new UserPaged();
    val.search = filter;
    return this.userService.autocompleteSimple(val);
  }

  loadRecord() {
    if (this.id) {
      this.accountInvoiceService.get(this.id).subscribe(result => {
        this.record = result;
      });
    }
  }

  get discountType() {
    return this.orderForm.get('discountType').value;
  }

  loadPaymentInfo() {
    if (this.id) {
      this.accountInvoiceService.getPaymentInfoJson(this.id).subscribe(result => {
        this.payments = result;
      });
    }
  }

  valueNormalizer(text: Observable<string>) {
    return text.pipe(
      map((a: string) => {
        return {
          value: null,
          text: a
        };
      })
    )
  };

  get getInvState() {
    return this.orderForm.get('state').value;
  }

  get getInvNumber() {
    return this.orderForm.get('number').value;
  }

  get getInvAmountTotal() {
    return this.orderForm.get('amountTotal').value;
  }

  get getInvResidual() {
    return this.orderForm.get('residual').value;
  }

  actionViewDotKham() {
    if (this.dotKhams.length) {
      if (this.dotKhams.length == 1) {
        this.router.navigate(['dot-khams/edit/', this.dotKhams[0].id]);
      }
    }
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

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
      if (result instanceof WindowCloseResult) {
      } else {
        let line = result as any;
        line.teeth = this.fb.array(line.teeth);
        this.invoiceLines.push(this.fb.group(line));

        this.computeAmountTotal();
      }
    });
  }

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

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
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

  lineProduct(line: FormGroup) {
    var product = line.get('product').value;
    return product ? product.name : '';
  }

  lineTeeth(line: FormGroup) {
    var teeth = line.get('teeth').value as any[];
    return teeth.map(x => x.name).join(',');
  }

  lineEmployee(line: FormGroup) {
    var employee = line.get('employee').value;
    return employee ? employee.name : '';
  }

  get invoiceLines() {
    return this.orderForm.get('invoiceLines') as FormArray;
  }

  onSave() {
    if (!this.orderForm.valid) {
      return;
    }

    var val = this.orderForm.value;
    val.partnerId = val.partner.id;
    val.userId = val.user ? val.user.id : null;
    val.dateInvoice = this.intlService.formatDate(val.dateInvoiceD, 'g', 'en-US');

    this.accountInvoiceService.create(val).subscribe(result => {
      this.router.navigate(['/customer-invoices/edit', result.id]);
    });
  }

  onUpdate() {
    if (!this.orderForm.valid) {
      return;
    }

    if (this.id) {
      var val = this.orderForm.value;
      val.partnerId = val.partner.id;
      val.userId = val.user ? val.user.id : null;
      val.dateInvoice = this.intlService.formatDate(val.dateInvoiceD, 'g', 'en-US');

      this.accountInvoiceService.update(this.id, val).subscribe(result => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.reloadData();
      });
    }
  }

  saveOrUpdate() {
    var val = this.orderForm.value;
    val.partnerId = val.partner.id;
    val.userId = val.user ? val.user.id : null;
    val.dateInvoice = this.intlService.formatDate(val.dateInvoiceD, 'g', 'en-US');

    if (this.id) {
      return this.accountInvoiceService.update(this.id, val);
    } else {
      this.accountInvoiceService.create(val);
    }
  }

  onSaveConfirm() {
    if (!this.orderForm.valid) {
      return;
    }

    var val = this.orderForm.value;
    val.partnerId = val.partner.id;
    val.userId = val.user ? val.user.id : null;
    val.dateInvoice = this.intlService.formatDate(val.dateInvoiceD, 'g', 'en-US');
    this.loading = true;
    this.accountInvoiceService.create(val).subscribe(result => {
      this.accountInvoiceService.invoiceOpen([result.id]).subscribe(() => {
        this.router.navigate(['/customer-invoices/edit', result.id]);
      }, () => {
        this.loading = false;
      });
    }, () => {
      this.loading = false;
    });
  }

  onConfirm() {
    if (this.id) {
      this.loading = true;
      this.accountInvoiceService.invoiceOpen([this.id]).subscribe(() => {
        this.loading = false;
        this.reloadData();
      }, () => {
        this.loading = false;
      });
    }
  }

  reloadData() {
    this.accountInvoiceService.get(this.id).subscribe(result => {
      console.log(result);
      this.orderForm.patchValue(result);
      let dateOrder = this.intlService.parseDate(result.dateOrder);
      this.orderForm.get('dateOrderObj').patchValue(dateOrder);

      let control = this.orderForm.get('invoiceLines') as FormArray;
      control = this.fb.array([]); //reset form array
      result.invoiceLines.forEach(line => {
        var g = this.fb.group(line);
        g.setControl('teeth', this.fb.array(line.teeth));
        control.push(g);
      });
    });

    this.loadPaymentInfo();
    this.loadDotKhamList();
  }

  partnerDisplayFn(partner) {
    if (partner) {
      return partner.name;
    }
  }

  userDisplayFn(user) {
    if (user) {
      return user.name;
    }
  }

  actionInvoicePayment() {
    const windowRef = this.windowService.open({
      title: 'Thanh toán',
      content: AccountInvoiceRegisterPaymentDialogComponent,
      resizable: false,
      autoFocusedElement: '[name="name"]',
    });

    const instance = windowRef.content.instance;
    instance.invoiceId = this.id;

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
      if (result instanceof WindowCloseResult) {
      } else {
        this.reloadData();
      }
    });
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
    this.orderForm.get('amountTotal').patchValue(total);
  }

  get discountFixed() {
    return this.orderForm.get('discountFixed').value || 0;
  }

  get discountPercent() {
    return this.orderForm.get('discountPercent').value || 0;
  }

  actionCancel() {
    if (this.id) {
      this.loading = true;
      this.accountInvoiceService.actionCancel([this.id]).subscribe(() => {
        this.loading = false;
        this.reloadData();
      }, () => {
        this.loading = false;
      });
    }
  }

  actionCancelDraft() {
    if (this.id) {
      this.loading = true;
      this.accountInvoiceService.actionCancelDraft([this.id]).subscribe(() => {
        this.loading = false;
        this.reloadData();
      }, () => {
        this.loading = false;
      });
    }
  }

  actionCreateDotKham() {
    const windowRef = this.windowService.open({
      title: 'Tạo đợt khám',
      content: InvoiceCreateDotkhamDialogComponent,
      resizable: false,
      autoFocusedElement: '[name="name"]',
    });

    const instance = windowRef.content.instance;
    instance.invoiceId = this.id;

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
      if (result instanceof WindowCloseResult) {
      } else {
        var val = result as any;
        if (val.view) {
          this.router.navigate(['/dot-khams/edit/', val.result.id]);
        } else {
          this.loadDotKhamList();
        }
      }
    });
  }

  actionCreateLabo() {
    const windowRef = this.windowService.open({
      title: 'Tạo labo',
      content: LaboOrderLineCuDialogComponent,
      resizable: false,
    });

    const instance = windowRef.content.instance;
    instance.invoiceId = this.id;

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
      if (result instanceof WindowCloseResult) {
      } else {
        this.notificationService.show({
          content: 'Tạo thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      }
    });
  }
}
