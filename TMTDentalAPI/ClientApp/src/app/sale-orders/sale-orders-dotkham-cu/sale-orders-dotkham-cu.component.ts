import { IterableDiffer, IterableDiffers, KeyValueDiffer, KeyValueDiffers } from '@angular/core';
import { Component, DoCheck, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { validator } from 'fast-json-patch';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { WebService } from 'src/app/core/services/web.service';
import { DotKhamService } from 'src/app/dot-khams/dot-kham.service';
import { DotKhamDisplay } from 'src/app/dot-khams/dot-khams';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ImageViewerComponent } from 'src/app/shared/image-viewer/image-viewer.component';
import { DotKhamLineDisplay, DotkhamOdataService } from 'src/app/shared/services/dotkham-odata.service';
import { EmployeesOdataService } from 'src/app/shared/services/employeeOdata.service';
import { PartnerImageBasic } from 'src/app/shared/services/partners.service';
import { SaleOrdersOdataService } from 'src/app/shared/services/sale-ordersOdata.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-sale-orders-dotkham-cu',
  templateUrl: './sale-orders-dotkham-cu.component.html',
  styleUrls: ['./sale-orders-dotkham-cu.component.css']
})
export class SaleOrdersDotkhamCuComponent implements OnInit, DoCheck {

  @ViewChild('empCbx', { static: true }) empCbx: ComboBoxComponent;
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
  kvDiffer: KeyValueDiffer<string, any>;
  differ: IterableDiffer<any>;
  webImageApi: string;
  webContentApi: string;

  editModeActive = false;

  constructor(
    private webService: WebService,
    private fb: FormBuilder,
    private empService: EmployeesOdataService,
    private intelService: IntlService,
    private authService: AuthService,
    private dotkhamODataService: DotkhamOdataService,
    private notificationService: NotificationService,
    private differs: KeyValueDiffers,
    private iterableDiffers: IterableDiffers,
    private modalService: NgbModal,
    private saleOrdersOdataService: SaleOrdersOdataService,
    private dotKhamService: DotKhamService
  ) { }
  ngDoCheck(): void {
    const changes = this.differ.diff(this.dotkham.Lines);
    if (changes) {
      // console.log('1');
      // changes.forEachOperation((record, previousIndex, currentIndex) => {
      //   var item = record.item;
      //   const linesFA = this.dotkhamForm.get('Lines') as FormArray;
      //   var g = this.fb.group({
      //     NameStep: null,
      //     ProductId: null,
      //     Product: null,
      //     Note: null,
      //     Teeth: this.fb.array([]),
      //     SaleOrderLineId: null,
      //   });

      //   g.patchValue(item);
      //   linesFA.push(g);
      // });
      // const linesFA = this.dotkhamForm.get('Lines') as FormArray;
      // linesFA.clear();
      // this.dotkham.Lines.forEach(line => {
      //   const lineFG = this.fb.group({
      //     NameStep: null,
      //     DotKhamId: null,
      //     ProductId: null,
      //     Product: null,
      //     Sequence: null,
      //     State: null,
      //     ToothIds: [],
      //     Note: null,
      //     Teeth: this.fb.array([]),
      //     SaleOrderLineId: null,
      //     SaleOrderLine: null
      //   });
      //   lineFG.patchValue(line);
      //   line.Teeth.forEach(t => {
      //     const teethFG = this.fb.group(t);
      //     (lineFG.get('Teeth') as FormArray).push(teethFG);
      //   });
      //   linesFA.push(lineFG);
      // });
    }
  }

  ngOnInit() {
    this.webImageApi = environment.uploadDomain + 'api/Web/Image';
    this.webContentApi = environment.uploadDomain + 'api/Web/Content';
    this.kvDiffer = this.differs.find(this.dotkham).create();
    this.differ = this.iterableDiffers.find(this.dotkham.Lines).create();

    this.dotkhamForm = this.fb.group({
      Date: [new Date(), Validators.required],
      Reason: [null],
      Doctor: [null, Validators.required],
      Lines: this.fb.array([]),
      DotKhamImages: this.fb.array([]),
    });

    this.loadRecord();

    this.loadEmployees();
    this.empCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.empCbx.loading = true)),
      switchMap(value => this.searchEmp(value))
    ).subscribe((result: any) => {
      this.empList = result.data;
      this.empCbx.loading = false;
    });
  }

  setEditModeActive(val: boolean) {
    this.editModeActive = val;
  }

  get Id() { return this.dotkhamForm.get('Id').value; }
  get Sequence() { return this.dotkhamForm.get('Sequence').value; }
  get imgsFA() { return this.dotkhamForm.get('DotKhamImages') as FormArray; }
  get linesFA() { return this.dotkhamForm.get('Lines') as FormArray; }
  get dotkhamDate() { return this.dotkhamForm.get('Date').value; }
  get employee() { return this.dotkhamForm.get('Doctor').value; }
  get reason() { return this.dotkhamForm.get('Reason').value; }

  stopPropagation(e) {
    e.stopPropagation();
  }

  loadRecord() {
    this.dotkhamForm.get('Date').setValue(new Date(this.dotkham.Date));
    this.dotkhamForm.get('Doctor').setValue(this.dotkham.Doctor);
    this.dotkhamForm.get('Reason').setValue(this.dotkham.Reason);
    this.imgsFA.clear();
    this.linesFA.clear();

    this.dotkham.DotKhamImages.forEach(e => {
      const imgFG = this.fb.group(e);
      this.imgsFA.push(imgFG);
    });

    this.dotkham.Lines.forEach(e => {
      const lineFG = this.fb.group({
        Id: null,
        NameStep: null,
        DotKhamId: null,
        ProductId: null,
        Product: null,
        Sequence: null,
        State: null,
        ToothIds: [],
        Note: null,
        Teeth: this.fb.array([]),
        SaleOrderLineId: null,
        SaleOrderLine: null
      });

      lineFG.patchValue(e);

      e.Teeth.forEach(t => {
        const teethFG = this.fb.group(t);
        (lineFG.get('Teeth') as FormArray).push(teethFG);
      });
      this.linesFA.push(lineFG);
    });

    // this.dotkhamForm.patchValue(this.dotkham);
  }

  searchEmp(val) {
    const state = {
      take: 20,
      filter: {
        logic: 'and',
        filters: [
          { field: 'Name', operator: 'contains', value: val || '' },
          { field: 'IsDoctor', operator: 'eq', value: true },
          { field: 'Active', operator: 'eq', value: true }
        ]
      }
    };
    const options = {
      select: 'Id,Name'
    };
    return this.empService.getFetch(state, options);
  }

  loadEmployees() {
    this.searchEmp('').subscribe(
      (result: any) => {
        this.empList = result.data;
        if (this.dotkham.Doctor) {
          this.empList = _.unionBy(this.empList, [this.dotkham.Doctor], 'Id');
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
        const imgObj = new PartnerImageBasic();
        imgObj.DotKhamId = this.dotkham.Id;
        imgObj.Name = img.fileName;
        imgObj.UploadId = img.fileUrl;
        imgObj.PartnerId = this.dotkham.PartnerId;
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
      this.dotKhamService.delete(this.dotkham.Id).subscribe(() => {
        this.btnDeleteEvent.emit(this.dotkham);
      }, (err) => {
        console.log(err);
      });
    }, () => { });
  }

  onSave() {
    if (this.dotkhamForm.invalid) {
      return;
    }
    const val = this.dotkhamForm.value;
    val.Date = this.intelService.formatDate(val.Date, 'yyyy-MM-dd');
    val.DoctorId = val.Doctor ? val.Doctor.Id : null;
    // val.CompanyId = this.authService.userInfo.companyId;
    val.Lines.forEach(line => {
      line.ToothIds = line.Teeth.map(x => x.Id);
    });

    if (!this.dotkham.Id) {
      delete val['Id'];
      val.Sequence = this.dotkham.Sequence;
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
  }

  onClose() {
    this.onEmitDotkham(null, false, null);
  }

  checkAccess() {
    if (this.activeDotkham && this.activeDotkham.Sequence !== this.dotkham.Sequence) {
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
    const teeth = line.get('Teeth').value;
    return teeth.map(x => x.Name).join(', ');
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
    lineControl.get('Teeth').clear();
    line.Teeth.forEach(t => {
      const g = this.fb.group(t);
      lineControl.get('Teeth').push(g);
    });
    this.dotkham.Lines[i] = line;
    // lineControl.get('ToothIds').clear();
    // line.Teeth.forEach(t => {
    //   const g = this.fb.group(t.Id);
    //   lineControl.get('ToothIds').push(g);
    // });
  }

  onViewImg(imgObj: PartnerImageBasic) {
    const modalRef = this.modalService.open(ImageViewerComponent, { windowClass: 'o_image_viewer o_modal_fullscreen' });
    const img = {
      id: imgObj.Id,
      name: imgObj.Name,
      date: imgObj.Date,
      note: imgObj.Note,
      uploadId: imgObj.UploadId
    };
    const imgs = this.imgsFA.value.map(x => {
      return {
        id: x.Id,
        name: x.Name,
        date: x.Date,
        note: x.Note,
        uploadId: x.UploadId
      };
    });
    modalRef.componentInstance.partnerImages = imgs;
    modalRef.componentInstance.partnerImageSelected = img;
  }
}
