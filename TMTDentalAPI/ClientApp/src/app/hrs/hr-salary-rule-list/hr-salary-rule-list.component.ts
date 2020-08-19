import { Component, OnInit, Input } from '@angular/core';
import { HrSalaryRuleDisplay, HrPayrollStructurePaged, HrPayrollStructureService } from '../hr-payroll-structure.service';
import { PageChangeEvent, GridDataResult } from '@progress/kendo-angular-grid';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { HrSalaryRuleCrudDialogComponent } from '../hr-salary-rule-crud-dialog/hr-salary-rule-crud-dialog.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-salary-rule-list',
  templateUrl: './hr-salary-rule-list.component.html',
  styleUrls: ['./hr-salary-rule-list.component.css']
})
export class HrSalaryRuleListComponent implements OnInit {

  id: string;
  AllData: any = [];
  listRule: GridDataResult = {
    data: [],
    total: 0
  };
  pageSize = 20;
  page = 1;
  loading = false;
  collectionSize = this.AllData.length;

  constructor(
    private activeroute: ActivatedRoute,
    private modalService: NgbModal,
    private payrollService: HrPayrollStructureService
  ) { }

  ngOnInit() {
    this.id = this.activeroute.snapshot.paramMap.get('id');
    if (this.id) {
      this.loadDataFromApi();
    }
  }

  loadDataFromApi() {
    if (this.id) {
      this.payrollService.GetListRuleByStructId(this.id).subscribe(res => {
        this.AllData = res.items;
        this.listRule = {
          data: this.AllData.slice((this.page - 1) * this.pageSize, (this.page - 1) * this.pageSize + this.pageSize),
          total: this.AllData.length
        };
        this.collectionSize = this.AllData.length;
      });
    }
  }

  pageChange(event: PageChangeEvent): void {
    this.listRule = {
      data: this.AllData.slice((this.page - 1) * this.pageSize, (this.page - 1) * this.pageSize + this.pageSize),
      total: this.AllData.length
    };
  }

  AllDataAdd(e) {
    const newRow: HrSalaryRuleDisplay = e;
    this.AllData.push(newRow);
    this.collectionSize = this.AllData.length;
  }

  removeHandler(e) {
    const modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa mẫu lương';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.AllData = this.AllData.filter(item => item.code !== e.code);
      this.listRule = {
        data: this.AllData.slice((this.page - 1) * this.pageSize, (this.page - 1) * this.pageSize + this.pageSize),
        total: this.AllData.length
      };
      this.collectionSize = this.AllData.length;
    });
  }

  ShowAddSalaryRulePopup() {
    const modalRef = this.modalService.open(HrSalaryRuleCrudDialogComponent,
      { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thuộc Tính Lương';
    modalRef.result.then((val) => {
      this.AllDataAdd(val);
      this.listRule = {
        data: this.AllData.slice((this.page - 1) * this.pageSize, (this.page - 1) * this.pageSize + this.pageSize),
        total: this.AllData.length
      };
    }, er => { });
  }

  editItem(item) {
    const modalRef = this.modalService.open(HrSalaryRuleCrudDialogComponent,
      { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thuộc Tính Lương';
    modalRef.componentInstance.rule = item;
    modalRef.result.then((val) => {
      this.UpdateAllData(val);
      this.listRule = {
        data: this.AllData.slice((this.page - 1) * this.pageSize, (this.page - 1) * this.pageSize + this.pageSize),
        total: this.AllData.length
      };
    }, er => { });
  }

  UpdateAllData(item) {
    if (item.id) {
      const index = this.AllData.findIndex(x => x.id === item.id);
      if (index !== -1) {
        this.AllData[index] = item;
      }
    } else {
      const index = this.AllData.findIndex(x => x.code === item.code);
      if (index !== -1) {
        this.AllData[index] = item;
      }
    }
  }
}
