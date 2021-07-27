import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { HrPayslipPaged } from 'src/app/hrs/hr-payslip.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { TcareMessageTemplateCuDialogComponent } from '../tcare-message-template-cu-dialog/tcare-message-template-cu-dialog.component';
import { TCareMessageTemplatePaged, TCareMessageTemplateService } from '../tcare-message-template.service';

@Component({
  selector: 'app-tcare-message-template-list',
  templateUrl: './tcare-message-template-list.component.html',
  styleUrls: ['./tcare-message-template-list.component.css']
})
export class TcareMessageTemplateListComponent implements OnInit {

  gridData: GridDataResult = {
    data: [],
    total: 0
  };
  limit = 20;
  skip = 0;
  loading = false;
  searchUpdate = new Subject<string>();
  searchForm: FormGroup;
  constructor(
    private templateService: TCareMessageTemplateService,
    private modalService: NgbModal,
    private fb: FormBuilder,
    private router: Router
  ) { }

  ngOnInit() {
    this.searchForm = this.fb.group({
      search: null
    });

    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
  }

  get searchcontrol() {return this.searchForm.get('search'); }

  loadDataFromApi() {
    this.loading = true;
    const val = new TCareMessageTemplatePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.searchcontrol.value || '';

    this.templateService.getPaged(val).pipe(
      map((res: any) => ({
        data: res.items,
        total: res.totalItems,
      } as GridDataResult))
    )
      .subscribe(res => {
        this.gridData = res;
        this.loading = false;
      }, err => {
        this.loading = false;
      });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }
  createItem() {
    const modalRef = this.modalService.open(TcareMessageTemplateCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Tạo mẫu tin';
    modalRef.result.then((val) => {
        this.loadDataFromApi();
    });
  }

  editItem(dataitem) {
    const modalRef = this.modalService.open(TcareMessageTemplateCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Sửa mẫu tin';
    modalRef.componentInstance.id = dataitem.id;
    modalRef.result.then(() => {
        this.loadDataFromApi();
    });
  }

  deleteItem(dataitem) {
    const modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa mẫu tin';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.templateService.delete(dataitem.id).subscribe(res => {
        this.loadDataFromApi();
      });
    });
  }

}
