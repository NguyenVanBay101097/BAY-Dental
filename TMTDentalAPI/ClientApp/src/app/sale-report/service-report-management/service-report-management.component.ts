import { Component, OnInit } from '@angular/core';
import { CellOptions, Workbook, WorkbookSheetColumn, WorkbookSheetRow, WorkbookSheetRowCell } from '@progress/kendo-angular-excel-export';
import { SaleReportService, ServiceReportDetailReq, ServiceReportDetailRes } from '../sale-report.service';
import { ServiceReportManageService } from './service-report-manage';
import { saveAs } from '@progress/kendo-file-saver';
import * as moment from 'moment';
import { Observable, zip } from 'rxjs';

@Component({
  selector: 'app-service-report-management',
  templateUrl: './service-report-management.component.html',
  styleUrls: ['./service-report-management.component.css'],
  providers:[ServiceReportManageService]
})
export class ServiceReportManagementComponent implements OnInit {

  constructor(
    private saleReportService: SaleReportService,
    private serviceReportManageService: ServiceReportManageService

  ) {
    this.serviceReportManageService.changeEmitted$.subscribe(res => {
      this.exportData(res);
    });
  }

  ngOnInit() {
  }

  exportData(e) {
    var stateDisplay= {
      sale:"Đang điều trị",
      done: "Hoàn thành"
    }

    var data = e.data;
    var args = e.args;
    var parentFilter = e.filter;
    var employeeFilter = e.employeeFilter || 'none';
    var title = e.title || 'báo cáo dịch vụ', header = e.header || "BÁO CÁO DỊCH VỤ";
    // Prevent automatically saving the file. We will save it manually after we fetch and add the details
    args.preventDefault();

    const observables = [];
    const workbook = args.workbook;

    const rows = workbook.sheets[0].rows;
    const columns = workbook.sheets[0].columns;
    var headerRow = Object.assign({},rows[0]);

    rows.forEach((row, index) => {
      // colspan
      row.cells[1].colSpan = 6;
      row.cells[1].textAlign = 'right';
      row.cells[2].colSpan = 3;
      row.cells[2].textAlign = 'right';
      if (row.type === 'data') {
        row.cells[2].format = '#,###0';
      } 
      //làm màu
      if (row.type === "header") {
        row.cells.forEach((cell: WorkbookSheetRowCell) => {
          cell.background = "#d0ece9";
          cell.color = "white";
        });
      }
    });

    // Get the default header styles.
    // Aternatively set custom styles for the details
    // https://www.telerik.com/kendo-angular-ui/components/excelexport/api/WorkbookSheetRowCell/
    const headerOptions = rows[0].cells[0];

    // rows.forEach((row, index) => {
    //   //colspan
    //   if (row.type === 'header' || row.type == "footer") {
    //     row.cells[1].colSpan = 6;
    //   }
    //   //làm màu
    //   if (row.type === "data" || row.type == 'footer') {
    //     row.cells[1].textAlign = 'right';
    //     row.cells[1].colSpan = 6;
    //   }
    // });

    if (data.length == 0) {
      var filter = new ServiceReportDetailReq();
      filter = Object.assign({}, parentFilter);
      //add title
      (rows as WorkbookSheetRow[]).unshift(<WorkbookSheetRow>{
        cells:[
          {
            colSpan: 9,
            rowSpan:1,
            value: header,
            color:'#4087b9',
            bold: true
          }
         
        ]
      },
      {
        cells:[
          {
            colSpan: 9,
            rowSpan:1,
            value: `Từ ngày ${ moment(filter.dateFrom).format('DD/MM/YYYY')} đến ngày ${ moment(filter.dateTo).format('DD/MM/YYYY')}`,
          }
        ]
      },
      {
        cells:[
          {
            colSpan: 1,
            rowSpan:1,
            value: ``
          }
        ]
      }
      );
      new Workbook(workbook).toDataURL().then((dataUrl: string) => {
        // https://www.telerik.com/kendo-angular-ui/components/filesaver/
        saveAs(dataUrl, 'baocaodoanhthu.xlsx');
      });
    }

    // Fetch the data for all details
    for (let idx = 0; idx < data.length; idx++) {
      var filter = new ServiceReportDetailReq();
      filter = Object.assign({}, parentFilter);
      filter.limit = 20;
      filter.offset = 0;
      filter.companyId = filter.companyId || '';
      var dataIndex = data[idx];
      filter.productId = dataIndex.productId || '';
      filter.employeeId = dataIndex.employeeId || '';
      filter.dateFrom = dataIndex.date2 ? moment(dataIndex.date2).format('YYYY/MM/DD')
        : (filter.dateFrom ? moment(filter.dateFrom).format('YYYY/MM/DD') : '');
      filter.dateTo = dataIndex.date2 ? moment(dataIndex.date2).format('YYYY/MM/DD')
        : (filter.dateTo ? moment(filter.dateTo).format('YYYY/MM/DD') : '');

        (filter.active as any) = (filter.active && filter.active !== null)? filter.active : '';
      observables.push(this.saleReportService.getServiceReportDetailPaged(filter));
    }

    zip.apply(Observable, observables).subscribe((result: any[][]) => {
      // add the detail data to the generated master sheet rows
      // loop backwards in order to avoid changing the rows index
      var listDetailHeaderIndex = [];
      for (let idx = result.length - 1; idx >= 0; idx--) {
        const lines = (<any>result[idx]).items;

        // add the detail data
        for (let productIdx = lines.length - 1; productIdx >= 0; productIdx--) {
          const line = lines[productIdx] as ServiceReportDetailRes;
          rows.splice(idx + 2, 0, {
            cells: [
              {},
              { value: moment(line.date).format('DD/MM/YYYY') },
              { value: line.orderPartnerName },
              { value: line.name },
              { value: line.employeeName },
              { value: line.teeth.map(x=> x.name).join(", ") },
              { value: line.productUOMQty },
              { value: line.priceSubTotal, textAlign: 'right',format : '#,###0' },
              { value: !line.isActive?'Ngừng điều trị' : stateDisplay[line.state] },
              { value: line.orderName },
            ]
          });
        }

        // add the detail header
        listDetailHeaderIndex.push(idx + 2);
        rows.splice(idx + 2, 0,{
          type: 'headerchild',
          cells: [
            {},
            Object.assign({}, headerOptions, { value: 'Ngày tạo', color:'white', width: 500 }),
            Object.assign({}, headerOptions, { value: 'Khách hàng', color:'white', width: 200 }),
            Object.assign({}, headerOptions, { value: 'Dịch vụ', color:'white', width: 100 }),
            Object.assign({}, headerOptions, { value: 'Bác sĩ', color:'white', width: 400 }),
            Object.assign({}, headerOptions, { value: 'Răng', color:'white', width: 200 }),
            Object.assign({}, headerOptions, { value: 'Số lượng', color:'white', width: 200, textAlign: 'right' }),
            Object.assign({}, headerOptions, { value: 'Thành tiền', color:'white', width: 500, textAlign: 'right' }),
            Object.assign({}, headerOptions, { value: 'Trạng thái', color:'white', width: 400 }),
            Object.assign({}, headerOptions, { value: 'Phiếu điều trị', color:'white', width: 300 })
          ]
        });

        if(idx != 0)// add header for each parent
        rows.splice(idx + 1, 0,headerRow);

      }

      //add title
      (rows as WorkbookSheetRow[]).unshift(<WorkbookSheetRow>{
        cells:[
          {
            colSpan: 9,
            rowSpan:1,
            value: header,
            color:'#4087b9',
            bold: true
          }
         
        ]
      },
      {
        cells:[
          {
            colSpan: 9,
            rowSpan:1,
            value: `Từ ngày ${ moment(filter.dateFrom).format('DD/MM/YYYY')} đến ngày ${ moment(filter.dateTo).format('DD/MM/YYYY')}`,
          }
        ]
      },
      {
        cells:[
          {
            colSpan: 1,
            rowSpan:1,
            value: ``
          }
        ]
      }
      );
      

      rows.forEach(r => {
         
        r.cells.forEach((cell: WorkbookSheetRowCell) => {
          if (r.type === "header") {
            cell.bold = true;
            cell.color = '#4087b9';
          }

          if (r.type === "headerchild") {
            console.log(cell);
            
            cell.bold = true;
            cell.color = 'black';
            delete cell.background
          }
        });
      
     
    });

      //format excel nè
      for (let index = 0; index < 8; index++) {
        columns.push(<WorkbookSheetColumn>{
          autoWidth: true
        });
      }
      columns.forEach(function(column){
        delete column.width;
        column.autoWidth = true;
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
