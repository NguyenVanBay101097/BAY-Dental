import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { result } from 'lodash';
import * as moment from 'moment';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
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
    this.cardCardsService.getPaged(val).pipe(
      map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe((res) => {
      this.gridData = res;
    });
  }

  createItem(){
    const modalRef = this.modalService.open(CardCardsMemberCuDialogComponent, { scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "T???o th??? th??nh vi??n";
    modalRef.result.then(result => {
      this.loadDataFromApi();
    }, () => { });
  }

  exportExcelFile(){
    var val = new CardCardPaged();
    val.search = this.search;
    let todayStr = moment(new Date()).format("YYYYMMDD");
    this.cardCardsService.excelServerExport(val).subscribe((rs) => {
      let filename = "The_thanh_vien"+todayStr;
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
    modalRef.componentInstance.isMemberCard = true;

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
    modalRef.componentInstance.title = "Ch???nh s???a th??? " + item.barcode;
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(result => {
      this.loadDataFromApi();
    }, () => { });
  }

  deleteItem(item){
    const modalRef = this.modalService.open(ConfirmDialogComponent, { size:'sm', scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "X??a th??? th??nh vi??n";
    modalRef.componentInstance.body = "B???n c?? ch???c ch???n mu???n x??a th??? th??nh vi??n n??y ?";

    modalRef.result.then(result => {
      if (item.state == 'draft'){
        this.cardCardsService.delete(item.id).subscribe(()=>{
          this.notifyService.notify('success', 'X??a th??? th??nh c??ng');
          this.loadDataFromApi();
        },() => {
         })
      }
      else {
          this.notifyService.notify('error', 'Th??? th??nh vi??n ???? k??ch ho???t kh??ng th??? x??a');
      }
    }, () => {
     });
  }
}
