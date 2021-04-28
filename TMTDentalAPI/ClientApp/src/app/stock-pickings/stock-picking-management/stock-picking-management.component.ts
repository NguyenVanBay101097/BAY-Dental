import { Component, OnInit } from '@angular/core';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';

@Component({
  selector: 'app-stock-picking-management',
  templateUrl: './stock-picking-management.component.html',
  styleUrls: ['./stock-picking-management.component.css']
})
export class StockPickingManagementComponent implements OnInit {
  showXuatNhapTon = false;
  showNhapKho = false;
  constructor(
    private checkPermissionService: CheckPermissionService
  ) { }

  ngOnInit() {
  }

  checkRole(){
    this.showXuatNhapTon = this.checkPermissionService.check(["Report.Stock"]);
    this.showNhapKho = this.checkPermissionService.check(["Stock.Picking.Read"]);
  }
}
