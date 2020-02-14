import { Component, OnInit, ViewChild, Injector, ElementRef, AfterViewInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { PartnerSimple, PartnerPaged } from 'src/app/partners/partner-simple';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { DotKhamService } from '../dot-kham.service';
import { IntlService } from '@progress/kendo-angular-intl';
import { PartnerService, PartnerFilter } from 'src/app/partners/partner.service';
import { debounceTime, tap, switchMap, map } from 'rxjs/operators';
import { AccountInvoiceCbx, AccountInvoiceService } from 'src/app/account-invoices/account-invoice.service';
import { Observable, pipe } from 'rxjs';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { UserSimple } from 'src/app/users/user-simple';
import { UserService } from 'src/app/users/user.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { DotKhamLineService, DotKhamLineDisplay, DotKhamLineBasic } from '../dot-kham-line.service';
import { DotKhamLineOperationService } from '../dot-kham-line-operation.service';
import { WindowService, WindowCloseResult, WindowRef, DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { ToaThuocCuDialogComponent } from 'src/app/toa-thuocs/toa-thuoc-cu-dialog/toa-thuoc-cu-dialog.component';
import { ToaThuocBasic, ToaThuocService, ToaThuocPrint } from 'src/app/toa-thuocs/toa-thuoc.service';
import * as _ from 'lodash';
import { DotKhamLineCuDialogComponent } from 'src/app/dot-kham-lines/dot-kham-line-cu-dialog/dot-kham-line-cu-dialog.component';
import { LaboOrderLineCuDialogComponent } from 'src/app/labo-order-lines/labo-order-line-cu-dialog/labo-order-line-cu-dialog.component';
import { LaboOrderLineBasic } from 'src/app/labo-order-lines/labo-order-line.service';
import { AppointmentCuDialogComponent } from 'src/app/appointment/appointment-cu-dialog/appointment-cu-dialog.component';
import { AppointmentBasic, AppointmentPaged } from 'src/app/appointment/appointment';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { ProductService } from 'src/app/products/product.service';
import { ProductStepDisplay } from 'src/app/products/product-step';
import { EmployeeSimple, EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { AppointmentCreateUpdateComponent } from 'src/app/appointment/appointment-create-update/appointment-create-update.component';
import { moveItemInArray, CdkDragDrop, transferArrayItem } from '@angular/cdk/drag-drop';
import { DotKhamStepDisplay, DotKhamDefaultGet, DotKhamStepSave, DotKhamPatch, DotKhamDisplay } from '../dot-khams';
import { timeInRange } from '@progress/kendo-angular-dateinputs/dist/es2015/util';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IrAttachmentSearchRead, IrAttachmentBasic } from 'src/app/shared/shared';
import { ImageViewerComponent } from 'src/app/shared/image-viewer/image-viewer.component';
import { DotKhamStepService, DotKhamStepAssignDotKhamVM, DotKhamStepSetDone } from '../dot-kham-step.service';
import { LaboOrderBasic } from 'src/app/labo-orders/labo-order.service';
import { environment } from 'src/environments/environment';
import { Operation } from 'fast-json-patch';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { SaleOrderCreateDotKhamDialogComponent } from 'src/app/sale-orders/sale-order-create-dot-kham-dialog/sale-order-create-dot-kham-dialog.component';
declare var $: any;

@Component({
  selector: 'app-dot-kham-create-update',
  templateUrl: './dot-kham-create-update.component.html',
  styleUrls: ['./dot-kham-create-update.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class DotKhamCreateUpdateComponent implements OnInit {
  id: string;//id đợt khám
  invoiceId: string;//id hóa đơn
  invoiceState: string;//Trạng thái của hóa đơn

  dotKhamForm: FormGroup;
  filteredPartners: PartnerSimple[];
  filteredUsers: UserSimple[];

  //Dấu check cho những dịch vụ đã xong
  // doneList: DotKhamStepDisplay[][] = [];
  // @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('invoiceCbx', { static: true }) invoiceCbx: ComboBoxComponent;
  @ViewChild('doctorCbx', { static: true }) doctorCbx: ComboBoxComponent;
  @ViewChild('assistantCbx', { static: true }) assistantCbx: ComboBoxComponent;
  @ViewChild('inputFile', { static: true }) inputFile: ElementRef;
  // @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;
  opened = false;
  toaThuocPrint: ToaThuocPrint;

  // dotKhamLines: DotKhamLineBasic[] = [];
  // dotKhamLinesList: DotKhamLineBasic[][] = [];
  dotKhamStepsList: DotKhamStepDisplay[][] = [];
  dotKhamStepList: DotKhamStepDisplay[] = [];

  toaThuocs: ToaThuocBasic[] = [];
  laboOrders: LaboOrderBasic[] = [];
  appointments: AppointmentBasic[] = [];
  customerInvoicesList: AccountInvoiceCbx[] = [];

  isChange = false;
  customerSimpleFilter: PartnerSimple[] = [];
  doctorSimpleFilter: EmployeeSimple[] = [];
  assistantSimpleFilter: EmployeeSimple[] = [];
  productSimpleList: ProductSimple[] = [];
  productSimpleFilteredList: ProductSimple[] = [];
  skip: number = 0;
  limit: number = 20;

  window: WindowRef;

  imagesPreview: IrAttachmentBasic[] = [];
  filesPreview: IrAttachmentBasic[] = [];

  dialog = false;//Component được mở dưới dạng Dialog hay Tab mới
  dotKham: DotKhamDisplay = new DotKhamDisplay();

  webImageApi: string;
  webContentApi: string;

  editingStep: DotKhamStepDisplay;


  constructor(private fb: FormBuilder, private dotKhamService: DotKhamService, private intlService: IntlService,
    private partnerService: PartnerService, private accountInvoiceService: AccountInvoiceService,
    private userService: UserService, private notificationService: NotificationService,
    private dialogService: DialogService, private router: Router, private route: ActivatedRoute,
    private toaThuocService: ToaThuocService, private appointmentService: AppointmentService, private productService: ProductService,
    private employeeService: EmployeeService, private injector: Injector, private modalService: NgbModal,
    private dotKhamStepService: DotKhamStepService) { }


  ngOnInit() {
    this.dotKhamForm = this.fb.group({
      name: null,
      partner: [null, Validators.required],
      dateObj: null,
      date: null,
      note: null,
      companyId: null,
      user: null,
      state: null,
      doctor: [null, Validators.required],
      assistant: null,
      saleOrderId: null,
      step: null,
      product: null,
      appointment: null,
      appointmentId: null,
      filter: "dotkham"
    });

    this.getCustomerList();
    this.getDoctorList();
    this.getAssistantList();

    if (this.dialog) {
      this.loadDataFromHistory();
    } else {
      this.getActiveRoute();
    }

    if (!this.id && !this.invoiceId) {
      this.dotKhamForm.get('invoice').disable();
    }

    // this.userCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.userCbx.loading = true)),
    //   switchMap(value => this.searchUsers(value))
    // ).subscribe(result => {
    //   this.filteredUsers = result;
    //   this.userCbx.loading = false;
    // });

    this.loadToaThuocs();
    // this.loadAppointments();
    // this.loadDotKhamSteps();
    this.loadDotKhamStepList();
    this.loadLaboOrders();
    this.loadProductSimpleList();

    this.filterChangeCombobox();
    this.valueChangeCombobox();
    this.getImageIds();

    this.webImageApi = environment.uploadDomain + 'api/Web/Image';
    this.webContentApi = environment.uploadDomain + 'api/Web/Content';
  }

  setEditingStep(step: DotKhamStepDisplay) {
    this.editingStep = step;
  }

  getActiveRoute() {
    this.route.paramMap.pipe(
      switchMap((params: ParamMap) => {
        this.id = params.get("id");
        if (this.id) {
          return this.dotKhamService.get(this.id);
        } else {
          var defaultVal = new DotKhamDefaultGet();
          return this.dotKhamService.defaultGet(defaultVal);
        }
      })).subscribe(result => {
        this.dotKham = result;
        this.dotKhamForm.patchValue(result);
        let date = new Date(result.date);
        this.dotKhamForm.get('dateObj').patchValue(date);
        if (result.doctor) {
          this.doctorSimpleFilter = _.unionBy(this.doctorSimpleFilter, [result.doctor], 'id');
        }
        if (result.assistant) {
          this.assistantSimpleFilter = _.unionBy(this.assistantSimpleFilter, [result.assistant], 'id');
        }
        if (result.user) {
          this.filteredUsers = _.unionBy(this.filteredUsers, [result.user], 'id');
        }
      });
  }

  updateStepName(event, step: DotKhamStepDisplay) {
    var name = event.target.value;
    if (!name) {
      alert('Tên công đoạn không được trống');
      return false;
    }
    var patch: Operation[] = [
      { op: "replace", path: "/name", value: name },
    ];
    this.dotKhamStepService.patch(step.id, patch).subscribe(() => {
      step.name = name;
      this.editingStep = null;
    });
  }

  loadDataFromHistory() {
    if (this.id) {
      this.dotKhamService.get(this.id).subscribe(
        result => {
          this.dotKhamForm.patchValue(result);
          let date = this.intlService.parseDate(result.date);
          this.dotKhamForm.get('dateObj').patchValue(date);

          // this.dotKhamForm.get('doctor').setValue(result.doctor);
          // this.dotKhamForm.get('assistant').setValue(result.doctor);

          if (result.user) {
            this.filteredUsers = _.unionBy(this.filteredUsers, [result.user], 'id');
          }
        })
    }
    else if (this.invoiceId) {
      var defaultVal = new DotKhamDefaultGet();
      defaultVal.invoiceId = this.invoiceId;
      this.dotKhamForm.get('invoice').enable();
      this.dotKhamService.defaultGet(defaultVal).subscribe(
        rs1 => {
          console.log(rs1);
          this.dotKhamService.getCustomerInvoices(rs1.partnerId).subscribe(
            rs2 => {
              this.customerInvoicesList = rs2;
              this.dotKhamForm.patchValue(rs1);
              let date = this.intlService.parseDate(rs1.date);
              this.dotKhamForm.get('dateObj').patchValue(date);
            });


          if (rs1.user) {
            this.filteredUsers = _.unionBy(this.filteredUsers, [rs1.user], 'id');
          }
        })
    }
  }

  loadToaThuocs() {
    if (this.id) {
      this.dotKhamService.getToaThuocs(this.id).subscribe(result => {
        this.toaThuocs = result;
      })
    }
  }

  copyInsert(step: DotKhamStepDisplay, cloneInsert: string, index) {
    this.dotKhamStepService.cloneInsert({ id: step.id, cloneInsert: cloneInsert }).subscribe(result => {
      if (cloneInsert == 'up') {
        this.dotKhamStepList.splice(index, 0, result);
      } else {
        this.dotKhamStepList.splice(index + 1, 0, result);
      }

      this.editingStep = result;
      setTimeout(() => {
        $('#table_details .updateStepName').focus();
        setTimeout(() => {
          $('#table_details .updateStepName').select();
        }, 70);
      }, 70);
    });
  }

  assignDotKham(step: DotKhamStepDisplay) {
    var val = new DotKhamStepAssignDotKhamVM();
    val.ids = [step.id];
    val.dotKhamId = this.id;
    this.dotKhamStepService.assignDotKham(val).subscribe(() => {
      this.loadDotKhamStepList();
    });
  }

  toggleIsDone(step: DotKhamStepDisplay, event) {
    var val = new DotKhamStepSetDone();
    val.ids = [step.id];
    val.dotKhamId = this.id;
    val.isDone = event.target.checked;
    this.dotKhamStepService.toggleIsDone(val).subscribe(() => {
      step.isDone = event.target.checked;
    }, () => {
      step.isDone = !event.target.checked;
    });
  }

  computeIsDoneAll() {
    var notDone = _.find(this.dotKhamStepList, o => !o.isDone);
    if (notDone) {
      return false;
    }
    return true;
  }

  toggleIsDoneAll(event) {
    var val = new DotKhamStepSetDone();
    val.ids = this.dotKhamStepList.map(x => x.id);
    val.dotKhamId = this.id;
    val.isDone = event.target.checked;
    this.dotKhamStepService.toggleIsDone(val).subscribe(() => {
      this.dotKhamStepList.forEach(step => {
        step.isDone = event.target.checked;
      });
    }, () => {
    });
  }

  // loadAppointments() {
  //   if (this.id) {
  //     var val = new AppointmentPaged();
  //     val.limit = 0;
  //     val.dotKhamId = this.id;
  //     this.appointmentService.getPaged(val).subscribe(result => {
  //       console.log(this.appointments);
  //       this.appointments = result.items;
  //     });
  //   }
  // }

  loadLaboOrders() {
    if (this.id) {
      this.dotKhamService.getLaboOrders(this.id).subscribe(result => {
        this.laboOrders = result;
      })
    }
  }

  // loadDotKhamLines() {
  //   if (this.id) {
  //     this.dotKhamService.getDotKhamLines2(this.id).subscribe(result => {
  //       console.log(result);
  //       this.dotKhamLinesList = result;
  //     })
  //   }
  // }

  loadDotKhamSteps() {
    if (this.id) {
      this.dotKhamService.getDotKhamStepsByDKId2(this.id, this.getDotKhamFilter).subscribe(result => {
        this.dotKhamStepsList = result;
        console.log(result);//Tên dịch vụ tham chiếu đến tên của invoiceLine
      })
    }
  }

  loadDotKhamStepList() {
    if (this.id) {
      this.dotKhamService.getDotKhamStepsByDKId(this.id).subscribe(result => {
        this.dotKhamStepList = result;
      })
    }
  }

  get getDotKhamFilter() {
    return this.dotKhamForm.get('filter').value;
  }

  loadProductSimpleList() {
    if (this.id) {
      this.dotKhamService.getInvoiceLineByInvoiceId(this.id).subscribe(result => {
        result.forEach(item => {
          var pdSimple = new ProductSimple();
          pdSimple.id = item.productId;
          pdSimple.name = item.name;
          this.productSimpleList.push(pdSimple);
          this.productSimpleFilteredList.push(pdSimple);
        });
      })
    }
  }

  createDKSteps() {
    var step = new DotKhamStepDisplay;
    step.invoicesId = this.dotKhamForm.get('invoice').value.id;
    step.name = this.dotKhamForm.get('step').value;
    step.productId = this.dotKhamForm.get('product').value.id;
    this.dotKhamService.createDKSteps(step).subscribe(
      rs => {
        this.loadDotKhamSteps();
        this.dotKhamForm.get('product').setValue(null);
        this.dotKhamForm.get('step').setValue(null);
      }
    )
  }

  deleteSteps(id) {
    const dialogRef: DialogRef = this.dialogService.open({
      title: 'Xóa công đoạn',
      content: 'Bạn chắc chắn muốn xóa công đoạn này ?',
      width: 450,
      height: 200,
      minWidth: 250,
      actions: [
        { text: 'Hủy', value: false },
        { text: 'Đồng ý', primary: true, value: true }
      ]
    });
    dialogRef.result.subscribe(
      rs => {
        if (!(rs instanceof DialogCloseResult)) {
          if (rs['value']) {
            this.dotKhamService.deleteDKSteps(id).subscribe(
              () => { this.loadDotKhamSteps(); }
            );
          }
        }
      }
    )
  }

  deleteStep(step: DotKhamStepDisplay, index) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa công đoạn';
    modalRef.result.then(() => {
      this.dotKhamStepService.delete(step.id).subscribe(() => {
        this.dotKhamStepList.splice(index, 1);
      });
    }, () => {
    });
  }

  actionCreateLabo() {
    if (this.id) {
      this.router.navigate(['/labo-orders/create'], { queryParams: { dot_kham_id: this.id } });
      // let modalRef = this.modalService.open(LaboOrderLineCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
      // modalRef.componentInstance.title = 'Tạo labo';
      // modalRef.componentInstance.dotKhamId = this.id;

      // modalRef.result.then(() => {
      //   this.loadLaboOrders();
      // }, () => {
      // });
    }
  }


  printToaThuoc(item: ToaThuocBasic) {
    this.toaThuocService.getPrint(item.id).subscribe(result => {
      this.toaThuocPrint = result;
      setTimeout(() => {
        var printContents = document.getElementById('printToaThuocDiv').innerHTML;
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
        this.toaThuocPrint = null;
      }, 500);
    });
  }

  // editDotKhamLine(item: DotKhamLineBasic) {
  //   const windowRef = this.windowService.open({
  //     title: 'Tiến trình dịch vụ',
  //     content: DotKhamLineCuDialogComponent,
  //     resizable: false,

  //     autoFocusedElement: '[name="name"]',
  //   });

  //   const instance = windowRef.content.instance;
  //   instance.id = item.id;

  //   this.opened = true;

  //   windowRef.result.subscribe((result) => {
  //     this.opened = false;
  //     if (result instanceof WindowCloseResult) {
  //     } else {
  //       this.loadDotKhamLines();
  //     }
  //   });
  // }


  editToaThuoc(item: ToaThuocBasic) {
    let modalRef = this.modalService.open(ToaThuocCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.title = 'Sửa toa thuốc';
    modalRef.componentInstance.id = item.id;

    modalRef.result.then(() => {
      this.loadToaThuocs();
    }, () => {
    });
  }

  // editAppointment(item: AppointmentBasic) {
  //   const windowRef = this.windowService.open({
  //     title: 'Sửa cuộc hẹn',
  //     content: AppointmentCreateUpdateComponent,
  //     resizable: false,
  //     autoFocusedElement: '[name="name"]',
  //   });

  //   const instance = windowRef.content.instance;
  //   instance.appointId = item.id;

  //   this.opened = true;

  //   windowRef.result.subscribe((result) => {
  //     this.opened = false;
  //     if (result instanceof WindowCloseResult) {
  //     } else {
  //       this.loadAppointments();
  //     }
  //   });
  // }

  editLaboLine(item: LaboOrderLineBasic) {
    let modalRef = this.modalService.open(LaboOrderLineCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.title = 'Sửa labo';
    modalRef.componentInstance.id = item.id;

    modalRef.result.then(() => {
      this.loadLaboOrders();
    }, () => {
    });
  }

  deleteToaThuoc(item: ToaThuocBasic) {
    this.toaThuocService.delete(item.id).subscribe(() => {
      this.loadToaThuocs();
    });
  }

  searchUsers(search?: string) {
    return this.userService.autocomplete(search);
  }

  get getName() {
    return this.dotKhamForm.get('name').value;
  }

  get getState() {
    return this.dotKhamForm.get('state').value;
  }

  get getInvoice() {
    return this.dotKhamForm.get('invoice').value;
  }

  get getPartner() {
    return this.dotKhamForm.get('partner').value;
  }

  // get getDoctor() {
  //   return this.dotKhamForm.get('doctor').value;
  // }

  // get getAssistant() {
  //   return this.dotKhamForm.get('assistant').value;
  // }

  actionCreateToaThuoc() {
    if (this.id) {
      let modalRef = this.modalService.open(ToaThuocCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
      modalRef.componentInstance.title = 'Kê toa thuốc';
      modalRef.componentInstance.dotKhamId = this.id;

      modalRef.result.then(result => {
        if (result.toaThuoc && result.print) {
          this.printToaThuoc(result.toaThuoc);
        }

        this.loadToaThuocs();
      }, () => {
      });
    }

    // const windowRef = this.windowService.open({
    //   title: 'Tạo toa thuốc',
    //   content: ToaThuocCuDialogComponent,
    //   resizable: false,
    //   autoFocusedElement: '[name="name"]',
    // });

    // const instance = windowRef.content.instance;
    // instance.dotKhamId = this.id;

    // this.opened = true;

    // windowRef.result.subscribe((result) => {
    //   this.opened = false;
    //   if (result instanceof WindowCloseResult) {
    //   } else {
    //     this.loadToaThuocs();
    //   }
    // });
  }

  // actionCreateAppointment() {
  //   const windowRef = this.windowService.open({
  //     title: 'Hẹn tái khám',
  //     content: AppointmentCreateUpdateComponent,
  //     resizable: false,
  //     autoFocusedElement: '[name="name"]',
  //   });

  //   const instance = windowRef.content.instance;
  //   instance.dotKhamId = this.id;

  //   this.opened = true;

  //   windowRef.result.subscribe((result) => {
  //     this.opened = false;
  //     if (result instanceof WindowCloseResult) {
  //     } else {
  //       this.loadAppointments();
  //     }
  //   });
  // }

  getDefault() {
    var val = new DotKhamDefaultGet();
    this.dotKhamService.defaultGet(val).subscribe(result => {
      this.dotKhamForm.patchValue(result);
      let date = this.intlService.parseDate(result.date);
      this.dotKhamForm.get('dateObj').patchValue(date);
    });
  }

  searchPartners(filter: string) {
    var val = new PartnerFilter();
    val.employee = true;
    val.search = filter;
    return this.partnerService.autocomplete2(val);
  }

  searchCustomers(search) {
    var partnerPaged = new PartnerPaged();
    partnerPaged.employee = false;
    partnerPaged.customer = true;
    partnerPaged.supplier = false;
    partnerPaged.limit = this.limit;
    partnerPaged.offset = this.skip;
    if (search != null) {
      partnerPaged.searchNamePhoneRef = search.toLowerCase();
    }
    return this.partnerService.autocompletePartner(partnerPaged);
  }

  searchInvoices(search?: string) {
    return this.accountInvoiceService.getOpenPaid(search);
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

  onSave() {
    if (!this.dotKhamForm.valid) {
      return;
    }

    var val = this.dotKhamForm.value;
    val.invoiceId = val.invoice ? val.invoice.id : null;
    val.partnerId = val.partner ? val.partner.id : null;
    val.doctorId = val.doctor ? val.doctor.id : null;
    val.assistantId = val.assistant ? val.assistant.id : null;
    val.userId = val.user ? val.user.id : null;

    this.dotKhamService.create(val).subscribe(result => {
      if (!this.dialog) {
        this.router.navigate(['/dot-khams/edit', result.id]);
      } else {
        this.id == result.id;
        this.closeWindow(result.id);
      }
    });
  }

  onUpdate() {
    debugger;

    if (!this.dotKhamForm.valid) {
      return;
    }

    if (this.id) {
      var data = this.prepareData();
      this.dotKhamService.update(this.id, data).subscribe(() => {
        this.notificationService.show({
          content: 'Lưu thay đổi thành công',
          hideAfter: 3000,
          position: { horizontal: 'right', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.reloadData();
      });
    }
  }

  onConfirm() {
    //nếu form dirty thì cần update rùi sau đó confirm
    if (this.id) {
      if (this.dotKhamForm.dirty) {
        var data = this.prepareData();
        this.dotKhamService.update(this.id, data).subscribe(() => {
          this.dotKhamService.actionConfirm(this.id).subscribe(() => {
            this.reloadData();
          })
        });
      } else {
        this.dotKhamService.actionConfirm(this.id).subscribe(() => {
          this.reloadData();
        })
      }
    }
  }

  reloadData() {
    if (this.id) {
      this.dotKhamService.get(this.id).subscribe(result => {
        this.dotKhamForm.patchValue(result);
        let date = this.intlService.parseDate(result.date);
        this.dotKhamForm.get('dateObj').patchValue(date);
      });
    }
  }

  prepareData() {
    var val = this.dotKhamForm.value;
    val.invoiceId = val.invoice ? val.invoice.id : null;
    val.doctorId = val.doctor ? val.doctor.id : null;
    val.partnerId = val.partner ? val.partner.id : null;
    val.assistantId = val.assistant ? val.assistant.id : null;
    val.userId = val.user ? val.user.id : null;
    return val;
  }

  // markLineDone(item: any) {
  //   this.dotKhamLineService.markDone(item.id).subscribe(() => {
  //     this.reloadData();
  //   });
  // }

  // markOperationDone(item: any) {
  //   this.dotKhamLineOperationService.markDone(item.id).subscribe(() => {
  //     this.reloadData();
  //   });
  // }

  getStateLine(state) {
    switch (state) {
      case 'done':
        return 'Đã xong';
      case 'progress':
        return 'Đang tiến hành';
      default:
        return 'Chưa tiến hành'
    }
  }

  getInvoiceLineName(id: string) {

  }

  // getStateOperation(state) {
  //   switch (state) {
  //     case 'done':
  //       return 'Hoàn thành';
  //     default:
  //       return 'Chưa hoàn thành'
  //   }
  // }


  //===============BỔ SUNG=======================
  getCustomerList() {
    var partnerPaged = new PartnerPaged();
    partnerPaged.employee = false;
    partnerPaged.customer = true;
    partnerPaged.supplier = false;
    partnerPaged.limit = this.limit;
    partnerPaged.offset = this.skip;
    this.partnerService.autocompletePartner(partnerPaged).subscribe(
      rs => {
        this.customerSimpleFilter = rs as PartnerSimple[];
      }
    )
  }

  filterChangeCombobox() {
    this.partnerCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => this.partnerCbx.loading = true),
      switchMap(val => this.searchCustomers(val.toString().toLowerCase()))
    ).subscribe(
      rs => {
        this.customerSimpleFilter = rs;
        this.partnerCbx.loading = false;
      }
    )

    this.doctorCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => this.doctorCbx.loading = true),
      switchMap(val => this.searchDoctors(val.toString().toLowerCase()))
    ).subscribe(
      rs => {
        this.doctorSimpleFilter = rs;
        this.doctorCbx.loading = false;
      }
    )

    this.assistantCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => this.assistantCbx.loading = true),
      switchMap(val => this.searchAssitants(val.toString().toLowerCase()))
    ).subscribe(
      rs => {
        this.assistantSimpleFilter = rs;
        this.assistantCbx.loading = false;
      }
    )
  }

  searchDoctors(search) {
    var empPaged = new EmployeePaged();
    empPaged.isDoctor = true;
    empPaged.limit = this.limit;
    empPaged.offset = this.skip;
    if (search != null) {
      empPaged.search = search.toLowerCase();
    }
    return this.employeeService.getEmployeeSimpleList(empPaged);
  }

  searchAssitants(search) {
    var empPaged = new EmployeePaged();
    empPaged.isAssistant = true;
    empPaged.limit = this.limit;
    empPaged.offset = this.skip;
    if (search != null) {
      empPaged.search = search.toLowerCase();
    }
    return this.employeeService.getEmployeeSimpleList(empPaged);
  }


  valueChangeCombobox() {
    this.partnerCbx.valueChange.asObservable().subscribe(
      rs => {
        if (rs) {
          this.dotKhamService.getCustomerInvoices(rs.id).subscribe(
            rs2 => {
              this.customerInvoicesList = rs2;
              this.dotKhamForm.get('invoice').enable();
              this.dotKhamForm.get('invoice').setValue(rs2[0]);
            }
          )
        } else {
          this.dotKhamForm.get('invoice').setValue(null);
          this.dotKhamForm.get('invoice').disable();
        }
      }
    );
  }

  checkState(id, state) {
    var save = new DotKhamStepSave;
    var ar = [];
    save.dotKhamId = state != "done" ? this.id : null;
    save.state = (state == "draft") ? "progress" : (state == "progress") ? "done" : "draft";
    save.isDone = state == "progress" ? true : false;
    for (var p in save) {
      var o = { op: 'replace', path: '/' + p, value: save[p] };
      ar.push(o);
    }
    this.dotKhamService.patchDotKhamStep(id, ar).subscribe(
      () => {
        this.loadDotKhamSteps();
      }
    );
  }

  getDoctorList() {
    var empPn = new EmployeePaged;
    empPn.isDoctor = true;
    empPn.limit = this.limit;
    empPn.offset = this.skip;
    this.employeeService.getEmployeeSimpleList(empPn).subscribe(
      rs => {
        this.doctorSimpleFilter = rs;
      });
  }

  getAssistantList() {
    var empPn = new EmployeePaged;
    empPn.isAssistant = true;
    empPn.limit = this.limit;
    empPn.offset = this.skip;
    this.employeeService.getEmployeeSimpleList(empPn).subscribe(
      rs => {
        this.assistantSimpleFilter = rs;
      });
  }

  closeWindow(id) {
    this.window = this.injector.get<WindowRef>(WindowRef);
    this.dialog = false;
    if (id) {
      this.window.close(id);
    }
    else {
      this.window.close();
    }
  }

  filterProductSimple(e) {
    this.productSimpleFilteredList = this.productSimpleList.filter((s) => s.name.toLowerCase().indexOf(e.toLowerCase()) !== -1);
  }

  actionCreateDotKham() {
    if (this.dotKham && this.dotKham.saleOrderId) {
      let modalRef = this.modalService.open(SaleOrderCreateDotKhamDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Tạo đợt khám';
      modalRef.componentInstance.saleOrderId = this.dotKham.saleOrderId;

      modalRef.result.then(result => {
        if (result.view) {
          this.router.navigate(['/dot-khams/edit/', result.result.id]);
        } else {
        }
      }, () => {
      });
    }
  }

  drop(event: CdkDragDrop<string[]>) {
    moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    this.dotKhamService.reorder(event.currentIndex, event.container.data).subscribe(
      rs => {
        this.loadDotKhamSteps();
      }
    )
  }

  get getAppointment() {
    return this.dotKhamForm.get('appointment').value ? this.dotKhamForm.get('appointment').value : '';
  }

  // get getAppointmentId() {
  //   console.log(this.dotKhamForm.get('appointmentId').value);
  //   return this.dotKhamForm.get('appointmentId').value;
  // }

  // appointmentCreate() {
  //   const windowRef = this.windowService.open({
  //     title: 'Lịch hẹn',
  //     content: AppointmentCreateUpdateComponent,
  //     resizable: false,
  //   });
  //   const instance = windowRef.content.instance;
  //   this.opened = true;
  //   if (this.getAppointment) {
  //     instance.appointId = this.getAppointment.id;
  //   }
  //   instance.dotKhamId = this.id

  //   windowRef.result.subscribe((result) => {
  //     this.opened = false;
  //     if (result instanceof WindowCloseResult) {
  //       console.log(1);
  //     } else {
  //       if (result['id']) {
  //         var dkpatch = new DotKhamPatch;
  //         dkpatch.appointmentId = result['id'];
  //         console.log(result);
  //         dkpatch.dotKhamId = this.id;
  //         var ar = [];
  //         for (var p in dkpatch) {
  //           var o = { op: 'replace', path: '/' + p, value: dkpatch[p] };
  //           ar.push(o);
  //         }

  //         this.dotKhamService.patch(this.id, ar).subscribe(
  //           rs => {
  //             this.getActiveRoute();
  //           }
  //         )
  //       } else {
  //         console.log(3);
  //         this.getActiveRoute();
  //       }
  //       this.notificationService.show({
  //         content: 'Cập nhật thành công',
  //         hideAfter: 3000,
  //         position: { horizontal: 'center', vertical: 'top' },
  //         animation: { type: 'fade', duration: 400 },
  //         type: { style: 'success', icon: true }
  //       });
  //     }
  //   });
  // }

  appointmentCreateModal() {
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    if (this.getAppointment) {
      modalRef.componentInstance.appointId = this.getAppointment.id;
    }
    modalRef.componentInstance.dotKhamId = this.id;
    modalRef.result.then(
      rs => {
        this.opened = false;
        if (rs.id) {
          var dkpatch = new DotKhamPatch;
          dkpatch.appointmentId = rs['id'];
          console.log(rs);
          dkpatch.dotKhamId = this.id;
          var ar = [];
          for (var p in dkpatch) {
            var o = { op: 'replace', path: '/' + p, value: dkpatch[p] };
            ar.push(o);
          }

          this.dotKhamService.patch(this.id, ar).subscribe(
            rs => {
              this.getActiveRoute();
            }
          )
        } else {
          this.getActiveRoute();
        }
        this.notificationService.show({
          content: 'Cập nhật thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });

      },
      er => { }
    )
  }

  updateAppointmentModal(id) {
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.appointId = id;
    modalRef.result.then(() => {
      this.notificationService.show({
        content: 'Cập nhật thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
    }, () => {
    });
  }

  get getAppointState() {
    if (this.getAppointment && this.getAppointment.state) {
      switch (this.getAppointment.state) {
        case 'confirmed':
          return { text: 'Đang hẹn', color: '#04c835' };
        case 'cancel':
          return { text: 'Đã hủy', color: '#cc0000' };
        case 'done':
          return { text: 'Đã tới', color: '#666666' };
        case 'waiting':
          return { text: 'Đang chờ', color: '#0080ff' };
        case 'expired':
          return { text: 'Quá hạn', color: '#ffbf00' };
      }
    }
  }

  //ĐÍNH KÈM FILE/HÌNH ẢNH
  addAttachments(e) {
    var file_node = e.target;
    var count = file_node.files.length;

    var formData = new FormData();

    console.log(file_node.files);
    for (let i = 0; i < count; i++) {
      var file = file_node.files[i];
      formData.append('files', file);
      var filereader = new FileReader();
      filereader.readAsDataURL(file);
      var self = this;
    }
    this.dotKhamService.uploadFiles(self.id, formData).subscribe(
      rs => {
        // this.getImageIds();
        rs.forEach(e => {
          if (this.getFileMineType(e.mineType) == 'image') {
            console.log(1);
            this.imagesPreview.push(e);
          } else {
            console.log(2);
            this.filesPreview.push(e);
          }
        })
      }
    )

  }

  //LOAD CÁC ATTACHMENT
  getImageIds() {
    this.imagesPreview = [];
    this.filesPreview = [];
    var ir = new IrAttachmentSearchRead;
    ir.model = 'dotkham';
    ir.resId = this.id;
    this.dotKhamService.getImageIds(ir).subscribe(
      rs => {
        rs.forEach(e => {
          // _.unionBy(this.imagesPreview, [src], 'id');
          if (this.getFileMineType(e.mineType) == 'image') {
            console.log(1);
            this.imagesPreview.push(e);
          } else {
            console.log(2);
            this.filesPreview.push(e);
          }
        });
      }
    )
  }

  stopPropagation(event) {
    event.stopPropagation();
  }

  //GET ID CỦA CÁC ATTACHMENT
  deleteAttachments(item, event) {
    event.stopPropagation();
    const dialogRef: DialogRef = this.dialogService.open({
      title: 'Xóa hình ảnh/đính kèm',
      content: 'Bạn chắc chắn muốn xóa ' + item.name + ' ?',
      actions: [
        { text: 'Thoát', value: false },
        { text: 'Đồng ý', primary: true, value: true }
      ]
    });
    dialogRef.result.subscribe(
      rs => {
        console.log(rs);
        if (!(rs instanceof DialogCloseResult)) {
          if (rs['value']) {
            this.dotKhamService.deleteAttachment(item.id).subscribe(
              () => {
                // this.getImageIds();
                if (this.getFileMineType(item.mineType) == 'image') {
                  var index = this.imagesPreview.findIndex(x => x.id == item.id);
                  if (index > -1) {
                    this.imagesPreview.splice(index, 1);
                  } else {
                    this.getImageIds();
                  }
                } else {
                  index = this.filesPreview.findIndex(x => x.id == item.id);
                  if (index > -1) {
                    this.filesPreview.splice(index, 1);
                  } else {
                    this.getImageIds();
                  }
                }
              }
            )
          }
        }
      }
    )
  }

  //LẤY ĐUÔI MỞ RỘNG CỦA FILE
  getFileExtension(name: string) {
    var type = name.substring(name.indexOf('.') + 1, name.length);
    if (type == 'jpg' || type == 'png' || type == 'jpeg' || type == 'bmp' || type == 'gif') {
      return 'image';
    } else {
      return 'file';
    }

  }

  getFileMineType(mine: string) {
    var type = mine.substring(0, mine.indexOf('/'));
    console.log(type);
    return type;
  }

  viewImage(attachment: IrAttachmentBasic) {
    var modalRef = this.modalService.open(ImageViewerComponent, { windowClass: 'o_image_viewer o_modal_fullscreen' });
    modalRef.componentInstance.attachments = this.imagesPreview;
    modalRef.componentInstance.attachmentSelected = attachment;
  }
  private propagateChange = (_: any) => { };

}
