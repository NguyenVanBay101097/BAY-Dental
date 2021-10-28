import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { result } from 'lodash';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { CardCardFilter, CardCardPaged, CardCardResponse, CardCardService } from 'src/app/card-cards/card-card.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { CardCardsMemberCuDialogComponent } from '../card-cards-member-cu-dialog/card-cards-member-cu-dialog.component';
import { ServiceCardCardsPreferentialImportDialogComponent } from '../service-card-cards-preferential-import-dialog/service-card-cards-preferential-import-dialog.component';

@Component({
  selector: 'app-card-cards-member',
  templateUrl: './card-cards-member.component.html',
  styleUrls: ['./card-cards-member.component.css']
})
export class CardCardsMemberComponent implements OnInit {
  pagerSettings: any;
  searchUpdate = new Subject<string>();
  search: string = '';
  skip = 0;
  limit = 20;
  loading = false;
  gridData: GridDataResult;
  partnerId: string;
  data: CardCardResponse[] = [];
  constructor(
    private modalService: NgbModal,
    private cardCardsService: CardCardService,
    private notifyService: NotifyService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit(): void {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });
    this.loadDataFromApi();
  }

  loadDataFromApi(){
    var val = new CardCardPaged();
    val.search = this.search;
    val.limit = this.limit;
    val.offset = this.skip;
    this.cardCardsService.getPaged(val).subscribe(result => {
      this.gridData = {
        data: result.items,
        total: result.totalItems
      }
    })
  }

  createItem(){
    const modalRef = this.modalService.open(CardCardsMemberCuDialogComponent, { scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Tạo thẻ thành viên";
    modalRef.result.then(result => {
      this.loadDataFromApi();
    }, () => { });
  }

  exportExcelFile(){
    var val = new CardCardPaged();
    val.search = this.search;
    this.cardCardsService.excelServerExport(val).subscribe((rs) => {
      let filename = "the_thanh_vien";
      let newBlob = new Blob([rs], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });

      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        // For Firefox it is necessary to delay revoking the ObjectURL
        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }

  importExcelFile(){
    const modalRef = this.modalService.open(ServiceCardCardsPreferentialImportDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.title = 'Import excel';
    modalRef.componentInstance.type = 'card_cards_member';

    modalRef.result.then((result) => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  onPageChange(event: PageChangeEvent){
    this.limit = event.take;
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  editItem(item){
    const modalRef = this.modalService.open(CardCardsMemberCuDialogComponent, { scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Chỉnh sửa thẻ " + item.barcode;
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(result => {
      this.loadDataFromApi();
    }, () => { });
  }

  deleteItem(item){
    const modalRef = this.modalService.open(ConfirmDialogComponent, { size:'sm', scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Xóa thẻ thành viên";
    modalRef.componentInstance.body = "Bạn có chắc chắn muốn xóa thẻ thành viên này ?";

    modalRef.result.then(result => {
      if (item.state == 'draft'){
        this.cardCardsService.delete(item.id).subscribe(()=>{
          this.notifyService.notify('success', 'Xóa thẻ thành công');
          this.loadDataFromApi();
        },() => {
         })
      }
      else {
          this.notifyService.notify('error', 'Thẻ thành viên đã kích hoạt không thể xóa');
      }
    }, () => {
     });
  }
}
