import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { LaboFinnishLineImportComponent } from 'src/app/labo-finish-lines/labo-finnish-line-import/labo-finnish-line-import.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { LaboBridgeCuDialogComponent } from '../labo-bridge-cu-dialog/labo-bridge-cu-dialog.component';
import { LaboBridgeBasic, LaboBridgePaged, LaboBridgeService } from '../labo-bridge.service';

@Component({
  selector: 'app-labo-bridge-list',
  templateUrl: './labo-bridge-list.component.html',
  styleUrls: ['./labo-bridge-list.component.css']
})
export class LaboBridgeListComponent implements OnInit {
  constructor(
    private laboBridgeService: LaboBridgeService, private notificationService: NotificationService,
    private modalService: NgbModal,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;

  search: string;
  searchCategId: string;
  searchUpdate = new Subject<string>();

  opened = false;

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
    this.loading = true;
    var val = new LaboBridgePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';

    this.laboBridgeService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
      console.log(this.gridData);
      
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  onPageChange(event: PageChangeEvent) {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  createItem() {
    let modalRef = this.modalService.open(LaboBridgeCuDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Th??m ki???u nh???p Labo';
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  editItem(item: LaboBridgeBasic) {
    let modalRef = this.modalService.open(LaboBridgeCuDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'S???a ki???u nh???p Labo';
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  notify(Style, Content) {
    this.notificationService.show({
      content: Content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: Style, icon: true }
    });
  }

  deleteItem(item: LaboBridgeBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'X??a ki???u nh???p Labo';
    modalRef.componentInstance.body = `B???n ch???c ch???n mu???n x??a ki???u nh???p Labo ${item.name}?`;
    modalRef.result.then(() => {
      this.laboBridgeService.delete(item.id).subscribe(() => {
        this.notify('success','X??a th??nh c??ng');
        this.loadDataFromApi();
      });
    }, () => {
    });
  }

  onImport() {
    let modalRef = this.modalService.open(LaboFinnishLineImportComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static',scrollable:true });
    modalRef.componentInstance.title = 'Import excel';
    modalRef.componentInstance.type = 'labo_bridge';
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }
}
