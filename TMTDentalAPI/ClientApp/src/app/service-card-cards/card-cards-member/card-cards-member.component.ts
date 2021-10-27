import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { CardCardFilter } from 'src/app/card-cards/card-card.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { ServiceCardCardService } from '../service-card-card.service';

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
  constructor(
    private modalService: NgbModal,
    private serviceCardsService: ServiceCardCardService,
    private notifyService: NotifyService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit(): void {
    var user_info = localStorage.getItem('user_info');
    if (user_info) {
      var userInfo = JSON.parse(user_info);
      this.partnerId = userInfo.id;
    }
    this.loadDataFromApi();
  }

  loadDataFromApi(){
    var val = new CardCardFilter();
    val.partnerId = this.partnerId;
  }

  createItem(){

  }

  exportExcelFile(){

  }

  importExcelFile(){
     
  }

  onPageChange(event: PageChangeEvent){
    this.limit = event.take;
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  editItem(item){

  }

  deleteItem(item){

  }
}
