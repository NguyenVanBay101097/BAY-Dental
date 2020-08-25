import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { HrPayrollStructureService } from '../hr-payroll-structure.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { HrPayslipService, HrPayslipPaged } from '../hr-payslip.service';

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
  listWorkDays: GridDataResult = {
    data: [],
    total: 0
  };
  pageSizeWD = 20;
  pageWD = 1;
  collectionSizeWD = this.AllData.length;


  constructor(
    private activeroute: ActivatedRoute,
    private modalService: NgbModal,
    private hrPayslipService: HrPayslipService
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

      this.listWorkDays.data =
        [
          {
            type: 'Đi làm', description: 'số ngày đi làm', numberOfHour: 3,
            numberOfDay: '20', total: 5000000
          },
          {
            type: 'nghỉ làm', description: 'số ngày nghỉ làm', numberOfHour: 43,
            numberOfDay: '2', total: 400000
          },
        ];
      this.collectionSizeWD = 4;
      console.log(this.employee);
      console.log(this.payslipForm);
      // this.hrPayslipService.GetWorkedDayInfo()
    } else {
      this.listWorkDays = {
        data: [],
        total: 0
      };
    }
  }

  pageChangeWD(event: PageChangeEvent): void {
    // this.listLines = {
    //   data: this.AllData.slice((this.page - 1) * this.pageSize, (this.page - 1) * this.pageSize + this.pageSize),
    //   total: this.AllData.length
    // };
    this.loadWordDayFromApi();
  }

}
