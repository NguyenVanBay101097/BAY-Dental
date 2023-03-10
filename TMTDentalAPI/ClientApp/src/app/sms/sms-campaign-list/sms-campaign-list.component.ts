import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { SmsCampaignCrUpComponent } from '../sms-campaign-cr-up/sms-campaign-cr-up.component';
import { SmsCampaignService, SmsCampaignPaged } from '../sms-campaign.service';
import { SmsMessageDialogComponent } from '../sms-message-dialog/sms-message-dialog.component';

@Component({
  selector: 'app-sms-campaign-list',
  templateUrl: './sms-campaign-list.component.html',
  styleUrls: ['./sms-campaign-list.component.css']
})
export class SmsCampaignListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  offset = 0;
  loading = false;
  searchUpdate = new Subject<string>();
  search: string;
  state: string;
  filterStatus = [
    { name: "Đang chạy", value: "running" },
    { name: "Chưa chạy", value: "draft" },
    { name: "Kết thúc", value: "shutdown" },
  ];

  constructor(
    private modalService: NgbModal,
    private smsCampaignService: SmsCampaignService,
    private router: Router,
  ) { }

  ngOnInit() {
    this.loadDefaultCampaign();
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.offset = 0;
        this.loadDataFromApi();
      });

  }

  loadDefaultCampaign() {
    this.smsCampaignService.getDefaultCampaign().subscribe(
      () => { }
    )
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new SmsCampaignPaged();
    val.limit = this.limit;
    val.offset = this.offset;
    val.search = this.search || "";
    val.state = this.state || '';
    this.smsCampaignService.getPaged(val).pipe(
      map(
        (response: any) =>
          <GridDataResult>{
            data: response.items,
            total: response.totalItems,
          }
      )
    )
      .subscribe(
        (res) => {
          this.gridData = res;
          this.loading = false;
        },
        (err) => {
          console.log(err);
          this.loading = false;
        }
      );
  }

  pageChange(event): void {
    this.offset = event.skip;
    this.loadDataFromApi();
  }

  createCampaign() {
    const modalRef = this.modalService.open(SmsCampaignCrUpComponent, { size: 'md', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Thêm chiến dịch';
    modalRef.result.then((val) => {
      this.loadDataFromApi();
    })
  }

  createMessage() {
    const modalRef = this.modalService.open(SmsMessageDialogComponent, { size: 'md', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Tạo tin nhắn';
    modalRef.componentInstance.templateTypeTab = "partnerCampaign";
    modalRef.result.then((val) => {
      this.loadDataFromApi();
    })
  }

  onStatusChange(event) {
    this.state = event ? event.value : '';
    this.offset = 0;
    this.loadDataFromApi();
  }

  cellClick(item) {
    this.router.navigate(['/sms/campaign', item.id]);
  }
}
