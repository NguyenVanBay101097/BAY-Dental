import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { SmsManualDialogComponent } from '../sms-manual-dialog/sms-manual-dialog.component';
import { SmsTemplateService } from '../sms-template.service';

@Component({
  selector: 'app-sms-birthday-form-manual',
  templateUrl: './sms-birthday-form-manual.component.html',
  styleUrls: ['./sms-birthday-form-manual.component.css']
})
export class SmsBirthdayFormManualComponent implements OnInit {

  gridData: any;
  filteredSMSAccount: any[];
  filteredTemplate: any[];
  skip: number = 0;
  limit: number = 20;
  isBirthday: boolean = true;
  isRowSelected: any[];
  search: string = '';
  selectedIds: string[] = [];
  month: any;
  searchUpdate = new Subject<string>();
  months = [
    { name: 'Tháng 1', value: 1 },
    { name: 'Tháng 2', value: 2 },
    { name: 'Tháng 3', value: 3 },
    { name: 'Tháng 4', value: 4 },
    { name: 'Tháng 5', value: 5 },
    { name: 'Tháng 6', value: 6 },
    { name: 'Tháng 7', value: 7 },
    { name: 'Tháng 8', value: 8 },
    { name: 'Tháng 9', value: 9 },
    { name: 'Tháng 10', value: 10 },
    { name: 'Tháng 11', value: 11 },
    { name: 'Tháng 12', value: 12 },
  ]
  constructor(
    private partnerService: PartnerService,
    private smsTemplateService: SmsTemplateService,
    private modalService: NgbModal,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.skip = 0;
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    var val = new PartnerPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.customer = true;
    val.supplier = false;
    val.month = this.month;
    val.isBirthday = this.isBirthday;
    this.partnerService.getCustomerBirthDay(val)
      .subscribe((res: any[]) => {
        this.gridData = res;
      }, err => {
        console.log(err);
      }
      )
  }

  searchSmsTemplate(q?: string) {
    return this.smsTemplateService.getAutoComplete(q);
  }

  pageChange(event) {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  onSend() {
    if (this.selectedIds.length == 0) {
      this.notify("chưa chọn khách hàng", false);
    }
    else {
      var modalRef = this.modalService.open(SmsManualDialogComponent, { size: "lg", windowClass: "o_technical_modal" });
      modalRef.componentInstance.title = "Tạo tin gửi";
      modalRef.componentInstance.ids = this.selectedIds ? this.selectedIds : [];
      modalRef.componentInstance.isBirthDayManual = true;
      modalRef.result.then(
        result => {

        }
      )
    }

  }
  notify(title, isSuccess = true) {
    this.notificationService.show({
      content: title,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: isSuccess ? 'success' : 'error', icon: true },
    });
  }

  selectMonthChange(event) {
    this.month = event.target.value;
    if (this.month != '0') {
      this.isBirthday = false;
    } else {
      this.isBirthday = true;
      this.month = '';
    }
    this.skip = 0;
    this.loadDataFromApi();
  }
}
