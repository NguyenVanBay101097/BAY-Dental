import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { DotKhamStepsOdataService } from 'src/app/shared/services/dot-kham-stepsOdata.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DotKhamCreateUpdateDialogComponent } from 'src/app/shared/dot-kham-create-update-dialog/dot-kham-create-update-dialog.component';
import { DotkhamOdataService, DotKhamVm } from 'src/app/shared/services/dotkham-odata.service';
import { SaleOrdersOdataService } from "src/app/shared/services/sale-ordersOdata.service";
import { SaleOrderCreateDotKhamDialogComponent } from '../sale-order-create-dot-kham-dialog/sale-order-create-dot-kham-dialog.component';
import { AuthService } from 'src/app/auth/auth.service';

@Component({
  selector: "app-treatment-process-service-list",
  templateUrl: "./treatment-process-service-list.component.html",
  styleUrls: ["./treatment-process-service-list.component.css"],
})
export class TreatmentProcessServiceListComponent implements OnInit {
  saleOrderId: string;
  services: any;
  dotkhams: any[];
  activeDotkham: any;

  constructor(
    private route: ActivatedRoute,
    private dotKhamStepsOdataService: DotKhamStepsOdataService,
    private dotkhamOdataService: DotkhamOdataService,
    private saleOrderOdataService: SaleOrdersOdataService,
    private modalService: NgbModal,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.saleOrderId = this.route.queryParams['value'].id;

    this.loadDotKhamList();
    if (this.saleOrderId) {
      this.saleOrderOdataService.getDotKhamStepByOrderLine(this.saleOrderId).subscribe(
        (result) => {
          this.services = result['value'];
        },
        (error) => { }
      );
    }
  }

  checkStatusDotKhamStep(step) {
    step.IsDone = !step.IsDone;
    var value = {
      Id: step.Id,
      IsDone: step.IsDone
    }
    this.dotKhamStepsOdataService.patch(step.Id, value).subscribe(
      (result) => {
        console.log(result);
      },
      (error) => { }
    );

    this.loadDotKhamList();
  }

  loadDotKhamList() {
    if (!this.saleOrderId) {
      return;
    }
    const state = {
      take: 10,
      filter: {
        logic: 'SaleOrderId',
        filters: [
          { field: 'SaleOrderId', operator: 'eq', value: this.saleOrderId }
        ]
      }
    };
    const options = {
      // expand: 'Lines'
    };
    this.dotkhamOdataService.fetch2(state, options).subscribe((res: any) => {
      this.dotkhams = res.data;
    });
  }

  onCreateDotKham() {
    if (this.activeDotkham) {
      return;
    }
    const dotkham = new DotKhamVm();
    dotkham.Date = new Date();
    dotkham.Name = 'Đợt khám ' + (this.dotkhams.length + 1);
    dotkham.SaleOrderId = this.saleOrderId;
    dotkham.CompanyId = this.authService.userInfo.companyId;
    this.dotkhams.unshift(dotkham);
    this.activeDotkham = Object.assign({}, dotkham);
  }

  cancelDotkham(dotkham) {
    debugger;
    const index = this.dotkhams.indexOf(dotkham);
    this.dotkhams.splice(index, 1);
    this.activeDotkham = null;
  }
}
