import { Component, OnInit, ViewChild } from '@angular/core';
import { DotKhamPaged } from 'src/app/dot-khams/dot-khams';
import { SaleOrderCreateDotKhamDialogComponent } from 'src/app/sale-orders/sale-order-create-dot-kham-dialog/sale-order-create-dot-kham-dialog.component';
import { AppointmentBasic, AppointmentSearchByDate, AppointmentPaged } from 'src/app/appointment/appointment';
import { PagedResult2 } from 'src/app/employee-categories/emp-category';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { forkJoin } from 'rxjs';
import { distinctUntilChanged, debounceTime, map } from 'rxjs/operators';
import { AppointmentVMService } from 'src/app/appointment/appointment-vm.service';
import { IntlService } from '@progress/kendo-angular-intl';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { NgbModal, NgbDropdownToggle } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { DotKhamService } from 'src/app/dot-khams/dot-kham.service';
import { Router, ActivatedRoute } from '@angular/router';
import * as _ from 'lodash';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { PartnerService } from '../partner.service';
import { PartnerDisplay } from '../partner-simple';
import { AppointmentCreateUpdateComponent } from 'src/app/shared/appointment-create-update/appointment-create-update.component';

@Component({
  selector: 'app-partner-customer-appointment',
  templateUrl: './partner-customer-appointment.component.html',
  styleUrls: ['./partner-customer-appointment.component.css'],
  providers: [AppointmentVMService]
})
export class PartnerCustomerAppointmentComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  id: string;
  skip = 0;
  loading = false;

  constructor(
    private appointmentService: AppointmentService,
    private modalService: NgbModal,
    private partnerService: PartnerService,
    private notificationService: NotificationService,
    private activeRoute: ActivatedRoute
  ) { }

  ngOnInit() {
    this.id = this.activeRoute.parent.snapshot.paramMap.get('id');
    this.loadData();
  }

  loadData() {
    this.loading = true;
    var val = new AppointmentPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.partnerId = this.id;

    this.appointmentService.getPaged(val).pipe(
      map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadData();
  }

  deleteAppointment(appointment: AppointmentBasic) {
    const modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa lịch hẹn';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa lịch hẹn này?';
    modalRef.result.then(() => {
      this.appointmentService.removeAppointment(appointment.id).subscribe(() => {
        this.loadData();
      });
    });
  }

  dialogAppointment(appointment: AppointmentBasic) {
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });

    if (appointment) {
      modalRef.componentInstance.appointId = appointment.id;
    } else {
      modalRef.componentInstance.defaultVal = {
        partnerId: this.id
      };
    }

    modalRef.result.then(() => {
      this.notify('success', 'Lưu thành công');
      this.loadData();
    }, () => {
    });
  }

  notify(type, content) {
    this.notificationService.show({
      content: content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: type, icon: true }
    });
  }

  stateGet(state) {
    switch (state) {
      case 'waiting':
        return 'Chờ khám';
      case 'examination':
        return 'Đang khám';
      case 'done':
        return 'Hoàn thành';
      case 'cancel':
        return 'Hủy hẹn';
      case 'all':
        return 'Tổng hẹn';
      default:
        return 'Đang hẹn';
    }
  }

}
