import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { HrPayrollStructureService, HrPayrollStructurePaged } from '../hr-payroll-structure.service';
import { Router } from '@angular/router';
import { map } from 'rxjs/internal/operators/map';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap, switchMap } from 'rxjs/operators';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { HrPayrollStructureTypePaged, HrPayrollStructureTypeService } from '../hr-payroll-structure-type.service';

@Component({
  selector: 'app-payroll-structure-list',
  templateUrl: './hr-payroll-structure-list.component.html',
  styleUrls: ['./hr-payroll-structure-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class HrPayrollStructureListComponent implements OnInit {

  @ViewChild('typeCbx', { static: true }) typeCbx: ComboBoxComponent;

  gridData: GridDataResult = {
    data: [],
    total: 0
  };
  limit = 20;
  skip = 0;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  typeList: any;

  constructor(
    private hrPayrollStructureService: HrPayrollStructureService,
    private hrPayrollStructureTypeService: HrPayrollStructureTypeService,
    private modalService: NgbModal,
    private router: Router) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });

    this.loadListTypePaged();
    this.typeCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.typeCbx.loading = true)),
      switchMap(value => this.searchType(value))
    ).subscribe((result: any) => {
      this.typeList = result.items;
      this.typeCbx.loading = false;
    });
    this.loadDataFromApi();
  }

  loadListTypePaged() {
    this.searchType().subscribe((res) => {
      this.typeList = res.items;
    });
  }
  searchType(search?: string) {
    const val = new HrPayrollStructureTypePaged();
    val.search = search ? search : '';
    return this.hrPayrollStructureTypeService.getPaged(val);
  }

  onchangeType(e) {
    const structureTypeId = e ? e.id : '';
    this.loadDataFromApi(structureTypeId);
  }

  loadDataFromApi(structureTypeId?: string) {
    this.loading = true;
    const val = new HrPayrollStructurePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.structureTypeId = structureTypeId ? structureTypeId : '';
    if (this.search) { val.filter = this.search; }

    this.hrPayrollStructureService.getPaged(val).pipe(
      map((res: any) => ({
        data: res.items,
        total: res.totalItems,
      } as GridDataResult))
    )
      .subscribe(res => {
        this.gridData = res;
        this.loading = false;
      }, err => {
        console.log(err);
        this.loading = false;
      });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  createItem() {
    this.router.navigate(['/hr/payroll-structures/create']);
  }

  editItem(dataitem) {
    this.router.navigate(['/hr/payroll-structures/edit/' + dataitem.id]);
  }

  deleteItem(dataitem) {
    const modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa mẫu lương: ' + dataitem.name;
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.hrPayrollStructureService.delete(dataitem.id).subscribe(res => {
        this.loadDataFromApi();
      });
    });
  }
}
