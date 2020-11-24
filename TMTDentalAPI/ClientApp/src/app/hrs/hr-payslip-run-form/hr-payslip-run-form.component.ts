import { HrPayslipService, HrPayslipPaged, HrPayslipSave, HrPayslipDisplay, HrPayslipSaveDefaultValue } from './../hr-payslip.service';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { HrPaysliprunService, HrPayslipRunDefaultGet, HrPayslipRunDisplay } from '../hr-paysliprun.service';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { IntlService } from '@progress/kendo-angular-intl';
import { HrPayslipRunConfirmDialogComponent } from '../hr-payslip-run-confirm-dialog/hr-payslip-run-confirm-dialog.component';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { map } from 'rxjs/operators';
import { validate, validator } from 'fast-json-patch';
import { error } from 'protractor';
import { SalaryPaymentModule } from 'src/app/salary-payment/salary-payment.module';
import { HrSalaryPaymentComponent } from '../hr-salary-payment/hr-salary-payment.component';
import { SalaryPaymentSave } from 'src/app/shared/services/salary-payment.service';

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
  date: Date;
  isNotChiluong:boolean = true;

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
      state: 'confirm',
      date: [null, Validators.required],
      slips: this.fb.array([])
    });

    this.route.queryParamMap.subscribe(params => {
      this.id = params.get('id');
      this.date = params.get('date')? new Date(params.get('date')) : new Date();
      if (!this.id) {
        this.checkExist();
      } else {
        this.loadRecord();
      }
    });
  }

  get state() { return this.FormGroup.get('state').value; }
  get name() { return this.FormGroup.get('name').value; }
  get slipsFormArray() { return this.FormGroup.get('slips') as FormArray; }
  get dateFC() { return this.FormGroup.get('date') as FormArray; }

  loadRecord() {
    if (this.id) {
      this.hrPaysliprunService.get(this.id).subscribe((result: any) => {
        // check is exist chi luong?
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
        e.employeeNameSearch = e.employee.name.toLowerCase() + ' ' + this.RemoveVietnamese(e.employee.name);
      });
    } else {
      result.slips = [];
    }
    this.FormGroup.patchValue(result);
  }

  checkExist() {
    const d = this.dateFC.value || new Date();

    this.hrPaysliprunService.CheckExist(d).subscribe((res: any) => {
      if (res && res.id) {
        this.router.navigateByUrl('hr/payslip-run/form?id=' + res.id);
      } else {
        if (this.id) {
          this.router.navigateByUrl('hr/payslip-run/form?date=' + d.toISOString());
        } else {
          this.getDefault();
        }
      }
    });
  }

  getDefault() {
    const val = new HrPayslipRunDefaultGet();
    this.hrPaysliprunService.default(val).subscribe((result: any) => {
      const d = this.date ? this.date : new Date(result.date);
      result.date = d;
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

  actionCancel() {
    if(this.id) {
      const modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Hủy bảng lương';
      modalRef.componentInstance.body = 'Bạn chắc chắn muốn hủy bảng lương?';
      modalRef.result.then(() => {
        this.hrPaysliprunService.actionCancel([this.id]).subscribe(() => {
          this.notify('success', 'Bảng lương đã được hủy thành công!');
          this.loadRecord();
        });
      });

    }
  }

  onChangeDate(e) {
    const newName = `Bảng lương tháng ${e.getMonth() + 1} năm ${e.getFullYear()}`;
    this.FormGroup.get('name').setValue(newName);
    this.dateFC.setValue(e);
    this.checkExist();
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
    this.search = this.search.trim().toLocaleLowerCase();
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

  printAllEmpSalary() {

    var value = this.getFormData();
    this.hrPaysliprunService.printAllEmpSalary(this.id, value).subscribe(
      result => {
        if (result && result['html']) {
          var popupWin = window.open('', '_blank', 'top=0,left=0,height=auto,width=auto');
          popupWin.document.open();

          popupWin.document.write(result['html']);
          popupWin.document.close();
        } else {
          this.notificationService.show({
            content: "Bạn chưa chọn nhân viên nào để in, vui lòng chọn nhân viên để tiếp tục",
            hideAfter: 5000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: "error", icon: true }
          });
        }
      }
    )
  }

  onExport() {
    const payslipIds = this.slipsFormArray.value.filter(x => x.isCheck === true).map(x => x.id);
    this.hrPaysliprunService.ExportExcelFile(payslipIds).subscribe((res: any) => {
      const filename = 'danh_sach_phieu_luong';
      const newBlob = new Blob([res], {
        type:
          'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      });

      const data = window.URL.createObjectURL(newBlob);
      const link = document.createElement('a');
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        // For Firefox it is necessary to delay revoking the ObjectURL
        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }

  onPayment() {
    const payslips = this.slipsFormArray.value.filter(x => x.isCheck === true);
    const payments = [];
    payslips.forEach((slip: HrPayslipSaveDefaultValue) => {
      const payment: SalaryPaymentSave = {
        Amount: slip.netSalary,
        CompanyId: slip.companyId,
        Date: this.dateFC.value,
        Employee: slip.employee,
        EmployeeId: slip.employeeId,
        Journal: null,
        JournalId: null,
        Name: slip.name,
        Reason: `Chi lương tháng ${this.dateFC.value.getMonth()}/${this.dateFC.value.getFullYear()}`,
        State: 'draft',
        Type: 'salary'
      };
      payments.push(payment);
    });
    const modalRef = this.modalService.open(HrSalaryPaymentComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = `PHIẾU CHI LƯƠNG THÁNG  ${this.dateFC.value.getMonth()}/${this.dateFC.value.getFullYear()}`;
    modalRef.componentInstance.payments = payments;
    modalRef.result.then((res: any) => {

    });
  }
}
