import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';

@Component({
  selector: 'app-medicine-order',
  templateUrl: './medicine-order.component.html',
  styleUrls: ['./medicine-order.component.css']
})
export class MedicineOrderComponent implements OnInit {

  showDonThuoc = false;
  showHDThuoc =false;

  constructor(
    private router: Router,
    private checkPermissionService: CheckPermissionService
  ) { }

  ngOnInit() {
    this.checkRole();
  }

  redirectComponent(value) {
    this.router.navigateByUrl("/medicine-orders/" + value)
  }

  checkRole(){
    this.showDonThuoc = this.checkPermissionService.check(["Medicine.ToaThuoc.Read"]);
    this.showHDThuoc = this.checkPermissionService.check(["Medicine.MedicineOrder.Read"]);
  }

}
