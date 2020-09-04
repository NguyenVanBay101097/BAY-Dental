import { HrPayslipDisplay, HrPayslipService, HrPayslipPaged } from './../hr-payslip.service';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { HrPaysliprunService, HrPayslipRunDefaultGet } from '../hr-paysliprun.service';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { IntlService } from '@progress/kendo-angular-intl';
import { AuthService } from 'src/app/auth/auth.service';
import { HrPayslipRunConfirmDialogComponent } from '../hr-payslip-run-confirm-dialog/hr-payslip-run-confirm-dialog.component';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { map } from 'rxjs/operators';
import { aggregateBy } from '@progress/kendo-data-query';

@Component({
  selector: 'app-hr-payslip-run-form',
  templateUrl: './hr-payslip-run-form.component.html',
  styleUrls: ['./hr-payslip-run-form.component.css']
})
export class HrPayslipRunFormComponent implements OnInit {
  title: string;
  type: string;
  itemId: string;
  myForm: FormGroup;
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  search: string;
  submitted = false;
  loading = false;
  public total: any;

  paySlip: any = [];
  paysliprun: any;

  payslipGridData: GridDataResult;

  public aggregates: any[] = [
    { field: 'totalAmount', aggregate: 'sum' },
  ];

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  constructor(private fb: FormBuilder,
    private hrPaysliprunService: HrPaysliprunService,
    private route: ActivatedRoute, private modalService: NgbModal,
    private notificationService: NotificationService,
    private hrPayslipService: HrPayslipService,
    private router: Router, private intlService: IntlService, private authService: AuthService) { }

  ngOnInit() {
    this.paysliprun = new Object();
    this.route.queryParamMap.subscribe(params => {
      this.itemId = params.get('id');
      if (!this.itemId) {
        this.getDefault();
      } else {
        this.loadRecord();
      }

      this.loadPayslip();
      this.getAllSlipData();
    });

    this.myForm = this.fb.group({
      name: [null, Validators.required],
      dateStartObj: [null, Validators.required],
      dateEndObj: [null, Validators.required],
      companyId: null,
      state: 'draft',
    });
  }

  loadRecord() {
    if (this.itemId) {
      this.hrPaysliprunService.get(this.itemId).subscribe((result: any) => {
        this.paysliprun = result;
        result.dateEndObj = new Date(result.dateEnd);
        result.dateStartObj = new Date(result.dateStart);
        this.myForm.patchValue(result);
      });
    }
  }

  reloadData() {
    this.loadRecord();
    this.loadPayslip();
    this.getAllSlipData();
  }

  getDefault() {
    var val = new HrPayslipRunDefaultGet();
    val.state = 'draft';
    this.hrPaysliprunService.default(val).subscribe((result: any) => {
      this.paysliprun = result;
      this.myForm.patchValue(result);
      this.myForm.get('dateStartObj').setValue(new Date(result.dateStart));
      this.myForm.get('dateEndObj').setValue(new Date(result.dateEnd));
      this.total = 0;
    });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadPayslip();
  }

  onSave() {
    if (!this.myForm.valid) {
      return;
    }

    var val = this.myForm.value;
    val.dateStart = this.intlService.formatDate(val.dateStartObj, 'yyyy-MM-ddTHH:mm');
    val.dateEnd = this.intlService.formatDate(val.dateEndObj, 'yyyy-MM-ddTHH:mm');
    if (!this.itemId) {
      this.hrPaysliprunService.create(val)
        .subscribe((result: any) => {
          this.router.navigateByUrl('hr/payslip-run/form?id=' + result.id)
          this.notificationService.show({
            content: 'Lưu thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
        }, err => {
          console.log(err);
        })
    } else {
      this.hrPaysliprunService.update(this.itemId, val)
        .subscribe(() => {
          this.notificationService.show({
            content: 'Lưu thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
        }, err => {
          console.log(err);
        })
    }
  }

  addPayslips() {
    if (!this.myForm.valid) {
      return;
    }

    var val = this.myForm.value;
    val.dateStart = this.intlService.formatDate(val.dateStartObj, 'yyyy-MM-ddTHH:mm');
    val.dateEnd = this.intlService.formatDate(val.dateEndObj, 'yyyy-MM-ddTHH:mm');

    if (!this.itemId) {
      this.hrPaysliprunService.create(val)
        .subscribe((result: any) => {
          this.router.navigate(['hr/payslip-run/form'], {
            queryParams: {
              id: result['id']
            },
          });
          this.openConfirmDialog(result.id);
        }, err => {
          console.log(err);
        })
    } else {
      this.openConfirmDialog(this.itemId)
    }


  }

  openConfirmDialog(id) {
    let modalRef = this.modalService.open(HrPayslipRunConfirmDialogComponent, { size: "lg", windowClass: "o_technical_modal", keyboard: false, backdrop: "static" });
    modalRef.componentInstance.title = "Tạo phiếu lương";
    modalRef.componentInstance.id = id;
    modalRef.result.then(
      () => {
        this.reloadData();

      },
      () => { }
    );
  }

  actionDone() {
    if (this.itemId) {
      this.hrPaysliprunService.actionDone([this.itemId]).subscribe(() => {
        this.reloadData();
      });
    }
  }

  createNew() {
    this.router.navigate(['hr/payslip-run/form']);
    this.getDefault();
  }

  removelItem(id) {
    const modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu lương';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.hrPayslipService.delete(id).subscribe(res => {
        this.reloadData();
      });
    });
  }

  loadPayslip() {
    if (this.itemId) {
      var val = new HrPayslipPaged();
      val.payslipRunId = this.itemId;
      val.limit = this.limit;
      val.offset = this.skip;

      this.hrPayslipService.getPaged(val).pipe(
        map(response => (<GridDataResult>{                 
          data: response.items,
          total: response.totalItems,
        }))
      ).subscribe(res => {
        this.payslipGridData = res;
        this.loading = false;
      }, err => {
        console.log(err);
        this.loading = false;
      });
    }
  }

   getAllSlipData() {   
    if(this.itemId){
      var val = new HrPayslipPaged();
      val.payslipRunId = this.itemId;
      this.hrPayslipService.getPaged(val).subscribe(res => {
        this.total = res.items.map(x => x.totalAmount).reduce((a, b) => a + b, 0);
        this.loading = false;
      }, err => {
        console.log(err);
        this.loading = false;
      });
    }else{
      this.total = 0;
    }  
  }



  get f() {
    return this.myForm.controls;
  }

  convertState(state) {
    switch (state) {
      case 'verify':
        return 'Đang xử lý';
      case 'done':
        return 'Hoàn thành';
      case 'draft':
        return 'Nháp';
    }
  }

  detailItem(id) {
    this.router.navigateByUrl('hr/payslips/edit/' + id);
  }


}
