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
  showXuatKho = false;
  showProductRequest = false;
  showSotckInventory = false;
  constructor(
    private checkPermissionService: CheckPermissionService
  ) { }

  ngOnInit() {
    this.checkRole();
  }

  checkRole(){
    this.showXuatNhapTon = this.checkPermissionService.check(["Report.Stock"]);
    this.showNhapKho = this.checkPermissionService.check(["Stock.Picking.Read"]);
    this.showXuatKho = this.checkPermissionService.check(["Stock.Picking.Read"]);
    this.showProductRequest = this.checkPermissionService.check(["Basic.ProductRequest.Read"]);
    this.showSotckInventory = this.checkPermissionService.check(["Stock.Inventory.Read"]);
  }
}
