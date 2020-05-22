import { Component, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TcareService, TCareCampaignPaged } from '../tcare.service';
import { Router } from '@angular/router';
import { TcareCampaignCreateDialogComponent } from '../tcare-campaign-create-dialog/tcare-campaign-create-dialog.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-tcare-campaign-list',
  templateUrl: './tcare-campaign-list.component.html',
  styleUrls: ['./tcare-campaign-list.component.css']
})
export class TcareCampaignListComponent implements OnInit {

  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  title = 'Kịch bản';
  loading = false;
  opened = false;
  searchUpdate = new Subject<string>();
  search: string;

  constructor(
    private modalService: NgbModal,
    private tcareService: TcareService,
    private router: Router
  ) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new TCareCampaignPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    this.tcareService.getPaged(val).pipe(
      map((response: any) =>
        (<GridDataResult>{
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

  createItem() {
    let modalRef = this.modalService.open(TcareCampaignCreateDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: ' + this.title;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  editItem(item) {
    this.router.navigateByUrl(`tcare/${item.id}`);
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa: ' + this.title;

    modalRef.result.then(() => {
      this.tcareService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }

}
