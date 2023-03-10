import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { HrJobCuDialogComponent } from '../hr-job-cu-dialog/hr-job-cu-dialog.component';
import { HrJobService, HrJobsPaged } from '../hr-job.service';

@Component({
  selector: 'app-hr-job-list',
  templateUrl: './hr-job-list.component.html',
  styleUrls: ['./hr-job-list.component.css']
})
export class HrJobListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;
  search: string = '';
  searchUpdate = new Subject<string>();
  constructor(
    private hrJobService: HrJobService,
    private modalService: NgbModal,
    private notifyService: NotifyService,
    private authService: AuthService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }


  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    let val = new HrJobsPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search;
    val.companyId = this.authService.userInfo.companyId;

    this.loading = true;
    this.hrJobService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe((res: any) => {
      this.gridData = res;

      this.loading = false;
    }, error => {
      console.log(error);
      this.loading = false;
    })
  }

  createItem() {
    const modalRef = this.modalService.open(HrJobCuDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Th??m ch???c v??? nh??n vi??n';
    modalRef.result.then((res: any) => {
      this.loadDataFromApi();
    })
  }

  editItem(item: any) {
    const modalRef = this.modalService.open(HrJobCuDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'S???a ch???c v??? nh??n vi??n';
    modalRef.componentInstance.id = item.id;
    modalRef.result.then((res: any) => {
      this.loadDataFromApi();
    })
  }

  deleteItem(item: any) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'X??a ch???c v??? nh??n vi??n';
    modalRef.componentInstance.body = 'B???n c?? ch???c ch???n mu???n x??a ch???c v??? nh??n vi??n?';
    modalRef.result.then(() => {
      this.hrJobService.delete(item.id).subscribe(() => {
        this.notifyService.notify('success','X??a th??nh c??ng');
        this.loadDataFromApi();
      },(error) => {
        console.log(error);
      });
    }, (error) => {
      console.log(error);
    });
  }

  pageChange(event: PageChangeEvent) {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }
}
