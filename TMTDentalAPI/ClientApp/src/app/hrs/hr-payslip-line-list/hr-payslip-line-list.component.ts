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
  @Input() id: any;

  AllData: any = [];
  listLines = [];
  pageSize = 20;
  page = 1;
  loading = false;
  collectionSize = this.AllData.length;
  // for table workday
  listWorkDays: any[];
  tongtien = 0;

  constructor(
    private activeroute: ActivatedRoute,
    private modalService: NgbModal,
    private hrPayslipService: HrPayslipService,
    private intlService: IntlService
  ) { }

  ngOnInit() {
    // this.loadWordDayFromApi();
    this.loadLineDataFromApi();
    if (this.id) {
      this.loadWordDayFromApi();
    }
  }

  onTabSelect(e) {

  }

  loadLineDataFromApi() {
    if (this.id) {
      const val = new HrPayslipPaged();
      val.limit = this.pageSize;
      val.offset = (this.page - 1) * this.pageSize;
      val.payslipId = this.id;

      this.hrPayslipService.getPayslipLines(val).subscribe((res: any) => {
        // this.AllData = res.items;
        // this.listLines = {
        //   data: this.AllData.slice((this.page - 1) * this.pageSize, (this.page - 1) * this.pageSize + this.pageSize),
        //   total: res.totalItems
        // };

        this.listLines = res.lines;
        this.tongtien = res.totalAmount;
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
  get workedDaysControl() { return this.payslipForm.get('listHrPayslipWorkedDaySave'); }

  OnEmployeeChange() {
    if (this.employee && this.employee.structureTypeId) {
      const val = Object();
      val.dateFrom = this.intlService.formatDate(this.dateFrom, 'g', 'en-US');
      val.dateTo = this.intlService.formatDate(this.dateTo, 'g', 'en-US');
      val.employeeId = this.employee.id;
      this.hrPayslipService.GetWorkedDayInfoByEmployee(val).subscribe((res: any) => {
        this.listWorkDays = res.workedDayLines;
        this.workedDaysControl.setValue(res.workedDayLines);
      });

    } else {
      this.listWorkDays = [];
    }
  }

  loadWordDayFromApi() {
    if (this.employee && this.employee.structureTypeId) {
      const val = new HrPayslipPaged();
      val.payslipId = this.id;
      this.hrPayslipService.GetWorkedDayInfoByPayslipId(val).subscribe((res: any) => {
        this.listWorkDays = res;
        this.workedDaysControl.setValue(res);
      });

    } else {
      this.listWorkDays = [];
    }
  }

}
