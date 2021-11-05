import { Component, Inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { result } from 'lodash';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { CardTypeService } from 'src/app/card-types/card-type.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { ServiceCardTypeService } from '../../service-card-type.service';

@Component({
  selector: 'app-member-card-list',
  templateUrl: './member-card-list.component.html',
  styleUrls: ['./member-card-list.component.css']
})
export class MemberCardListComponent implements OnInit {
  searchUpdate = new Subject<string>();
  gridData: GridDataResult;
  pagerSettings: any;
  limit = 20;
  skip = 0;
  loading = false;
  search: string = '';
  constructor(
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig,
    private router: Router,
    private cardService: CardTypeService,
    private notificationService: NotificationService,
    private modalService: NgbModal
  ) {this.pagerSettings = config.pagerSettings; }

  ngOnInit(): void {
    this.loadDataFromApi();
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(() => this.loadDataFromApi());
    this.loadDataFromApi();
  }

  loadDataFromApi(){
    var val = {search: this.search, offset: this.skip, limit: this.limit};
    this.cardService.getPaged(val).subscribe(result => {
      this.gridData = {
        data: result.items,
        total: result.totalItems
      }
    })
  }

  createCardLevel(){
    this.router.navigate(['card-types/member-cards/form']);
  }

  pageChange(event: PageChangeEvent){

  }

  editItem(item){
    this.router.navigate(['card-types/member-cards/form'], { queryParams: { id: item.id } });
  }

  deleteItem(item){
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa hạng thẻ';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa hạng thẻ này?';
    modalRef.result.then(() => {
      this.cardService.delete(item.id).subscribe(()=> {
        this.notificationService.show({
          content: 'Xóa thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadDataFromApi();
      }, error => {
      })
    });
  }

}
