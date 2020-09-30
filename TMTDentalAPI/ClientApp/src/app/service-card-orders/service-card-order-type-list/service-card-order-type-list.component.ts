import { ViewChild } from '@angular/core';
import { Component, ElementRef, EventEmitter, HostListener, OnInit, Output } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { ServiceCardTypePaged } from 'src/app/service-card-types/service-card-type-paged';
import { ServiceCardTypeService } from 'src/app/service-card-types/service-card-type.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-service-card-order-type-list',
  templateUrl: './service-card-order-type-list.component.html',
  styleUrls: ['./service-card-order-type-list.component.css']
})
export class ServiceCardOrderTypeListComponent implements OnInit {
  @Output() typeCard = new EventEmitter<any>();
  @ViewChild('search', { static: true }) searchElement: ElementRef;

  gridData: GridDataResult;
  page: number = 1;
  pageSize: number = 10;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  typeCardList: any[] = [];
  listFilter: any[] = [];
  total: number;



  constructor(private cardTypeService: ServiceCardTypeService,
    private modalService: NgbModal) {

  }

  @HostListener('window:keydown', ['$event'])
  keyEvent(event: KeyboardEvent) {
    let charCode = (event.which) ? event.which : event.keyCode;
    if (charCode == 13) {
      this.onKeydownData();
    } else if (charCode == 113) {
      setTimeout(() => {
        this.searchElement.nativeElement.focus();
      }, 0);
    } else if (charCode == 46) {
      console.log('Delete Key Pressed');
    }

  }

  ngOnInit() {
    this.loadDataFromApi();
  }


  loadDataFromApi() {
    this.loading = true;
    var val = new ServiceCardTypePaged();
    val.limit = 100;
    val.search = this.search || '';

    this.cardTypeService.getPaged(val).subscribe((result: any) => {
      this.listFilter = result.items;
      this.typeCardList = result.items;
      this.total = result.totalItems;
    });
  }

  onChangeSearch(value) {
    if (value == '' || !value) {
      this.listFilter = this.typeCardList
    } else {
      this.listFilter = this.typeCardList.filter(x => this.RemoveVietnamese(x.name).includes(value));
    }
    return this.listFilter;
  }

  onKeydownData() {
    if ((this.listFilter.length - 1) == 0) {
      var val = this.listFilter[0];
      this.typeCard.emit(val);
    }
  }

  onClickData(val: any) {
    this.typeCard.emit(val);
  }

  pageChange(page): void {
    this.page = page;
    this.loadDataFromApi();
  }

  RemoveVietnamese(text) {
    text = text.toLowerCase().trim();
    text = text.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    text = text.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    text = text.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    text = text.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    text = text.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    text = text.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    text = text.replace(/đ/g, "d");
    // Some system encode vietnamese combining accent as individual utf-8 characters
    text = text.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, ""); // Huyền sắc hỏi ngã nặng 
    text = text.replace(/\u02C6|\u0306|\u031B/g, ""); // Â, Ê, Ă, Ơ, Ư
    return text;
  }


}
