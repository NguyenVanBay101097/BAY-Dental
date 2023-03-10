import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { map } from 'rxjs/operators';
import { AppointmentBasic, AppointmentPaged } from 'src/app/appointment/appointment';
import { AppointmentVMService } from 'src/app/appointment/appointment-vm.service';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { AuthService } from 'src/app/auth/auth.service';
import { AppointmentCreateUpdateComponent } from 'src/app/shared/appointment-create-update/appointment-create-update.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { PrintService } from 'src/app/shared/services/print.service';

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
  pagerSettings: any;
  loading = false;

  constructor(
    private appointmentService: AppointmentService,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private activeRoute: ActivatedRoute,
    private authService: AuthService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig,
    private printService: PrintService
  ) { this.pagerSettings = config.pagerSettings }

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
    val.companyId = this.authService.userInfo.companyId;

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
    this.limit = event.take;
    this.loadData();
  }
  
  deleteAppointment(appointment: AppointmentBasic) {
    const modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'X??a l???ch h???n';
    modalRef.componentInstance.body = 'B???n ch???c ch???n mu???n x??a l???ch h???n n??y?';
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
        return 'Ch??? kh??m';
      case 'examination':
        return '??ang kh??m';
      case 'done':
        return '???? ?????n';
      case 'cancel':
        return 'H???y h???n';
      case 'all':
        return 'T???ng h???n';
      default:
        return '??ang h???n';
    }
  }

  onPrint(id) {
    this.appointmentService.print(id).subscribe((res: any) => {
      this.printService.printHtml(res.html);
    });
  }
}
