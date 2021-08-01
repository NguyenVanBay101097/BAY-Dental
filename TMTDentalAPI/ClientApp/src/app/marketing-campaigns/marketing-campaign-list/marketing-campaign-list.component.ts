import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { MarketingCampaignService, MarketingCampaignPaged } from '../marketing-campaign.service';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-marketing-campaign-list',
  templateUrl: './marketing-campaign-list.component.html',
  styleUrls: ['./marketing-campaign-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})

export class MarketingCampaignListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  opened = false;
  search: string;
  stateFilter: string;
  searchUpdate = new Subject<string>();

  selectedIds: string[] = [];

  constructor(private modalService: NgbModal, private router: Router, 
    private marketingCampaignService: MarketingCampaignService, 
    private notificationService: NotificationService) { }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      }
    );
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new MarketingCampaignPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    if (this.stateFilter) {
      val.state = this.stateFilter;
    }
    console.log(val);
    this.marketingCampaignService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      console.log(this.gridData);
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
        return 'Nháp';
    }
  }

  createItem() {
    this.router.navigate(['/marketing-campaigns/form']);
  }

  editItem(item: any) {
    this.router.navigate(['/marketing-campaigns/form'], { queryParams: { id: item.id } });
  }

  deleteItem(item: any) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa chiến dịch marketing';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.marketingCampaignService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      });
    });
  }
}
