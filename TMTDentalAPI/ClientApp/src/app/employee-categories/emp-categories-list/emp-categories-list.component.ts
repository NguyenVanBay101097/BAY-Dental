import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { EmpCategoriesCreateUpdateComponent } from '../emp-categories-create-update/emp-categories-create-update.component';
import { EmployeeCategoryBasic, EmployeeCategoryPaged } from '../emp-category';
import { EmpCategoryService } from '../emp-category.service';

@Component({
  selector: 'app-emp-categories-list',
  templateUrl: './emp-categories-list.component.html',
  styleUrls: ['./emp-categories-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class EmpCategoriesListComponent implements OnInit {

  constructor(
    private service: EmpCategoryService,
    private modalService: NgbModal,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  loading = false;
  gridView: GridDataResult;
  windowOpened: boolean = false;
  skip = 0;
  limit = 20;
  pagerSettings: any;

  search: string;
  searchUpdate = new Subject<string>();

  ngOnInit() {
    this.getCategEmployeesList();
    this.searchChange();

  }

  getCategEmployeesList() {
    this.loading = true;
    var empPaged = new EmployeeCategoryPaged();
    empPaged.limit = this.limit;
    empPaged.offset = this.skip;
    if (this.search) {
      empPaged.search = this.search;
    }

    this.service.getCategEmployeePaged(empPaged).pipe(
      map(rs1 => (<GridDataResult>{
        data: rs1.items,
        total: rs1.totalItems
      }))
    ).subscribe(rs2 => {
      this.gridView = rs2;
      this.loading = false;
    }, er => {
      this.loading = true;
      console.log(er);
    }
    )
  }

  searchChange() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.getCategEmployeesList();
      });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.getCategEmployeesList();
  }

  openModal(item: EmployeeCategoryBasic) {
    const modalRef = this.modalService.open(EmpCategoriesCreateUpdateComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    if (item) {
      modalRef.componentInstance.empCategId = item.id;
      modalRef.componentInstance.title = 'S???a nh??m nh??n vi??n';
    } else {
      modalRef.componentInstance.title = 'Th??m nh??m nh??n vi??n'
    }
    modalRef.result.then(
      rs => {
        this.getCategEmployeesList();
      },
      er => { }
    )
  }

  deleteCategEmployee(id) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'X??a nh??m nh??n vi??n';
    modalRef.result.then(() => {
      this.service.deleteCategEmployee(id).subscribe(
        () => { this.getCategEmployeesList(); }
      );
    }, () => {
    });
  }
}
