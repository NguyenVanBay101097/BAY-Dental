import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { TimeKeepingService, ChamCongBasic, ChamCongSave } from '../time-keeping.service';
import { EmployeeSimple, EmployeeBasic } from 'src/app/employees/employee';
import { IntlService } from '@progress/kendo-angular-intl';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { offset } from '@progress/kendo-date-math';
import { WorkEntryType, WorkEntryTypeService, WorkEntryTypePage } from 'src/app/work-entry-types/work-entry-type.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-time-keeping-setup-dialog',
  templateUrl: './time-keeping-setup-dialog.component.html',
  styleUrls: ['./time-keeping-setup-dialog.component.css']
})
export class TimeKeepingSetupDialogComponent implements OnInit {

  @ViewChild('workCbx', { static: true }) workCbx: ComboBoxComponent;
  @ViewChild('timeOutRef', { static: false }) timeOutRef: ElementRef;
  formGroup: FormGroup;
  id: string;
  dateTime: Date;
  timeIn: any;
  today: Date = new Date();
  timeOut: any;
  title: string;
  employee: EmployeeBasic;
  filterdWorks: WorkEntryType[] = [];
  chamCong: ChamCongBasic = new ChamCongBasic();
  error = false;
  constructor(
    private activeModal: NgbActiveModal,
    private fb: FormBuilder,
    private timeKeepingServive: TimeKeepingService,
    private intl: IntlService,
    private workEntryTypeService: WorkEntryTypeService,
    private notificationService: NotificationService,
    private showErrorService: AppSharedShowErrorService,
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      timeIn: [null, Validators.required],
      timeOut: null,
      workEntryTypeId: [null, Validators.required]
    })


    if (this.id) {
      this.loadFormApi();
    }
    // if (this.today.getDate() > this.dateTime.getDate()) {
    //   this.formGroup.disable();
    // }
    this.loadWorkEntryType();
    this.workCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.workCbx.loading = true)),
      switchMap(value => this.searchWorkEntryType(value))
    ).subscribe(result => {
      this.filterdWorks = result.items;
      this.workCbx.loading = false;
    });

  }

  loadWorkEntryType() {
    this.searchWorkEntryType().subscribe(
      result => {
        this.filterdWorks = result.items;
        if (this.filterdWorks && this.filterdWorks.length > 0 && !this.chamCong.workEntryTypeId)
          this.formGroup.get('workEntryTypeId').patchValue(this.filterdWorks[0].id);
      }
    )
  }

  searchWorkEntryType(search?: string) {
    var page = new WorkEntryTypePage();
    page.limit = 20;
    page.offset = 0;
    page.filter = search ? search : '';
    return this.workEntryTypeService.getPaged(page);
  }

  loadFormApi() {
    this.timeKeepingServive.get(this.id).subscribe(
      result => {
        this.chamCong = result;
        this.formGroup.get('workEntryTypeId').patchValue(this.chamCong.workEntryTypeId);
        if (this.chamCong && this.chamCong.timeIn) {
          this.formGroup.get('timeIn').setValue(new Date(this.chamCong.timeIn));
          this.timeIn = new Date(this.chamCong.timeIn);
        }
        if (this.chamCong && this.chamCong.timeOut) {
          this.formGroup.get('timeOut').setValue(new Date(this.chamCong.timeOut));
          this.timeOut = new Date(this.chamCong.timeOut);
        }
      }
    )
  }

  checkTimeIn() {
    var val = {
      employeeId: this.employee.id,
      date: this.intl.formatDate(this.dateTime, "yyyy-MM-dd"),
    }
    return this.timeKeepingServive.getLastChamCong(val);
  }

  changeTimeIn(time: Date) {
    if (time) {
      this.timeIn = new Date(this.dateTime.getFullYear(), this.dateTime.getMonth(), this.dateTime.getDate(), time.getHours(), time.getMinutes());
      // if (this.timeOut && this.timeIn > this.timeOut) {
      //   alert("Thời gian vào không được lớn hơn thời gian ra. Vui lòng nhập lại thời gian vào");
      //   this.error = true;
      //   return;
      // }
      // this.checkTimeIn().subscribe(
      //   result => {
      //     if (result.timeOut && this.timeIn < new Date(result.timeOut)) {
      //       this.error = true;
      //       alert(`Giờ vào của chấm công tiếp theo phải lơn hơn giờ ra của chấm công cũ (${this.intl.formatDate(new Date(result.timeOut), "HH:mm")}) trong ngày ${this.intl.formatDate(this.dateTime, "dd-MM-yyyy")}`);
      //     } else if (!result.timeOut) {
      //       this.error = true;
      //       alert("Bạn phải hoàn thành chấm công trước đó trước khi tạo 1 chấm công mới. Vui lòng hoàn thành và thao tác lại !")
      //     } else {
      //       this.error = false;
      //     }
      //   }
      // );
    }
    else {
      this.timeIn = null;
    }
  }

  changeTimeOut(time: Date) {
    if (time) {
      this.timeOut = new Date(this.dateTime.getFullYear(), this.dateTime.getMonth(), this.dateTime.getDate(), time.getHours(), time.getMinutes());
      // if (this.timeIn > this.timeOut) {
      //   this.error = true;
      //   alert("Thời gian ra không được nhỏ hơn thời gian vào. Vui lòng nhập lại thời gian ra");
      // } else {
      //   this.error = false;
      // }
    }
    else {
      this.timeOut = null;
    }
  }

  delete(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.body = `Bạn chắc chắn muốn xóa chấm công vào ${this.intl.formatDate(new Date(this.chamCong.timeIn), "EEEE dd/MM/yyyy")} của nhân viên ${this.employee.name}`;
    modalRef.componentInstance.title = "Xóa chấm công";
    modalRef.result.then(() => {
      this.timeKeepingServive.deleteChamCong(item.id).subscribe(
        () => {
          this.activeModal.close(this.employee.id);
        }
      )
    });
  }

  onSave() {
    if (!this.employee)
      return false;
    if (this.formGroup.invalid)
      return false;
    var val = new ChamCongSave();
    val.timeIn = this.timeIn ? this.intl.formatDate(this.timeIn, "yyyy-MM-ddTHH:mm") : '';
    val.timeOut = this.timeOut ? this.intl.formatDate(this.timeOut, "yyyy-MM-ddTHH:mm") : '';
    val.employeeId = this.employee.id;
    val.date = this.intl.formatDate(this.dateTime, "yyyy-MM-dd");
    val.workEntryTypeId = this.formGroup.get('workEntryTypeId').value;
    if (this.id) {
      this.timeKeepingServive.update(this.id, val).subscribe(
        x => {
          this.activeModal.close(this.employee.id);
        }, err => {
          this.showErrorService.show(err);
        }
      )
    }
    else {
      this.timeKeepingServive.create(val).subscribe(
        result => {
          this.activeModal.close(this.employee.id);
        }, err => {
          this.showErrorService.show(err);
        }
      )
    }
    // else if (this.today.getDate() == this.dateTime.getDate()) {
    //   this.timeKeepingServive.create(val).subscribe(
    //     result => {
    //       this.activeModal.close();
    //     }
    //   )
    // } else {
    //   this.activeModal.close();
    // }
  }

}
