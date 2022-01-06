import { Component, Input, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { CommentCuDialogComponent } from 'src/app/mail-messages/comment-cu-dialog/comment-cu-dialog.component';
import { MailMessageSubTypeService } from 'src/app/mail-messages/mail-message-subType.service';
import { LogForPartnerRequest, LogForPartnerResponse, MailMessageService, TimeLineLogForPartnerResponse } from 'src/app/mail-messages/mail-message.service';
import { AppointmentCreateUpdateComponent } from 'src/app/shared/appointment-create-update/appointment-create-update.component';

@Component({
  selector: 'app-partner-activity-history',
  templateUrl: './partner-activity-history.component.html',
  styleUrls: ['./partner-activity-history.component.css']
})
export class PartnerActivityHistoryComponent implements OnInit {

  @Input() partnerId: string;
  filter: LogForPartnerRequest = new LogForPartnerRequest();
  listMessages: TimeLineLogForPartnerResponse[] = [];
  today = this.intl.formatDate(new Date(), "dd/MM/yyyy");
  listMessageSubType = [];
  constructor(
    private intl: IntlService,
    private modalService: NgbModal,
    private messageService: MailMessageService,
    private messageSubTypeService: MailMessageSubTypeService
  ) { }

  ngOnInit(): void {
    this.initFilterData();
    this.loadListMessageSubType();
    this.loadDataFromApi();
  }

  loadListMessageSubType(){
    this.messageSubTypeService.get().subscribe(res => {
      this.listMessageSubType = res;
    })
  }

  loadDataFromApi(){
    var val = Object.assign({}, this.filter);
    val.dateFrom = val.dateFrom ? this.intl.formatDate(val.dateFrom, 'yyyy-MM-dd') : null;
    val.dateTo = val.dateTo ? this.intl.formatDate(val.dateTo, 'yyyy-MM-dd') : null;
    val.threadModel = 'res.partner';
    val.threadId = this.partnerId; 
    this.messageService.getLogsForPartner(val).subscribe((res:TimeLineLogForPartnerResponse[]) => {
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
  }

  searchChangeDate(value: any) {
    this.filter.dateFrom = value.dateFrom;
    this.filter.dateTo = value.dateTo;
    this.loadDataFromApi();
  }

  onCreateAppointment(){
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

  onCreateComment(){
    const modalRef = this.modalService.open(CommentCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
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

  onDeleteMessage(item) {

  }

  onChangeSubType(e) {
    this.filter.SubtypeId = e ? e.id : '';
    this.loadDataFromApi();
  }
}
