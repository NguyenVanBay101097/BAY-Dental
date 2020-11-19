import { HrPayslipService, HrPayslipPaged, HrPayslipSave, HrPayslipDisplay, HrPayslipSaveDefaultValue } from './../hr-payslip.service';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { HrPaysliprunService, HrPayslipRunDefaultGet } from '../hr-paysliprun.service';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { IntlService } from '@progress/kendo-angular-intl';
import { HrPayslipRunConfirmDialogComponent } from '../hr-payslip-run-confirm-dialog/hr-payslip-run-confirm-dialog.component';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { map } from 'rxjs/operators';
import { validate, validator } from 'fast-json-patch';

@Component({
  selector: 'app-hr-payslip-run-form',
  templateUrl: './hr-payslip-run-form.component.html',
  styleUrls: ['./hr-payslip-run-form.component.css']
})
export class HrPayslipRunFormComponent implements OnInit {
  id: string;
  FormGroup: FormGroup;
  isCompact = false;

  constructor(private fb: FormBuilder,
    private hrPaysliprunService: HrPaysliprunService,
    private route: ActivatedRoute, private modalService: NgbModal,
    private notificationService: NotificationService,
    private hrPayslipService: HrPayslipService,
    private router: Router, private intlService: IntlService) { }

  ngOnInit() {
    this.FormGroup = this.fb.group({
      name: [null, Validators.required],
      companyId: null,
      state: 'draft',
      date: [null, Validators.required],
      slips: this.fb.array([])
    });

    this.route.queryParamMap.subscribe(params => {
      this.id = params.get('id');
      if (!this.id) {
        this.getDefault();
      } else {
        this.loadRecord();
      }
    });
  }

  get state() { return this.FormGroup.get('state').value; }
  get name() { return this.FormGroup.get('name').value; }
  get slipsFormArray() { return this.FormGroup.get('slips') as FormArray; }

  loadRecord() {
    if (this.id) {
      this.hrPaysliprunService.get(this.id).subscribe((result: any) => {
        this.patchValue(result);
      });
    }
  }

  patchValue(result) {
    result.date = new Date(result.date);
    if (result.slips) {
      this.slipsFormArray.clear();
      result.slips.forEach(e => {
        const fg = this.fb.group(new HrPayslipSaveDefaultValue());
        fg.patchValue(e);
        this.slipsFormArray.push(fg);
      });
    } else {
      result.slips = [];
    }
    this.FormGroup.patchValue(result);
  }

  getDefault() {
    const val = new HrPayslipRunDefaultGet();
    val.state = 'draft';
    this.hrPaysliprunService.default(val).subscribe((result: any) => {
      const d = new Date(result.date);
      const newName = `Bảng lương tháng ${d.getMonth() + 1} năm ${d.getFullYear()}`;
      result.name = newName;
      this.patchValue(result);
    });
  }

  getFormData() {
    debugger;
    const val = this.FormGroup.value;
    val.date = this.intlService.formatDate(val.date, 'yyyy-MM-ddTHH:mm');
    return val;
  }

  ComputeSalary() {
    if (!this.FormGroup.valid) {
      return;
    }

    const val = this.getFormData();
    if (!this.id) {
      this.hrPaysliprunService.create(val)
        .subscribe((result: any) => {
          this.hrPaysliprunService.CreatePayslipByRunId(result.id).subscribe(() => {
            this.router.navigateByUrl('hr/payslip-run/form?id=' + result.id);
          });
        }, err => {
          console.log(err);
        });
    } else {
      this.hrPaysliprunService.ReComputeSalary(this.id).subscribe(() => {
        this.notify('success', 'lưu thành công');
        this.loadRecord();
      });
    }
  }

  onSave() {
    if (!this.FormGroup.valid) {
      return;
    }

    const val = this.getFormData();
    if (!this.id) {
      this.hrPaysliprunService.create(val)
        .subscribe((result: any) => {
          this.hrPaysliprunService.CreatePayslipByRunId(result.id).subscribe(() => {
            this.router.navigateByUrl('hr/payslip-run/form?id=' + result.id);
          });
        }, err => {
          console.log(err);
        });
    } else {
      this.hrPaysliprunService.update(this.id, val)
        .subscribe(() => {
          this.notify('success', 'lưu thành công');
          this.loadRecord();
        }, err => {
          console.log(err);
        });
    }
  }

  onChangeDate(e) {
    const newName = `Bảng lương tháng ${e.getMonth() + 1} năm ${e.getFullYear()}`;
    this.FormGroup.get('name').setValue(newName);
  }

  onSearchEmployee(e) {
    const value = e.target.value;

  }

  checkAll(e) {
    const val = e.target.checked;
    this.slipsFormArray.controls.forEach(control => {
      control.get('isCheck').patchValue(val);
    });
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
  convertState(state) {
    switch (state) {
      case 'verify':
        return 'Chờ xác nhận';
      case 'done':
        return 'Hoàn thành';
      case 'draft':
        return 'Nháp';
    }
  }
}
