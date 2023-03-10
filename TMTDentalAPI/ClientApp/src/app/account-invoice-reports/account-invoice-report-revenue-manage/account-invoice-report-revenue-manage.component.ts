import { Component, OnInit } from '@angular/core';
import { Workbook } from '@progress/kendo-angular-excel-export';
import { saveAs } from '@progress/kendo-file-saver';
import * as moment from 'moment';
import { Observable, zip } from 'rxjs';
import { AccountInvoiceReportService, RevenueReportDetailPaged } from '../account-invoice-report.service';
import { RevenueManageService } from './revenue-manage.service';
@Component({
  selector: 'app-account-invoice-report-revenue',
  templateUrl: './account-invoice-report-revenue-manage.component.html',
  styleUrls: ['./account-invoice-report-revenue-manage.component.css'],
  host: {
    class: "o_action o_view_controller",
  },
  providers:[RevenueManageService]
})
export class AccountInvoiceReportRevenueManageComponent implements OnInit {

  constructor(
    private accInvService: AccountInvoiceReportService,
    private revenueManageService: RevenueManageService
  ) {
    this.revenueManageService.changeEmitted$.subscribe(res=>{
      this.exportData(res);
    });
  }

  ngOnInit() {
  }

  exportData(e) {
    var data = e.data;
    var args = e.args;
    var filter = e.filter;
    // var employeeFilter = e.employeeFilter || 'none';
    var title = e.title || 'doanhthu';
    // var showPartner = typeof e.showPartner == 'undefined' ? true: false;
    // var showInvoiceDate = typeof e.showInvoiceDate == 'undefined' ? true: false;
    
    
    
    // Prevent automatically saving the file. We will save it manually after we fetch and add the details
    args.preventDefault();

    const observables = [];
    const workbook = args.workbook;

    const rows = workbook.sheets[0].rows;

    // Get the default header styles.
    // Aternatively set custom styles for the details
    // https://www.telerik.com/kendo-angular-ui/components/excelexport/api/WorkbookSheetRowCell/
    const headerOptions = rows[0].cells[0];

    rows.forEach((row, index) => {
      //làm màu
      if (row.type === "data" || row.type == 'footer') {
        row.cells[0].bold = true;
        row.cells[1].format = '#,##0'
        row.cells[1].textAlign = 'right';
        row.cells[1].colSpan = 6;
        row.cells[1].bold = true;
      }
    });

    if (data.length == 0) {
      new Workbook(workbook).toDataURL().then((dataUrl: string) => {
        // https://www.telerik.com/kendo-angular-ui/components/filesaver/
        saveAs(dataUrl, 'baocaodoanhthu.xlsx');
      });
    }

   


    // Fetch the data for all details
    for (let idx = 0; idx < data.length; idx++) {
      var val = new RevenueReportDetailPaged();
      val.dateFrom = filter.dateFrom ? moment(filter.dateFrom).format('YYYY/MM/DD') : '';
      val.dateTo = filter.dateTo ? moment(filter.dateTo).format('YYYY/MM/DD') : '';
      val.companyId = filter.companyId || '';
      val.limit = 0;

      var dataIndex = data[idx];
      val.dateFrom = dataIndex.invoiceDate ?  moment(dataIndex.invoiceDate).format('YYYY/MM/DD'): val.dateFrom ;
      val.dateTo = dataIndex.invoiceDate ?  moment(dataIndex.invoiceDate).format('YYYY/MM/DD'): val.dateTo ;
      val.productId = dataIndex.productId || '';
      val.partnerId = dataIndex.partnerId || '';
      val.assistantId = dataIndex.employeeId || '';
      val.employeeId = dataIndex.groupBy && dataIndex.groupBy == 'employee'? dataIndex.toDetailEmployeeId : '';
      val.assistantId = dataIndex.groupBy && dataIndex.groupBy == 'assistant'? dataIndex.toDetailEmployeeId : '';
      observables.push(this.accInvService.getRevenueReportDetailPaged(val));
    }

    zip.apply(Observable, observables).subscribe((result: any[][]) => {
      // add the detail data to the generated master sheet rows
      // loop backwards in order to avoid changing the rows index
      var listDetailHeaderIndex = [];
      for (let idx = result.length - 1; idx >= 0; idx--) {
        const lines = (<any>result[idx]).items;

        // add the detail data
        for (let productIdx = lines.length - 1; productIdx >= 0; productIdx--) {
          const line = lines[productIdx];
          // if (showPartner == false){
          //   rows.splice(idx + 4, 0, {
          //     cells: [
          //       { value: moment(line.invoiceDate).format('DD/MM/YYYY') },
          //       { value: line.invoiceOrigin },
          //       { value: line.employeeName },
          //       { value: line.assistantName },
          //       { value: line.productName },
          //       { value: line.priceSubTotal.toLocaleString('vi'), textAlign: 'right', format:'#,##0' }
          //     ]
          //   });
          // }
          // else if (showInvoiceDate == false){
          //   rows.splice(idx + 4, 0, {
          //     cells: [
          //       { value: line.invoiceOrigin },
          //       { value: line.partnerName },
          //       { value: line.employeeName },
          //       { value: line.assistantName },
          //       { value: line.productName },
          //       { value: line.priceSubTotal.toLocaleString('vi'), textAlign: 'right',format:'#,##0' }
          //     ]
          //   });
          // }
          // else {
          //   rows.splice(idx + 4, 0, {
          //     cells: [
          //       { value: moment(line.invoiceDate).format('DD/MM/YYYY') },
          //       { value: line.invoiceOrigin },
          //       { value: line.partnerName },
          //       { value: line.employeeName },
          //       { value: line.assistantName },
          //       { value: line.productName },
          //       { value: line.priceSubTotal.toLocaleString('vi'), textAlign: 'right',format: '#,##0'}
          //     ]
          //   });
          // }
          rows.splice(idx + 4, 0, {
            cells: [
              { value: moment(line.invoiceDate).format('DD/MM/YYYY') },
              { value: line.invoiceOrigin },
              { value: line.partnerName },
              { value: line.employeeName },
              { value: line.assistantName },
              { value: line.productName },
              { value: line.priceSubTotal.toLocaleString('vi'), textAlign: 'right',format: '#,##0'}
            ]
          });
        }
        // add the detail header
        listDetailHeaderIndex.push(idx + 4);
        // if (showPartner == false){
        //   rows.splice(idx + 4, 0, {
        //     cells: [
        //       Object.assign({}, headerOptions, { value: 'Ngày thanh toán', background: '#aabbcc', width: 20 }),
        //       Object.assign({}, headerOptions, { value: 'Số phiếu', background: '#aabbcc', width: 200 }),
        //       Object.assign({}, headerOptions, { value: 'Bác sĩ', background: '#aabbcc', width: 200 }),
        //       Object.assign({}, headerOptions, { value: 'Phụ tá', background: '#aabbcc', width: 200 }),
        //       Object.assign({}, headerOptions, { value: 'Dịch vụ/Thuốc', background: '#aabbcc', width: 200 }),
        //       Object.assign({}, headerOptions, { value: 'Thanh toán', background: '#aabbcc', width: 200 })
        //     ]
        //   });
        // }
        // else if (showInvoiceDate == false) {
        //   rows.splice(idx + 4, 0, {
        //     cells: [
        //       Object.assign({}, headerOptions, { value: 'Số phiếu', background: '#aabbcc', width: 200 }),
        //       Object.assign({}, headerOptions, { value: 'Khách hàng', background: '#aabbcc', width: 200 }),
        //       Object.assign({}, headerOptions, { value: 'Bác sĩ', background: '#aabbcc', width: 200 }),
        //       Object.assign({}, headerOptions, { value: 'Phụ tá', background: '#aabbcc', width: 200 }),
        //       Object.assign({}, headerOptions, { value: 'Dịch vụ/Thuốc', background: '#aabbcc', width: 200 }),
        //       Object.assign({}, headerOptions, { value: 'Thanh toán', background: '#aabbcc', width: 200 })
        //     ]
        //   });
        // }
        // else {
        //   rows.splice(idx + 4, 0, {
        //     cells: [
        //       Object.assign({}, headerOptions, { value: 'Ngày thanh toán', background: '#aabbcc', width: 20 }),
        //       Object.assign({}, headerOptions, { value: 'Số phiếu', background: '#aabbcc', width: 200 }),
        //       Object.assign({}, headerOptions, { value: 'Khách hàng', background: '#aabbcc', width: 200 }),
        //       Object.assign({}, headerOptions, { value: 'Bác sĩ', background: '#aabbcc', width: 200 }),
        //       Object.assign({}, headerOptions, { value: 'Phụ tá', background: '#aabbcc', width: 200 }),
        //       Object.assign({}, headerOptions, { value: 'Dịch vụ/Thuốc', background: '#aabbcc', width: 200 }),
        //       Object.assign({}, headerOptions, { value: 'Thanh toán', background: '#aabbcc', width: 200 })
        //     ]
        //   });
        // }
        rows.splice(idx + 4, 0, {
          cells: [
            Object.assign({}, headerOptions, { value: 'Ngày thanh toán', background: '#aabbcc', width: 20 }),
            Object.assign({}, headerOptions, { value: 'Số phiếu', background: '#aabbcc', width: 200 }),
            Object.assign({}, headerOptions, { value: 'Khách hàng', background: '#aabbcc', width: 200 }),
            Object.assign({}, headerOptions, { value: 'Bác sĩ', background: '#aabbcc', width: 200 }),
            Object.assign({}, headerOptions, { value: 'Phụ tá', background: '#aabbcc', width: 200 }),
            Object.assign({}, headerOptions, { value: 'Dịch vụ/Thuốc', background: '#aabbcc', width: 200 }),
            Object.assign({}, headerOptions, { value: 'Thanh toán', background: '#aabbcc', width: 200 })
          ]
        });
      }
      var a = workbook;
      delete a.sheets[0].columns[1].width;
      a.sheets[0].columns[1].autoWidth = true;
      a.sheets[0].columns[2] = {
        width: 120
      };
      a.sheets[0].columns[3] = {
        width: 200
      };
      a.sheets[0].columns[4] = {
        width: 200
      };
      a.sheets[0].columns[5] = {
        width: 200
      };
      a.sheets[0].columns[6] = {
        width: 220
      };
      a.sheets[0].columns[7] = {
        width: 150
      };
      rows.forEach((row, index) => {
        //colspan
        if (row.type == "footer") {
          rows.splice(index,1);
          if (row.type === 'header') {
            rows[index + 1].cells[1].colSpan = 6;
          }
        }
        // if (index == 2){
        //   rows.splice(index,1);
        // }
        //làm màu
        if (row.type === "header") {
          if (index != 0 && index != 1){
            row.cells.forEach((cell,index) => {
              cell.background = "#aabbcc";
            });
          }
          
        }
      });
      new Workbook(workbook).toDataURL().then((dataUrl: string) => {
        // https://www.telerik.com/kendo-angular-ui/components/filesaver/
        saveAs(dataUrl, `${title}.xlsx`);
      });
      // create a Workbook and save the generated data URL
      // https://www.telerik.com/kendo-angular-ui/components/excelexport/api/Workbook/

    });
  }

}
