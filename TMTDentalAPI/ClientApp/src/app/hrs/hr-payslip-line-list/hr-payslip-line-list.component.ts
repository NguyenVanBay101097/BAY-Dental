import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { HrPayrollStructureService } from '../hr-payroll-structure.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { HrPayslipService, HrPayslipPaged } from '../hr-payslip.service';
import { IntlService } from '@progress/kendo-angular-intl';

@Component({
  selector: 'app-hr-payslip-line-list',
  templateUrl: './hr-payslip-line-list.component.html',
  styleUrls: ['./hr-payslip-line-list.component.css']
})
export class HrPayslipLineListComponent implements OnInit {

  @Input() payslipForm: any;

  id: string;
  AllData: any = [];
  listLines: GridDataResult = {
    data: [],
    total: 0
  };
  pageSize = 20;
  page = 1;
  loading = false;
  collectionSize = this.AllData.length;
  // for table workday
  listWorkDays: any[];

  constructor(
    private activeroute: ActivatedRoute,
    private modalService: NgbModal,
    private hrPayslipService: HrPayslipService,
    private intlService: IntlService
  ) { }

  ngOnInit() {
    this.id = this.activeroute.snapshot.paramMap.get('id');
    // this.loadWordDayFromApi();
  }

  onTabSelect(e) {
    if (e.index === 1) {
      this.loadLineDataFromApi();
    } else if (e.index === 0) {
      this.loadWordDayFromApi();
    }
  }

  loadLineDataFromApi() {
    if (this.id) {
      const val = new HrPayslipPaged();
      val.limit = this.pageSize;
      val.offset = (this.page - 1) * this.pageSize;
      val.payslipId = this.id;

      this.hrPayslipService.getPayslipLinePaged(val).subscribe(res => {
        // this.AllData = res.items;
        // this.listLines = {
        //   data: this.AllData.slice((this.page - 1) * this.pageSize, (this.page - 1) * this.pageSize + this.pageSize),
        //   total: res.totalItems
        // };
        this.listLines = {
          data: res.items,
          total: res.totalItems
        };
        this.collectionSize = res.totalItems;
      });
    }
  }

  pageChange(event: PageChangeEvent): void {
    // this.listLines = {
    //   data: this.AllData.slice((this.page - 1) * this.pageSize, (this.page - 1) * this.pageSize + this.pageSize),
    //   total: this.AllData.length
    // };
    this.loadLineDataFromApi();
  }

  get employee() { return this.payslipForm.get('employee').value; }
  get dateFrom() { return this.payslipForm.get('dateFrom').value; }
  get dateTo() { return this.payslipForm.get('dateTo').value; }

  loadWordDayFromApi() {
    if (this.employee && this.employee.structureTypeId) {
      const val = Object();
      val.dateFrom = this.intlService.formatDate(this.dateFrom, 'g', 'en-US');
      val.dateTo = this.intlService.formatDate(this.dateTo, 'g', 'en-US');
      val.employeeId = this.employee.id;
      this.hrPayslipService.GetWorkedDayInfo(val).subscribe((res: any) => {
        this.listWorkDays = res.workedDayLines;
      });

    } else {
      this.listWorkDays = [];
    }
  }

}
