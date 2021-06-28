import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GridDataResult } from '@progress/kendo-angular-grid';
import * as moment from 'moment';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { CompanyBasic, CompanyPaged, CompanyService } from 'src/app/companies/company.service';
import { SaleOrderPaged, SaleOrderService } from 'src/app/core/services/sale-order.service';

@Component({
  selector: 'app-sale-order-management',
  templateUrl: './sale-order-management.component.html',
  styleUrls: ['./sale-order-management.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class SaleOrderManagementComponent implements OnInit {

  search: string;
  searchUpdate = new Subject<string>();
  company: CompanyBasic;
  ranges: any = [
    {text:'1 tháng', value: moment().subtract(1,'month')},
    {text:'3 tháng', value: moment().subtract(3,'month')},
    {text:'6 tháng', value: moment().subtract(6,'month')},
    {text:'12 tháng', value: moment().subtract(12,'month')},
  ];
  dateOrderTo = this.ranges[0];
  companies: CompanyBasic[] = [];
  saleOrdersData: GridDataResult;
  loading = false;
  limit = 20;
  skip = 0;
  constructor(
    private companyService: CompanyService,
    private saleOrderService: SaleOrderService,
    private router: Router
    ) { }

  ngOnInit() {
    this.loadCompanies();
    this.loadDataFromApi();
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.skip = 0;
        this.loadDataFromApi();
      });
  }

  loadCompanies(){
    var val = new CompanyPaged();
    val.active = true;
    val.limit = 1000;
    this.companyService.getPaged(val).subscribe(result => {
      this.companies = result.items;
    })
  }

  loadDataFromApi(){
    var val = new SaleOrderPaged();
    val.search = this.search? this.search : '';
    val.dateOrderTo = moment(this.dateOrderTo.value).format('YYYY/MM/DD');
    val.companyId = this.company? this.company.id : "";
    val.state = "sale";
    val.limit = this.limit;
    val.offset = this.skip;
    this.saleOrderService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.saleOrdersData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  handleFilterDateOrder(){
    this.skip = 0;
    this.loadDataFromApi();
  }

  handleFilterCompany(){
    this.skip = 0;
    this.loadDataFromApi();
  }

  getFormSaleOrder(id){
    this.router.navigate(['/sale-orders/form'], { queryParams: { id: id } });
  }

  pageChange(event){
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  exportExcelFile(){
    var paged = new SaleOrderPaged();
    paged.search = this.search || '';
    paged.dateOrderTo = moment(this.dateOrderTo.value).format('YYYY/MM/DD');
    paged.companyId = this.company? this.company.id : "";
    this.saleOrderService.exportExcelFile(paged).subscribe((res) => {
      let filename = "Quản lý điều trị chưa hoàn thành";

      let newBlob = new Blob([res], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });

      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        // For Firefox it is necessary to delay revoking the ObjectURL
        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }

}
