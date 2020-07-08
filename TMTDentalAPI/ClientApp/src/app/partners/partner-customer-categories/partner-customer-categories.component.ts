import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent, SelectableSettings, SelectAllCheckboxState } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { PartnerCategoryPaged, PartnerCategoryService } from 'src/app/partner-categories/partner-category.service';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ActivatedRoute } from '@angular/router';
import { PartnerService, PartnerAddRemoveTags } from '../partner.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-partner-customer-categories',
  templateUrl: './partner-customer-categories.component.html',
  styleUrls: ['./partner-customer-categories.component.css']
})
export class PartnerCustomerCategoriesComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;

  searchUpdate = new Subject<string>();

  partnerId: string;
  partnerCategories: any;

  search_partnerCategory: string;
  value_partnerCategory: any;

  selectedPartnerCategories: string[] = [];

  showBtnManipulation: boolean = false;

  constructor(private partnerCategoryService: PartnerCategoryService,
    private modalService: NgbModal, private activeRoute: ActivatedRoute, 
    private partnerService: PartnerService, private notificationService: NotificationService) { }

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
      this.notificationService.show({
        content: 'Thêm nhãn thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
    });
  }

  deletePartnerCategory(dataItem) {
    var val = new PartnerAddRemoveTags();
    val.id = this.partnerId; 
    val.tagIds = [dataItem.id];

    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa: Nhãn';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa nhãn này?';
    modalRef.result.then(() => {
        this.partnerService.removeTags(val).subscribe(() => {
          this.loadDataFromApi();
          for (let i = 0; i < this.selectedPartnerCategories.length; i++) {
            if (this.selectedPartnerCategories[i] == dataItem.id) 
              this.selectedPartnerCategories.splice(i, 1);
          }
          if (this.selectedPartnerCategories.length == 0)
            this.showBtnManipulation = false;
          this.notificationService.show({
            content: 'Xóa nhãn thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
        });
      }, () => {
    });

  }

  deletePartnerCategories() {
    var val = new PartnerAddRemoveTags();
    val.id = this.partnerId; 
    val.tagIds = this.selectedPartnerCategories;
    
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa: Nhãn';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa nhãn này?';
    modalRef.result.then(() => {
        this.partnerService.removeTags(val).subscribe(() => {
          this.loadDataFromApi();
          this.selectedPartnerCategories = [];
          this.showBtnManipulation = false;
          this.notificationService.show({
            content: 'Xóa nhãn thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
        });
      }, () => {
    });
  }
}
