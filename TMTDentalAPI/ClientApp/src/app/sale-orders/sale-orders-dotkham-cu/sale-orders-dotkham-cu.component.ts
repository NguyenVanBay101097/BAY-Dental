import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { validator } from 'fast-json-patch';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { WebService } from 'src/app/core/services/web.service';
import { EmployeesOdataService } from 'src/app/shared/services/employeeOdata.service';
import { PartnerImageBasic } from 'src/app/shared/services/partners.service';

@Component({
  selector: 'app-sale-orders-dotkham-cu',
  templateUrl: './sale-orders-dotkham-cu.component.html',
  styleUrls: ['./sale-orders-dotkham-cu.component.css']
})
export class SaleOrdersDotkhamCuComponent implements OnInit {

  @ViewChild('empCbx', {static: true}) empCbx: ComboBoxComponent;
  @Input() dotkham: any;
  @Input() activeDotkham: any;

  dotkhamForm: FormGroup;
  empList: any[];

  constructor(
    private webService: WebService,
    private fb: FormBuilder,
    private empService: EmployeesOdataService,
    private intelService: IntlService
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
      Steps: this.fb.array([]),
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

  get imgsFA() {return this.dotkhamForm.get('DotKhamImages') as FormArray; }
  get linesFA() { return this.dotkhamForm.get('Lines') as FormArray; }

  loadRecord() {
   if (this.dotkham) {
    this.dotkham.Date = new Date(this.dotkham.Date);
    this.dotkham.DotKhamImages.forEach(e => {
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

  }

  onSave() {
    const val = this.dotkhamForm.value;
    val.Date = this.intelService.formatDate(val.Date, 'yyyy-MM-dd');
  }

  onCancel() {

  }

  onClose() {

  }
}
