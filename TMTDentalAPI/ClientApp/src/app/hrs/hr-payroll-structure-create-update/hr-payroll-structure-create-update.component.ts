import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { HrPayrollStructureService, HrPayrollStructurePaged, HrPayrollStructureDisplay } from '../hr-PayrollStructure.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { HrSalaryRuleListComponent } from '../hr-salary-rule-list/hr-salary-rule-list.component';
import { debounceTime } from 'rxjs/internal/operators/debounceTime';
import { tap } from 'rxjs/internal/operators/tap';
import { switchMap } from 'rxjs/internal/operators/switchMap';

@Component({
  selector: 'app-payroll-structure-create-update',
  templateUrl: './hr-payroll-structure-create-update.component.html',
  styleUrls: ['./hr-payroll-structure-create-update.component.css']
})
export class HrPayrollStructureCreateUpdateComponent implements OnInit {

  @ViewChild('typeCbx', { static: true }) typeCbx: ComboBoxComponent;
  @ViewChild(HrSalaryRuleListComponent, { static: false }) rulesComp: HrSalaryRuleListComponent;

  payrollForm: FormGroup;
  id: string;
  listType: any[];
  payrollRecord: HrPayrollStructureDisplay;

  constructor(
    private fb: FormBuilder, private router: Router,
    private payrollService: HrPayrollStructureService,
    private notificationService: NotificationService,
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
    val.rules = this.rulesComp.AllData;
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

  ShowStructTypeModal() {

  }
}
