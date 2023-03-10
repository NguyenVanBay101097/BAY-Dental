import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Operation } from 'fast-json-patch';
import * as _ from 'lodash';
import { Observable } from 'rxjs';
import { debounceTime, map, switchMap, tap } from 'rxjs/operators';
import { AppointmentBasic } from 'src/app/appointment/appointment';
import { DotKhamStepAssignDotKhamVM, DotKhamStepService, DotKhamStepSetDone } from 'src/app/dot-khams/dot-kham-step.service';
import { DotKhamService } from 'src/app/dot-khams/dot-kham.service';
import { DotKhamDefaultGet, DotKhamDisplay, DotKhamPatch, DotKhamStepDisplay, DotKhamStepSave } from 'src/app/dot-khams/dot-khams';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { LaboOrderLineCuDialogComponent } from 'src/app/labo-order-lines/labo-order-line-cu-dialog/labo-order-line-cu-dialog.component';
import { LaboOrderLineBasic } from 'src/app/labo-order-lines/labo-order-line.service';
import { LaboOrderBasic } from 'src/app/labo-orders/labo-order.service';
import { PartnerPaged, PartnerSimple } from 'src/app/partners/partner-simple';
import { PartnerFilter, PartnerImageBasic, PartnerImageViewModel, PartnerService } from 'src/app/partners/partner.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { SaleOrderCreateDotKhamDialogComponent } from 'src/app/sale-orders/sale-order-create-dot-kham-dialog/sale-order-create-dot-kham-dialog.component';
import { AppointmentCreateUpdateComponent } from 'src/app/shared/appointment-create-update/appointment-create-update.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ImageViewerComponent } from 'src/app/shared/image-viewer/image-viewer.component';
import { ToaThuocCuDialogSaveComponent } from 'src/app/shared/toa-thuoc-cu-dialog-save/toa-thuoc-cu-dialog-save.component';
import { ToaThuocPrintComponent } from 'src/app/shared/toa-thuoc-print/toa-thuoc-print.component';
import { ToaThuocBasic, ToaThuocPrint, ToaThuocService } from 'src/app/toa-thuocs/toa-thuoc.service';
import { UserSimple } from 'src/app/users/user-simple';
import { UserPaged, UserService } from 'src/app/users/user.service';
import { environment } from 'src/environments/environment';
declare var $: any;

@Component({
  selector: 'app-dot-kham-create-update-dialog',
  templateUrl: './dot-kham-create-update-dialog.component.html',
  styleUrls: ['./dot-kham-create-update-dialog.component.css']
})
export class DotKhamCreateUpdateDialogComponent implements OnInit {

  id: string;//id ?????t kh??m
  invoiceId: string;//id h??a ????n
  invoiceState: string;//Tr???ng th??i c???a h??a ????n
  userId: string;
  partner: any;


  dotKhamForm: FormGroup;
  filteredUsers: UserSimple[];

  //D???u check cho nh???ng d???ch v??? ???? xong
  // doneList: DotKhamStepDisplay[][] = [];
  // @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('invoiceCbx', { static: true }) invoiceCbx: ComboBoxComponent;
  @ViewChild('doctorCbx', { static: true }) doctorCbx: ComboBoxComponent;
  @ViewChild('assistantCbx', { static: true }) assistantCbx: ComboBoxComponent;
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;
  @ViewChild('assistantUserCbx', { static: true }) assistantUserCbx: ComboBoxComponent;
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
  partnerId: string;
  isChange = false;
  customerSimpleFilter: PartnerSimple[] = [];
  doctorSimpleFilter: EmployeeSimple[] = [];
  imageViewModels: PartnerImageViewModel[] = [];
  assistantSimpleFilter: EmployeeSimple[] = [];
  userSimpleFilter: UserSimple[] = [];
  assistantUserSimpleFilter: UserSimple[] = [];
  productSimpleList: ProductSimple[] = [];
  productSimpleFilteredList: ProductSimple[] = [];
  skip: number = 0;
  limit: number = 20;

  imagesPreview: PartnerImageBasic[] = [];

  dialog = false;//Component ???????c m??? d?????i d???ng Dialog hay Tab m???i
  dotKham: DotKhamDisplay = new DotKhamDisplay();

  webImageApi: string;
  webContentApi: string;

  editingStep: DotKhamStepDisplay;

  title: string;
  activeTabId = 1;

  @ViewChild(ToaThuocPrintComponent, { static: true }) toaThuocPrintComponent: ToaThuocPrintComponent;


  constructor(
    private fb: FormBuilder,
    private dotKhamService: DotKhamService,
    private intlService: IntlService,
    private partnerService: PartnerService,
    private userService: UserService,
    private notificationService: NotificationService,
    private router: Router,
    private toaThuocService: ToaThuocService,
    private employeeService: EmployeeService,
    private modalService: NgbModal,
    private dotKhamStepService: DotKhamStepService,
    public activeModal: NgbActiveModal,
  ) { }


  ngOnInit() {
    this.dotKhamForm = this.fb.group({
      name: null,
      dateObj: null,
      date: null,
      note: null,
      companyId: null,
      doctor: [null, Validators.required],
      assistant: null,
      state: null,
      saleOrderId: null,
      step: null,
      product: null,
      appointment: null,
      appointmentId: null,
      filter: "dotkham"
    });

    setTimeout(() => {
      this.getDoctorList();
      this.getAssistantList();

      if (this.id) {
        this.id = this.id;
        this.loadData();
      } else {
        this.loadDefaultFormGroup();
      }

      this.filterChangeCombobox();
    });

    this.webImageApi = environment.uploadDomain + 'api/Web/Image';
    this.webContentApi = environment.uploadDomain + 'api/Web/Content';
  }

  setEditingStep(step: DotKhamStepDisplay) {
    this.editingStep = step;
  }

  loadRecordUpdateFormGroup() {
    this.dotKhamService.get(this.id).subscribe(result => {
      this.updateFormGroup(result);
    });
  }

  loadDefaultFormGroup() {
    var defaultVal = new DotKhamDefaultGet();
    this.dotKhamService.defaultGet(defaultVal).subscribe(result => {
      this.updateFormGroup(result);
    });
  }

  updateFormGroup(result) {
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
  }

  updateStepName(event, step: DotKhamStepDisplay) {
    var name = event.target.value;
    if (!name) {
      alert('T??n c??ng ??o???n kh??ng ???????c tr???ng');
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

  loadLaboOrders() {
    if (this.id) {
      this.dotKhamService.getLaboOrders(this.id).subscribe(result => {
        this.laboOrders = result;
      })
    }
  }

  loadDotKhamSteps() {
    if (this.id) {
      this.dotKhamService.getDotKhamStepsByDKId2(this.id, this.getDotKhamFilter).subscribe(result => {
        this.dotKhamStepsList = result;
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

  deleteStep(step: DotKhamStepDisplay, index) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'X??a c??ng ??o???n';
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
      // let modalRef = this.modalService.open(LaboOrderLineCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
      // modalRef.componentInstance.title = 'T???o labo';
      // modalRef.componentInstance.dotKhamId = this.id;

      // modalRef.result.then(() => {
      //   this.loadLaboOrders();
      // }, () => {
      // });
    }
  }

  printToaThuoc(item: ToaThuocBasic) {
    this.toaThuocService.getPrint(item.id).subscribe((result: any) => {
      this.toaThuocPrintComponent.print(result.html);
    });
  }

  // editDotKhamLine(item: DotKhamLineBasic) {
  //   const windowRef = this.windowService.open({
  //     title: 'Ti???n tr??nh d???ch v???',
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
    let modalRef = this.modalService.open(ToaThuocCuDialogSaveComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'S???a ????n thu???c';
    modalRef.componentInstance.id = item.id;

    modalRef.result.then((result: any) => {
      this.loadToaThuocs();
      if (result.print) {
        this.printToaThuoc(item);
      }
    }, () => {
    });
  }

  // editAppointment(item: AppointmentBasic) {
  //   const windowRef = this.windowService.open({
  //     title: 'S???a cu???c h???n',
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
    let modalRef = this.modalService.open(LaboOrderLineCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.title = 'S???a labo';
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
      let modalRef = this.modalService.open(ToaThuocCuDialogSaveComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Th??m ????n thu???c';
      modalRef.componentInstance.defaultVal = { dotKhamId: this.id };

      modalRef.result.then(result => {
        this.loadToaThuocs();
        if (result.print) {
          this.printToaThuoc(result.item);
        }
      }, () => {
      });
    }
  }

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

  searchUsers(filter: string) {
    var val = new UserPaged();
    val.search = filter;
    return this.userService.autocompleteSimple(val);
  }

  searchCustomers(search) {
    var partnerPaged = new PartnerPaged();
    partnerPaged.employee = false;
    partnerPaged.customer = true;
    partnerPaged.supplier = false;
    partnerPaged.limit = this.limit;
    partnerPaged.offset = this.skip;
    if (search != null) {
      partnerPaged.search = search.toLowerCase();
    }
    return this.partnerService.autocompletePartner(partnerPaged);
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

    this.dotKhamService.create(val).subscribe(result => {
      this.activeModal.close(result);
    });
  }

  onUpdate() {
    if (!this.dotKhamForm.valid) {
      return;
    }

    if (this.id) {
      var val = this.dotKhamForm.value;
      var data = this.prepareData();
      data.userId = val.user ? val.user.id : data.userId;
      data.assistantUserId = val.assistantUser ? val.assistantUser.id : null;
      this.dotKhamService.update(this.id, data).subscribe(() => {
        this.notificationService.show({
          content: 'L??u thay ?????i th??nh c??ng',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadData();
        if (this.id)
          this.activeModal.close();
      });
    }
  }

  onConfirm() {
    //n???u form dirty th?? c???n update r??i sau ???? confirm
    if (this.id) {
      if (this.dotKhamForm.dirty) {
        var data = this.prepareData();
        this.dotKhamService.update(this.id, data).subscribe(() => {
          this.dotKhamService.actionConfirm(this.id).subscribe(() => {
            this.activeModal.close();
          })
        });
      } else {
        this.dotKhamService.actionConfirm(this.id).subscribe(() => {
          this.activeModal.close();
        })
      }
    }
  }

  loadData() {
    if (this.id) {
      this.loadRecordUpdateFormGroup();
      this.loadToaThuocs();
      this.loadDotKhamStepList();
      this.getImageIds();
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
  //     this.loadData();
  //   });
  // }

  // markOperationDone(item: any) {
  //   this.dotKhamLineOperationService.markDone(item.id).subscribe(() => {
  //     this.loadData();
  //   });
  // }

  getStateLine(state) {
    switch (state) {
      case 'done':
        return '???? xong';
      case 'progress':
        return '??ang ti???n h??nh';
      default:
        return 'Ch??a ti???n h??nh'
    }
  }

  getInvoiceLineName(id: string) {

  }

  // getStateOperation(state) {
  //   switch (state) {
  //     case 'done':
  //       return 'Ho??n th??nh';
  //     default:
  //       return 'Ch??a ho??n th??nh'
  //   }
  // }


  //===============B??? SUNG=======================

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
    empPaged.isDoctor = true;
    empPaged.limit = this.limit;
    empPaged.offset = this.skip;
    if (search != null) {
      empPaged.search = search.toLowerCase();
    }
    return this.employeeService.getEmployeeSimpleList(empPaged);
  }

  checkState(id, state) {
    var save = new DotKhamStepSave();
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
    empPn.isDoctor = true;
    empPn.limit = this.limit;
    empPn.offset = this.skip;
    this.employeeService.getEmployeeSimpleList(empPn).subscribe(
      rs => {
        this.assistantSimpleFilter = rs;
      });
  }
  getUserList() {
    var userlst = new UserPaged;
    userlst.limit = this.limit;
    userlst.offset = this.skip;
    this.userService.autocompleteSimple(userlst).subscribe(
      rs => {
        this.userSimpleFilter = rs;
        this.assistantUserSimpleFilter = rs;
      });
  }

  filterProductSimple(e) {
    this.productSimpleFilteredList = this.productSimpleList.filter((s) => s.name.toLowerCase().indexOf(e.toLowerCase()) !== -1);
  }

  actionCreateDotKham() {
    if (this.dotKham && this.dotKham.saleOrderId) {
      let modalRef = this.modalService.open(SaleOrderCreateDotKhamDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'T???o ?????t kh??m';
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

  appointmentCreateModal() {
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    if (this.getAppointment)
      modalRef.componentInstance.appointId = this.getAppointment.id;

    modalRef.componentInstance.dotKhamId = this.id;
    modalRef.result.then(
      rs => {
        this.opened = false;
        if (rs.id) {
          var dkpatch = new DotKhamPatch();
          dkpatch.appointmentId = rs['id'];
          dkpatch.dotKhamId = this.id;
          var ar = [];
          for (var p in dkpatch) {
            var o = { op: 'replace', path: '/' + p, value: dkpatch[p] };
            ar.push(o);
          }

          this.dotKhamService.patch(this.id, ar).subscribe(
            rs => {
              this.loadData()
            }
          )
        } else {
          this.loadDefaultFormGroup();
        }
        this.notificationService.show({
          content: 'C???p nh???t th??nh c??ng',
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
    modalRef.result.then(rs => {
      var dkpatch = new DotKhamPatch;
      dkpatch.appointmentId = rs['id'];
      dkpatch.dotKhamId = this.id;
      var ar = [];
      for (var p in dkpatch) {
        var o = { op: 'replace', path: '/' + p, value: dkpatch[p] };
        ar.push(o);
      }

      this.dotKhamService.patch(this.id, ar).subscribe(
        rs => {
          this.loadData()
        }
      )

      this.notificationService.show({
        content: 'C???p nh???t th??nh c??ng',
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
          return { text: '??ang h???n', color: '#04c835' };
        case 'cancel':
          return { text: '???? h???y', color: '#cc0000' };
        case 'done':
          return { text: '???? t???i', color: '#666666' };
        case 'waiting':
          return { text: '??ang ch???', color: '#0080ff' };
        case 'expired':
          return { text: 'Qu?? h???n', color: '#ffbf00' };
      }
    }
  }

  //????NH K??M FILE/H??NH ???NH
  addPartnerImages(e) {
    var file_node = e.target;
    var count = file_node.files.length;
    var formData = new FormData();
    formData.append('partnerId', this.partnerId);
    formData.append('dotKhamId', this.id);
    for (let i = 0; i < count; i++) {
      var file = file_node.files[i];
      formData.append('files', file);
      var filereader = new FileReader();
      filereader.readAsDataURL(file);
    }
    this.partnerService.uploadPartnerImage(formData).subscribe(
      rs => {
        this.getImageIds();
      })

  }

  deleteAttachments(index, event) {
    event.stopPropagation();
    var item = this.imagesPreview[index];
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'X??a h??nh ???nh ' + item.name;

    modalRef.result.then(() => {
      this.partnerService.deleteParnerImage(item.id).subscribe(
        () => {
          this.imagesPreview.splice(index, 1);
        })
    })
  }

  getImageIds() {
    this.imagesPreview = [];
    var value = {
      dotKhamId: this.id
    }

    this.partnerService.getPartnerImageIds(value).subscribe(
      rs => {
        rs.forEach(e => {
          this.imagesPreview.push(e);
        });
      }
    )
  }

  stopPropagation(event) {
    event.stopPropagation();
  }

  //L???Y ??U??I M??? R???NG C???A FILE
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
    return type;
  }

  viewImage(partnerImage: PartnerImageBasic) {
    var modalRef = this.modalService.open(ImageViewerComponent, { windowClass: 'o_image_viewer o_modal_fullscreen' });
    modalRef.componentInstance.partnerImages = this.imageViewModels;
    modalRef.componentInstance.partnerImageSelected = partnerImage;
  }
  // private propagateChange = (_: any) => { };
}
