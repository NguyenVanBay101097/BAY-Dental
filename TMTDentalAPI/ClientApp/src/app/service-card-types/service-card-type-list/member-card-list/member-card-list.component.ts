import { Component, Inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
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
    private cardService: ServiceCardTypeService,
    private notificationService: NotificationService,
    private modalService: NgbModal
  ) {this.pagerSettings = config.pagerSettings; }

  ngOnInit(): void {
    this.loadDataFromApi();
  }

  loadDataFromApi(){

  }

  createCardLevel(){
    this.router.navigate(['card-types/member-cards/form']);
  }

  pageChange(event: PageChangeEvent){

  }

  editItem(item){

  }

  deleteItem(item){

  }

}
