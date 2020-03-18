import { Component, OnInit } from '@angular/core';
import { MarketingCampaignService, MarketingCampaignPaged } from 'src/app/marketing-campaigns/marketing-campaign.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-facebook-page-marketing-campaign-list',
  templateUrl: './facebook-page-marketing-campaign-list.component.html',
  styleUrls: ['./facebook-page-marketing-campaign-list.component.css']
})
export class FacebookPageMarketingCampaignListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;

  constructor(private marketingCampaignService: MarketingCampaignService,
    private router: Router, private modalService: NgbModal) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new MarketingCampaignPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    this.marketingCampaignService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
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
    this.loadDataFromApi();
  }

  stateGet(state) {
    switch (state) {
      case 'running':
        return 'Đang chạy';
      case 'stopped':
        return 'Đã dừng';
      default:
        return 'Mới';
    }
  }

  editItem(item: any) {
    this.router.navigate(['/facebook-management/campaigns/form'], { queryParams: { id: item.id } });
  }

  deleteItem(item: any) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa chiến dịch';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.marketingCampaignService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      });
    });
  }
}
