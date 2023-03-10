import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { PartnerCategoryPaged, PartnerCategoryService } from 'src/app/partner-categories/partner-category.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { PartnerAddRemoveTags, PartnerService } from '../partner.service';

@Component({
  selector: 'app-partner-customer-categories',
  templateUrl: './partner-customer-categories.component.html',
  styleUrls: ['./partner-customer-categories.component.css']
})
export class PartnerCustomerCategoriesComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;

  searchUpdate = new Subject<string>();

  partnerId: string;
  partnerCategories: any;

  search_partnerCategory: string;
  value_partnerCategory: any;

  selectedPartnerCategories: string[] = [];

  showBtnManipulation: boolean = false;

  constructor(private partnerCategoryService: PartnerCategoryService,
    private modalService: NgbModal, 
    private activeRoute: ActivatedRoute, 
    private partnerService: PartnerService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.partnerId = this.activeRoute.parent.snapshot.paramMap.get('id');
    this.loadPartnerCategories();
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadPartnerCategories();
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new PartnerCategoryPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.partnerId = this.partnerId || '';

    this.partnerCategoryService.getPaged(val).pipe(
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

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  onSelectedKeysChange(value) {
    if (value.length > 0) {
      this.showBtnManipulation = true;
    } else {
      this.showBtnManipulation = false;
    }
  }

  handleFilter(value) {
    this.search_partnerCategory = value;
  }

  loadPartnerCategories() {
    var val = new PartnerCategoryPaged();
    val.limit = 20;
    val.offset = 0;
    val.search = this.search_partnerCategory || '';

    this.partnerCategoryService.getPaged(val).subscribe(res => {
      this.partnerCategories = res.items;
    }, err => {
      console.log(err);
    })
  }

  addPartnerCategories() {
    var val = new PartnerAddRemoveTags();
    val.id = this.partnerId; 
    val.tagIds = [];
    this.value_partnerCategory.forEach(element => {
      val.tagIds.push(element.id);
    });
    this.partnerService.addTags(val).subscribe(() => {
      this.loadDataFromApi();
      this.value_partnerCategory = [];
    });
  }

  deletePartnerCategory(dataItem) {
    var val = new PartnerAddRemoveTags();
    val.id = this.partnerId; 
    val.tagIds = [dataItem.id];

    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'X??a: Nh??n';
    modalRef.componentInstance.body = 'B???n ch???c ch???n mu???n x??a nh??n n??y?';
    modalRef.result.then(() => {
        this.partnerService.removeTags(val).subscribe(() => {
          this.loadDataFromApi();
          for (let i = 0; i < this.selectedPartnerCategories.length; i++) {
            if (this.selectedPartnerCategories[i] == dataItem.id) 
              this.selectedPartnerCategories.splice(i, 1);
          }
          if (this.selectedPartnerCategories.length == 0)
            this.showBtnManipulation = false;
        });
      }, () => {
    });

  }

  deletePartnerCategories() {
    var val = new PartnerAddRemoveTags();
    val.id = this.partnerId; 
    val.tagIds = this.selectedPartnerCategories;
    
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'X??a: Nh??n';
    modalRef.componentInstance.body = 'B???n ch???c ch???n mu???n x??a nh??n n??y?';
    modalRef.result.then(() => {
        this.partnerService.removeTags(val).subscribe(() => {
          this.loadDataFromApi();
          this.selectedPartnerCategories = [];
          this.showBtnManipulation = false;
        });
      }, () => {
    });
  }
}
