import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { HrPayrollStructureService } from '../hr-payroll-structure.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { HrPayslipService } from '../hr-payslip.service';

@Component({
  selector: 'app-hr-payslip-line-list',
  templateUrl: './hr-payslip-line-list.component.html',
  styleUrls: ['./hr-payslip-line-list.component.css']
})
export class HrPayslipLineListComponent implements OnInit {

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
    private hrPayslipService: HrPayslipService
  ) { }

  ngOnInit() {
    this.id = this.activeroute.snapshot.paramMap.get('id');
    if (this.id) {
      this.loadDataFromApi();
    }
  }

  loadDataFromApi() {
    if (this.id) {
      this.hrPayslipService.get(this.id).subscribe(res => {
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


}
