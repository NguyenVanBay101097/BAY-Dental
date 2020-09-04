import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { HrPayrollStructureService, HrPayrollStructurePaged, HrPayrollStructureDisplay, HrSalaryRuleDisplay } from '../hr-payroll-structure.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime } from 'rxjs/internal/operators/debounceTime';
import { tap } from 'rxjs/internal/operators/tap';
import { switchMap } from 'rxjs/internal/operators/switchMap';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { HrPayrollStructureTypeCreateComponent } from '../hr-payroll-structure-type-create/hr-payroll-structure-type-create.component';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { HrSalaryRuleCrudDialogComponent } from '../hr-salary-rule-crud-dialog/hr-salary-rule-crud-dialog.component';

@Component({
  selector: 'app-payroll-structure-create-update',
  templateUrl: './hr-payroll-structure-create-update.component.html',
  styleUrls: ['./hr-payroll-structure-create-update.component.css']
})
export class HrPayrollStructureCreateUpdateComponent implements OnInit {

  @ViewChild('typeCbx', { static: true }) typeCbx: ComboBoxComponent;

  payrollForm: FormGroup;
  id: string;
  listType: any[];
  payrollRecord: HrPayrollStructureDisplay;

  AllData: any = [];

  constructor(
    private fb: FormBuilder, private router: Router,
    private payrollService: HrPayrollStructureService,
    private notificationService: NotificationService,
    private modalService: NgbModal,
    private activeroute: ActivatedRoute) { }

  ngOnInit() {
    this.payrollForm = this.fb.group({
      name: [null, [Validators.required]],
      active: false,
      schedulePay: null,
      note: null,
      regularPay: false,
      typeId: [null],
      type: [null, [Validators.required]],
      useWorkedDayLines: false,
      rules: null,
      id: null
    });


    this.id = this.activeroute.snapshot.paramMap.get('id');
    this.LoadListType();
    if (this.id) {
      this.LoadRecord();
    }

  }

  LoadRecord() {
    this.payrollService.get(this.id).subscribe(res => {
      this.payrollForm.patchValue(res);
      this.payrollRecord = Object.assign({}, res);
      // this.payrollForm.disable();
      this.loadSalaryRuleFromApi();
    });
  }

  LoadListType() {
    const page = new HrPayrollStructurePaged();
    this.payrollService.GetAllPayrollStructureType(page).subscribe(
      res => {
        this.listType = res.items;
      }
    );

    this.typeCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.typeCbx.loading = true)),
      switchMap(value => this.SearchType(value))
    ).subscribe((result: any) => {
      this.listType = result.items;
      this.typeCbx.loading = false;
    });
  }

  SearchType(value: string) {
    const val = new HrPayrollStructurePaged();
    val.filter = value;
    return this.payrollService.GetAllPayrollStructureType(val);
  }

  onSaveOrUpdate() {
    for (const i in this.payrollForm.controls) {
      this.payrollForm.controls[i].markAsDirty();
      this.payrollForm.controls[i].updateValueAndValidity();
    }

    if (!this.payrollForm.valid && this.payrollForm.enabled) {
      return false;
    }
    const val = this.payrollForm.value;
    val.rules = this.AllData;
    val.typeId = val.type.id;
    if (this.id) {
      this.payrollService.update(this.id, val).subscribe(
        res => {
          this.notificationService.show({
            content: 'Thành công!',
            hideAfter: 2000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
        },
        err => {
          console.log(err);
        }
      );
    } else {
      this.payrollService.create(val).subscribe(
        res => {
          this.notificationService.show({
            content: 'Thành công!',
            hideAfter: 2000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          this.router.navigate(['/hr/payroll-structures/edit/' + res.id]);
        }
      );
    }
  }

  ShowStructTypeCreateModal() {
    let modalRef = this.modalService.open(HrPayrollStructureTypeCreateComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: Loại mẫu lương';
    modalRef.result.then((entity) => {
      this.payrollForm.get('type').setValue(entity);
      this.LoadListType();
    });
  }

  loadSalaryRuleFromApi() {
    if (this.id) {
      this.payrollService.GetListRuleByStructId(this.id).subscribe(res => {
        this.AllData = res;
      });
    }
  }

  AllDataAdd(e) {
    const newRow: HrSalaryRuleDisplay = e;
    this.AllData.push(newRow);
  }

  removeHandler(e) {
    this.AllData = this.AllData.filter(item => item !== e);
  }

  ShowAddSalaryRulePopup() {
    const modalRef = this.modalService.open(HrSalaryRuleCrudDialogComponent,
      { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm quy tắc lương';
    modalRef.result.then((val) => {
      this.AllDataAdd(val);
    }, er => { });
  }

  editItem(item) {
    const modalRef = this.modalService.open(HrSalaryRuleCrudDialogComponent,
      { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa quy tắc lương';
    modalRef.componentInstance.rule = item;
    modalRef.result.then((val) => {
      this.UpdateAllData(val);
    }, er => { });
  }

  UpdateAllData(item) {
    if (item.id) {
      const index = this.AllData.findIndex(x => x.id === item.id);
      if (index !== -1) {
        this.AllData[index] = item;
      }
    } else {
      const index = this.AllData.findIndex(x => x.code === item.code);
      if (index !== -1) {
        this.AllData[index] = item;
      }
    }
  }
}
