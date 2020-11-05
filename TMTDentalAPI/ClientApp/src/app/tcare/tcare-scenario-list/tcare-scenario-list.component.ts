import { Component, OnInit } from '@angular/core';
import { PageChangeEvent, GridDataResult } from '@progress/kendo-angular-grid';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { TcareService, TCareScenarioPaged } from '../tcare.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { TcareScenarioCrDialogComponent } from '../tcare-scenario-cr-dialog/tcare-scenario-cr-dialog.component';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-tcare-scenario-list',
  templateUrl: './tcare-scenario-list.component.html',
  styleUrls: ['./tcare-scenario-list.component.css']
})
export class TcareScenarioListComponent implements OnInit {

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
    private router: Router,
    private notificationService: NotificationService
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
    var val = new TCareScenarioPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    this.tcareService.getPagedScenario(val).pipe(
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
    this.router.navigateByUrl("tcare/scenarios/form");
    // let modalRef = this.modalService.open(TcareScenarioCrDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    // modalRef.componentInstance.title = 'Thêm: ' + this.title;
    // modalRef.result.then((result: any) => {
    //   this.router.navigateByUrl(`tcare/scenario/${result.id}`);
    // }, () => {
    // });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  editItem(item) {
    this.router.navigateByUrl("tcare/scenarios/form?id=" + item.id);
    //this.router.navigateByUrl(`tcare/scenario/${item.id}`);
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa: ' + this.title;

    modalRef.result.then(() => {
      this.tcareService.deleteScenario(item.id).subscribe(() => {
        this.loadDataFromApi();
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }

  actionStart(item: any) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Chạy kịch bản';

    modalRef.result.then(() => {
      this.tcareService.actionStartScenario([item.id]).subscribe((res: any) => {
        this.notificationService.show({
          content: 'thành công!',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true },
        });
      });
    }, () => {
    });
   
  }
}
