import { Component, OnInit, Input } from '@angular/core';
import { PartnerBasic, PartnerSimple } from '../partner-simple';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CardCardCuDialogComponent } from 'src/app/card-cards/card-card-cu-dialog/card-card-cu-dialog.component';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { CardCardPaged, CardCardService } from 'src/app/card-cards/card-card.service';
import { map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-partner-cards-tab-pane',
  templateUrl: './partner-cards-tab-pane.component.html',
  styleUrls: ['./partner-cards-tab-pane.component.css']
})
export class PartnerCardsTabPaneComponent implements OnInit {
  @Input() partner: PartnerBasic;
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;

  constructor(private modalService: NgbModal, private cardCardService: CardCardService) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  addNewCard() {
    let modalRef = this.modalService.open(CardCardCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    var partner = new PartnerSimple();
    partner.id = this.partner.id;
    partner.name = this.partner.name;
    modalRef.componentInstance.partner = partner;

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new CardCardPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.partnerId = this.partner.id;

    this.cardCardService.getPaged(val).pipe(
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

  onPageClick(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  onEditClick(item) {
    let modalRef = this.modalService.open(CardCardCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.id = item.id;

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  onDeleteClick(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa thẻ thành viên';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.cardCardService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      });
    });
  }
}
