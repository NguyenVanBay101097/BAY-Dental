import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { mergeMap } from 'rxjs/operators';
import { SalaryPaymentService } from 'src/app/salary-payment/salary-payment.service';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PrintService } from "src/app/shared/services/print.service";
import { HrPayslipRunDefaultGet, HrPaysliprunService } from '../hr-paysliprun.service';
import { HrSalaryPaymentComponent } from '../hr-salary-payment/hr-salary-payment.component';
import { HrPayslipSaveDefaultValue } from './../hr-payslip.service';

@Component({
  selector: 'app-hr-payslip-run-form',
  templateUrl: './hr-payslip-run-form.component.html',
  styleUrls: ['./hr-payslip-run-form.component.css'],
   host: {
    class: "o_action o_view_controller",
  },
})
export class HrPayslipRunFormComponent implements OnInit {
  FormGroup: FormGroup;
  isCompact = false;
  search: string = '';
  isExistSalaryPayment: boolean = false;
  payslipRun: any;
  checkAll = false;
  canAdd = false;
  canUpdate = false;
  filterDate = new Date();
  constructor(private fb: FormBuilder,
    private hrPaysliprunService: HrPaysliprunService,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private printService: PrintService,
    private paymentService: SalaryPaymentService,private intlService: IntlService,
    private checkPermissionService: CheckPermissionService
  ) { }

  ngOnInit() {
    this.FormGroup = this.fb.group({
      id: [null],
      name: [null],
      companyId: null,
      state: 'confirm',
      date: [new Date(), Validators.required],
      slips: this.fb.array([])
    });
    this.checkExist();
    this.checkRole();
  }

  get state() { return this.FormGroup.get('state').value; }
  get id() { return this.FormGroup.get('id').value; }
  get name() { return this.FormGroup.get('name').value; }
  get slipsFormArray() { return this.FormGroup.get('slips') as FormArray; }
  get dateFC() { return this.FormGroup.get('date') as FormArray; }
  get dateNext() {
    if (!this.dateFC.value) {
      return new Date();
    }
    const d = new Date();
    d.setDate(1);
    d.setMonth(this.dateFC.value.getMonth() + 1);
    return d;
  }

  patchValue(result) {
    if (!result) {
      this.FormGroup.get('id').reset();
      return;
    }
    this.checkAll = false;
    this.isExistSalaryPayment = result.isExistSalaryPayment;
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

  loadRecord() {
    if (this.id) {
      this.hrPaysliprunService.get(this.id).subscribe((result: any) => {
        this.payslipRun = result;
        this.patchValue(result);
      });
    }
  }

  checkExist() {
    const d = this.dateFC.value || new Date();

    this.hrPaysliprunService.CheckExist(d).subscribe((res: any) => {
      if (!res) {
        this.getDefault();
      }
      this.payslipRun = res;
      this.patchValue(res);
    });
  }

  getFormData() {
    const val = this.FormGroup.value;
    val.date = this.intlService.formatDate(val.date, 'yyyy-MM-ddTHH:mm');
    return val;
  }
  getDefault() {
    const val = new HrPayslipRunDefaultGet();
    this.hrPaysliprunService.default(val).subscribe((result: any) => {
      const d = this.dateFC.value || new Date(result.date);
      result.date = d;
      const newName = `B???ng l????ng th??ng ${d.getMonth() + 1} n??m ${d.getFullYear()}`;
      result.name = newName;
      this.patchValue(result);
    });
  }

  ComputeSalary() {
    if (!this.FormGroup.valid) {
      return;
    }

    const val = this.getFormData();
    if (!this.id) {
      this.hrPaysliprunService.create(val)
        .subscribe((result: any) => {
          this.hrPaysliprunService.CreatePayslipByRunId(result.id).subscribe((res: any) => {
            this.payslipRun = res;
            this.patchValue(res);
          });
        }, err => {
          console.log(err);
        });
    } else {
      this.hrPaysliprunService.ReComputeSalary(this.id).subscribe(() => {
        this.notify('success', 'T??nh l????ng th??nh c??ng');
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
          this.hrPaysliprunService.CreatePayslipByRunId(result.id).subscribe((res: any) => {
            this.payslipRun = res;
            this.patchValue(res);
          });
        }, err => {
          console.log(err);
        });
    } else {
      this.hrPaysliprunService.update(this.id, val)
        .subscribe(() => {
          this.notify('success', 'L??u th??nh c??ng');
          this.loadRecord();
        }, err => {
          console.log(err);
        });
    }
  }

  onConfirm() {
    if (!this.FormGroup.valid) {
      return;
    }

    const val = this.getFormData();

    const modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'X??c nh???n b???ng l????ng';
    modalRef.componentInstance.body = 'B???n ch???c ch???n x??c nh???n b???ng l????ng?';
    modalRef.componentInstance.body2 = 'B???n s??? kh??ng th??? ??i???u ch???nh sau khi x??c nh???n.';
    modalRef.result.then(() => {
      if (this.id) {
        this.hrPaysliprunService.update(this.id, val).pipe(
          mergeMap((result: any) => {
            return this.hrPaysliprunService.get(this.id);
          })
        ).subscribe((result: any) => {
          this.hrPaysliprunService.actionConfirm(this.id).subscribe(() => {
            this.notify('success', 'B???ng l????ng x??c nh???n th??nh c??ng');
            this.loadRecord();
          });
        });
      } else {
        this.hrPaysliprunService.create(val).pipe(
          mergeMap((result: any) => {
            return this.hrPaysliprunService.CreatePayslipByRunId(result.id);
          })
        ).subscribe((result: any) => {
          this.hrPaysliprunService.actionConfirm(result.id).subscribe(() => {
            this.notify('success', 'B???ng l????ng x??c nh???n th??nh c??ng');
            this.loadRecord();
          });
        });
      }
    });

  }

  actionCancel() {
    if (this.id) {
      const modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'H???y b???ng l????ng';
      modalRef.componentInstance.body = 'B???n ch???c ch???n mu???n h???y b???ng l????ng?';
      modalRef.result.then(() => {
        this.hrPaysliprunService.actionCancel([this.id]).subscribe(() => {
          this.notify('success', 'B???ng l????ng ???? ???????c h???y th??nh c??ng!');
          this.loadRecord();
        });
      });

    }
  }

  onChangeDate(e) {
    const newName = `B???ng l????ng th??ng ${e.getMonth() + 1} n??m ${e.getFullYear()}`;
    this.FormGroup.get('name').setValue(newName);
    this.dateFC.setValue(e);
    this.checkExist();
  }

  RemoveVietnamese(text) {
    text = text.replace(/??|??|???|???|??|??|???|???|???|???|???|??|???|???|???|???|???/g, "a");
    text = text.replace(/??|??|???|???|???|??|???|???|???|???|???/g, "e");
    text = text.replace(/??|??|???|???|??/g, "i");
    text = text.replace(/??|??|???|???|??|??|???|???|???|???|???|??|???|???|???|???|???/g, "o");
    text = text.replace(/??|??|???|???|??|??|???|???|???|???|???/g, "u");
    text = text.replace(/???|??|???|???|???/g, "y");
    text = text.replace(/??/g, "d");
    text = text.replace(/??|??|???|???|??|??|???|???|???|???|???|??|???|???|???|???|???/g, "A");
    text = text.replace(/??|??|???|???|???|??|???|???|???|???|???/g, "E");
    text = text.replace(/??|??|???|???|??/g, "I");
    text = text.replace(/??|??|???|???|??|??|???|???|???|???|???|??|???|???|???|???|???/g, "O");
    text = text.replace(/??|??|???|???|??|??|???|???|???|???|???/g, "U");
    text = text.replace(/???|??|???|???|???/g, "Y");
    text = text.replace(/??/g, "D");
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

  onCheckAll(e) {
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
        return 'Ch??? x??c nh???n';
      case 'done':
        return 'Ho??n th??nh';
      case 'draft':
        return 'Nh??p';
    }
  }

  printAllEmpSalary() {
    const slipIds = this.slipsFormArray.value.filter(x => x.isCheck === true).map(x => x.id);
    if (!slipIds.length) {
      this.notify('error', 'B???n ch??a ch???n nh??n vi??n n??o ????? in, vui l??ng ch???n nh??n vi??n ????? ti???p t???c.');
      return false;
    }

    var value = this.getFormData();
    if (value)
      this.hrPaysliprunService.printAllEmpSalary(this.id, value).subscribe(
        (result: any) => {
          if (result) {
            this.printService.printHtml(result.html);
          } else {
            alert('B???n ch??a ch???n nh??n vi??n n??o ????? in, vui l??ng ch???n nh??n vi??n ????? ti???p t???c.');
          }
        }
      )
  }

  onExport() {
    const payslipIds = this.slipsFormArray.value.filter(x => x.isCheck === true).map(x => x.id);
    if (payslipIds.length === 0) {
      this.notify('error', 'Ph???i ch???n nh??n vi??n tr?????c khi xu???t file');
      return;
    }
    this.hrPaysliprunService.ExportExcelFile(payslipIds).subscribe((res: any) => {
      const filename = `Bang_Luong_${this.dateFC.value.getMonth() + 1}_${this.dateFC.value.getFullYear()}`;
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
    const slipIds = this.slipsFormArray.value.filter(x => x.isCheck === true).map(x => x.id);
    if (slipIds.length == 0) this.notify('error', 'Ch??a ch???n phi???u l????ng ????? chi l????ng');

    this.paymentService.defaulCreateBy(slipIds).subscribe((res: any) => {

      if (res.data.length > 0) {

        const modalRef = this.modalService.open(HrSalaryPaymentComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
        modalRef.componentInstance.title = `PHI???U CHI L????NG TH??NG  ${this.dateFC.value.getMonth() + 1}/${this.dateFC.value.getFullYear()}`;
        modalRef.componentInstance.payslipIds = slipIds;
        modalRef.componentInstance.payments = res.data;
        modalRef.result.then((res: any) => {
          this.loadRecord();
        });
      }

      res.errors.forEach(e => {
        this.notify('error', e);
      });
    }
    );
  }

  onCheckItem(i, val) {
    if (this.checkAll === true) {
      this.checkAll = false;
    }
    this.slipsFormArray.controls[i].get('isCheck').setValue(val);
  }

  checkRole() {
    this.canAdd = this.checkPermissionService.check(['Salary.HrPayslipRun.Create']);
    this.canUpdate = this.checkPermissionService.check(['Salary.HrPayslipRun.Update']);
  }
}
