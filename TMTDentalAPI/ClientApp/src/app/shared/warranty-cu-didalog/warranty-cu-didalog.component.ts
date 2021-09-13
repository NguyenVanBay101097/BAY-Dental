import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { Observable } from 'rxjs';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { LaboWarrantySave, LaboWarrantyService } from 'src/app/labo-orders/labo-warranty.service';
import { ToothDisplay } from 'src/app/teeth/tooth.service';
import { NotifyService } from '../services/notify.service';

@Component({
  selector: 'app-warranty-cu-didalog',
  templateUrl: './warranty-cu-didalog.component.html',
  styleUrls: ['./warranty-cu-didalog.component.css']
})
export class WarrantyCuDidalogComponent implements OnInit {
  @Input() laboWarrantyId: string;
  @Input() laboOrderId: any;
  @ViewChild('doctorCbx', { static: true }) doctorCbx: ComboBoxComponent;

  title: string = "Tạo phiếu bảo hành";
  myForm: FormGroup;
  id: string;
  filteredDoctors: any = [];
  infoLabo: any;
  submitted = false;
  laboWarrantyName: string = '';
  laboTeeth: any;
  invalid: boolean = false;
  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private intlService: IntlService,
    private employeeService: EmployeeService,
    private laboWarrantyService: LaboWarrantyService,
    private notifyService: NotifyService,
  ) { }

  ngOnInit() {
    this.myForm = this.fb.group({
      state: 'draft',
      doctor: [null, Validators.required],
      dateReceiptObj: [new Date(), Validators.required],
      reason: [null, Validators.required],
      content: null,
      note: null,
      teeth: this.fb.array([]),
    });

    this.doctorCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.doctorCbx.loading = true)),
      switchMap(value => this.searchDoctors(value))
    ).subscribe(result => {
      this.filteredDoctors = result;
      this.doctorCbx.loading = false;
    });
    this.loadDoctors();

    if (this.laboWarrantyId) {
      this.loadData();
      this.title = "Cập nhật bảo hành";
    }
    else {
      if (this.laboOrderId) {
        this.loadDefault();
      }
    }

    // Handle Form control valueChanges
    this.myForm.controls['reason'].valueChanges.subscribe(
      (selectedValue) => {
        this.invalid = selectedValue ? false : true;
      }
    );
  }

  getControlFC(key: string) {
    return this.myForm.get(key);
  }

  getValueFC(key: string) {
    return this.myForm.get(key).value;
  }

  get teethFA() { return this.myForm.get('teeth') as FormArray; }

  get f() {
    return this.myForm.controls;
  }

  searchDoctors(filter?: string) {
    var val = new EmployeePaged();
    val.isDoctor = true;
    val.search = filter;
    return this.employeeService.getEmployeeSimpleList(val);
  }

  loadDoctors() {
    this.searchDoctors().subscribe(result => {
      this.filteredDoctors = _.unionBy(this.filteredDoctors, result, 'id');
    });
  }

  loadData() {
    this.laboWarrantyService.get(this.laboWarrantyId).subscribe((res: any) => {
      this.infoLabo = res;
      this.laboTeeth = res.teethLaboOrder;
      this.laboWarrantyName = res.name;
      this.myForm.patchValue(res);
      this.myForm.controls['dateReceiptObj'].setValue(new Date(res.dateReceiptWarranty));
      this.myForm.patchValue(res);
      this.myForm.controls['doctor'].setValue(res.employee);
      this.teethFA.clear();
      res.teeth.forEach(tooth => {
        this.teethFA.push(this.fb.group(tooth));
      });
    }, err => {
      console.log(err);
    })
  }

  loadDefault() {
    let val = {
      laboOrderId: this.laboOrderId
    }
    this.laboWarrantyService.getDefault(val).subscribe((res: any) => {
      this.infoLabo = res;
      this.laboTeeth = res.teethLaboOrder;
      this.myForm.controls['doctor'].setValue(res.employee);
    }, err => {
      console.log(err);
    })
  }

  getDataFormGroup() {
    let valForm = this.myForm.value;
    let val = new LaboWarrantySave();
    val.laboOrderId = this.laboOrderId || '';
    val.name = this.laboWarrantyName || '';
    val.employeeId = valForm.doctor.id;
    val.dateReceiptWarranty = this.intlService.formatDate(valForm.dateReceiptObj, 'yyyy-MM-ddTHH:mm:ss');
    val.teeth = valForm.teeth;
    val.reason = valForm.reason;
    val.note = valForm.note;
    val.content = valForm.content;
    val.state = valForm.state;
    return val;
  }

  onSave$(): Observable<any> {
    let val = this.getDataFormGroup();
    if (this.laboWarrantyId) {
      return this.laboWarrantyService.update(this.laboWarrantyId, val);
    } else {
      return this.laboWarrantyService.create(val);
    }
  }

  onSave() {
    this.submitted = true;

    if (this.myForm.invalid) {
      this.invalid = true;
      return;
    }

    this.onSave$().subscribe((res: any) => {
      this.notifyService.notify("success", "Lưu thành công");
      this.activeModal.close();
    })
  }

  onConfirm() {
    this.submitted = true;

    if (this.myForm.invalid) {
      return;
    }

    this.onSave$().subscribe((res: any) => {
      if (!this.laboWarrantyId) {
        this.laboWarrantyId = res.id;
      }
      this.laboWarrantyService.buttonConfirm([this.laboWarrantyId]).subscribe(() => {
        this.activeModal.close();
        this.notifyService.notify('success', 'Xác nhận thành công');
      });
    });
  }

  onCancel() {
    this.laboWarrantyService.buttonCancel([this.laboWarrantyId]).subscribe(() => {
      this.notifyService.notify('success', 'Hủy phiếu thành công');
      this.activeModal.close(true);
    });
  }

  onClose() {
    // if (this.id) {
    //   this.activeModal.close(true);
    // } else {
    //   this.activeModal.close();
    // }
    this.activeModal.dismiss()
  }

  isToothSelected(tooth: ToothDisplay) {
    const index = this.getValueFC('teeth').findIndex(x => x.id == tooth.id);
    return index >= 0 ? true : false;
  }

  onToothSelected(tooth: ToothDisplay) {
    if (this.isToothSelected(tooth)) {
      var index = this.getValueFC('teeth').findIndex(x => x.id == tooth.id);
      this.teethFA.removeAt(index);
    } else {
      this.teethFA.push(this.fb.group(tooth));
    }
  }
}
