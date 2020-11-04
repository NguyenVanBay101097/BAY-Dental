import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-partner-overview-advisory',
  templateUrl: './partner-overview-advisory.component.html',
  styleUrls: ['./partner-overview-advisory.component.css']
})
export class PartnerOverviewAdvisoryComponent implements OnInit {

  @Input() saleQuotations: any;
  constructor() { }

  ngOnInit() {
    console.log(this.saleQuotations);
    
  }

  // updateTeeth(line) {
  //   var val = this.getFormDataSave();
  //   this.saleOrderService.update(this.saleOrderId, val).subscribe(() => {
  //     this.notificationService.show({
  //       content: 'Lưu thành công',
  //       hideAfter: 3000,
  //       position: { horizontal: 'center', vertical: 'top' },
  //       animation: { type: 'fade', duration: 400 },
  //       type: { style: 'success', icon: true }
  //     });
  //     this.loadRecord();
  //   }, () => {
  //     this.loadRecord();
  //   });
  // }

}
