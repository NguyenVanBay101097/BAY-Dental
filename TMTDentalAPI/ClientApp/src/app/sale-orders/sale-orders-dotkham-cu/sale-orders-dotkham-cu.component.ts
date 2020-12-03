import { IterableDiffer, IterableDiffers, KeyValueDiffer, KeyValueDiffers } from '@angular/core';
import { Component, DoCheck, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { validator } from 'fast-json-patch';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { WebService } from 'src/app/core/services/web.service';
import { DotKhamDisplay } from 'src/app/dot-khams/dot-khams';
import { DotKhamLineDisplay, DotkhamOdataService } from 'src/app/shared/services/dotkham-odata.service';
import { EmployeesOdataService } from 'src/app/shared/services/employeeOdata.service';
import { PartnerImageBasic } from 'src/app/shared/services/partners.service';

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

  dotkhamForm: FormGroup;
  empList: any[];
  kvDiffer: KeyValueDiffer<string, any>;
  differ: IterableDiffer<any>;

  constructor(
    private webService: WebService,
    private fb: FormBuilder,
    private empService: EmployeesOdataService,
    private intelService: IntlService,
    private authService: AuthService,
    private dotkhamService: DotkhamOdataService,
    private notificationService: NotificationService,
    private differs: KeyValueDiffers,
    private iterableDiffers: IterableDiffers
  ) { }
  ngDoCheck(): void {
    const changes = this.differ.diff(this.dotkham.Lines);
    if (changes) {
      this.loadRecord();
    }
  }

  ngOnInit() {
    this.kvDiffer = this.differs.find(this.dotkham).create();
    this.differ = this.iterableDiffers.find(this.dotkham.Lines).create();

    this.dotkhamForm = this.fb.group({
      Id: null,
      Name: [null, Validators.required],
      SaleOrderId: [null],
      PartnerId: null,
      Date: [new Date(), Validators.required],
      UserId: [null],
      Reason: [null],
      State: 'draft',
      CompanyId: null,
      DoctorId: null,
      Doctor: null,
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

  get Id() { return this.dotkhamForm.get('Id').value; }
  get Name() { return this.dotkhamForm.get('Name').value; }
  get imgsFA() { return this.dotkhamForm.get('DotKhamImages') as FormArray; }
  get linesFA() { return this.dotkhamForm.get('Lines') as FormArray; }
  get dotkhamDate() { return this.dotkhamForm.get('Date').value; }
  get employee() { return this.dotkhamForm.get('Doctor').value; }
  get reason() { return this.dotkhamForm.get('Reason').value; }

  loadRecord() {
    if (this.dotkham) {
      this.dotkham.Date = new Date(this.dotkham.Date);
      this.imgsFA.clear();
      this.linesFA.clear();
      this.dotkham.DotKhamImages.forEach(e => {
        const imgFG = this.fb.group(e);
        this.imgsFA.push(imgFG);
      });
      this.dotkham.Lines.forEach(e => {
        e.ToothIds = e.Teeth.map(x => x.Id);
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
        this.linesFA.push(lineFG);
      });
      this.dotkhamForm.patchValue(this.dotkham);
    }
  }

  searchEmp(val) {
    const state = {
      take: 20,
      filter: {
        logic: 'and',
        filters: [
          { field: 'Name', operator: 'contains', value: val || '' }
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
        this.empList = _.unionBy(this.empList, [this.dotkham.Doctor], 'Id');
      }
    );
  }
  onFileChange(e) {
    const file_node = e.target;
    const file = file_node.files[0];
    const formData = new FormData();
    formData.append('file', file);

    this.webService.uploadImage(formData).subscribe((res: any) => {
      const imgObj = new PartnerImageBasic();
      imgObj.DotKhamId = this.dotkham.Id;
      imgObj.Name = res.fileName;
      imgObj.UploadId = res.fileUrl;
      imgObj.PartnerId = this.dotkham.PartnerId;
      const imgFG = this.fb.group(imgObj);
      this.imgsFA.push(imgFG);
    });
  }

  onRemoveImg(i) {
    this.imgsFA.removeAt(i);
  }

  onEditDotkham() {
    if (!this.checkAccess()) {
      return;
    }

    this.onEmitDotkham(this.dotkham, false, this.dotkham);
  }

  onSave() {
    if (this.dotkhamForm.invalid) {
      return;
    }
    const val = this.dotkhamForm.value;
    val.Date = this.intelService.formatDate(val.Date, 'yyyy-MM-ddTHH:mm:ss');
    val.DoctorId = val.Doctor ? val.Doctor.Id : null;
    val.CompanyId = this.authService.userInfo.companyId;
    val.Lines.forEach(line => {
      line.ToothIds = line.Teeth.map(x => x.Id);
    });
    
    if (!this.Id) {
      this.dotkhamService.create(val).subscribe((res: any) => {
        this.notify('success', 'Lưu thành công');
        this.dotkham = res;
        this.onEmitDotkham(this.dotkham, false, null);
        this.loadRecord();
      });
    } else {
      this.dotkhamService.update(this.Id, val).subscribe((res: any) => {
        this.notify('success', 'Lưu thành công');
        this.dotkham = val;
        this.onEmitDotkham(this.dotkham, false, null);
      });
    }
  }

  onCancel() {
    this.onEmitDotkham(this.dotkham, true, null);
  }

  onClose() {
    this.onEmitDotkham(null, false, null);
  }

  checkAccess() {
    if (this.activeDotkham && this.activeDotkham !== this.dotkham) {
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
    this.dotkham.Lines.splice(i, 1);
    console.log(this.dotkhamForm);

  }

  eventTeeth(line, lineControl) {
    lineControl.patchValue(line);
    lineControl.get('Teeth').clear();
    line.Teeth.forEach(t => {
      const g = this.fb.group(t);
      lineControl.get('Teeth').push(g);
    });
    // lineControl.get('ToothIds').clear();
    // line.Teeth.forEach(t => {
    //   const g = this.fb.group(t.Id);
    //   lineControl.get('ToothIds').push(g);
    // });
  }
}
