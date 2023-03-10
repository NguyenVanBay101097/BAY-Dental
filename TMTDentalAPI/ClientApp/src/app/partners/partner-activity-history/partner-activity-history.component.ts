import { Component, Input, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { of, Subject } from 'rxjs';
import { groupBy, map, mergeMap, switchMap, tap, toArray } from 'rxjs/operators';
import { CommentCuDialogComponent } from 'src/app/mail-messages/comment-cu-dialog/comment-cu-dialog.component';
import { MailMessageSubTypeService } from 'src/app/mail-messages/mail-message-subType.service';
import { LogForPartnerRequest, LogForPartnerResponse, MailMessageService, TimeLineLogForPartnerResponse } from 'src/app/mail-messages/mail-message.service';
import { AppointmentCreateUpdateComponent } from 'src/app/shared/appointment-create-update/appointment-create-update.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { GetPartnerThreadMessageResponse, GetThreadMessageForPartnerRequest, MailMessageFormat, PartnerService } from '../partner.service';

@Component({
  selector: 'app-partner-activity-history',
  templateUrl: './partner-activity-history.component.html',
  styleUrls: ['./partner-activity-history.component.css']
})
export class PartnerActivityHistoryComponent implements OnInit {
  public options: any = {
    locale: { format: 'YYYY-MM-DD' },
    alwaysShowCalendars: false,
  };
  @Input() partnerId: string;
  filter: GetThreadMessageForPartnerRequest = new GetThreadMessageForPartnerRequest();
  listMessages: { date: any, logs: MailMessageFormat[] }[] = [];
  today = this.intl.formatDate(new Date(), "dd/MM/yyyy");
  listMessageSubType = [];
  constructor(
    private intl: IntlService,
    private modalService: NgbModal,
    private messageService: MailMessageService,
    private messageSubTypeService: MailMessageSubTypeService,
    private notifyService: NotifyService,
    private partnerSerivce: PartnerService
  ) { }

  ngOnInit(): void {
    this.initFilterData();
    this.loadListMessageSubType();
    this.loadDataFromApi();
  }

  loadListMessageSubType() {
    this.messageSubTypeService.get().subscribe(res => {
      this.listMessageSubType = res;
    })
  }

  loadDataFromApi() {
    var val = Object.assign({}, this.filter);
    val.dateFrom = val.dateFrom ? this.intl.formatDate(val.dateFrom, 'yyyy-MM-dd') : null;
    val.dateTo = val.dateTo ? this.intl.formatDate(val.dateTo, 'yyyy-MM-dd') : null;
    this.partnerSerivce.getThreadMessages(this.partnerId, val).pipe(
      mergeMap(res => res.messages),
      groupBy((item: any) => {
        return item.date.split('T')[0];
      }),
      mergeMap(group => group.pipe(toArray())),
      map((arr: any) => ({date: arr[0].date, logs: arr.slice() })),
      toArray()
    ).subscribe((res: any) => {
      
      this.listMessages = res;
    });
  }

  formatDate(date) {
    return this.intl.formatDate(new Date(date), "dd/MM/yyyy");
  }

  initFilterData() {
    var date = new Date(), y = date.getFullYear(), m = date.getMonth();
    this.filter.dateFrom = new Date(y, m, 1);
    this.filter.dateTo = new Date(y, m + 1, 0);
    this.filter.limit = 0;
  }

  searchChangeDate(value: any) {
    this.filter.dateFrom = value.dateFrom;
    this.filter.dateTo = value.dateTo;
    this.loadDataFromApi();
  }

  onCreateAppointment() {
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.defaultVal = {
      partnerId: this.partnerId
    };

    modalRef.result.then(result => {
      if (result) {
        this.loadDataFromApi();
      }
    }, () => {
    });
  }

  onCreateComment() {
    const modalRef = this.modalService.open(CommentCuDialogComponent, { size: 'sm', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
    (modalRef.componentInstance.onSaveSubj as Subject<any>).pipe(
      switchMap((val: any) => {
        return this.partnerSerivce.createComment(this.partnerId, val);
      })
    ).subscribe(res => {
      modalRef.componentInstance.activeModal.close(true);
    });
    modalRef.result.then(result => {
      if (result) {
        this.loadDataFromApi();
      }
    }, () => {
    });
  }

  onDeleteMessage(item) {
    const modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'md', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "X??a ho???t ?????ng";
    modalRef.componentInstance.body = "B???n c?? ch???c ch???n mu???n x??a ho???t ?????ng n??y?";
    modalRef.result.then(result => {
      this.messageService.delete(item.id).subscribe(res => {
        this.notifyService.notify("success", "X??a ho???t ?????ng th??nh c??ng");
        this.loadDataFromApi();
      })
    }, () => {
    });

  }

  onChangeSubType(e) {
    this.filter.subtypeId = e ? e.id : '';
    this.loadDataFromApi();
  }
}
