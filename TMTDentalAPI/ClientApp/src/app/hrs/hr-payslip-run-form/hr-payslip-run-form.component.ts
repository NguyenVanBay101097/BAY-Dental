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
  search: string = '';
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
      console.log(new Date());

      this.id = params.get('id');
      if (!this.id) {
        this.hrPaysliprunService.CheckExist(new Date()).subscribe((res: any) => {
          if (res && res.id) {
            this.router.navigateByUrl('hr/payslip-run/form?id=' + res.id);
          } else {
            this.getDefault();
          }
        });
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
        e.employeeNameSearch = e.employee.name + ' ' + this.RemoveVietnamese(e.employee.name);
      });
    } else {
      result.slips = [];
    }
    console.log(result);

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
        this.notify('success', 'tính lương thành công');
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

  onConfirm() {
    if (this.id) {
      const modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Xác nhận bảng lương';
      modalRef.componentInstance.body = 'Bạn chắc chắn xác nhận bảng lương?';
      modalRef.componentInstance.body2 = ' bạn sẽ không thể điều chỉnh sau khi xác nhận.';
      modalRef.result.then(() => {
        this.hrPaysliprunService.actionConfirm(this.id).subscribe(() => {
          this.notify('success', 'bảng lương xác nhận thành công');
          this.loadRecord();
        });
      });

    }
  }

  onChangeDate(e) {
    const newName = `Bảng lương tháng ${e.getMonth() + 1} năm ${e.getFullYear()}`;
    this.FormGroup.get('name').setValue(newName);
  }

  RemoveVietnamese(text) {
    text = text.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    text = text.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    text = text.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    text = text.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    text = text.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    text = text.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    text = text.replace(/đ/g, "d");
    text = text.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
    text = text.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
    text = text.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
    text = text.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
    text = text.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
    text = text.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
    text = text.replace(/Đ/g, "D");
    text = text.toLowerCase();
    text = text
      .replace(/[&]/g, "-and-")
      .replace(/[^a-zA-Z0-9._-]/g, " ")
      .replace(/[-]+/g, " ")
      .replace(/-$/, "");
    return text;
  }

  onSearchEmployee(e) {
    this.search = this.search.trim();
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
