import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
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
import { NotifyService } from '../services/notify.service';

@Component({
  selector: 'app-warranty-cu-didalog',
  templateUrl: './warranty-cu-didalog.component.html',
  styleUrls: ['./warranty-cu-didalog.component.css']
})
export class WarrantyCuDidalogComponent implements OnInit {
  @Input() laboId: string;
  @Input() laboWarrantyId: string;
  @ViewChild('doctorCbx', { static: true }) doctorCbx: ComboBoxComponent;

  title: string = "Tạo phiếu bảo hành";
  myForm: FormGroup;
  id: string;
  filteredDoctors: any = [];
  infoLabo: any;
  submitted = false;
  laboWarrantyName: string = '';
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

    if (this.laboId) {
      this.loadDefault();
    }
  }

  getControlFC(key: string) {
    return this.myForm.get(key);
  }

  getValueFC(key: string) {
    return this.myForm.get(key).value;
  }

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
      this.laboWarrantyName = res.name;
      this.myForm.patchValue(res);
      this.myForm.controls['dateReceiptObj'].setValue(new Date(res.dateReceiptWarranty));
      this.myForm.patchValue(res);
      this.myForm.controls['doctor'].setValue(res.employee);
    }, err => {
      console.log(err);
    })
  }

  loadDefault() {
    let val = {
      laboOrderId: this.laboId
    }
    this.laboWarrantyService.getDefault(val).subscribe((res) => {
      this.infoLabo = res;
    }, err => {
      console.log(err);
    })
  }

  getDataFormGroup() {
    let valForm = this.myForm.value;
    let val = new LaboWarrantySave();
    val.laboOrderId = this.laboId || '';
    val.name = this.laboWarrantyName || '';
    val.employeeId = valForm.doctor.id;
    val.dateReceiptWarranty = this.intlService.formatDate(valForm.dateReceiptObj, 'yyyy-MM-ddTHH:mm:ss');
    val.teeth = [];
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
      return;
    }

    this.onSave$().subscribe((res: any) => {
      this.notifyService.notify("success", "Lưu thành công");
      this.activeModal.close();
    })
  }

  onConfirm() {
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
    if (this.id) {
      this.activeModal.close(true);
    } else {
      this.activeModal.close();
    }
  }
}
