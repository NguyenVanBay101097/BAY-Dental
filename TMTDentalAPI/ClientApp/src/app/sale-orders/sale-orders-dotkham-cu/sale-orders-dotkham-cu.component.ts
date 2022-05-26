import { Component, EventEmitter, Input, IterableDiffer, IterableDiffers, KeyValueDiffer, KeyValueDiffers, OnInit, Output, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { WebService } from 'src/app/core/services/web.service';
import { DotKhamService } from 'src/app/dot-khams/dot-kham.service';
import { EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ImageViewerComponent } from 'src/app/shared/image-viewer/image-viewer.component';
import { PartnerImageBasic } from 'src/app/shared/services/partners.service';
import { IrAttachmentBasic } from 'src/app/shared/shared';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-sale-orders-dotkham-cu',
  templateUrl: './sale-orders-dotkham-cu.component.html',
  styleUrls: ['./sale-orders-dotkham-cu.component.css']
})
export class SaleOrdersDotkhamCuComponent implements OnInit {

  @ViewChild('empCbx', { static: true }) empCbx: ComboBoxComponent;
  @ViewChild('assCbx', { static: true }) assCbx: ComboBoxComponent;
  @Input() dotkham: any;
  @Output() dotkhamChange = new EventEmitter<any>();
  @Input() activeDotkham: any;
  @Input() index: number = 0;
  @Input() sequence: number = 0;

  @Output() btnSaveEvent = new EventEmitter<any>();
  @Output() btnCancelEvent = new EventEmitter<any>();
  @Output() btnEditEvent = new EventEmitter<any>();
  @Output() btnDeleteEvent = new EventEmitter<any>();

  dotkhamForm: FormGroup;
  empList: any[];
  assList: any[];
  kvDiffer: KeyValueDiffer<string, any>;
  differ: IterableDiffer<any>;
  webImageApi: string;
  webContentApi: string;
  submitted = false;
  editModeActive = false;

  constructor(
    private webService: WebService,
    private fb: FormBuilder,
    private empService: EmployeeService,
    private intelService: IntlService,
    private notificationService: NotificationService,
    private differs: KeyValueDiffers,
    private iterableDiffers: IterableDiffers,
    private modalService: NgbModal,
    private dotKhamService: DotKhamService,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.webImageApi = environment.uploadDomain + 'api/Web/Image';
    this.webContentApi = environment.uploadDomain + 'api/Web/Content';
    this.kvDiffer = this.differs.find(this.dotkham).create();
    this.differ = this.iterableDiffers.find(this.dotkham.lines).create();

    this.dotkhamForm = this.fb.group({
      date: [new Date(), Validators.required],
      reason: [null],
      doctor: [null, Validators.required],
      lines: this.fb.array([]),
      irAttachments: this.fb.array([]),
      sequence: this.sequence,
      assistant: null
    });

    this.loadRecord();

    this.loadEmployees();
    this.empCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.empCbx.loading = true)),
      switchMap(value => this.searchEmp(value))
    ).subscribe((result: any) => {
      this.empList = result.items;
      this.empCbx.loading = false;
    });

    this.assCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.assCbx.loading = true)),
      switchMap(value => this.searchEmp(value))
    ).subscribe((result: any) => {
      this.assList = result.items;
      this.assCbx.loading = false;
    });
  }

  setEditModeActive(val: boolean) {
    this.editModeActive = val;
  }
  get f() { return this.dotkhamForm.controls; }
  get Id() { return this.dotkhamForm.get('id').value; }
  get Sequence() { return this.dotkhamForm.get('sequence').value; }
  get imgsFA() { return this.dotkhamForm.get('irAttachments') as FormArray; }
  get linesFA() { return this.dotkhamForm.get('lines') as FormArray; }
  get dotkhamDate() { return this.dotkhamForm.get('date').value; }
  get employee() { return this.dotkhamForm.get('doctor').value; }
  get assistant() { return this.dotkhamForm.get('assistant').value; }
  get reason() { return this.dotkhamForm.get('reason').value; }

  stopPropagation(e) {
    e.stopPropagation();
  }

  loadRecord() {
    this.dotkham.date = new Date(this.dotkham.date);
    this.dotkhamForm.patchValue(this.dotkham);
    this.imgsFA.clear();
    this.linesFA.clear();

    this.dotkham.irAttachments.forEach(e => {
      const imgFG = this.fb.group(e);
      this.imgsFA.push(imgFG);
    });

    this.dotkham.lines.forEach(e => {
      const lineFG = this.fb.group({
        id: null,
        nameStep: null,
        dotKhamId: null,
        productId: null,
        product: null,
        sequence: null,
        state: null,
        toothIds: [],
        note: null,
        teeth: this.fb.array([]),
        saleOrderLineId: null,
        saleOrderLine: null
      });

      lineFG.patchValue(e);

      e.teeth.forEach(t => {
        const teethFG = this.fb.group(t);
        (lineFG.get('teeth') as FormArray).push(teethFG);
      });
      this.linesFA.push(lineFG);
    });

    // this.dotkhamForm.patchValue(this.dotkham);
  }

  searchEmp(val) {
    return this.empService.getEmployeePaged(<EmployeePaged>{
      search: val || '',
      isDoctor: true,
      active: true,
      companyId: this.authService.userInfo.companyId
    });
  }

  loadEmployees() {
    this.searchEmp('').subscribe(
      (result: any) => {
        this.empList = result.items;
        this.assList = result.items;
        if (this.dotkham.doctor) {
          this.empList = _.unionBy(this.empList, [this.dotkham.doctor], 'id');
        }

        if (this.dotkham.assistant) {
          this.assList = _.unionBy(this.assList, [this.dotkham.assistant], 'id');
        }
      }
    );
  }
  onFileChange(e) {
    const file_node = e.target;
    const formData = new FormData();
    for (var i = 0; i < file_node.files.length; i++) {
      formData.append('files', file_node.files[i]);
    }

    this.webService.uploadImages(formData).subscribe((res: any) => {
      res.forEach(img => {
        const imgObj = new IrAttachmentBasic();
        imgObj.name = img.fileName;
        imgObj.url = img.fileUrl;
        const imgFG = this.fb.group(imgObj);
        this.imgsFA.push(imgFG);
      });
    });
  }

  onRemoveImg(i) {
    this.imgsFA.removeAt(i);
  }

  onEditDotkham() {
    // if (!this.checkAccess()) {
    //   return;
    // }

    // this.editModeActive = true;
    this.btnEditEvent.emit(null);
    // this.onEmitDotkham(this.dotkham, false, this.dotkham);
  }

  onDeleteDotkham() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, {
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Xóa đợt khám " + this.sequence;
    modalRef.componentInstance.body = "Bạn có chắc chắn xóa đợt khám?";

    modalRef.result.then(() => {
      this.dotKhamService.delete(this.dotkham.id).subscribe(() => {
        this.btnDeleteEvent.emit(this.dotkham);
        this.notificationService.show({
          content: 'Xóa thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      }, (err) => {
        console.log(err);
      });
    }, () => { });
  }

  onSave() {
    this.submitted = true;
    if (this.dotkhamForm.invalid) {
      return;
    }
    const val = this.dotkhamForm.value;
    val.date = this.intelService.formatDate(val.date, 'yyyy-MM-dd');
    val.doctorId = val.doctor ? val.doctor.id : null;
    val.assistantId = val.assistant ? val.assistant.id : null;
    // val.CompanyId = this.authService.userInfo.companyId;
    val.lines.forEach(line => {
      line.toothIds = line.teeth.map(x => x.id);
    });

    if (!this.dotkham.id) {
      delete val['id'];
      val.sequence = this.dotkham.sequence;
      // val.SaleOrderId = this.dotkham.SaleOrderId;
      // this.saleOrdersOdataService.createDotkham(this.dotkham.SaleOrderId, val).subscribe((res: any) => {
      //   this.notify('success', 'Lưu thành công');
      //   this.editModeActive = false;
      //   this.btnSaveEvent.emit(res);
      // });
      this.btnSaveEvent.emit(val);
    } else {
      // this.dotkhamODataService.update(this.Id, val).subscribe((res: any) => {
      //   this.notify('success', 'Lưu thành công');
      //   this.dotkhamODataService.getInfo(this.Id).subscribe((res2: any) => {
      //     this.dotkham = res2;
      //     this.onEmitDotkham(this.dotkham, false, null);
      //   });
      // });
      this.btnSaveEvent.emit(val);
    }

    // if (!this.Id) {
    //   this.dotkhamODataService.create(val).subscribe((res: any) => {
    //     this.notify('success', 'Lưu thành công');
    //     this.dotkhamODataService.getInfo(res.Id).subscribe((res2: any) => {
    //       this.dotkham = res2;
    //       this.onEmitDotkham(this.dotkham, false, null);
    //       this.loadRecord();
    //     });
    //   });
    // } else {
    //   this.dotkhamODataService.update(this.Id, val).subscribe((res: any) => {
    //     this.notify('success', 'Lưu thành công');
    //     this.dotkhamODataService.getInfo(this.Id).subscribe((res2: any) => {
    //       this.dotkham = res2;
    //       this.onEmitDotkham(this.dotkham, false, null);
    //     });
    //   });
    // }
  }

  onCancel() {
    // this.onEmitDotkham(this.dotkham, true, null);
    this.btnCancelEvent.emit(null);
    this.submitted = false;
  }

  onClose() {
    this.onEmitDotkham(null, false, null);
  }

  checkAccess() {
    if (this.activeDotkham && this.activeDotkham.sequence !== this.dotkham.sequence) {
      this.notify('error', 'Bạn phải hoàn tất đợt khám đang thao tác');
      return false;
    }
    return true;
  }

  notify(Style, Content) {
    this.notificationService.show({
      content: Content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: Style, icon: true }
    });
  }

  showLineTeeth(line: FormGroup) {
    const teeth = line.get('teeth').value;
    return teeth.map(x => x.name).join(', ');
  }

  onEmitDotkham(dotkham, isDelete = false, activeDotkham) {
    const e = {
      dotkham,
      isDelete,
      activeDotkham
    };
    this.dotkhamChange.emit(e);
  }

  onRemoveLine(i) {
    this.linesFA.removeAt(i);
  }

  eventTeeth(line, lineControl, i) {
    lineControl.patchValue(line);
    lineControl.get('teeth').clear();
    line.teeth.forEach(t => {
      const g = this.fb.group(t);
      lineControl.get('teeth').push(g);
    });
    this.dotkham.lines[i] = line;
    // lineControl.get('ToothIds').clear();
    // line.Teeth.forEach(t => {
    //   const g = this.fb.group(t.Id);
    //   lineControl.get('ToothIds').push(g);
    // });
  }

  onViewImg(img: PartnerImageBasic) {
    const modalRef = this.modalService.open(ImageViewerComponent, { windowClass: 'o_image_viewer o_modal_fullscreen' });
    const imgs = this.imgsFA.value;
    modalRef.componentInstance.images = imgs;
    modalRef.componentInstance.selectedImage = img;
  }
}
