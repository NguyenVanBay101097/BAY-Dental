import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { validator } from 'fast-json-patch';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { WebService } from 'src/app/core/services/web.service';
import { DotKhamLineDisplay, DotkhamOdataService } from 'src/app/shared/services/dotkham-odata.service';
import { EmployeesOdataService } from 'src/app/shared/services/employeeOdata.service';
import { PartnerImageBasic } from 'src/app/shared/services/partners.service';

@Component({
  selector: 'app-sale-orders-dotkham-cu',
  templateUrl: './sale-orders-dotkham-cu.component.html',
  styleUrls: ['./sale-orders-dotkham-cu.component.css']
})
export class SaleOrdersDotkhamCuComponent implements OnInit {

  @ViewChild('empCbx', { static: true }) empCbx: ComboBoxComponent;
  @Input() dotkham: any;
  @Output() dotkhamChange = new EventEmitter<any>();
  @Input() activeDotkham: any;
  @Output() activeDotkhamChange = new EventEmitter<any>();

  dotkhamForm: FormGroup;
  empList: any[];

  constructor(
    private webService: WebService,
    private fb: FormBuilder,
    private empService: EmployeesOdataService,
    private intelService: IntlService,
    private authService: AuthService,
    private dotkhamService: DotkhamOdataService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
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
        const imgFG = this.fb.group(e);
        this.linesFA.push(imgFG);
      });
      this.dotkhamForm.patchValue(this.dotkham);
      console.log(this.dotkhamForm);

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
    this.checkAccess();
    this.activeDotkhamChange.emit(this.dotkham);
  }

  onApllyLine(e) {
    const line = new DotKhamLineDisplay();
    line.Name = e.Name;
    line.DotKhamId = this.dotkham.Id;
    line.ProductId = e.ProductId;
    line.Product = e.Product;
    line.State = 'draft';
    line.Sequence = this.dotkham.Lines.length + 1;
    const lineFG = this.fb.group(e);
    this.linesFA.push(lineFG);
  }

  onSave() {
    if (this.dotkhamForm.invalid) {
      return;
    }
    const val = this.dotkhamForm.value;
    val.Date = this.intelService.formatDate(val.Date, 'yyyy-MM-ddTHH:mm:ss');
    val.DoctorId = val.Doctor ? val.Doctor.Id : null;
    val.CompanyId = this.authService.userInfo.companyId;
debugger;
    if (!this.Id) {
      this.dotkhamService.create(val).subscribe((res: any) => {
        this.notify('success', 'Lưu thành công');
        this.dotkhamChange.emit(res);
        this.loadRecord();
        this.activeDotkhamChange.emit(null);
      });
    } else {
      this.dotkhamService.update(this.Id, val).subscribe((res: any) => {
        this.notify('success', 'Lưu thành công');
        this.activeDotkhamChange.emit(null);
      });
    }
  }

  onCancel() {
    this.dotkhamChange.emit(this.dotkham);
    this.activeDotkhamChange.emit(null);
  }

  onClose() {
    this.activeDotkhamChange.emit(null);
  }

  checkAccess() {
    if (this.activeDotkham && this.activeDotkham !== this.dotkham) {
      this.notify('error', 'Bạn phải hoàn tất đợt khám đang thao tác');
      return;
    }
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
}
